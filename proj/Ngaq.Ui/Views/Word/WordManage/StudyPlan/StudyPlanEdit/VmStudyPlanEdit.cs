namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanEdit;

using System;
using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Infra.IF;
using Ngaq.Core.Shared.StudyPlan.Models.Po.StudyPlan;
using Ngaq.Core.Shared.StudyPlan.Svc;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Tsinswreng.CsTools;
using Ctx = VmStudyPlanEdit;

/// StudyPlan 編輯頁 ViewModel。
/// 當前僅支持修改與刪除既有實體，不支持新增。
public partial class VmStudyPlanEdit: ViewModelBase, IMk<Ctx>{
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

	ISvcStudyPlan? SvcStudyPlan{get;set;}
	IFrontendUserCtxMgr? UserCtxMgr{get;set;}

	public VmStudyPlanEdit(
		ISvcStudyPlan? SvcStudyPlan
		,IFrontendUserCtxMgr? UserCtxMgr
	){
		this.SvcStudyPlan = SvcStudyPlan;
		this.UserCtxMgr = UserCtxMgr;
	}

	public bool IsCreateMode{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = false;

	public str LastError{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public bool HasError => !str.IsNullOrWhiteSpace(LastError);

	public str PoIdText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str PoUniqName{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str PoDescr{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str PreFilterIdText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str WeightCalculatorIdText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str WeightArgIdText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public PoStudyPlan PoStudyPlan{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = new();

	public nil SetCreateMode(bool IsCreate){
		IsCreateMode = IsCreate;
		return NIL;
	}

	public nil FromPoStudyPlan(PoStudyPlan? PoStudyPlan){
		this.PoStudyPlan = ClonePoStudyPlan(PoStudyPlan);
		IsCreateMode = PoStudyPlan is null || PoStudyPlan.Id == IdStudyPlan.Zero;
		SyncFromPo();
		return NIL;
	}

	public async Task<nil> Save(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			return NIL;
		}
		if(IsCreateMode){
			LastError = Todo.I18n("StudyPlan 暫不支持新增");
			OnPropertyChanged(nameof(HasError));
			ShowMsg(LastError);
			return NIL;
		}
		try{
			var po = BuildPoFromFields();
			await SvcStudyPlan.BatUpdStudyPlan(UserCtxMgr.GetDbUserCtx(), ToolAsyE.ToAsyE([po]), Ct);
			PoStudyPlan = po;
			SyncFromPo();
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			ShowMsg(Todo.I18n("Saved"));
		}catch(Exception e){
			LastError = e.Message;
			OnPropertyChanged(nameof(HasError));
			HandleErr(e);
		}
		return NIL;
	}

	public async Task<nil> Delete(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			return NIL;
		}
		try{
			var po = BuildPoFromFields();
			if(!IsCreateMode && po.Id != IdStudyPlan.Zero){
				await SvcStudyPlan.BatSoftDelStudyPlan(UserCtxMgr.GetDbUserCtx(), ToolAsyE.ToAsyE([po]), Ct);
			}
			PoStudyPlan = new PoStudyPlan();
			IsCreateMode = true;
			SyncFromPo();
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			ShowMsg(Todo.I18n("Deleted"));
		}catch(Exception e){
			LastError = e.Message;
			OnPropertyChanged(nameof(HasError));
			HandleErr(e);
		}
		return NIL;
	}

	void SyncFromPo(){
		var po = PoStudyPlan ?? new PoStudyPlan();
		PoIdText = po.Id.ToString();
		PoUniqName = po.UniqName ?? "";
		PoDescr = po.Descr ?? "";
		PreFilterIdText = po.PreFilterId.ToString();
		WeightCalculatorIdText = po.WeightCalculatorId.ToString();
		WeightArgIdText = po.WeightArgId.ToString();
	}

	PoStudyPlan BuildPoFromFields(){
		var po = ClonePoStudyPlan(PoStudyPlan);
		po.UniqName = str.IsNullOrWhiteSpace(PoUniqName) ? null : PoUniqName.Trim();
		po.Descr = PoDescr?.Trim() ?? "";
		return po;
	}

	static PoStudyPlan ClonePoStudyPlan(PoStudyPlan? src){
		src ??= new PoStudyPlan();
		return new PoStudyPlan{
			DbCreatedAt = src.DbCreatedAt,
			DbUpdatedAt = src.DbUpdatedAt,
			DelAt = src.DelAt,
			BizCreatedAt = src.BizCreatedAt,
			BizUpdatedAt = src.BizUpdatedAt,
			Id = src.Id,
			Owner = src.Owner,
			UniqName = src.UniqName,
			Descr = src.Descr,
			PreFilterId = src.PreFilterId,
			WeightCalculatorId = src.WeightCalculatorId,
			WeightArgId = src.WeightArgId,
		};
	}
}
