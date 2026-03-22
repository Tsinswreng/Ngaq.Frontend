namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanEdit;
using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;

using Ctx = VmStudyPlanEdit;
public partial class VmStudyPlanEdit: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmStudyPlanEdit(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmStudyPlanEdit(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}


	public str YYY{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";


}
