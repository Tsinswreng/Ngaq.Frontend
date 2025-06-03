namespace Ngaq.Ui.Views.Word.WordCard;
using System.Collections.ObjectModel;
using Avalonia.Media;
using Ngaq.Core.Model.Bo;
using Ngaq.Core.Model.Samples.Word;
using Ngaq.Core.Service.Word.Learn_.Models;
using Ngaq.Ui.ViewModels;


using Ctx = VmWordListCard;
public partial class VmWordListCard: ViewModelBase{

	public static ObservableCollection<Ctx> Samples = [];
	static VmWordListCard(){
		{
			var o = new Ctx();
			Samples.Add(o);
			o.FromBo(SampleWord.Inst.Samples[0]);
		}
		{
			var o = new Ctx();
			Samples.Add(o);
			o.FromBo(SampleWord.Inst.Samples[1]);
		}
	}

	public Ctx FromBo(JoinedWord BoWord){
		this.BoWord = BoWord;
		Head = BoWord.PoWord.Head;
		Lang = BoWord.PoWord.Lang;
		WordForLearn = new WordForLearn(BoWord);
		return this;
	}


	protected IWordForLearn? _WordForLearn;
	public IWordForLearn? WordForLearn{
		get{return _WordForLearn;}
		set{SetProperty(ref _WordForLearn, value);}
	}


	protected JoinedWord? _JWord;
	public JoinedWord? BoWord{
		get{return _JWord;}
		set{SetProperty(ref _JWord, value);}
	}




	protected str _Head = "";
	public str Head{
		get{return _Head;}
		set{SetProperty(ref _Head, value);}
	}

	protected str _Lang = "";
	public str Lang{
		get{return _Lang;}
		set{SetProperty(ref _Lang, value);}
	}

	protected IBrush _BgColor = Brushes.Black;
	public IBrush BgColor{
		get{return _BgColor;}
		set{SetProperty(ref _BgColor, value);}
	}

}
