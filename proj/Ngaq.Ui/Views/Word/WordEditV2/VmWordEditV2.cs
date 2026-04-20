namespace Ngaq.Ui.Views.Word.WordEditV2;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.User.Models.Po;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Learn;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Core.Tools.Json;
using Ngaq.Ui.Infra;
using Tsinswreng.CsTools;
using JsonNode = System.Text.Json.Nodes.JsonNode;

using Ctx = VmWordEditV2;
using Ngaq.Core.Tools;
using Tsinswreng.CsTempus;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

public partial class VmWordEditV2: ViewModelBase, IMk<Ctx>{
	protected VmWordEditV2(){
		InitRowEvents();
	}
	public static Ctx Mk(){
		return new Ctx();
	}

	ISvcWordV2? SvcWordV2;
	IJsonSerializer? JsonSerializer;
	IFrontendUserCtxMgr? UserCtxMgr;

	public VmWordEditV2(
		ISvcWordV2? SvcWordV2
		,IJsonSerializer? JsonSerializer
		,IFrontendUserCtxMgr? UserCtxMgr
	){
		this.SvcWordV2 = SvcWordV2;
		this.JsonSerializer = JsonSerializer;
		this.UserCtxMgr = UserCtxMgr;
		InitRowEvents();
	}

	bool _isHydrating = false;

	public enum ETabIdx{
		Basic = 0,
		Props = 1,
		Learns = 2,
		Json = 3,
	}

	/// 儲存策略：普通編輯走按 BizId 同步；詞典入口走合併。
	public enum ESaveMode{
		DetailOps = 0,
		Merge = 1,
	}

	void InitRowEvents(){
		PropRows.CollectionChanged -= OnPropRowsChanged;
		LearnRows.CollectionChanged -= OnLearnRowsChanged;
		PropRows.CollectionChanged += OnPropRowsChanged;
		LearnRows.CollectionChanged += OnLearnRowsChanged;
		foreach(var item in PropRows){
			item.PropertyChanged -= OnChildRowChanged;
			item.PropertyChanged += OnChildRowChanged;
		}
		foreach(var item in LearnRows){
			item.PropertyChanged -= OnChildRowChanged;
			item.PropertyChanged += OnChildRowChanged;
		}
	}

	void OnPropRowsChanged(object? sender, NotifyCollectionChangedEventArgs e){
		if(e.NewItems is not null){
			foreach(var item in e.NewItems.OfType<VmWordPropRow>()){
				item.PropertyChanged += OnChildRowChanged;
			}
		}
		if(e.OldItems is not null){
			foreach(var item in e.OldItems.OfType<VmWordPropRow>()){
				item.PropertyChanged -= OnChildRowChanged;
			}
		}
		Touch();
	}

	void OnLearnRowsChanged(object? sender, NotifyCollectionChangedEventArgs e){
		if(e.NewItems is not null){
			foreach(var item in e.NewItems.OfType<VmWordLearnRow>()){
				item.PropertyChanged += OnChildRowChanged;
			}
		}
		if(e.OldItems is not null){
			foreach(var item in e.OldItems.OfType<VmWordLearnRow>()){
				item.PropertyChanged -= OnChildRowChanged;
			}
		}
		Touch();
	}

	void OnChildRowChanged(object? sender, PropertyChangedEventArgs e){
		Touch();
	}

	void Touch(){
		if(_isHydrating){
			return;
		}
		IsDirty = true;
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
		set{
			if(SetProperty(ref field, value)){
				Touch();
			}
		}
	} = (i32)ETabIdx.Basic;

	public str Head{
		get{return field;}
		set{if(SetProperty(ref field, value)){Touch();}}
	} = "";

	public str Lang{
		get{return field;}
		set{if(SetProperty(ref field, value)){Touch();}}
	} = "";

	public str StoredAtIso{
		get{return field;}
		set{if(SetProperty(ref field, value)){Touch();}}
	} = "";

	public str DelAtUnixMs{
		get{return field;}
		set{if(SetProperty(ref field, value)){Touch();}}
	} = "";

	/// 聚合根主鍵（唯讀顯示）。
	public str WordIdText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	/// 業務創建時間（可編輯）。
	public str BizCreatedAtIso{
		get{return field;}
		set{if(SetProperty(ref field, value)){Touch();}}
	} = "";

	/// 業務更新時間（可編輯）。
	public str BizUpdatedAtIso{
		get{return field;}
		set{if(SetProperty(ref field, value)){Touch();}}
	} = "";

	public ObservableCollection<VmWordPropRow> PropRows{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = [];

	public ObservableCollection<VmWordLearnRow> LearnRows{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = [];

	public str JsonText{
		get{return field;}
		set{if(SetProperty(ref field, value)){Touch();}}
	} = "";

	public str LastError{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public bool IsDirty{
		get{return field;}
		set{SetProperty(ref field, value);}
	}

	/// 保存策略。默認為普通編輯同步；由詞典入口顯式切到 Merge。
	public ESaveMode SaveMode{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = ESaveMode.DetailOps;

	public bool HasError => !str.IsNullOrWhiteSpace(LastError);

	public nil FromJnWord(IJnWord JnWord){
		Src = JnWord;
		Draft = JnWord.DeepClone().AsOrToJnWord();
		LoadFromDraft();
		return NIL;
	}

	void LoadFromDraft(){
		if(Draft is null){
			return;
		}
		_isHydrating = true;
		try{
			Head = Draft.Word.Head;
			Lang = Draft.Word.Lang;
			StoredAtIso = Draft.Word.StoredAt.ToIso();
			DelAtUnixMs = Draft.Word.DelAt.IsNullOrDefault() ? "" : (Draft.Word.DelAt.Value+"");
			WordIdText = Draft.Word.Id.ToString();
			BizCreatedAtIso = Draft.Word.BizCreatedAt.ToIso();
			BizUpdatedAtIso = Draft.Word.BizUpdatedAt.ToIso();
			PropRows = new ObservableCollection<VmWordPropRow>(
				Draft.Props.Select(x=>VmWordPropRow.FromPo(x))
			);
			LearnRows = new ObservableCollection<VmWordLearnRow>(
				Draft.Learns.Select(x=>VmWordLearnRow.FromPo(x))
			);
			InitRowEvents();
			SyncJsonFromDraft();
			LastError = "";
			IsDirty = false;
			OnPropertyChanged(nameof(HasError));
		}finally{
			_isHydrating = false;
		}
	}

	public nil AddPropRow(){
		PropRows.Add(VmWordPropRow.NewRow());
		return NIL;
	}

	public nil RemovePropRow(VmWordPropRow Row){
		PropRows.Remove(Row);
		return NIL;
	}

	public nil AddLearnRow(){
		LearnRows.Add(VmWordLearnRow.NewRow());
		return NIL;
	}

	public nil RemoveLearnRow(VmWordLearnRow Row){
		LearnRows.Remove(Row);
		return NIL;
	}

	public nil ResetFromSource(){
		if(Src is null){
			return NIL;
		}
		Draft = Src.DeepClone().AsOrToJnWord();
		LoadFromDraft();
		return NIL;
	}

	public nil SyncJsonFromDraft(){
		if(!TryApplyFormToDraft(out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			return NIL;
		}
		if(Draft is null){
			return NIL;
		}
		var json = JsonSerializer?.Stringify(Draft) ?? "";
		JsonText = FormatJson(json);
		LastError = "";
		OnPropertyChanged(nameof(HasError));
		IsDirty = false;
		return NIL;
	}

	public nil ApplyJsonToForm(){
		if(JsonSerializer is null){
			LastError = I18n[K.NoJsonSerializer];
			OnPropertyChanged(nameof(HasError));
			return NIL;
		}
		try{
			var neo = JsonSerializer.Parse<JnWord>(JsonText);
			if(neo is null){
				LastError = I18n[K.JsonParseFailed];
				OnPropertyChanged(nameof(HasError));
				return NIL;
			}
			Draft = neo;
			LoadFromDraft();
			IsDirty = true;
		}catch(Exception ex){
			LastError = ex.Message;
			OnPropertyChanged(nameof(HasError));
		}
		return NIL;
	}

	public async Task<nil> Save(CT Ct){
		if(SvcWordV2 is null || UserCtxMgr is null){
			return NIL;
		}
		if(!TryApplyFormToDraft(out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowDialog(err);
			return NIL;
		}
		if(Draft is null){
			ShowDialog(I18n[K.NoDraft]);
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
			OnPropertyChanged(nameof(HasError));
			IsDirty = false;
			ShowToast(I18n[K.Saved]);
		}catch(Exception ex){
			LastError = ex.Message;
			OnPropertyChanged(nameof(HasError));
			HandleErr(ex);
		}
		return NIL;
	}

	/// 詞典入口保存：按 (Head,Lang) 合併，避免重複加詞。
	async Task<nil> SaveByMerge(CT Ct){
		if(SvcWordV2 is null || UserCtxMgr is null || Draft is null){
			return NIL;
		}
		await SvcWordV2.MergeWord(
			UserCtxMgr.GetDbUserCtx(),
			ToolAsyE.ToAsyE([Draft]),
			Ct
		);
		return NIL;
	}

	/// 普通編輯保存：按聚合根/Prop/Learn 細分操作，不走 BatSync。
	async Task<nil> SaveByDetailOps(CT Ct){
		if(SvcWordV2 is null || UserCtxMgr is null || Draft is null){
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
		if(SvcWordV2 is null || Draft is null){
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
		var addProps = DraftWord.Props
			.Where(x=>x.Id.IsNullOrDefault())
			.Select(x=>{
				var neo = (PoWordProp)x.ShallowCloneSelf();
				neo.WordId = DraftWord.Word.Id;
				return neo;
			})
		;
		await SvcWordV2.BatAddWordProp(DbCtx, ToolAsyE.ToAsyE(addProps), Ct);

		var updProps = DraftWord.Props
			.Where(x=>!x.Id.IsNullOrDefault())
			.Select(x=>{
				var upd = (PoWordProp)x.ShallowCloneSelf();
				upd.WordId = DraftWord.Word.Id;
				return upd;
			})
		;
		await SvcWordV2.BatUpdWordProp(DbCtx, ToolAsyE.ToAsyE(updProps), Ct);

		if(SrcWord is null || HasMovedToOtherWord){
			return NIL;
		}
		var keepPropIds = DraftWord.Props
			.Where(x=>!x.Id.IsNullOrDefault())
			.Select(x=>x.Id)
			.ToHashSet()
		;
		var delPropIds = SrcWord.Props
			.Where(x=>!x.Id.IsNullOrDefault() && !keepPropIds.Contains(x.Id))
			.Select(x=>x.Id)
		;
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
		var addLearns = DraftWord.Learns
			.Where(x=>x.Id.IsNullOrDefault())
			.Select(x=>{
				var neo = (PoWordLearn)x.ShallowCloneSelf();
				neo.WordId = DraftWord.Word.Id;
				return neo;
			})
		;
		await SvcWordV2.BatAddWordLearn(DbCtx, ToolAsyE.ToAsyE(addLearns), Ct);

		var updLearns = DraftWord.Learns
			.Where(x=>!x.Id.IsNullOrDefault())
			.Select(x=>{
				var upd = (PoWordLearn)x.ShallowCloneSelf();
				upd.WordId = DraftWord.Word.Id;
				return upd;
			})
		;
		await SvcWordV2.BatUpdWordLearn(DbCtx, ToolAsyE.ToAsyE(updLearns), Ct);

		if(SrcWord is null || HasMovedToOtherWord){
			return NIL;
		}
		var keepLearnIds = DraftWord.Learns
			.Where(x=>!x.Id.IsNullOrDefault())
			.Select(x=>x.Id)
			.ToHashSet()
		;
		var delLearnIds = SrcWord.Learns
			.Where(x=>!x.Id.IsNullOrDefault() && !keepLearnIds.Contains(x.Id))
			.Select(x=>x.Id)
		;
		await SvcWordV2.DelWordLearnInId(DbCtx, ToolAsyE.ToAsyE(delLearnIds), Ct);
		return NIL;
	}
	public async Task<nil> Delete(CT Ct){
		if(SvcWordV2 is null || UserCtxMgr is null){
			return NIL;
		}
		if(Draft is null){
			ShowDialog(I18n[K.NoDraft]);
			return NIL;
		}
		try{
			await SvcWordV2.SoftDelJnWordInId(
				UserCtxMgr.GetDbUserCtx(),
				ToolAsyE.ToAsyE([Draft.Word.Id]),
				Ct
			);
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			IsDirty = false;
			ShowDialog(I18n[K.Deleted]);
			ViewNavi?.Back();
		}catch(Exception ex){
			LastError = ex.Message;
			OnPropertyChanged(nameof(HasError));
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
		if(str.IsNullOrWhiteSpace(Head)){
			Err = I18n[K.HeadIsRequired];
			return false;
		}
		if(str.IsNullOrWhiteSpace(Lang)){
			Err = I18n[K.LangIsRequired];
			return false;
		}

		Draft.Word.Head = Head.Trim();
		Draft.Word.Lang = Lang.Trim();

		try{
			Draft.Word.StoredAt = Tempus.FromIso(StoredAtIso.Trim());
		}catch{
			Err = I18n[K.StoredAtMustBeIsoTime];
			return false;
		}

		if(str.IsNullOrWhiteSpace(DelAtUnixMs)){
			Draft.Word.DelAt = default;
		}else{
			if(!i64.TryParse(DelAtUnixMs.Trim(), out var delMs)){
				Err = I18n[K.DelAtMustBeUnixMilliseconds];
				return false;
			}
			Draft.Word.DelAt = IdDel.FromUnixMs(delMs);
		}

		try{
			Draft.Word.BizCreatedAt = Tempus.FromIso(BizCreatedAtIso.Trim());
		}catch{
			Err = I18n[K.BizCreatedAtMustBeIsoTime];
			return false;
		}
		try{
			Draft.Word.BizUpdatedAt = Tempus.FromIso(BizUpdatedAtIso.Trim());
		}catch{
			Err = I18n[K.BizUpdatedAtMustBeIsoTime];
			return false;
		}

		var propErrs = new List<str>();
		var nextProps = new List<PoWordProp>();
		for(i32 i = 0; i < PropRows.Count; i++){
			var row = PropRows[i];
			if(!row.TryToPo(Draft.Word.Id, out var po, out var rowErr)){
				propErrs.Add(I18n.Get(K.Prop__Err__, i+1, rowErr));
				continue;
			}
			nextProps.Add(po);
		}
		if(propErrs.Count > 0){
			Err = str.Join("\n", propErrs);
			return false;
		}

		var learnErrs = new List<str>();
		var nextLearns = new List<PoWordLearn>();
		for(i32 i = 0; i < LearnRows.Count; i++){
			var row = LearnRows[i];
			if(!row.TryToPo(Draft.Word.Id, out var po, out var rowErr)){
				learnErrs.Add(I18n.Get(K.Learn__Err__, i+1, rowErr));
				continue;
			}
			nextLearns.Add(po);
		}
		if(learnErrs.Count > 0){
			Err = str.Join("\n", learnErrs);
			return false;
		}

		Draft.Props = nextProps;
		Draft.Learns = nextLearns;
		return true;
	}

	static str FormatJson(str UglyJson){
		if(str.IsNullOrWhiteSpace(UglyJson)){
			return "";
		}
		try{
			JsonNode? node = JsonNode.Parse(UglyJson);
			if(node is null){
				return UglyJson;
			}
			return node.ToJsonString(new JsonSerializerOptions{
				WriteIndented = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			});
		}catch{
			return UglyJson;
		}
	}
}






