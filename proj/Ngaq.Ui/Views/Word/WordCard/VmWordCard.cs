namespace Ngaq.Ui.Views.Word.WordCard;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Media;
using Ngaq.Core.Domains.Word.Models.Learn_;
using Ngaq.Core.Infra.Core;
using Ngaq.Ui.Infra;
using Ctx = VmWordListCard;
public partial class VmWordListCard
	:ViewModelBase
{

	public static ObservableCollection<Ctx> Samples = [];
	static VmWordListCard(){
		#if DEBUG
		// {
		// 	var o = new Ctx();
		// 	Samples.Add(o);
		// 	o.FromJnWord(SampleWord.Inst.Samples[0]);
		// }
		// {
		// 	var o = new Ctx();
		// 	Samples.Add(o);
		// 	o.FromJnWord(SampleWord.Inst.Samples[1]);
		// }
		#endif
	}

	public Ctx FromIWordForLearn(IWordForLearn Word){
		this.WordForLearn = Word;
		Init();
		return this;
	}

	nil Init(){
		if(Bo == null){
			return NIL;
		}
		Head = Bo.Head;
		Lang = Bo.Lang;
		Index = Bo.Index;
		Weight = Bo.Weight;
		Learn_Records = Bo.Learn_Records;
		SavedLearnRecords = Bo.LearnRecords;
		LastLearnedTime = Bo.LastLearnedTime_();
		FontColor = AddCntToFontColor((u64)Learn_Records[ELearn.Add].Count);
		return NIL;
	}

	void OnBoChanged(object? sender, PropertyChangedEventArgs e){
		Init();
	}

	protected IWordForLearn? _WordForLearn;
	public IWordForLearn? WordForLearn{
		get{return _WordForLearn;}
		set{
			ForceSetProp(ref _WordForLearn, value);
			if(value == null){return;}
			_WordForLearn!.PropertyChanged += OnBoChanged;
		}
	}

	protected IWordForLearn? Bo{
		get{return WordForLearn;}
		set{WordForLearn = value;}
	}


	// protected JnWord? _JWord;
	// public JnWord? JnWord{
	// 	get{return _JWord;}
	// 	set{SetProperty(ref _JWord, value);}
	// }

	protected str _Head = "";
	public str Head{
		get{return _Head;}
		set{
			if(Bo!=null){Bo.Head = value;}
			SetProperty(ref _Head, value);
		}
	}

	protected str _Lang = "";
	public str Lang{
		get{return _Lang;}
		set{
			if(Bo!=null){Bo.Lang = value;}
			SetProperty(ref _Lang, value);
		}
	}


	protected u64? _Index;
	public u64? Index{
		get{return _Index;}
		set{
			if(Bo!=null){Bo.Index = value;}
			SetProperty(ref _Index, value);
		}
	}


	protected f64? _Weight;
	public f64? Weight{
		get{return _Weight;}
		set{
			if(Bo!=null){Bo.Weight = value;}
			SetProperty(ref _Weight, value);
		}
	}

	protected IDictionary<ELearn, IList<ILearnRecord>> _Learn_Records = new Dictionary<ELearn, IList<ILearnRecord>>();
	public IDictionary<ELearn, IList<ILearnRecord>> Learn_Records{
		get{return _Learn_Records;}
		set{
			if(Bo!=null){Bo.Learn_Records = value;}
			ForceSetProp(ref _Learn_Records, value);
			// SetProperty(ref _Learn_Records, value);
			// OnPropertyChanged(nameof(Learn_Records));
		}
	}

	protected IList<ILearnRecord> _SavedLearnRecords = new List<ILearnRecord>();
	public IList<ILearnRecord> SavedLearnRecords{
		get{return _SavedLearnRecords;}
		set{
			if(Bo!=null){Bo.LearnRecords = value;}
			//OnPropertyChanged(nameof(_SavedLearnRecords));
			ForceSetProp(ref _SavedLearnRecords, value);
		}
	}

	protected i64 _LastLearnedTime = 0;
	public i64 LastLearnedTime{
		get{return _LastLearnedTime;}
		set{SetProperty(ref _LastLearnedTime, value);}
	}

	protected IBrush _LearnedColor = Brushes.Transparent;
	public IBrush LearnedColor{
		get{return _LearnedColor;}
		set{SetProperty(ref _LearnedColor, value);}
	}

	protected IBrush _FontColor = Brushes.White;
	public IBrush FontColor{
		get{return _FontColor;}
		set{SetProperty(ref _FontColor, value);}
	}



	public static str LearnToSymbol(ELearn Learn){
		if(Learn == ELearn.Add){
			return "🤔";
		}else if(Learn == ELearn.Rmb){
			return "✅";
		}else if(Learn == ELearn.Fgt){
			return "❌";
		}
		throw new FatalLogicErr("Unknown Learn: " + Learn);
	}

	public static IBrush AddCntToFontColor(u64 Cnt){
		var rgb = (u8 r,u8 g,u8 b)=>{
			return new SolidColorBrush(Color.FromRgb(r,g,b));
		};
		if(Cnt <= 1){
			return Brushes.White;
		}else if(Cnt == 2){
			return rgb(0,255,128);
		}else if(Cnt == 3){
			return rgb(0,255,255);
		}else if(Cnt == 4){
			return rgb(255,255,0);
		}else if(Cnt == 5){
			return rgb(200, 100, 255);
		}else{
			return rgb(255,50,50);
		}
	}

	public static str FormatUnixMsDiff(
		i64 MsDiff
	){
		var ToI64 = (f64 N)=>{
			return (i64)N;
		};
		var S = MsDiff/1000.0;
		var M = S/60.0;
		if(M < 1){
			return ToI64(S)+"s";
		}
		var H = M/60.0;
		if(H < 1){
			return ToI64(M)+"m";
		}
		var D = H/24.0;
		if(H < 100){
			return ToI64(H)+"h";
		}
		return ToI64(D)+"d";
	}


	public static string FmtNum(double num, int fix){
		if(f64.IsInfinity(num)){
			return "∞";
		}
		string exp = num.ToString("e");  // 例如 "1.030000e+003"
		var parts = exp.Split('e');      // ["1.030000", "+003"]

		double baseValue = double.Parse(parts[0]);
		baseValue = Math.Round(baseValue, fix);
		string baseStr = baseValue.ToString($"F{fix}");

		string expPart = parts[1];       // "+003" or "-004"

		// 提取符号和数字部分
		char sign = expPart[0];
		string digits = expPart.Substring(1);

		// 转成整数去掉前导0
		int expNum = int.Parse(digits);

		// 拼接指数，去掉多余的零，比如 "e+003" -> "e3"
		string result = baseStr + "e" + (sign == '+' ? "+" : "-") + expNum.ToString();

		return result;
	}


}
