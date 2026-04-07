namespace Ngaq.Ui.Views.Word.WordManage.SearchWords;

using System.Collections.ObjectModel;
using Avalonia.Threading;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Ui.Components.PageBar;
using Ngaq.Ui.Infra;
using Tsinswreng.CsPage;
using Tsinswreng.CsTools;
using Ctx = VmSearchWords;

public partial class VmSearchWords: ViewModelBase{
	void Init(){
		PageBar = VmPageBar.Mk();
		PageBar.PageSize = 10;
		PageBar.FnPrevPage = OnPrevPage;
		PageBar.FnNextPage = OnNextPage;
	}

	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmSearchWords(){
		Init();
	}
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
	IFrontendUserCtxMgr? IUserCtxMgr;
	public VmSearchWords(
		IFrontendUserCtxMgr? IUserCtxMgr
		,ISvcWord? SvcWord
	):this(){
		this.SvcWord = SvcWord;
		this.IUserCtxMgr = IUserCtxMgr;
	}

	public VmPageBar PageBar{get;set;} = null!;

	protected str _Input = "";
	public str Input{
		get{return _Input;}
		set{SetProperty(ref _Input, value);}
	}

	protected IList<ITypedObj> _GotWords = new List<ITypedObj>();
	public IList<ITypedObj> GotWords{
		get{return _GotWords;}
		set{SetProperty(ref _GotWords, value);}
	}

	public async Task<nil> InitSearchAsy(CT Ct){
		PageBar.PageNum = 1;
		return await SearchAsy(Ct);
	}

	protected async Task<nil> SearchAsy(CT Ct=default){
		await Task.Run(async()=>{
			if(SvcWord is null || IUserCtxMgr is null){
				return;
			}
			var UserCtx = IUserCtxMgr.GetUserCtx();
			if(UserCtx is null){
				return;
			}
			var pageQry = PageBar.ToPageQry();
			pageQry.WantTotCnt = true;
			var req = new ReqSearchWord{
				RawStr = Input,
			};
			var R = await SvcWord.PageSearch(UserCtx, pageQry, req, Ct);

			Dispatcher.UIThread.Post(()=>{
				GotWords = [];//勿刪
				GotWords = R.Data??[];
				PageBar.FromPageResultInfo(R);
			});
		});
		return NIL;
	}

	async Task<nil> OnPrevPage(VmPageBar PageBar, CT Ct){
		if(PageBar.PageNum <= 1){
			PageBar.PageNum = 1;
			return NIL;
		}
		PageBar.PageNum--;
		return await SearchAsy(Ct);
	}

	async Task<nil> OnNextPage(VmPageBar PageBar, CT Ct){
		if(PageBar.TotPageCnt is u64 TotalPage && TotalPage > 0 && PageBar.PageNum >= TotalPage){
			return NIL;
		}
		PageBar.PageNum++;
		return await SearchAsy(Ct);
	}
}
