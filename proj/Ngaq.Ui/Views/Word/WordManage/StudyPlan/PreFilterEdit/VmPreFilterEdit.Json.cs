namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using Ngaq.Core.Shared.StudyPlan.Models;
using Ngaq.Core.Shared.StudyPlan.Models.Po.PreFilter;

public partial class VmPreFilterEdit{
	public str PoPreFilterJson{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str PreFilterJson{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	void SyncJsonFromBo(){
		var po = BoPreFilter?.PoPreFilter ?? MkEmptyBoPreFilter().PoPreFilter;
		var pre = BoPreFilter?.PreFilter ?? MkEmptyBoPreFilter().PreFilter;
		PoPreFilterJson = FormatJson(JsonSerializer.Stringify(po));
		PreFilterJson = FormatJson(JsonSerializer.Stringify(pre));
	}

	bool TryBuildBoFromJson(
		out BoPreFilter Bo,
		out str Err
	){
		Bo = BoPreFilter ?? MkEmptyBoPreFilter();
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
			po.DataSchemaVer = pre.Version;
			po.Data = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Stringify(pre));
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
