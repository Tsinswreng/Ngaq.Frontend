using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;

namespace Ngaq.Ui.Views.BottomBar;
using Ctx = Vm_BottomBar;
public partial class Vm_BottomBar: ViewModelBase{

	public static ObservableCollection<Ctx> Samples = [];
	static Vm_BottomBar(){
		{
			var o = new Ctx();
			Samples.Add(o);

		}
	}

}
