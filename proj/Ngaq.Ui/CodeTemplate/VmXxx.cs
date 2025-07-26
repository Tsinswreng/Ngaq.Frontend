namespace Xxx;
using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;

using Ctx = VmXxx;
public partial class VmXxx: ViewModelBase{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmXxx(){}
	public static VmXxx Mk(){
		return new VmXxx();
	}

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
