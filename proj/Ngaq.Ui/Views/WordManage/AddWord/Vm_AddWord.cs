using System.Collections.ObjectModel;
using Ngaq.Ui.ViewModels;

namespace Ngaq.Ui.Views.WordManage.AddWord;
using Ctx = Vm_AddWord;
public partial class Vm_AddWord: ViewModelBase{

	public static ObservableCollection<Ctx> Samples = [];
	static Vm_AddWord(){
		{
			var o = new Ctx();
			Samples.Add(o);

		}
	}

	protected str _Path = "";
	public str Path{
		get{return _Path;}
		set{SetProperty(ref _Path, value);}
	}

	protected str _Text = "";
	public str Text{
		get{return _Text;}
		set{SetProperty(ref _Text, value);}
	}


	public nil Confirm(){
		if(str.IsNullOrEmpty(Path) || str.IsNullOrEmpty(Text)){
			return null!;
		}
		if(!str.IsNullOrEmpty(Text)){

		}
		return null!;
	}



}
