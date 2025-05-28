using System.Collections.ObjectModel;
using Ngaq.Ui.ViewModels;

namespace Ngaq.Ui.Views.WordInfo;
using Ctx = VmWordInfo;
public partial class VmWordInfo: ViewModelBase{

	public static ObservableCollection<Ctx> Samples = [];
	static VmWordInfo(){
		{
			var o = new Ctx();
			Samples.Add(o);
		}
	}

	protected str _Id = "";
	public str Id{
		get{return _Id;}
		set{SetProperty(ref _Id, value);}
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
