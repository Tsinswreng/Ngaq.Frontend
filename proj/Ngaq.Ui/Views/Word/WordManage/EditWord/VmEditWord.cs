namespace Ngaq.Ui.Views.Word.WordManage.EditWord;
using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Ngaq.Core.Domains.User.UserCtx;
using Ngaq.Core.Tools.Json;
using Ngaq.Core.Word.Models;
using Ngaq.Core.Word.Svc;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordManage.SearchWords.SearchedWordCard;
using Tsinswreng.CsTools;
using Ctx = VmEditWord;
public partial class VmEditWord: ViewModelBase{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmEditWord(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmEditWord(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	public VmEditWord(
		ISvcWord? SvcWord
		,IJsonSerializer? JsonSerializer
		,IUserCtxMgr? UserCtxMgr
	){
		this.SvcWord = SvcWord;
		this.JsonSerializer = JsonSerializer;
		this.UserCtxMgr = UserCtxMgr;

	}
	ISvcWord? SvcWord;
	IJsonSerializer? JsonSerializer;
	IUserCtxMgr? UserCtxMgr;
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
		JsonNode? node = JsonNode.Parse(uglyJson);
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

	public nil FromJnWord(JnWord JnWord){
		this.Bo = JnWord;
		ISimpleJnWord simple = JnWord;
		this.Json = JsonSerializer?.Stringify(simple)??"";
		Json = FormatJson(Json);
		return NIL;
	}


	protected JnWord? _Bo;
	public JnWord? Bo{
		get{return _Bo;}
		set{SetProperty(ref _Bo, value);}
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
			if(t.IsFaulted){
				System.Console.WriteLine(t.Exception);//t
			}
		});

		return NIL;
	}

/// <summary>
/// 注意: 改Props或Learns之內容旹 需刪原ʹ時間、否則diff不到
/// </summary>
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
			if(t.IsFaulted){
				System.Console.WriteLine(t.Exception);//t
			}
		});

		return NIL;
	}

}
