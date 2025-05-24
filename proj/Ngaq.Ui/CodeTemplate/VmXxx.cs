using System.Collections.ObjectModel;
using Ngaq.Ui.ViewModels;

namespace Xxx;
using Ctx = VmXxx;
public partial class VmXxx: ViewModelBase{

	public static ObservableCollection<Ctx> Samples = [];
	static VmXxx(){
		{
			var o = new Ctx();
			Samples.Add(o);
		}
	}

}
