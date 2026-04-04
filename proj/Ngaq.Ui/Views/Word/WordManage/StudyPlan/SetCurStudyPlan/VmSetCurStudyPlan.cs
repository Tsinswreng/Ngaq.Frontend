namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.SetCurStudyPlan;

using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.StudyPlan.Models;
using Ngaq.Core.Shared.StudyPlan.Models.Po.StudyPlan;
using Ngaq.Core.Shared.StudyPlan.Svc;
using Ngaq.Ui.Infra;

using Ctx = VmSetCurStudyPlan;

/// <summary>
/// 「設置當前學習方案」頁面的 ViewModel。
/// 負責讀取當前學習方案，並提供恢復內置方案的操作。
/// </summary>
public partial class VmSetCurStudyPlan: ViewModelBase, IMk<Ctx>{
	protected VmSetCurStudyPlan(){}

	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmSetCurStudyPlan(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	ISvcStudyPlan? SvcStudyPlan{get;set;}
	IFrontendUserCtxMgr? UserCtxMgr{get;set;}

	/// <summary>
	/// 依賴注入構造器。
	/// </summary>
	public VmSetCurStudyPlan(
		ISvcStudyPlan? SvcStudyPlan
		,IFrontendUserCtxMgr? UserCtxMgr
	){
		this.SvcStudyPlan = SvcStudyPlan;
		this.UserCtxMgr = UserCtxMgr;
	}

	public str LastError{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public bool HasError => !str.IsNullOrWhiteSpace(LastError);

	public str CurId{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str CurUniqName{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str CurDescr{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	/// <summary>
	/// 当前已加载/已选中的 StudyPlan。
	/// 供 View 层跳转到编辑页时复用，避免重复查询。
	/// </summary>
	public PoStudyPlan? CurPoStudyPlan{get;private set;}

	/// <summary>
	/// 讀取後端「當前學習方案」。
	/// 成功後刷新當前方案的展示字段。
	/// </summary>
	public async Task<nil> LoadCurStudyPlan(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			return NIL;
		}
		try{
			var dbCtx = UserCtxMgr.GetDbUserCtx();
			var jn = await SvcStudyPlan.GetCurJnStudyPlan(dbCtx, Ct);
			BoStudyPlan? bo = null;
			if(jn is not null){
				bo = new BoStudyPlan{
					PoStudyPlan = jn.StudyPlan,
					PoPreFilter = jn.PreFilter,
					PoWeightCalculator = jn.WeightCalculator,
					PoWeightArg = jn.WeightArg,
				};
			}
			AssignCurFields(bo?.PoStudyPlan);
			LastError = "";
			OnPropertyChanged(nameof(HasError));
		}catch(Exception e){
			LastError = e.Message;
			OnPropertyChanged(nameof(HasError));
			HandleErr(e);
		}
		return NIL;
	}

	/// <summary>
	/// 把所选學習方案設為當前方案，設置成功後刷新展示字段。
	/// </summary>
	public async Task<nil> ApplySelectedStudyPlan(PoStudyPlan? PoStudyPlan, CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr) || PoStudyPlan is null){
			return NIL;
		}
		try{
			await SvcStudyPlan.SetCurStudyPlanId(UserCtxMgr.GetDbUserCtx(), PoStudyPlan.Id, Ct);
			AssignCurFields(PoStudyPlan);
			ShowMsg(Todo.I18n("設置成功"));
			LastError = "";
			OnPropertyChanged(nameof(HasError));
		}catch(Exception e){
			LastError = e.Message;
			OnPropertyChanged(nameof(HasError));
			HandleErr(e);
		}
		return NIL;
	}

	/// <summary>
	/// 把內置學習方案恢復回數據庫，然後重新載入當前學習方案。
	/// </summary>
	public async Task<nil> RestoreBuiltin(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			return NIL;
		}
		try{
			await SvcStudyPlan.RestoreBuiltinStudyPlan(UserCtxMgr.GetDbUserCtx(), Ct);
			ShowMsg(Todo.I18n("RestoreBuiltinDone"));
			return await LoadCurStudyPlan(Ct);
		}catch(Exception e){
			LastError = e.Message;
			OnPropertyChanged(nameof(HasError));
			HandleErr(e);
		}
		return NIL;
	}

	void AssignCurFields(PoStudyPlan? poStudyPlan){
		CurPoStudyPlan = poStudyPlan;
		CurId = poStudyPlan?.Id.ToString() ?? "";
		CurUniqName = poStudyPlan?.UniqName ?? "";
		CurDescr = poStudyPlan?.Descr ?? "";
	}
}
