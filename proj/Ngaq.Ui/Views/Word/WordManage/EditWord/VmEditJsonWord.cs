namespace Ngaq.Ui.Views.Word.WordManage.EditWord;

using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Shared.User.UserCtx;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Core.Tools.Json;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordManage.SearchWords.SearchedWordCard;
using Tsinswreng.CsTools;
using Ctx = VmEditJsonWord;
public partial class VmEditJsonWord: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmEditJsonWord(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmEditJsonWord(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	public VmEditJsonWord(
		ISvcWord? SvcWord
		,IJsonSerializer? JsonSerializer
		,IFrontendUserCtxMgr? UserCtxMgr
	){
		this.SvcWord = SvcWord;
		this.JsonSerializer = JsonSerializer;
		this.UserCtxMgr = UserCtxMgr;

	}
	ISvcWord? SvcWord;
	IJsonSerializer? JsonSerializer;
	IFrontendUserCtxMgr? UserCtxMgr;
	CancellationTokenSource Cts = new();

	static str FormatJson(string uglyJson){
		// // 1. 把字串讀進 JSON DOM
		// using JsonDocument doc = JsonDocument.Parse(uglyJson);

		// // 2. 再寫出來，打開 WriteIndented
		// var options = new JsonSerializerOptions{
		// 	WriteIndented = true
		// };

		// string pretty = System.Text.Json.JsonSerializer.Serialize(doc.RootElement, options);
		// return pretty;
		System.Text.Json.Nodes.JsonNode? node = System.Text.Json.Nodes.JsonNode.Parse(uglyJson);
		string pretty = node!.ToJsonString(new JsonSerializerOptions {
			WriteIndented = true
			,Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 允許原樣輸出
		});
		return pretty;
	}

	public nil FromTypedObj(ITypedObj Obj){
		var JnWord = VmSearchedWordCard.GetJnWordFromTypedObj(Obj);
		return FromJnWord(JnWord);
	}

	public nil FromJnWord(IJnWord JnWord){
		this.Bo = JnWord;
		IJnWord simple = JnWord;
		this.Json = JsonSerializer?.Stringify(simple)??"";
		Json = FormatJson(Json);
		return NIL;
	}


	public IJnWord? Bo{
		get{return field;}
		set{SetProperty(ref field, value);}
	}


	protected str _Json = "";
	public str Json{
		get{return _Json;}
		set{SetProperty(ref _Json, value);}
	}
	public nil Deserialize(){
		if(JsonSerializer is null){
			return NIL;
		}
		try{
			Bo = JsonSerializer.Parse<JnWord>(Json);
		}catch (System.Exception e){
			System.Console.WriteLine(e);//t
		}
		return NIL;
	}


	public nil Delete(){
		if(SvcWord is null
			|| UserCtxMgr is null
		){
			return NIL;
		}
		Deserialize();
		if(Bo is null){
			return NIL;
		}
		var User = UserCtxMgr.GetUserCtx();
		var Ct = Cts.Token;
		SvcWord.SoftDelJnWordsByIds(User, [Bo.Id], Ct).ContinueWith(t=>{
			HandleErr(t);
		});

		return NIL;
	}


/// 注意: 改Props或Learns之內容旹 需刪原ʹ時間、否則diff不到

/// <returns></returns>
	public nil Save(){
		if(SvcWord is null || UserCtxMgr is null){
			return NIL;
		}
		Deserialize();
		if(Bo is null){
			return NIL;
		}
		var User = UserCtxMgr.GetUserCtx();
		var Ct = Cts.Token;
		SvcWord.UpdJnWord(User, Bo, Ct).ContinueWith(t=>{
			HandleErr(t);
		});

		return NIL;
	}

}
