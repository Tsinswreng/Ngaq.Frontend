namespace Ngaq.Ui.Views.Word.Query;
using System.Collections.ObjectModel;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Ngaq.Core.Word.Svc;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordInfo;
using Tsinswreng.CsPage;
using Ngaq.Ui.Infra;

using Ctx = VmLearnWords;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Frontend.ImgBg;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Shared.Word;

using Avalonia.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Tsinswreng.CsCfg;
using Ngaq.Core.Infra.Cfg;
using System.Diagnostics;
using Tsinswreng.CsTools;
using Ngaq.Core.Tools;
using Avalonia.Logging;
using Ngaq.Core.Shared.Base.Models.Po;

public partial class VmLearnWords
	:ViewModelBase
{

	public partial class Cfg_{
		/// <summary>
		/// 單詞條長按
		/// </summary>
		public i64 LongPressDurationMs = 200;
		public IBrush ColorDflt = Brushes.Black;
		public IBrush ColorNone = Brushes.Transparent;
		// public IBrush ColorRmb = new SolidColorBrush(Color.FromArgb((u8)(0.8*0xff), 0, 80, 0));
		// public IBrush ColorFgt = new SolidColorBrush(Color.FromArgb((u8)(0.8*0xff), 80, 0, 0));
		public IBrush ColorRmb = new SolidColorBrush(Color.FromArgb((u8)(1*0xff), 0, 0xff, 0));
		public IBrush ColorFgt = new SolidColorBrush(Color.FromArgb((u8)(1*0xff), 0xff, 0, 0));
	}

	public Cfg_ CfgUi = new();

	// public VmWordQuery(){
	// 	_Init();
	// }
public ISvcWord SvcWord;
public IFrontendUserCtxMgr UserCtxMgr;

public MgrLearn MgrLearn{get;set;}
public IImgGetter? SvcImg{get;set;}
ICfgAccessor? Cfg;
	public VmLearnWords(
		ISvcWord SvcWord
		,IFrontendUserCtxMgr UserCtxMgr
		,MgrLearn MgrLearn
		,IImgGetter? SvcImg
		,ICfgAccessor? Cfg
	){
		this.SvcImg = SvcImg;
		this.SvcWord = SvcWord;
		this.UserCtxMgr = UserCtxMgr;
		this.MgrLearn = MgrLearn;
		this.Cfg = Cfg;
		_Init();
	}


	nil _Init(){
		return _InitLearnMgr();
	}
	nil _InitLearnMgr(){
		MgrLearn.OnErr += (s,e)=>{
			HandleErr(e.Err);
		};
		MgrLearn.OnLearnOrUndo += (s,e)=>{
			ChangeBg().ContinueWith(t=>{
				if(t.IsFaulted){
					HandleErr(t.Exception);
				}
			});
		};
		return NIL;
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmLearnWords(){
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

	//protected ObservableCollection<VmWordListCard> _WordCards = new();
	public ObservableCollection<VmWordListCard> WordCards{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = new();

	//protected VmWordInfo _VmWordInfo = new();
	public VmWordInfo CurWordInfo{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = new();


	protected ELearnOpRtn _LearnOrUndo(VmWordListCard Vm, ELearn Learn){
		if(Vm.WordForLearn == null){
			return ELearnOpRtn.Invalid;
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

	public nil ClickWordCard(VmWordListCard Vm){
		var CurLearnRecord = Vm.WordForLearn?.GetLastUnsavedLearnRecord();
		if(CurLearnRecord == null){
			//->Rmg
			_LearnOrUndo(Vm, ELearn.Rmb);
			Vm.LearnedColor = CfgUi.ColorRmb;
		}else if(CurLearnRecord.Learn == ELearn.Rmb){
			//-> Fgt
			_LearnOrUndo(Vm, ELearn.Fgt);//Make it undo
			_LearnOrUndo(Vm, ELearn.Fgt);//Make it be fgt
			Vm.LearnedColor = CfgUi.ColorFgt;
		}else if(CurLearnRecord.Learn == ELearn.Fgt){
			//->Clear
			_LearnOrUndo(Vm, ELearn.Fgt);//Make it undo
			Vm.LearnedColor = CfgUi.ColorNone;
		}
		return NIL;
	}

	// public nil ClickVmWordCard(
	// 	VmWordListCard Vm
	// ){
	// 	if(_LearnOrUndo(Vm, ELearn.Rmb) == (i64)MgrLearn.ELearnOpRtn.Learn){
	// 		Vm.LearnedColor = Cfg.ColorRmb;
	// 	}else{
	// 		Vm.LearnedColor = Cfg.ColorNone;
	// 	}
	// 	return NIL;
	// }

	// public nil OnLongPressed(VmWordListCard Vm){
	// 	if(_LearnOrUndo(Vm, ELearn.Fgt) == (i64)MgrLearn.ELearnOpRtn.Learn){
	// 		Vm.LearnedColor = Cfg.ColorFgt;
	// 	}else{
	// 		Vm.LearnedColor = Cfg.ColorNone;
	// 	}
	// 	return NIL;
	// }

	public nil SetCurVmWord(VmWordListCard Vm){
		if(Vm.WordForLearn == null){
			return NIL;
		}
		CurWordInfo.FromIWordForLearn(Vm.WordForLearn);
		return NIL;
	}

	public async Task<nil> LoadEtStartAsy(CT Ct){
		var sw = Stopwatch.StartNew();
		await Task.Run(async()=>{
				if(!MgrLearn.State.OperationStatus.Load){
				var Page = await SvcWord.PageWord(
					UserCtxMgr.GetUserCtx()
					,PageQry.SlctAll()
					,Ct
				);
				//須先DBʹ詞ˇ全載入內存後交予MgrLearn。否則算權重旹併發讀則使Sqlite出錯
				var sw2 = Stopwatch.StartNew();
				var loadedAll = await Page.ToListPage(Ct);
				sw2.Stop();
				LogInfo($"LoadAllWordFromDb: {sw2.ElapsedMilliseconds} ms");
				var dataAsyE = (loadedAll.Data??[]).ToAsyncEnumerable();
				//await MgrLearn.LoadEtCalcWeightAsy(Page.DataAsyE.OrEmpty(), Ct);
				await MgrLearn.LoadEtCalcWeightAsy(dataAsyE, Ct);
			}
			await MgrLearn.StartAsy(Ct);
		});
		RenderWordList();
		sw.Stop();
		LogInfo($"LoadEtStartAsy: {sw.ElapsedMilliseconds} ms");
		return NIL;
	}

	public async Task<nil> SaveEtRestartAsy(CT Ct){
		var sw = Stopwatch.StartNew();
		await MgrLearn.SaveAsy(Ct);//只背一個單詞45ms于安卓
		sw.Stop();
		var t1 = sw.ElapsedMilliseconds;
		sw = Stopwatch.StartNew();
		await MgrLearn.CalcWeightEtStartAsy(Ct);//只背一個單詞567ms于安卓
		sw.Stop();
		var t2 = sw.ElapsedMilliseconds;
		sw = Stopwatch.StartNew();
		RenderWordList();////只背一個單詞104ms于安卓 限示50個單詞 虛渲染
		sw.Stop();
		var t3 = sw.ElapsedMilliseconds;
		// Dispatcher.UIThread.Post(()=>{
		// 	ShowMsg($"SaveEtRestartAsy: {t1}ms, CalcWeightEtStartAsy: {t2}ms, RenderWordList: {t3}ms");
		// });
		return NIL;
	}

	public nil RenderWordList(){
		WordCards.Clear();
		var MaxDisplayedWordCount = Cfg?.Get(ItemsClientCfg.Word.MaxDisplayedWordCount)??50;
		MgrLearn.State.WordsToLearn.Select(x=>{
			if((u64)WordCards.Count < MaxDisplayedWordCount){
				var vm = new VmWordListCard();
				vm.InitFromIWordForLearn(x);
				WordCards.Add(vm);
			}
			return 0;
		}).ToList();
		return NIL;
	}

	public async Task<nil> ResetAsy(CT Ct){
		//MgrLearn = App.GetSvc<MgrLearn>();
		BgBrush = CfgUi.ColorDflt;
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
		MgrLearn.SaveAsy(Ct).ContinueWith(t=>{
			HandleErr(t);
		});
		return NIL;
	}

	protected IBrush _BgBrush = Brushes.Black;
	public IBrush BgBrush{
		get{return _BgBrush;}
		set{SetProperty(ref _BgBrush, value);}
	}



	// 类级字段（只加这一行）
	private readonly ConcurrentQueue<Bitmap> _bgCache = new();

	public async Task<nil> ChangeBg()
	{
		try{
			await Task.Run(()=>{
	/* 1. 缓存有就拿一张用 */
				if (_bgCache.Count > 0){
					Dispatcher.UIThread.Post(()=>{
						if(_bgCache.TryDequeue(out var bm)){
							BgBrush = new ImageBrush(bm) { Stretch = Stretch.UniformToFill };
						}
					});
				}
				/* 2. 缓存没货才抓一批（第一次或异常后） */
				else{
					if (SvcImg == null) return NIL;
					var batch = SvcImg.GetN(3);
					if (batch == null) return NIL;
					int idx = 0;
					foreach (var obj in batch){
						if (obj.Type != typeof(Stream) || obj.Data == null) continue;
						using Stream s = (Stream)obj.Data;
						var bmp = new Bitmap(s);
						if (idx == 0){// 第一张直接上屏
							Dispatcher.UIThread.Post(()=>{
								BgBrush = new ImageBrush(bmp) { Stretch = Stretch.UniformToFill };
							});
						}else{// 其余先囤着
							_bgCache.Enqueue(bmp);
						}
						idx++;
					}
				}

				/* 3. 用掉一张立刻补一张，保持库存 2 张 */
				if (_bgCache.Count < 2 && SvcImg != null){
					var obj = SvcImg.GetN(1).FirstOrDefault();
					if (obj != null && obj.Type == typeof(Stream) && obj.Data != null){
						using (Stream s = (Stream)obj.Data){
							_bgCache.Enqueue(new Bitmap(s));
						}
					}
				}
				return NIL;
			});
		}
		catch (Exception e){
			LogError(e.ToString());
		}
		return NIL;
	}


}
