namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.SetCurStudyPlan;

using System;
using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
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

	/// <summary>
	/// 讀取後端「當前學習方案」。
	/// 成功後把 StudyPlan 主體回調給 View 層更新編輯頁。
	/// </summary>
	public async Task<nil> LoadCurStudyPlan(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			return NIL;
		}
		try{
			var jn = await SvcStudyPlan.GetCurJnStudyPlan(UserCtxMgr.GetDbUserCtx(), Ct);
			OnLoadedCurStudyPlan?.Invoke(jn?.StudyPlan);
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

	/// <summary>
	/// View 監聽此事件，把當前 StudyPlan 灌入子編輯頁。
	/// </summary>
	public event Action<PoStudyPlan?>? OnLoadedCurStudyPlan;
}
