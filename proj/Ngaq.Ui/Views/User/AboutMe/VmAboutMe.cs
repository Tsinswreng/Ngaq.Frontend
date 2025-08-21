namespace Ngaq.Ui.Views.User.AboutMe;
using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;

using Ctx = VmAboutMe;
public partial class VmAboutMe: ViewModelBase{

	public static ObservableCollection<Ctx> Samples = [];
	static VmAboutMe(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}
}
