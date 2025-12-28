namespace Ngaq.Ui.Views.Dictionary.SimpleWord;
using System.Collections.ObjectModel;
using Ngaq.Core.Shared.Word.Models.DictionaryApi;
using Ngaq.Ui.Infra;

using Ctx = VmSimpleWord;
public partial class VmSimpleWord: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmSimpleWord(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmSimpleWord(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
			o.Head = "word";
			o.Pronunciation = "wɜːd";
			o.Description =
"""
n.	詞；單詞；字；消息
v.	措辭；用詞
int.	說得對
网絡	話；一個字；文字處理
""";
		}
		#endif
	}



	public str Head{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	//可有多個
	public str Pronunciation{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";


	public str Description{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";





}
