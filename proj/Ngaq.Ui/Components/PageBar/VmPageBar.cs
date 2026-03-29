namespace Ngaq.Ui.Components.PageBar;
using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;
using Tsinswreng.CsCore;
using Tsinswreng.CsPage;
using Ctx = VmPageBar;
public partial class VmPageBar: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmPageBar(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmPageBar(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}


	[Doc(@$"Page number shown in GUI.
	ususally starts from 1,
	unlike {nameof(IPageQry.PageIdx)} which starts from 0.
	")]
	public u64 PageNum{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=0;

	public u64 PageSize{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=0;

	// public u64 PageSize{
	// 	get;
	// 	set{SetProperty(ref field, value);}
	// }=0;




}
