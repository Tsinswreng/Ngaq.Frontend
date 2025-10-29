namespace Ngaq.Ui.Views.Word.Query;
using System.Collections.ObjectModel;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Ngaq.Core.Word.Svc;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordInfo;
using Tsinswreng.CsPage;
using Ngaq.Ui.Infra;

using Ctx = VmWordQuery;
using Ngaq.Core.Shared.User.UserCtx;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Frontend.ImgBg;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Shared.Word;

public partial class VmWordQuery
	:ViewModelBase
{

	public  partial class Cfg_{
		/// <summary>
		/// 單詞條長按
		/// </summary>
		public i64 LongPressDurationMs = 200;
		public IBrush ColorNone = Brushes.Transparent;
		// public IBrush ColorRmb = new SolidColorBrush(Color.FromArgb((u8)(0.8*0xff), 0, 80, 0));
		// public IBrush ColorFgt = new SolidColorBrush(Color.FromArgb((u8)(0.8*0xff), 80, 0, 0));
		public IBrush ColorRmb = new SolidColorBrush(Color.FromArgb((u8)(1*0xff), 0, 0xff, 0));
		public IBrush ColorFgt = new SolidColorBrush(Color.FromArgb((u8)(1*0xff), 0xff, 0, 0));
	}

	public Cfg_ Cfg = new();

	// public VmWordQuery(){
	// 	_Init();
	// }
public ISvcWord SvcWord;
public IFrontendUserCtxMgr UserCtxMgr;

public MgrLearn MgrLearn{get;set;}
public IImgGetter? SvcImg{get;set;}
	public VmWordQuery(
		ISvcWord SvcWord
		,IFrontendUserCtxMgr UserCtxMgr
		,MgrLearn MgrLearn
		,IImgGetter? SvcImg
	){
		this.SvcImg = SvcImg;
		this.SvcWord = SvcWord;
		this.UserCtxMgr = UserCtxMgr;
		this.MgrLearn = MgrLearn;
		_Init();
	}


	nil _Init(){
		return _InitLearnMgr();
	}
	nil _InitLearnMgr(){
		MgrLearn.OnErr += (s,e)=>{
			System.Console.WriteLine(e);//t
			System.Console.WriteLine(e.Err);
			AddMsg(e.Err+""); // TODO i18n
			ShowMsg();
		};
		MgrLearn.OnLearnOrUndo += (s,e)=>{
			ChangeBg();
		};
		return NIL;
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

	protected i64 _LearnOrUndo(VmWordListCard Vm, ELearn Learn){
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
		if(_LearnOrUndo(Vm, ELearn.Rmb) == (i64)MgrLearn.ELearnOpRtn.Learn){
			Vm.LearnedColor = Cfg.ColorRmb;
		}else{
			Vm.LearnedColor = Cfg.ColorNone;
		}
		return NIL;
	}

	public nil OnLongPressed(VmWordListCard Vm){
		if(_LearnOrUndo(Vm, ELearn.Fgt) == (i64)MgrLearn.ELearnOpRtn.Learn){
			Vm.LearnedColor = Cfg.ColorFgt;
		}else{
			Vm.LearnedColor = Cfg.ColorNone;
		}
		return NIL;
	}

	public nil SetCurVmWord(VmWordListCard Vm){
		if(Vm.WordForLearn == null){
			return NIL;
		}
		CurWordInfo.FromIWordForLearn(Vm.WordForLearn);
		return NIL;
	}

	public async Task<nil> LoadEtStartAsy(CT Ct){
		if(!MgrLearn.State.OperationStatus.Load){
			var Page = await SvcWord.PageWord(
				UserCtxMgr.GetUserCtx()
				,PageQry.SlctAll()
				,Ct
			);

			var Words = Page.Data??[];
			MgrLearn.Load(Words);//
		}
		await MgrLearn.StartAsy(Ct);
		RenderWordList();
		return NIL;
	}

	public async Task<nil> SaveEtRestartAsy(CT Ct){
		await MgrLearn.SaveAsy(Ct);
		await MgrLearn.StartAsy(Ct);
		RenderWordList();
		return NIL;
	}

	public nil RenderWordList(){
		WordCards.Clear();
		MgrLearn.State.WordsToLearn.Select(x=>{
			WordCards.Add(new VmWordListCard().FromIWordForLearn(x));
			return 0;
		}).ToList();
		return NIL;
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
		return NIL;
	}

	public nil SaveEtRestart(){
		CT Ct = default;
		SaveEtRestartAsy(Ct).ContinueWith(d=>{
			if(d.IsFaulted){
				Msgs.Add(d?.ToString()??"");
				ShowMsg();
			}
		});
		return NIL;
	}

	public nil Reset(){
		//MgrLearn = App.GetSvc<MgrLearn>();
		MgrLearn.Reset();
		WordCards = new();
		return NIL;
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
		return NIL;
	}

	protected IBrush _BgBrush = Brushes.Black;
	public IBrush BgBrush{
		get{return _BgBrush;}
		set{SetProperty(ref _BgBrush, value);}
	}


	public nil ChangeBg(){//TODO緩存
		try{
			if(SvcImg == null){
				return NIL;
			}
			var typedObj = SvcImg.GetN(1).First();
			if(typedObj.Type != typeof(Stream) || typedObj.Data == null){
				return NIL;
			}
			using Stream BgFileStream = (Stream)typedObj.Data!;
			var BitMap = new Bitmap(BgFileStream);
			var imageBrush = new ImageBrush(BitMap){
				Stretch = Stretch.UniformToFill
			};
			BgBrush = imageBrush;
			return NIL;
		}
		catch (System.Exception e){
			System.Console.Error.WriteLine(e);//t
		}
		return NIL;
	}
}
