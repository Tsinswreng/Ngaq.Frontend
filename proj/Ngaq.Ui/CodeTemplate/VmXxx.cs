using System.Collections.ObjectModel;
using Ngaq.Ui.ViewModels;

namespace Xxx;
using Ctx = Vm_Xxx;
public partial class Vm_Xxx: ViewModelBase{

	public static ObservableCollection<Ctx> Samples = [];
	static Vm_Xxx(){
		{
			var o = new Ctx();
			Samples.Add(o);

		}
	}

}
