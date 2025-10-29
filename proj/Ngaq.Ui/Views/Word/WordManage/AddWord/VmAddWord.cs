using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MethodTimer;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Shared.User.UserCtx;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Tools;
using Ngaq.Core.Word.Svc;
using Ngaq.Ui.Infra;



namespace Ngaq.Ui.Views.Word.WordManage.AddWord;
using Ctx = VmAddWord;
public partial class VmAddWord
	:ViewModelBase
{

	public VmAddWord(){

	}

	public VmAddWord(
		ISvcWord? SvcWord = null
		,IFrontendUserCtxMgr? UserCtxMgr = null
	){
		this.SvcWord = SvcWord!;
		this.UserCtxMgr = UserCtxMgr!;
	}

	ISvcWord SvcWord{get;set;} = null!;
	IFrontendUserCtxMgr UserCtxMgr{get;set;} = null!;

	public static ObservableCollection<Ctx> Samples = [];
	static VmAddWord(){
		{
			var o = new Ctx();
			Samples.Add(o);

		}
	}

	protected str _WordTxtPath = "";
	public str WordTxtPath{
		get{return _WordTxtPath;}
		set{SetProperty(ref _WordTxtPath, value);}
	}


	protected str _WordJsonsPath = "";
	public str WordJsonsPath{
		get{return _WordJsonsPath;}
		set{SetProperty(ref _WordJsonsPath, value);}
	}

	protected str _Text = "";
	public str Text{
		get{return _Text;}
		set{SetProperty(ref _Text, value);}
	}

	protected str _Json = "";
	public str Json{
		get{return _Json;}
		set{SetProperty(ref _Json, value);}
	}

	protected i32 _TabIndex = 0;
	public i32 TabIndex{
		get{return _TabIndex;}
		set{SetProperty(ref _TabIndex, value);}
	}

	public enum ETabIdx{
		TxtPath = 0,
		JsonsPath=1,
		Text = 2,
		Json = 3,
	}




	protected str _ErrStr="";//t
	public str ErrStr{
		get{return _ErrStr;}
		set{SetProperty(ref _ErrStr, value);}
	}

	CancellationTokenSource Cts = new();

	[Time]
	public async Task<nil> Confirm(){
		var Ct = Cts.Token;
		var UserCtx = UserCtxMgr.GetUserCtx();
		var FnThen = (Task<nil> t)=>{
			if(t.IsFaulted){
				Console.WriteLine(t.Exception);//t
				//this.OnMsg(d.Exception.ToString());
				//ErrStr = d.Exception.ToString();
				ShowMsg();
			}
		};

		if(
			TabIndex == (int)ETabIdx.Text
			&& !str.IsNullOrEmpty(Text)
		){
			SvcWord?.AddWordsFromText(
				UserCtx,Text,Ct
			).ContinueWith(FnThen);
		}else if(
			TabIndex == (int)ETabIdx.Json
			&& !str.IsNullOrEmpty(Json)
		){
			var JnWords = JSON.parse<IList<JnWord>>(Json);// 測AOT兼容
			if(JnWords == null){
				ErrStr = "Json parse error.";
				ShowMsg();
				return NIL;
			}
			SvcWord?.AddEtMergeWords(
				UserCtx,JnWords,Ct
			).ContinueWith(FnThen);
		}else if(
			TabIndex == (int)ETabIdx.JsonsPath
			&& !str.IsNullOrEmpty(WordJsonsPath)
		){
			var Enumr = ReadLinesAsync(WordJsonsPath, Ct);
			Task.Run(()=>{
				SvcWord?.AddWordsByJsonLineIter(UserCtx, Enumr, Ct).ContinueWith(FnThen);
			});
		}
		return NIL;
	}

	public static async IAsyncEnumerable<string> ReadLinesAsync(
		string path,
		[EnumeratorCancellation] CancellationToken Ct = default
	){
		using var sr = new StreamReader(path);   // 默认 UTF-8，可带编码参数
		string? line;
		while ((line = await sr.ReadLineAsync().ConfigureAwait(false)) != null){
			Ct.ThrowIfCancellationRequested();
			yield return line;
		}
	}
}

