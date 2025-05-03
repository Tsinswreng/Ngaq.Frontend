namespace Ngaq.Ui.Views.WordCard;
using System.Collections.ObjectModel;
using Ngaq.Ui.ViewModels;


using Ctx = Vm_WordCard;
public partial class Vm_WordCard: ViewModelBase{

	public static ObservableCollection<Ctx> Samples = [];
	static Vm_WordCard(){
		{
			var o = new Ctx();
			Samples.Add(o);

		}
	}

}
