using System.Collections.ObjectModel;
using Ngaq.Ui.ViewModels;

namespace Ngaq.Ui.Views.Tools;
using Ctx = VmXxx;
public partial class VmXxx: ViewModelBase{

	public static ObservableCollection<Ctx> Samples = [];
	static VmXxx(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

}
