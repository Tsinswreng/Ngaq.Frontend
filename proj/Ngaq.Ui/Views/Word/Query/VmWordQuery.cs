using System.Collections.ObjectModel;
using Ngaq.Core.Infra.Page;
using Ngaq.Core.Model.Bo;
using Ngaq.Core.Model.UserCtx;
using Ngaq.Core.Service.Word;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordInfo;

namespace Ngaq.Ui.Views.Word.Query;
using Ctx = VmWordQuery;
public partial class VmWordQuery
	:ViewModelBase
{

	public VmWordQuery(){}

	public VmWordQuery(
		ISvcWord SvcWord
		,IUserCtxMgr? UserCtxMgr
	){
		this.SvcWord = SvcWord;
		this.UserCtxMgr = UserCtxMgr;
	}

	public ISvcWord? SvcWord;
	public IUserCtxMgr? UserCtxMgr;


	public static ObservableCollection<Ctx> Samples = [];
	static VmWordQuery(){
		{
			var o = new Ctx();
			Samples.Add(o);
			for(var i = 0; i < 20; i++){
				o.WordCards.Add(VmWordListCard.Samples[0]);
				o.WordCards.Add(VmWordListCard.Samples[1]);
			}
		}
	}

	protected ObservableCollection<VmWordListCard> _WordCards = new();
	public ObservableCollection<VmWordListCard> WordCards{
		get{return _WordCards;}
		set{SetProperty(ref _WordCards, value);}
	}

	protected VmWordInfo _VmWordInfo = new();
	public VmWordInfo CurWordInfo{
		get{return _VmWordInfo;}
		set{SetProperty(ref _VmWordInfo, value);}
	}

	public nil SetCurBoWord(BoWord BoWord){
		CurWordInfo.FromBo(BoWord);
		// if(CurWordInfo != null){
		// 	CurWordInfo.FromBo(BoWord);
		// }
		// CurWordInfo = new();
		// CurWordInfo.FromBo(BoWord);
		return Nil;
	}

	public nil GetAllWords(){
		CancellationToken Ct = default;
		SvcWord?.PageBoWord(
			UserCtxMgr?.GetUserCtx()
			,PageQuery.SelectAll()
			,Ct
		).ContinueWith(d=>{
			if(d.IsFaulted){
				System.Console.WriteLine(d.Exception.ToString());//t
				AddMsg(d.Exception.ToString());
				ShowMsg();
			}else{
				WordCards.Clear();
				var Page = d.Result;
				var BoWords = Page.DataAsy?.ToBlockingEnumerable(Ct)??[];
				foreach(var BoWord in BoWords){
					WordCards.Add(new VmWordListCard().FromBo(BoWord));
				}
			}
		});
		return Nil;
	}


}
