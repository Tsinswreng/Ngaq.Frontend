namespace Ngaq.Ui.Views.Word.WordManage.SearchWords;
using System.Collections.ObjectModel;
using System.Security.AccessControl;
using Ngaq.Core.Model.UserCtx;
using Ngaq.Core.Word.Models;
using Ngaq.Core.Word.Models.Dto;
using Ngaq.Core.Word.Svc;
using Ngaq.Ui.Infra;
using Tsinswreng.CsPage;
using Ctx = VmSearchWords;
public partial class VmSearchWords: ViewModelBase{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmSearchWords(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmSearchWords(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}
	ISvcWord? SvcWord;
	IUserCtxMgr? IUserCtxMgr;
	public VmSearchWords(
		IUserCtxMgr? IUserCtxMgr
		,ISvcWord? SvcWord
	){
		this.SvcWord = SvcWord;
		this.IUserCtxMgr = IUserCtxMgr;
	}

	protected str _Input = "";
	public str Input{
		get{return _Input;}
		set{SetProperty(ref _Input, value);}
	}

	protected u64 _PageIdx = 0;
	public u64 PageIdx{
		get{return _PageIdx;}
		set{SetProperty(ref _PageIdx, value);}
	}


	protected IList<JnWord> _GotWords = new List<JnWord>();
	public IList<JnWord> GotWords{
		get{return _GotWords;}
		set{SetProperty(ref _GotWords, value);}
	}

	CancellationTokenSource Cts = new();
	public nil InitSearch(){
		PageIdx = 0;
		return Search();
	}
	protected nil Search(){
		if(SvcWord is null || IUserCtxMgr is null){
			return NIL;
		}
		var UserCtx = IUserCtxMgr.GetUserCtx();
		var Ct = Cts.Token;
		var pageQry = new PageQry{
			PageSize = 10
			,PageIdx = PageIdx
		};
		var req = new ReqSearchWord{
			RawStr = Input
		};
		if(UserCtx == null){
			return NIL;
		}
		SvcWord.SearchWord(UserCtx, pageQry, req, Ct).ContinueWith(t=>{
			if(t.IsFaulted){
				System.Console.WriteLine(t.Exception);//t
				return;
			}
			var R = t.Result;
//虛列表渲染旹 若項數未變則
// R.ItemTemplate = new FuncDataTemplate<JnWord>((jnWord, b)=>{} 中 jnWord 皆潙null
			GotWords = [];//勿刪
			GotWords = R.Data??[];
		});
		return NIL;
	}

	public nil NextPage(){
		PageIdx++;
		return Search();
	}

	public nil PrevPage(){
		PageIdx--;
		return Search();
	}


}
