namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanPage;
using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.StudyPlan.Svc;
using Ngaq.Ui.Infra;

using Ctx = VmStudyPlanPage;
public partial class VmStudyPlanPage: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmStudyPlanPage(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmStudyPlanPage(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	ISvcStudyPlan? SvcStudyPlan;
	IFrontendUserCtxMgr? UserCtxMgr;
	public VmStudyPlanPage(
		ISvcStudyPlan? SvcStudyPlan
		,IFrontendUserCtxMgr? UserCtxMgr
	){
		this.SvcStudyPlan = SvcStudyPlan;
		this.UserCtxMgr = UserCtxMgr;
	}

	public async Task<nil> RestoreStudyPlan(CT Ct){
		if(AnyNull(SvcStudyPlan)){
			return NIL;
		}
		try{
			await Task.Run(async()=>{
				await SvcStudyPlan.RestoreBuiltinStudyPlan(
					UserCtxMgr.GetDbUserCtx(), Ct
				);
			});
		}
		catch (System.Exception e){
			HandleErr(e);
		}
		return NIL;
	}


	public str YYY{
		get;
		set{SetProperty(ref field, value);}
	}="";


}
