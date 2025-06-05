namespace Ngaq.Ui.Views.Word.WordCard;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Ngaq.Core.Infra.Core;
using Ngaq.Core.Model.Bo;
using Ngaq.Core.Model.Samples.Word;
using Ngaq.Core.Word.Models.Learn_;
using Ngaq.Ui.ViewModels;
using Tsinswreng.Avalonia.Tools;
using Ctx = VmWordListCard;
public partial class VmWordListCard
	:ViewModelBase
{

	public static ObservableCollection<Ctx> Samples = [];
	static VmWordListCard(){
		{
			var o = new Ctx();
			Samples.Add(o);
			o.FromJnWord(SampleWord.Inst.Samples[0]);
		}
		{
			var o = new Ctx();
			Samples.Add(o);
			o.FromJnWord(SampleWord.Inst.Samples[1]);
		}
	}

	public Ctx FromJnWord(JnWord JnWord){
		this.JnWord = JnWord;
		WordForLearn = new WordForLearn(JnWord);
		Init();
		return this;
	}

	nil Init(){
		if(Bo == null){
			return Nil;
		}
		Head = Bo.Head;
		Lang = Bo.Lang;
		Learn_Records = Bo.Learn_Records;
		SavedLearnRecords = Bo.SavedLearnRecords;
		LastLearnedTime = Bo.LastLearnedTime_();
		return Nil;
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


	protected JnWord? _JWord;
	public JnWord? JnWord{
		get{return _JWord;}
		set{SetProperty(ref _JWord, value);}
	}

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

	protected IBrush _BgColor = Brushes.Black;
	public IBrush BgColor{
		get{return _BgColor;}
		set{SetProperty(ref _BgColor, value);}
	}

	protected IDictionary<Learn, IList<ILearnRecord>> _Learn_Records = new Dictionary<Learn, IList<ILearnRecord>>();
	public IDictionary<Learn, IList<ILearnRecord>> Learn_Records{
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
			if(Bo!=null){Bo.SavedLearnRecords = value;}
			//OnPropertyChanged(nameof(_SavedLearnRecords));
			ForceSetProp(ref _SavedLearnRecords, value);
		}
	}

	protected i64 _LastLearnedTime = 0;
	public i64 LastLearnedTime{
		get{return _LastLearnedTime;}
		set{SetProperty(ref _LastLearnedTime, value);}
	}

	public static str LearnToSymbol(Learn Learn){
		var E = ELearn.Inst;
		if(Learn == E.Add){
			return "🤔";
		}else if(Learn == E.Rmb){
			return "✅";
		}else if(Learn == E.Fgt){
			return "❌";
		}
		throw new FatalLogicErr("Unknown Learn: " + Learn);
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
		if(D < 100){
			return ToI64(H)+"h";
		}
		return ToI64(D)+"d";
	}


}
