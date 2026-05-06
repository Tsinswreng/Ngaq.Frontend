namespace Ngaq.Ui.Views.Word.WordEditV2;

using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.Word.Models;
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
		get{return field;}
		set{SetProperty(ref field, value);}
	}

	public JnWord? Draft{
		get{return field;}
		set{SetProperty(ref field, value);}
	}

	public i32 TabIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	}

	public VmPoWordEdit PoWordEdit{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = new();

	public VmWordPropPage WordPropPage{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = new();

	public VmWordLearnPage WordLearnPage{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = new();

	public str LastError{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				OnPropertyChanged(nameof(HasError));
			}
		}
	} = "";

	public bool HasError => !str.IsNullOrWhiteSpace(LastError);

	public bool IsDirty{
		get{return field;}
		set{SetProperty(ref field, value);}
	}

	public ESaveMode SaveMode{
		get{return field;}
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
		var srcWord = Src?.AsOrToJnWord();
		var oldId = Draft.Word.Id;
		var finalId = await UpdRootAndGetFinalId(dbCtx, Ct);
		var hasMovedToOtherWord = oldId != finalId;
		if(hasMovedToOtherWord){
			Draft.SetIdEtEnsureFKey(finalId);
		}

		await SavePropsByDiff(dbCtx, srcWord, Draft, hasMovedToOtherWord, Ct);
		await SaveLearnsByDiff(dbCtx, srcWord, Draft, hasMovedToOtherWord, Ct);
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

	async Task<nil> SavePropsByDiff(
		IDbUserCtx DbCtx,
		IJnWord? SrcWord,
		JnWord DraftWord,
		bool HasMovedToOtherWord,
		CT Ct
	){
		if(SvcWordV2 is null){
			return NIL;
		}
		var addProps = DraftWord.Props.Where(x=>x.Id.IsNullOrDefault()).Select(x=>{
			var neo = (PoWordProp)x.ShallowCloneSelf();
			neo.WordId = DraftWord.Word.Id;
			return neo;
		});
		await SvcWordV2.BatAddWordProp(DbCtx, ToolAsyE.ToAsyE(addProps), Ct);

		var updProps = DraftWord.Props.Where(x=>!x.Id.IsNullOrDefault()).Select(x=>{
			var upd = (PoWordProp)x.ShallowCloneSelf();
			upd.WordId = DraftWord.Word.Id;
			return upd;
		});
		await SvcWordV2.BatUpdWordProp(DbCtx, ToolAsyE.ToAsyE(updProps), Ct);

		if(SrcWord is null || HasMovedToOtherWord){
			return NIL;
		}
		var keepPropIds = DraftWord.Props
			.Where(x=>!x.Id.IsNullOrDefault())
			.Select(x=>x.Id)
			.ToHashSet();
		var delPropIds = SrcWord.Props
			.Where(x=>!x.Id.IsNullOrDefault() && !keepPropIds.Contains(x.Id))
			.Select(x=>x.Id);
		await SvcWordV2.DelWordPropInId(DbCtx, ToolAsyE.ToAsyE(delPropIds), Ct);
		return NIL;
	}

	async Task<nil> SaveLearnsByDiff(
		IDbUserCtx DbCtx,
		IJnWord? SrcWord,
		JnWord DraftWord,
		bool HasMovedToOtherWord,
		CT Ct
	){
		if(SvcWordV2 is null){
			return NIL;
		}
		var addLearns = DraftWord.Learns.Where(x=>x.Id.IsNullOrDefault()).Select(x=>{
			var neo = (PoWordLearn)x.ShallowCloneSelf();
			neo.WordId = DraftWord.Word.Id;
			return neo;
		});
		await SvcWordV2.BatAddWordLearn(DbCtx, ToolAsyE.ToAsyE(addLearns), Ct);

		var updLearns = DraftWord.Learns.Where(x=>!x.Id.IsNullOrDefault()).Select(x=>{
			var upd = (PoWordLearn)x.ShallowCloneSelf();
			upd.WordId = DraftWord.Word.Id;
			return upd;
		});
		await SvcWordV2.BatUpdWordLearn(DbCtx, ToolAsyE.ToAsyE(updLearns), Ct);

		if(SrcWord is null || HasMovedToOtherWord){
			return NIL;
		}
		var keepLearnIds = DraftWord.Learns
			.Where(x=>!x.Id.IsNullOrDefault())
			.Select(x=>x.Id)
			.ToHashSet();
		var delLearnIds = SrcWord.Learns
			.Where(x=>!x.Id.IsNullOrDefault() && !keepLearnIds.Contains(x.Id))
			.Select(x=>x.Id);
		await SvcWordV2.DelWordLearnInId(DbCtx, ToolAsyE.ToAsyE(delLearnIds), Ct);
		return NIL;
	}

	public async Task<nil> Delete(CT Ct){
		if(AnyNull(SvcWordV2, UserCtxMgr, Draft)){
			return NIL;
		}
		try{
			await SvcWordV2.SoftDelJnWordInId(
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


