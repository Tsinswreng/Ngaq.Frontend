namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;
using System.Collections.ObjectModel;
using Ngaq.Core.Shared.StudyPlan.Models;
using Ngaq.Core.Shared.StudyPlan.Models.Po.PreFilter;
using Ngaq.Core.Tools.Json;
using Ngaq.Ui.Infra;

using Ctx = VmPreFilterEdit;
public partial class VmPreFilterEdit: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmPreFilterEdit(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmPreFilterEdit(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}


	IJsonSerializer JsonSerializer {get;set;} = AppJsonSerializer.Inst;

	public VmPreFilterEdit(
		IJsonSerializer? JsonSerializer
	){
		this.JsonSerializer = JsonSerializer ?? AppJsonSerializer.Inst;
		BoPreFilter = MkEmptyBoPreFilter();
		SyncJsonFromBo();
	}

	public BoPreFilter BoPreFilter{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = MkEmptyBoPreFilter();

	public i32 TabIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=0;

	public str PoPreFilterJson{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str PreFilterJson{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str LastError{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public nil FromPoPreFilter(PoPreFilter? PoPreFilter){
		var bo = MkEmptyBoPreFilter();
		if(PoPreFilter is not null){
			bo.FromPoPreFilter(PoPreFilter);
		}
		return FromBoPreFilter(bo);
	}

	public nil FromBoPreFilter(BoPreFilter? BoPreFilter){
		this.BoPreFilter = BoPreFilter ?? MkEmptyBoPreFilter();
		SyncJsonFromBo();
		return NIL;
	}

	public BoPreFilter ToBoPreFilter(){
		if(!TryBuildBoFromJson(out var bo, out var err)){
			LastError = err;
			return this.BoPreFilter;
		}
		LastError = "";
		this.BoPreFilter = bo;
		return bo;
	}

	public async Task<nil> Save(CT Ct = default){
		await Task.Yield();
		if(!TryBuildBoFromJson(out var bo, out var err)){
			LastError = err;
			ShowMsg(err);
			return NIL;
		}
		LastError = "";
		this.BoPreFilter = bo;
		ShowMsg("Saved");
		return NIL;
	}

	public async Task<nil> Delete(CT Ct = default){
		await Task.Yield();
		LastError = "";
		this.BoPreFilter = MkEmptyBoPreFilter();
		SyncJsonFromBo();
		ShowMsg("Deleted");
		return NIL;
	}

	protected static BoPreFilter MkEmptyBoPreFilter(){
		return new BoPreFilter{
			PoPreFilter = new PoPreFilter{
				Type = EPreFilterType.Json,
			},
			PreFilter = new Ngaq.Core.Shared.StudyPlan.Models.PreFilter.PreFilter(),
		};
	}

	protected nil SyncJsonFromBo(){
		var po = BoPreFilter?.PoPreFilter ?? MkEmptyBoPreFilter().PoPreFilter;
		var pre = BoPreFilter?.PreFilter ?? MkEmptyBoPreFilter().PreFilter;
		PoPreFilterJson = FormatJson(JsonSerializer.Stringify(po));
		PreFilterJson = FormatJson(JsonSerializer.Stringify(pre));
		return NIL;
	}

	protected bool TryBuildBoFromJson(
		out BoPreFilter Bo
		,out str Err
	){
		Bo = this.BoPreFilter ?? MkEmptyBoPreFilter();
		Err = "";
		try{
			var po = JsonSerializer.Parse<PoPreFilter>(PoPreFilterJson);
			if(po is null){
				Err = "PoPreFilter JSON parse failed";
				return false;
			}
			var pre = JsonSerializer.Parse<Ngaq.Core.Shared.StudyPlan.Models.PreFilter.PreFilter>(PreFilterJson);
			if(pre is null){
				Err = "PreFilter JSON parse failed";
				return false;
			}
			Bo = new BoPreFilter{
				PoPreFilter = po,
				PreFilter = pre,
			};
			return true;
		}catch(Exception e){
			Err = e.Message;
			return false;
		}
	}

	protected static str FormatJson(str UglyJson){
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

	public str YYY{
		get;
		set{SetProperty(ref field, value);}
	}="";


}
