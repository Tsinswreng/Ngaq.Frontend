namespace Ngaq.Ui.Views.Dictionary;
using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;

using Ctx = VmDictionary;
public partial class VmDictionary: ViewModelBase{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmDictionary(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmDictionary(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}


	public str Input{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";


	public async Task<nil> SearchAsy(CT Ct){
		return NIL;
	}


}
