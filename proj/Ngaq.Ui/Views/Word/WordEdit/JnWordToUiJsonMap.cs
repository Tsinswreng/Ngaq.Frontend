using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Tools.JsonMap;
using Tsinswreng.CsTools;

namespace Ngaq.Ui.Views.Word.WordEdit;
using W = Ngaq.Core.Shared.Word.Models.Po.Word.PoWord;
public class JnWordToUiJsonMap{
	public static IUiJsonMap MkPoWord(IDictionary<str, obj?> Dict){
		var PathToUiMap = new Dictionary<str, IUiJsonMapItem>();
		var R = new UiJsonMap(){
			PathToUiMap = PathToUiMap,
			Raw = new JsonNode(Dict),
		};
		foreach(var (k,v) in Dict){
			PathToUiMap[k] = new UiJsonMapItem(R, k){
				DisplayName = UiT(k),
				Descr = UiT(""),
			};
		}

		return R;
	}

	public static UiText UiT(str Raw){
		return UiText.FromRawText(Raw);
	}

}
