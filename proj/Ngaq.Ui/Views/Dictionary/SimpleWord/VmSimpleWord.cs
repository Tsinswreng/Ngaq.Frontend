namespace Ngaq.Ui.Views.Dictionary.SimpleWord;
using System.Collections.ObjectModel;
using Ngaq.Core.Shared.Dictionary.Models;
using Ngaq.Core.Shared.Word.Models;
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
			o.Pronunciations = [Pronunciation.Sample.Samples[0]];
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


	public nil FromRespLlmDict(IRespLlmDict Resp){
		Head = Resp.Head;
		Pronunciations = Resp.Pronunciations.Select(p => new Pronunciation{
			TextType = p.TextType,
			Text = p.Text,
		}).ToList();
		Description = string.Join("\n", Resp.Descrs);
		return NIL;
	}

	/// <summary>
	/// 開始新的流式查詢，重置狀態
	/// </summary>
	public nil StartStreaming(string QueryTerm){
		Head = QueryTerm;
		Description = "";
		return NIL;
	}

	/// <summary>
	/// 接收流式響應的新片段
	/// </summary>
	public nil GotNewSeg(DtoOnNewSeg NewSeg){
		Description += NewSeg.NewSeg;
		return NIL;
	}



	public str Head{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	public IList<Pronunciation> Pronunciations{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=[];


	public str Description{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

}
