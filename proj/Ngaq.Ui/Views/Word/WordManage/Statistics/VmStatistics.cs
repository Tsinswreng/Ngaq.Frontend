namespace Ngaq.Ui.Views.Word.WordManage.Statistics;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Word.Svc;
using Ngaq.Ui.Infra;
using ScottPlot;
using Tsinswreng.CsCore;
using Tsinswreng.CsPage;
using Ctx = VmStatistics;
public partial class VmStatistics: ViewModelBase{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmStatistics(){}
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
	){
		this.SvcWord = SvcWord;
	}


	public u64 PageIdx{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=0;

	public u64 PageSize{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=20;

	public Tempus TimeStart{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=new Tempus();

	public Tempus TimeEnd{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=new Tempus();

	public Tempus TimeInterval{
		get{return field;}
		set{SetProperty(ref field, value);}
	}

	public str LearnResult{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=ELearn.Add+"";


	// public IList<f64> Times{
	// 	get{return field;}
	// 	set{SetProperty(ref field, value);}
	// }=[];

	// public IList<f64> Cnts{
	// 	get{return field;}
	// 	set{SetProperty(ref field, value);}
	// }=[];

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
			PageQry = new PageQry{
				PageIdx = PageIdx,
				PageSize = PageSize,
			}
		};
		await Task.Run(async()=>{
			var Resp = await SvcWord.ScltAddedWordsByTimeInterval(Req, Ct);
			var Intervals = await Resp.IntervalPage.DataAsyE.OrEmpty().ToListAsync(Ct);
			Dispatcher.UIThread.Post(()=>{
				Points.Clear();
				foreach(var Interval in Intervals){
					Points.Add(new Coordinates(Interval.TimeStart, Interval.Cnt));
				}
				GraphChanged?.Invoke(this, EventArgs.Empty);
			});

		});
		return NIL;
	}

}
