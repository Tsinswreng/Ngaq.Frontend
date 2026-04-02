namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.WeightArgPage;

using System;
using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.StudyPlan.Models.Po.WeightArg;
using Ngaq.Core.Shared.StudyPlan.Models.Req;
using Ngaq.Core.Shared.StudyPlan.Svc;
using Ngaq.Ui.Components.PageBar;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.WeightArgEdit;

using Ctx = VmWeightArgPage;

/// WeightArg 列表頁 ViewModel。
/// 使用後端接口分頁讀取，不使用假數據。
public partial class VmWeightArgPage: ViewModelBase, IMk<Ctx>{
	void Init(){
		PageBar = VmPageBar.Mk();
		PageBar.PageSize = 10;
		PageBar.FnPrevPage = OnPrevPage;
		PageBar.FnNextPage = OnNextPage;
	}

	protected VmWeightArgPage(){
		Init();
	}

	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmWeightArgPage(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	ISvcStudyPlan? SvcStudyPlan{get;set;}
	IFrontendUserCtxMgr? UserCtxMgr{get;set;}

	/// 依賴注入構造器。
	public VmWeightArgPage(
		ISvcStudyPlan? SvcStudyPlan
		,IFrontendUserCtxMgr? UserCtxMgr
	):this(){
		this.SvcStudyPlan = SvcStudyPlan;
		this.UserCtxMgr = UserCtxMgr;
		Init();
	}

	public VmPageBar PageBar{get;set;} = null!;

	public str Input{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public ObservableCollection<RowWeightArg> Rows{get;set;} = [];

	/// 列表行模型。
	public class RowWeightArg{
		public bool IsChecked{get;set;} = false;
		public u64 UiIdx{get;set;}
		public str UiIdxText{get;set;} = "";
		public str Name{get;set;} = "";
		public str ModifiedTime{get;set;} = "";
		public PoWeightArg? Raw{get;set;} = null;
	}

	static str FormatBizTime(PoWeightArg po){
		var updated = po.BizUpdatedAt == Tempus.Zero ? po.BizCreatedAt : po.BizUpdatedAt;
		if(updated == Tempus.Zero){
			return "-";
		}
		return updated.ToIso();
	}

	public async Task<nil> InitSearch(CT Ct = default){
		PageBar.PageNum = 1;
		return await Search(Ct);
	}

	/// 根據輸入條件查詢後端分頁，並回填 UI 列表。
	public async Task<nil> Search(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			return NIL;
		}
		try{
			var pageQry = PageBar.ToPageQry();
			pageQry.WantTotCnt = true;
			var req = new ReqPageWeightArg{
				PageQry = pageQry,
				UniqNameSearch = str.IsNullOrWhiteSpace(Input) ? null : Input.Trim(),
			};

			var page = await SvcStudyPlan.PageWeightArg(UserCtxMgr.GetDbUserCtx(), req, Ct);
			PageBar.FromPageResultInfo(page);

			Rows.Clear();
			var startUiIdx = page.PageIdx * page.PageSize;
			var localIdx = 0UL;
			if(page.DataAsyE is not null){
				await foreach(var po in page.DataAsyE){
					localIdx++;
					var uiIdx = startUiIdx + localIdx;
					Rows.Add(new RowWeightArg{
						UiIdx = uiIdx,
						UiIdxText = uiIdx.ToString(),
						Name = po.UniqName ?? "",
						ModifiedTime = FormatBizTime(po),
						Raw = po,
					});
				}
			}
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}

	protected async Task<nil> OnPrevPage(VmPageBar pageBar, CT Ct){
		if(pageBar.PageNum <= 1){
			pageBar.PageNum = 1;
			return NIL;
		}
		pageBar.PageNum--;
		return await Search(Ct);
	}

	protected async Task<nil> OnNextPage(VmPageBar pageBar, CT Ct){
		// TotPageCnt == null means backend does not provide total pages.
		// In this case, do not block paging forward by a fake "1 page" limit.
		if(pageBar.TotPageCnt is u64 totalPage && pageBar.PageNum >= totalPage){
			return NIL;
		}
		pageBar.PageNum++;
		return await Search(Ct);
	}

	// TODO 頁面跳轉邏輯不應放在 Vm層。 Vm不應該引用View層的控件
	public nil OpenDetail(RowWeightArg? row = null){
		var view = new ViewWeightArgEdit();
		view.Ctx?.SetCreateMode(row is null);
		view.Ctx?.FromPoWeightArg(row?.Raw);
		var title = row?.Name ?? Todo.I18n("新增權重參數");
		var titled = ToolView.WithTitle(title, view);
		ViewNavi?.GoTo(titled);
		return NIL;
	}
}
