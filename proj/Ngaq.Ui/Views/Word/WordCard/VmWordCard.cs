namespace Ngaq.Ui.Views.Word.WordCard;
using System.Collections.ObjectModel;
using Ngaq.Core.Model.Bo;
using Ngaq.Core.Model.Samples.Word;
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

	public Ctx FromBo(BoWord BoWord){
		this.BoWord = BoWord;
		Head = BoWord.PoWord.Head;
		Lang = BoWord.PoWord.Lang;
		return this;
	}

	protected BoWord? _BoWord;
	public BoWord? BoWord{
		get{return _BoWord;}
		set{SetProperty(ref _BoWord, value);}
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




}
