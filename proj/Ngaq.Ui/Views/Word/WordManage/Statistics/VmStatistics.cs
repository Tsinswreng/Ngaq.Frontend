namespace Ngaq.Ui.Views.Word.WordManage.Statistics;

using System.Collections.ObjectModel;
using Avalonia.Threading;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Ui.Components.PageBar;
using Ngaq.Ui.Infra;
using ScottPlot;
using Tsinswreng.CsCore;
using Tsinswreng.CsPage;
using Ctx = VmStatistics;

public partial class VmStatistics: ViewModelBase{
	void Init(){
		PageBar = VmPageBar.Mk();
		PageBar.PageSize = 20;
		PageBar.FnPrevPage = OnPrevPage;
		PageBar.FnNextPage = OnNextPage;
	}

	public enum ETimeUnit{
		Second,
		Minute,
		Hour,
		Day,
		Week,
		Month,
		Year,
	}
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmStatistics(){
		Init();
	}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmStatistics(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}


	ISvcWord? SvcWord;
	public VmStatistics(
		ISvcWord? SvcWord
	):this(){
		this.SvcWord = SvcWord;
	}


	public VmPageBar PageBar{get;set;} = null!;

	public u64 PageIdx{
		get{
			if(PageBar.PageNum == 0){
				return 0;
			}
			return PageBar.PageNum-1;
		}
		set{PageBar.PageNum = value+1;}
	}

	public u64 PageSize{
		get{return PageBar.PageSize;}
		set{PageBar.PageSize = value;}
	}

	public Tempus TimeStart{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=new Tempus();

	public Tempus TimeEnd{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=new Tempus();

	public i64 IntervalNoUnit{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				OnPropertyChanged(nameof(TimeInterval));
			}
		}
	}=1;

	public ETimeUnit IntervalUnit{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				OnPropertyChanged(nameof(TimeInterval));
			}
		}
	}=ETimeUnit.Week;

	public Tempus TimeInterval{
		get{return ValueUnitToTempus(IntervalNoUnit, IntervalUnit);}
		//set{SetProperty(ref field, value);}
	}

	public static IReadOnlyList<str> LearnResultOptions{get;} = [
		nameof(ELearn.Add),
		nameof(ELearn.Rmb),
		nameof(ELearn.Fgt),
	];

	public i32 LearnResultIndex{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				OnPropertyChanged(nameof(LearnResult));
			}
		}
	}=0;

	public str LearnResult{
		get{return GetLearnResultByIndex(LearnResultIndex).ToString();}
	}


	// public IList<f64> Times{
	// 	get{return field;}
	// 	set{SetProperty(ref field, value);}
	// }=[];

	// public IList<f64> Cnts{
	// 	get{return field;}
	// 	set{SetProperty(ref field, value);}
	// }=[];

	public static Tempus ValueUnitToTempus(i64 Value, ETimeUnit Unit){
		return Unit switch{
			ETimeUnit.Second => new Tempus(Value*InMillisecond.Second),
			ETimeUnit.Minute => new Tempus(Value*InMillisecond.Minute),
			ETimeUnit.Hour => new Tempus(Value*InMillisecond.Hour),
			ETimeUnit.Day => new Tempus(Value*InMillisecond.Day),
			ETimeUnit.Week => new Tempus(Value*InMillisecond.Week),
			ETimeUnit.Month => new Tempus(Value*InMillisecond.Month),
			ETimeUnit.Year => new Tempus(Value*InMillisecond.Year),
			_ => throw new Exception("Invalid TimeUnit"),
		};
	}

	static ELearn GetLearnResultByIndex(i32 Index){
		if(Index < 0 || Index >= LearnResultOptions.Count){
			return ELearn.Add;
		}
		var text = LearnResultOptions[Index];
		if(Enum.TryParse<ELearn>(text, out var parsed)){
			return parsed;
		}
		return ELearn.Add;
	}

	public List<Coordinates> Points{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=[];

	public event EventHandler? GraphChanged;

	public async Task<nil> GetDataAsy(CT Ct){
		if(AnyNull(SvcWord)){
			return NIL;
		}
		var Req = new ReqScltWordsOfLearnResultByTimeInterval{
			TimeStart = TimeStart,
			TimeEnd = TimeEnd,
			LearnResult = LearnResult,
			TimeInterval = TimeInterval,
			PageQry = PageBar.ToPageQry(),
		};
		Req.PageQry.WantTotCnt = true;
		await Task.Run(async()=>{
			var Resp = await SvcWord.ScltAddedWordsByTimeInterval(Req, Ct);
			var Intervals = await Resp.IntervalPage.DataAsyE.OrEmpty().ToListAsync(Ct);
			Dispatcher.UIThread.Post(()=>{
				PageBar.FromPageResultInfo(Resp.IntervalPage);
				Points.Clear();
				foreach(var Interval in Intervals){
					Points.Add(new Coordinates(Interval.TimeStart, Interval.Cnt));
				}
				GraphChanged?.Invoke(this, EventArgs.Empty);
			});

		});
		return NIL;
	}

	protected async Task<nil> OnPrevPage(VmPageBar PageBar, CT Ct){
		if(PageBar.PageNum <= 1){
			PageBar.PageNum = 1;
			return NIL;
		}
		PageBar.PageNum--;
		return await GetDataAsy(Ct);
	}

	protected async Task<nil> OnNextPage(VmPageBar PageBar, CT Ct){
		if(PageBar.TotPageCnt is u64 TotPageCnt && TotPageCnt > 0 && PageBar.PageNum >= TotPageCnt){
			return NIL;
		}
		PageBar.PageNum++;
		return await GetDataAsy(Ct);
	}

}
