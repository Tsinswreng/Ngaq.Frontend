namespace Ngaq.Ui.Views.Dictionary.SimpleWord;
using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;

using Ctx = VmSimpleWord;
public partial class VmSimpleWord: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmSimpleWord(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmSimpleWord(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}


	public str Head{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	//可有多個
	public str Pronunciation{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";


	public str Description{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	



}
