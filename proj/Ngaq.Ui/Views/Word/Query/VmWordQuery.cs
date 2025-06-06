using System.Collections.ObjectModel;
using Avalonia.Media;
using Ngaq.Core.Infra.Page;
using Ngaq.Core.Model.Bo;
using Ngaq.Core.Model.UserCtx;
using Ngaq.Core.Service.Word;
using Ngaq.Core.Word;
using Ngaq.Core.Word.Models.Learn_;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordInfo;
using Tsinswreng.CsCore.Tools;

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
		public IBrush ColorNone = Brushes.Transparent;
		public IBrush ColorRmb = new SolidColorBrush(Color.FromArgb((u8)(0.8*0xff), 0, 80, 0));
		public IBrush ColorFgt = new SolidColorBrush(Color.FromArgb((u8)(0.8*0xff), 80, 0, 0));
	}

	public Cfg_ Cfg = new();

	// public VmWordQuery(){
	// 	_Init();
	// }

	public VmWordQuery(
		ISvcWord SvcWord
		,IUserCtxMgr UserCtxMgr
		,MgrLearn MgrLearn
	){
		this.SvcWord = SvcWord;
		this.UserCtxMgr = UserCtxMgr;
		this.MgrLearn = MgrLearn;
		_Init();
	}

	public ISvcWord SvcWord;
	public IUserCtxMgr UserCtxMgr;

	public MgrLearn MgrLearn{get;set;}
	nil _Init(){
		return _InitLearnMgr();
	}
	nil _InitLearnMgr(){
		MgrLearn.OnErr += (s,e)=>{
			System.Console.WriteLine(e);//t
			System.Console.WriteLine(e.Err);
			AddMsg(App.ErrI18n?.Parse(e.Err)??e.Err+"");
			ShowMsg();
		};
		return Nil;
	}


	public static ObservableCollection<Ctx> Samples = [];
	static VmWordQuery(){
		#if false//||DEBUG
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

	protected i64 _LearnOrUndo(VmWordListCard Vm, Learn Learn){
		if(Vm.WordForLearn == null){
			return (i64)MgrLearn.ELearnOpRtn.Invalid;
		}
		var R = MgrLearn.LearnOrUndo(Vm.WordForLearn, Learn);
		SetCurVmWord(Vm);
		return R;
	}

	// IBrush MatchBg(i64 ELearnOptRtn){
	// 	if(ELearnOptRtn == (i64)MgrLearn.ELearnOpRtn.Learn){
	// 		return Cfg.ColorRmb;
	// 	}else if(ELearnOptRtn == (i64)MgrLearn.ELearnOpRtn.Undo){
	// 		return Cfg.ColorNone;
	// 	}
	// }

	public nil ClickVmWordCard(
		VmWordListCard Vm
	){
		if(_LearnOrUndo(Vm, ELearn.Inst.Rmb) == (i64)MgrLearn.ELearnOpRtn.Learn){
			Vm.BgColor = Cfg.ColorRmb;
		}else{
			Vm.BgColor = Cfg.ColorNone;
		}
		return Nil;
	}

	public nil OnLongPressed(VmWordListCard Vm){
		if(_LearnOrUndo(Vm, ELearn.Inst.Fgt) == (i64)MgrLearn.ELearnOpRtn.Learn){
			Vm.BgColor = Cfg.ColorFgt;
		}else{
			Vm.BgColor = Cfg.ColorNone;
		}
		return Nil;
	}

	public nil SetCurVmWord(VmWordListCard Vm){
		if(Vm.WordForLearn == null){
			return Nil;
		}
		CurWordInfo.FromIWordForLearn(Vm.WordForLearn);
		return Nil;
	}

	public async Task<nil> LoadEtStartAsy(CT Ct){
		if(!MgrLearn.State.OperationStatus.Load){
			var Page = await SvcWord.PageBoWord(
				UserCtxMgr.GetUserCtx()
				,PageQuery.SelectAll()
				,Ct
			);

			var Words = Page.DataAsy?.ToBlockingEnumerable(Ct)??[];
			MgrLearn.Load(Words);
		}
		await MgrLearn.StartAsy(Ct);
		RenderWordList();
		return Nil;
	}

	public async Task<nil> SaveEtRestartAsy(CT Ct){
		await MgrLearn.SaveAsy(Ct);
		await MgrLearn.StartAsy(Ct);
		RenderWordList();
		return Nil;
	}

	public nil RenderWordList(){
		WordCards.Clear();
		MgrLearn.State.WordsToLearn.Select(x=>{
			WordCards.Add(new VmWordListCard().FromIWordForLearn(x));
			return 0;
		}).ToList();
		return Nil;
	}

	public nil LoadEtStart(){
		CT Ct = default;
		LoadEtStartAsy(Ct).ContinueWith(d=>{
			if(d.IsFaulted){
				var e = d.Exception.ToString();
				System.Console.WriteLine(e);
				Msgs.Add(e);
				ShowMsg();
			}
		});
		return Nil;
	}

	public nil SaveEtRestart(){
		CT Ct = default;
		SaveEtRestartAsy(Ct).ContinueWith(d=>{
			if(d.IsFaulted){
				Msgs.Add(d?.ToString()??"");
				ShowMsg();
			}
		});
		return Nil;
	}

	public nil Reset(){
		MgrLearn = App.GetSvc<MgrLearn>();
		WordCards = new();
		return Nil;
	}




	// public nil GetAllWords(){
	// 	CancellationToken Ct = default;
	// 	SvcWord?.PageBoWord(
	// 		UserCtxMgr?.GetUserCtx()
	// 		,PageQuery.SelectAll()
	// 		,Ct
	// 	).ContinueWith(d=>{
	// 		if(d.IsFaulted){
	// 			System.Console.WriteLine(d.Exception.ToString());//t
	// 			AddMsg(d.Exception.ToString());
	// 			ShowMsg();
	// 		}else{
	// 			WordCards.Clear();
	// 			var Page = d.Result;
	// 			var BoWords = Page.DataAsy?.ToBlockingEnumerable(Ct)??[];
	// 			foreach(var BoWord in BoWords){
	// 				WordCards.Add(new VmWordListCard().FromJnWord(BoWord));
	// 			}
	// 			MgrLearn.Load(BoWords);
	// 		}
	// 	});
	// 	return Nil;
	// }

	public nil Save(){
		CT Ct = default;
		MgrLearn.SaveAsy(Ct).ContinueWith(d=>{
			if(d.IsFaulted){
				System.Console.WriteLine(d.Exception.ToString());//t
				AddMsg(d.Exception.ToString());
				ShowMsg();
			}
		});
		return Nil;
	}


}
