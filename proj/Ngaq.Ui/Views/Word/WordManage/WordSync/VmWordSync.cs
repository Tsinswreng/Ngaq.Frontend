namespace Ngaq.Ui.Views.Word.WordManage.WordSync;
using System.Collections.ObjectModel;
using Ngaq.Core.Domains.User.Svc;
using Ngaq.Ui.Infra;

using Ctx = VmWordSync;
public partial class VmWordSync: ViewModelBase{
	ISvcKv? SvcDbCfg;
	public VmWordSync(ISvcKv? SvcDbCfg){
		this.SvcDbCfg = SvcDbCfg;
	}

	protected VmWordSync(){}
	public static Ctx Mk(){
		return new Ctx();
	}
	public static ObservableCollection<Ctx> Samples = [];
	static VmWordSync(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

}
