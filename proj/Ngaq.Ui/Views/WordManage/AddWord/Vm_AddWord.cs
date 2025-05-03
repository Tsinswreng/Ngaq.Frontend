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

}
