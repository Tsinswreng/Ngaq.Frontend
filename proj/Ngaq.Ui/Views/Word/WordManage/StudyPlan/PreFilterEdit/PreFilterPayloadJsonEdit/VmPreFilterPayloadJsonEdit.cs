namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.PreFilterPayloadJsonEdit;

using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Infra.IF;
using Ngaq.Core.Shared.StudyPlan.Models.Po.PreFilter;
using Ngaq.Core.Shared.StudyPlan.Svc;
using Ngaq.Core.Tools.Json;
using Ngaq.Ui.Infra;
using Tsinswreng.CsTools;

using Ctx = VmPreFilterPayloadJsonEdit;
using PreFilterModel = Ngaq.Core.Shared.StudyPlan.Models.PreFilter.PreFilter;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;
using JsonNode = System.Text.Json.Nodes.JsonNode;


/// PreFilter 載荷 JSON 子頁 ViewModel。
/// 只編輯 <see cref="PoPreFilter.Text"/>，保存/刪除直接調後端。
public class VmPreFilterPayloadJsonEdit: ViewModelBase, IMk<Ctx>{
	protected VmPreFilterPayloadJsonEdit(){}

	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmPreFilterPayloadJsonEdit(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	ISvcStudyPlan? SvcStudyPlan{get;set;}
	IFrontendUserCtxMgr? UserCtxMgr{get;set;}
	IJsonSerializer JsonSerializer{get;set;} = AppJsonSerializer.Inst;


	/// 當保存或刪除完成後，把最新實體回寫給父頁；刪除時回傳 null。

	Action<PoPreFilter?>? OnSavedOrDeleted{get;set;}


	/// 當前正在編輯的 PreFilter 實體快照。

	PoPreFilter EditingPo{get;set;} = new PoPreFilter{
		Type = EPreFilterType.Json,
	};

	public VmPreFilterPayloadJsonEdit(
		ISvcStudyPlan? SvcStudyPlan
		,IFrontendUserCtxMgr? UserCtxMgr
		,IJsonSerializer? JsonSerializer
	){
		this.SvcStudyPlan = SvcStudyPlan;
		this.UserCtxMgr = UserCtxMgr;
		this.JsonSerializer = JsonSerializer ?? AppJsonSerializer.Inst;
	}

	public str LastError{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public bool HasError => !str.IsNullOrWhiteSpace(LastError);


	/// 供文本編輯器雙向同步的載荷 JSON 文本。

	public str PayloadJson{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";


	/// 由父頁傳入當前實體與回寫回調。

	public nil Load(PoPreFilter? PoPreFilter, Action<PoPreFilter?>? OnSavedOrDeleted){
		EditingPo = ClonePoPreFilter(PoPreFilter);
		this.OnSavedOrDeleted = OnSavedOrDeleted;
		PayloadJson = FormatJson(EditingPo.Text ?? "");
		LastError = "";
		OnPropertyChanged(nameof(HasError));
		return NIL;
	}


	/// 驗證載荷 JSON 並保存到後端。

	public async Task<nil> Save(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			return NIL;
		}
		if(!TryBuildPo(out var po, out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowDialog(err);
			return NIL;
		}

		try{
			var dbCtx = UserCtxMgr.GetDbUserCtx();
			if(po.Id == IdPreFilter.Zero){
				await SvcStudyPlan.BatAddPreFilter(dbCtx, ToolAsyE.ToAsyE([po]), Ct);
			}else{
				await SvcStudyPlan.BatUpdPreFilter(dbCtx, ToolAsyE.ToAsyE([po]), Ct);
			}
			EditingPo = po;
			PayloadJson = FormatJson(po.Text ?? "");
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			OnSavedOrDeleted?.Invoke(po);
			ShowToast(I18n[K.Saved]);
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}


	/// 軟刪除當前 PreFilter；新增模式下只提示不可刪。

	public async Task<nil> Delete(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			return NIL;
		}
		if(EditingPo.Id == IdPreFilter.Zero){
			ShowDialog(I18n[K.NoPersistedIdToDelete]);
			return NIL;
		}

		try{
			await SvcStudyPlan.BatSoftDelPreFilter(UserCtxMgr.GetDbUserCtx(), ToolAsyE.ToAsyE([EditingPo]), Ct);
			EditingPo = new PoPreFilter{
				Type = EPreFilterType.Json,
			};
			PayloadJson = "";
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			OnSavedOrDeleted?.Invoke(null);
			ShowToast(I18n[K.Deleted]);
			ViewNavi?.Back();
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}


	/// 解析載荷 JSON，生成待保存的實體。

	bool TryBuildPo(out PoPreFilter Po, out str Err){
		Po = ClonePoPreFilter(EditingPo);
		Err = "";
		try{
			var parsed = JsonSerializer.Parse<PreFilterModel>(PayloadJson);
			if(parsed is null){
				Err = I18n[K.JsonParseFailed];
				return false;
			}
			Po.Text = FormatJson(JsonSerializer.Stringify(parsed));
			Po.DataSchemaVer = parsed.Version ?? new Version(1, 0, 0, 0);
			Po.Type = EPreFilterType.Json;
			Po.Binary = null;
			return true;
		}catch(Exception e){
			Err = e.Message;
			return false;
		}
	}


	/// 格式化 JSON 供編輯器展示；失敗時返回原文。

	static str FormatJson(str UglyJson){
		if(str.IsNullOrWhiteSpace(UglyJson)){
			return "";
		}
		try{
			var node = JsonNode.Parse(UglyJson);
			return node?.ToJsonString(new JsonSerializerOptions{
				WriteIndented = true,
				Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			}) ?? UglyJson;
		}catch{
			return UglyJson;
		}
	}


	/// 複製 Po，避免子頁直接修改父頁持有的引用。

	static PoPreFilter ClonePoPreFilter(PoPreFilter? Src){
		Src ??= new PoPreFilter{
			Type = EPreFilterType.Json,
		};
		return new PoPreFilter{
			DbCreatedAt = Src.DbCreatedAt,
			DbUpdatedAt = Src.DbUpdatedAt,
			DelAt = Src.DelAt,
			BizCreatedAt = Src.BizCreatedAt,
			BizUpdatedAt = Src.BizUpdatedAt,
			Id = Src.Id,
			Owner = Src.Owner,
			UniqName = Src.UniqName,
			Descr = Src.Descr,
			Type = Src.Type,
			DataSchemaVer = Src.DataSchemaVer,
			Binary = Src.Binary?.ToArray() ?? [],
			Text = Src.Text,
		};
	}
}
