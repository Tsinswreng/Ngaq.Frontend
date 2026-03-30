namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;
using System.Collections.ObjectModel;
using Ngaq.Core.Shared.StudyPlan.Models;
using Ngaq.Ui.Infra;

using Ctx = VmPreFilterEdit;
public partial class VmPreFilterEdit: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmPreFilterEdit(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmPreFilterEdit(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}


	public BoPreFilter BoPreFilter{get;set;}

	public void FromBoPreFilter(){
		throw new NotImplementedException();
	}

	public BoPreFilter ToBoPreFilter(){
		throw new NotImplementedException();
	}

	public str YYY{
		get;
		set{SetProperty(ref field, value);}
	}="";


}
