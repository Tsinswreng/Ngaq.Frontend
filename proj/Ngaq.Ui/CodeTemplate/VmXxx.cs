namespace Xxx;
using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;

using Ctx = VmXxx;
public partial class VmXxx: ViewModelBase{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmXxx(){}
	public static Ctx Mk(){
		return new Ctx();
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

	public CancellationTokenSource Cts = new();

/*
	public str YYY{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";
 */

}
