using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Tools.JsonMap;
using Tsinswreng.CsTools;

namespace Ngaq.Ui.Views.Word.WordEdit;
using W = Ngaq.Core.Shared.Word.Models.Po.Word.PoWord;
public class JnWordToUiJsonMap{
	public IJnWord JnWord{get;set;}
	public IJsonNode Raw{get;set;}
	public UiJsonMap UiJsonMap{get;set;}
	public void mkPoWord(IJsonNode Raw){
		var R = new UiJsonMap();
		R.Raw = Raw;
		Todo.I18n();
		R.PathToUiMap = new Dictionary<str, IUiJsonMapItem>(){
			[nameof(W.Id)] = new UiJsonMapItem{
				DisplayName = UiT("Id"),
				Descr = UiT("Unique identifier of the word."),
			},
			[nameof(W.Head)] = new UiJsonMapItem{
				DisplayName = UiT("Head"),
				Descr = UiT("(Head ,Language) also uniquely identifies a word of the same owner."),
			},
			[nameof(W.Lang)] = new UiJsonMapItem{
				DisplayName = UiT("Language"),
				Descr = UiT("Language identifier of the word."),
			},
			[nameof(W.Owner)] = new UiJsonMapItem{
				DisplayName = UiT("Owner"),
				Descr = UiT("Owner of the word."),
			},
			[nameof(W.StoredAt)] = new UiJsonMapItem{
				DisplayName = UiT("Stored At"),
				Descr = UiT("The time when the word was first stored in database."),
			},
		};
	}
	public static UiText UiT(str Raw){
		return UiText.FromRawText(Raw);
	}

}
