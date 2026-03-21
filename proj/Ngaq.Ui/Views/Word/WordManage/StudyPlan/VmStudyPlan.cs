namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;
using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;

using Ctx = VmStudyPlan;
public partial class VmStudyPlan: ViewModelBase, IMk<Ctx>{
	protected VmStudyPlan(){
		StudyPlanUiStore.EnsureInit();
	}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmStudyPlan(){
		#if DEBUG
		Samples.Add(new Ctx());
		#endif
	}
}
