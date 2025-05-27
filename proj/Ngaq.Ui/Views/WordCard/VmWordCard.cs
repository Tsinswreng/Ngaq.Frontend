namespace Ngaq.Ui.Views.WordCard;
using System.Collections.ObjectModel;
using Ngaq.Ui.ViewModels;


using Ctx = VmWordCard;
public partial class VmWordCard: ViewModelBase{

	public static ObservableCollection<Ctx> Samples = [];
	static VmWordCard(){
		{
			var o = new Ctx();
			Samples.Add(o);
			o.Head = "Hello";
			o.Lang = "English";
		}
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
