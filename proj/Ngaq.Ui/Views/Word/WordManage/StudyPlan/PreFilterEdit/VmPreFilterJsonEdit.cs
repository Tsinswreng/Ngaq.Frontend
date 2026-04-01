namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Infra.IF;
using Ngaq.Core.Shared.StudyPlan.Models.Po.PreFilter;
using Ngaq.Core.Shared.StudyPlan.Svc;
using Ngaq.Core.Tools.Json;
using Ngaq.Ui.Infra;
using Tsinswreng.CsTools;

using Ctx = VmPreFilterJsonEdit;

/// <summary>
/// PoPreFilter JSON 專用編輯 ViewModel。
/// 與 GUI 編輯 ViewModel 分離，不做雙向同步。
/// </summary>
public class VmPreFilterJsonEdit: ViewModelBase, IMk<Ctx>{
	protected VmPreFilterJsonEdit(){}

	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmPreFilterJsonEdit(){
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

	public VmPreFilterJsonEdit(
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

	/// <summary>
	/// 唯一 JSON 編輯字段。
	/// </summary>
	public str PoPreFilterJson{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	/// <summary>
	/// 從已有 Po 實體初始化 JSON 文本。
	/// </summary>
	public nil FromPoPreFilter(PoPreFilter? PoPreFilter){
		var po = PoPreFilter ?? new PoPreFilter{
			Type = EPreFilterType.Json,
		};
		PoPreFilterJson = FormatJson(JsonSerializer.Stringify(po));
		LastError = "";
		OnPropertyChanged(nameof(HasError));
		return NIL;
	}

	/// <summary>
	/// 解析 JSON 並保存到後端。
	/// </summary>
	public async Task<nil> Save(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			ShowMsg("Service not ready");
			return NIL;
		}
		if(!TryParsePo(out var po, out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowMsg(err);
			return NIL;
		}

		try{
			var dbCtx = UserCtxMgr.GetDbUserCtx();
			if(po.Id == IdPreFilter.Zero){
				await SvcStudyPlan.BatAddPreFilter(dbCtx, ToolAsyE.ToAsyE([po]), Ct);
			}else{
				await SvcStudyPlan.BatUpdPreFilter(dbCtx, ToolAsyE.ToAsyE([po]), Ct);
			}
			PoPreFilterJson = FormatJson(JsonSerializer.Stringify(po));
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			ShowMsg("Saved");
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}

	/// <summary>
	/// 軟刪除當前 JSON 實體。
	/// </summary>
	public async Task<nil> Delete(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			ShowMsg("Service not ready");
			return NIL;
		}
		if(!TryParsePo(out var po, out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowMsg(err);
			return NIL;
		}
		if(po.Id == IdPreFilter.Zero){
			PoPreFilterJson = "";
			ShowMsg("No persisted Id to delete");
			return NIL;
		}

		try{
			await SvcStudyPlan.BatSoftDelPreFilter(UserCtxMgr.GetDbUserCtx(), ToolAsyE.ToAsyE([po]), Ct);
			PoPreFilterJson = "";
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			ShowMsg("Deleted");
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}

	bool TryParsePo(out PoPreFilter Po, out str Err){
		Po = new PoPreFilter();
		Err = "";
		try{
			var parsed = JsonSerializer.Parse<PoPreFilter>(PoPreFilterJson);
			if(parsed is null){
				Err = "PoPreFilter JSON parse failed";
				return false;
			}
			Po = parsed;
			return true;
		}catch(Exception e){
			Err = e.Message;
			return false;
		}
	}

	static str FormatJson(str UglyJson){
		if(str.IsNullOrWhiteSpace(UglyJson)){
			return "";
		}
		try{
			var node = System.Text.Json.Nodes.JsonNode.Parse(UglyJson);
			return node?.ToJsonString(new System.Text.Json.JsonSerializerOptions{
				WriteIndented = true,
				Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			}) ?? UglyJson;
		}catch{
			return UglyJson;
		}
	}
}
