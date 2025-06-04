using System.Collections.ObjectModel;
using Avalonia.Media;
using Ngaq.Core.Infra.Page;
using Ngaq.Core.Model.Bo;
using Ngaq.Core.Model.UserCtx;
using Ngaq.Core.Service.Word;
using Ngaq.Core.Service.Word.Learn_.Models;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordInfo;

namespace Ngaq.Ui.Views.Word.Query;
using Ctx = VmWordQuery;

public partial class VmWordQuery
	:ViewModelBase
{

	public class Cfg_{
		/// <summary>
		/// 單詞條長按
		/// </summary>
		public i64 LongPressDurationMs = 100;
		public IBrush ColorRmb = new SolidColorBrush(Color.FromArgb((u8)(0.8*0xff), 0, 80, 0));
		public IBrush ColorFgt = new SolidColorBrush(Color.FromArgb((u8)(0.8*0xff), 80, 0, 0));
	}

	public Cfg_ Cfg = new();

	public VmWordQuery(){
		_Init();
	}

	public VmWordQuery(
		ISvcWord SvcWord
		,IUserCtxMgr? UserCtxMgr
	){
		this.SvcWord = SvcWord;
		this.UserCtxMgr = UserCtxMgr;
		_Init();
	}

	public ISvcWord? SvcWord;
	public IUserCtxMgr? UserCtxMgr;

	public LearnMgr LearnMgr = new ();
	nil _Init(){
		return _InitLearnMgr();
	}
	nil _InitLearnMgr(){
		LearnMgr.OnErr += (s,e)=>{
			AddMsg(App.ErrI18n?.Parse(e.Err)??e.Err+"");
			ShowMsg();
		};
		return Nil;
	}


	public static ObservableCollection<Ctx> Samples = [];
	static VmWordQuery(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
			for(var i = 0; i < 20; i++){
				o.WordCards.Add(VmWordListCard.Samples[0]);
				o.WordCards.Add(VmWordListCard.Samples[1]);
			}
		}
		#endif
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

	protected nil _Learn(VmWordListCard Vm, Learn Learn){
		if(Vm.WordForLearn == null){
			return Nil;
		}
		LearnMgr.Learn(Vm.WordForLearn, Learn);
		SetCurVmWord(Vm);
		return Nil;

	}

	public nil ClickVmWordCard(
		VmWordListCard Vm
	){
		_Learn(Vm, ELearn.Inst.Rmb);
		Vm.BgColor = Cfg.ColorRmb;
		return Nil;
	}

	public nil OnLongPressed(VmWordListCard Vm){
		_Learn(Vm, ELearn.Inst.Fgt);
		Vm.BgColor = Cfg.ColorFgt;
		return Nil;
	}

	public nil SetCurVmWord(VmWordListCard Vm){
		if(Vm.JnWord == null){
			return Nil;
		}
		CurWordInfo.FromBo(Vm.JnWord);
		return Nil;
	}


	// public nil SetCurWord(JoinedWord JWord){
	// 	CurWordInfo.FromBo(JWord);
	// 	return Nil;
	// }

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
					WordCards.Add(new VmWordListCard().FromJnWord(BoWord));
				}
				LearnMgr.Load(BoWords);
			}
		});
		return Nil;
	}

	public nil Save(){
		CT Ct = default;
		LearnMgr.SaveAsy(Ct).ContinueWith(d=>{
			if(d.IsFaulted){
				System.Console.WriteLine(d.Exception.ToString());//t
				AddMsg(d.Exception.ToString());
				ShowMsg();
			}
		});
		return Nil;
	}


}
