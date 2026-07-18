namespace Ngaq.Ui.Views.Word.WordEditV2;

using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Model.Po.Kv;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Model.Po.Learn_;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Learn;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.PoWordEdit;
using Ngaq.Ui.Views.Word.WordLearnPage;
using Ngaq.Ui.Views.Word.WordPropPage;
using Tsinswreng.CsTools;
using Tsinswreng.CsCore;
using Ctx = VmWordEditV2;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;
using Ngaq.Core.Tools;
using Tsinswreng.CsTempus;

/// 單詞編輯主頁 ViewModel。職責：協調三個分頁，統一保存/刪除。
public partial class VmWordEditV2: ViewModelBase, IMk<Ctx>{
	protected VmWordEditV2(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	ISvcWordV2? SvcWordV2;
	IFrontendUserCtxMgr? UserCtxMgr;

	public VmWordEditV2(
		ISvcWordV2? SvcWordV2,
		IFrontendUserCtxMgr? UserCtxMgr
	){
		this.SvcWordV2 = SvcWordV2;
		this.UserCtxMgr = UserCtxMgr;
	}

	/// 保存策略：普通編輯走細分操作；詞典入口可切到 Merge。
	public enum ESaveMode{
		DetailOps = 0,
		Merge = 1,
	}

	public IJnWord? Src{
		get;
		set{SetProperty(ref field, value);}
	}

	public JnWord? Draft{
		get;
		set{SetProperty(ref field, value);}
	}

	public i32 TabIndex{
		get;
		set{SetProperty(ref field, value);}
	}

	public VmPoWordEdit PoWordEdit{
		get;
		set{SetProperty(ref field, value);}
	} = new();

	public VmWordPropPage WordPropPage{
		get;
		set{SetProperty(ref field, value);}
	} = new();

	public VmWordLearnPage WordLearnPage{
		get;
		set{SetProperty(ref field, value);}
	} = new();

	public str LastError{
		get;
		set{
			if(SetProperty(ref field, value)){
				OnPropertyChanged(nameof(HasError));
			}
		}
	} = "";

	public bool HasError => !str.IsNullOrWhiteSpace(LastError);

	public bool IsDirty{
		get;
		set{SetProperty(ref field, value);}
	}

	public ESaveMode SaveMode{
		get;
		set{SetProperty(ref field, value);}
	} = ESaveMode.DetailOps;

	/// 由外部入口傳入單詞，初始化三個子頁狀態。
	public nil FromJnWord(IJnWord JnWord){
		Src = JnWord;
		Draft = JnWord.DeepClone().AsOrToJnWord();
		LoadFromDraft();
		return NIL;
	}

	/// 自由加詞入口：參照詞典保存新詞流程，直接建立可 Merge 的空白草稿。
	/// 同時補一條 Add 學習記錄，避免新詞缺少首次加入詞庫的事件。
	public nil InitFreeAddDraft(str Lang = ""){
		var now = UnixMs.Now();
		var draft = new JnWord{
			Word = new PoWord{
				Head = "",
				Lang = Lang,
				StoredAt = now,
				BizCreatedAt = now,
				BizUpdatedAt = now,
			},
			Props = [],
			Learns = [
				new PoWordLearn{
					Id = new IdWordLearn(),
					LearnResult = ELearn.Add,
					BizCreatedAt = now,
				}
			],
		};
		draft.EnsureForeignId();
		Src = null;
		Draft = draft;
		SaveMode = ESaveMode.Merge;
		LoadFromDraft();
		return NIL;
	}

	void LoadFromDraft(){
		if(Draft is null){
			return;
		}
		PoWordEdit.LoadFromPo(Draft.Word);
		WordPropPage.LoadFromPoProps(Draft.Props);
		WordLearnPage.LoadFromPoLearns(Draft.Learns);
		LastError = "";
		IsDirty = false;
	}

	public async Task<nil> Save(CT Ct){
		if(AnyNull(SvcWordV2, UserCtxMgr)){
			return NIL;
		}
		if(Draft is null){
			ShowDialog(I18n[K.NoDraft]);
			return NIL;
		}
		if(!TryApplyFormToDraft(out var err)){
			LastError = err;
			ShowDialog(err);
			return NIL;
		}
		try{
			Draft.EnsureForeignId();
			if(SaveMode == ESaveMode.Merge){
				await SaveByMerge(Ct);
			}else{
				await SaveByDetailOps(Ct);
			}
			Src = Draft.DeepClone().AsOrToJnWord();
			SaveMode = ESaveMode.DetailOps;
			LastError = "";
			IsDirty = false;
			ShowToast(I18n[K.Saved]);
		}catch(Exception ex){
			LastError = ex.Message;
			HandleErr(ex);
		}
		return NIL;
	}

	async Task<nil> SaveByMerge(CT Ct){
		if(AnyNull(SvcWordV2, UserCtxMgr, Draft)){
			return NIL;
		}
		await SvcWordV2.MergeWord(
			UserCtxMgr.GetDbUserCtx(),
			ToolAsyE.ToAsyE([Draft]),
			Ct
		);
		return NIL;
	}

	async Task<nil> SaveByDetailOps(CT Ct){
		if(AnyNull(SvcWordV2, UserCtxMgr, Draft)){
			return NIL;
		}
		var dbCtx = UserCtxMgr.GetDbUserCtx();
		var oldId = Draft.Word.Id;
		var finalId = await UpdRootAndGetFinalId(dbCtx, Ct);
		var hasMovedToOtherWord = oldId != finalId;
		if(hasMovedToOtherWord){
			Draft.SetIdEtEnsureFKey(finalId);
		}

		var wordId = Draft.Word.Id;
		await SavePropsAtomized(dbCtx, wordId, Ct);
		await SaveLearnsAtomized(dbCtx, wordId, Ct);
		WordPropPage.OnSaved();
		WordLearnPage.OnSaved();
		return NIL;
	}

	async Task<IdWord> UpdRootAndGetFinalId(IDbUserCtx DbCtx, CT Ct){
		if(AnyNull(SvcWordV2, Draft)){
			return default;
		}
		var respAsyE = await SvcWordV2.BatUpdPoWord(DbCtx, ToolAsyE.ToAsyE([Draft.Word]), Ct);
		var resp = await respAsyE.FirstOrDefaultAsync(Ct);
		if(resp is null){
			throw new InvalidOperationException("BatUpdPoWord returned empty response");
		}
		return resp.FinalId;
	}

	/// 根據行 DmlState 原子化保存 Props：Added → BatAdd / Modified → BatUpd / Removed → Del。
	async Task<nil> SavePropsAtomized(IDbUserCtx DbCtx, IdWord WordId, CT Ct){
		if(SvcWordV2 is null){
			return NIL;
		}

		// BatAdd
		var addPos = new List<PoWordProp>();
		foreach(var r in WordPropPage.AddedRows){
			if(r.TryToPo(WordId, out var po, out _)){
				addPos.Add(po);
			}
		}
		if(addPos.Count > 0){
			await SvcWordV2.OrdAddWordProp(DbCtx, ToolAsyE.ToAsyE(addPos), Ct);
		}

		// BatUpd
		var updPos = new List<PoWordProp>();
		foreach(var r in WordPropPage.ModifiedRows){
			if(r.TryToPo(WordId, out var po, out _)){
				updPos.Add(po);
			}
		}
		if(updPos.Count > 0){
			await SvcWordV2.OrdUpdWordProp(DbCtx, ToolAsyE.ToAsyE(updPos), Ct);
		}

		// Del
		if(WordPropPage.RemovedRows.Count > 0){
			var ids = WordPropPage.RemovedRows.Select(r => r.Raw.Id);
			await SvcWordV2.DelWordPropInId(DbCtx, ToolAsyE.ToAsyE(ids), Ct);
		}
		return NIL;
	}

	/// 根據行 DmlState 原子化保存 Learns：Added → BatAdd / Modified → BatUpd / Removed → Del。
	async Task<nil> SaveLearnsAtomized(IDbUserCtx DbCtx, IdWord WordId, CT Ct){
		if(SvcWordV2 is null){
			return NIL;
		}

		// BatAdd
		var addPos = new List<PoWordLearn>();
		foreach(var r in WordLearnPage.AddedRows){
			if(r.TryToPo(WordId, out var po, out _)){
				addPos.Add(po);
			}
		}
		if(addPos.Count > 0){
			await SvcWordV2.OrdAddWordLearn(DbCtx, ToolAsyE.ToAsyE(addPos), Ct);
		}

		// BatUpd
		var updPos = new List<PoWordLearn>();
		foreach(var r in WordLearnPage.ModifiedRows){
			if(r.TryToPo(WordId, out var po, out _)){
				updPos.Add(po);
			}
		}
		if(updPos.Count > 0){
			await SvcWordV2.OrdUpdWordLearn(DbCtx, ToolAsyE.ToAsyE(updPos), Ct);
		}

		// Del
		if(WordLearnPage.RemovedRows.Count > 0){
			var ids = WordLearnPage.RemovedRows.Select(r => r.Raw.Id);
			await SvcWordV2.DelWordLearnInId(DbCtx, ToolAsyE.ToAsyE(ids), Ct);
		}
		return NIL;
	}

	/// 屬性編輯頁按刪除時直接調對應刪除接口；新建未保存行只做本地移除。
	public async Task<bool> DeletePropRow(VmWordPropRow Row, CT Ct){
		if(Row.DmlState == EDmlState.Added){
			WordPropPage.RemoveRow(Row);
			return true;
		}
		if(AnyNull(SvcWordV2, UserCtxMgr)){
			return false;
		}
		try{
			await SvcWordV2.DelWordPropInId(
				UserCtxMgr.GetDbUserCtx(),
				ToolAsyE.ToAsyE([Row.Raw.Id]),
				Ct
			);
			WordPropPage.RemovePersistedRow(Row);
			SyncDeletedPropToLocalState(Row.Raw.Id);
			ShowToast(I18n[K.Deleted]);
			return true;
		}catch(Exception ex){
			LastError = ex.Message;
			HandleErr(ex);
			return false;
		}
	}

	/// 學習記錄的刪除規則與屬性一致：已有數據立刻落庫，新建行僅本地撤銷。
	public async Task<bool> DeleteLearnRow(VmWordLearnRow Row, CT Ct){
		if(Row.DmlState == EDmlState.Added){
			WordLearnPage.RemoveRow(Row);
			return true;
		}
		if(AnyNull(SvcWordV2, UserCtxMgr)){
			return false;
		}
		try{
			await SvcWordV2.DelWordLearnInId(
				UserCtxMgr.GetDbUserCtx(),
				ToolAsyE.ToAsyE([Row.Raw.Id]),
				Ct
			);
			WordLearnPage.RemovePersistedRow(Row);
			SyncDeletedLearnToLocalState(Row.Raw.Id);
			ShowToast(I18n[K.Deleted]);
			return true;
		}catch(Exception ex){
			LastError = ex.Message;
			HandleErr(ex);
			return false;
		}
	}

	/// 單行已直接落庫刪除後，同步清理本地草稿與源數據，避免後續保存又把它帶回來。
	void SyncDeletedPropToLocalState(IdWordProp Id){
		if(Draft is not null){
			Draft.Props = Draft.Props.Where(x=>x.Id != Id).ToList();
		}
		// 來源對象不一定是 JnWord 具體類，很多入口只保證是 IJnWord。
		// 直接按接口回寫，避免重新進編輯頁時仍讀到舊內存快照。
		if(Src is not null){
			Src.Props = Src.Props.Where(x=>x.Id != Id).ToList();
		}
	}

	/// Learn 的本地同步與 Prop 相同。
	void SyncDeletedLearnToLocalState(IdWordLearn Id){
		if(Draft is not null){
			Draft.Learns = Draft.Learns.Where(x=>x.Id != Id).ToList();
		}
		// 同上：回寫接口對象本身，確保列表卡片/詳情頁再次打開時拿到的是新狀態。
		if(Src is not null){
			Src.Learns = Src.Learns.Where(x=>x.Id != Id).ToList();
		}
	}

	public async Task<nil> Delete(CT Ct){
		if(AnyNull(SvcWordV2, UserCtxMgr, Draft)){
			return NIL;
		}
		try{
			await SvcWordV2.SoftDelPoWordInId(
				UserCtxMgr.GetDbUserCtx(),
				ToolAsyE.ToAsyE([Draft.Word.Id]),
				Ct
			);
			LastError = "";
			IsDirty = false;
			ShowToast(I18n[K.Deleted]);
			ViewNavi?.Back();
		}catch(Exception ex){
			LastError = ex.Message;
			HandleErr(ex);
		}
		return NIL;
	}

	bool TryApplyFormToDraft(out str Err){
		Err = "";
		if(Draft is null){
			Err = I18n[K.NoDraft];
			return false;
		}
		if(!PoWordEdit.TryApplyToPo(Draft.Word, out Err)){
			return false;
		}
		if(!WordPropPage.TryBuildPoProps(Draft.Word.Id, out var nextProps, out var propErr)){
			Err = propErr;
			return false;
		}
		if(!WordLearnPage.TryBuildPoLearns(Draft.Word.Id, out var nextLearns, out var learnErr)){
			Err = learnErr;
			return false;
		}
		Draft.Props = nextProps;
		Draft.Learns = nextLearns;
		return true;
	}
}


