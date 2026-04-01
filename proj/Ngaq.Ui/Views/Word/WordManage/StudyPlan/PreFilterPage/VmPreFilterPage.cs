namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterPage;

using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.StudyPlan.Models.Po.PreFilter;
using Ngaq.Core.Shared.StudyPlan.Models.Req;
using Ngaq.Core.Shared.StudyPlan.Svc;
using Ngaq.Ui.Components.PageBar;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using Ctx = VmPreFilterPage;

/// <summary>
/// PreFilter 列表頁 ViewModel。
/// 使用後端接口分頁讀取，不再使用假數據。
/// </summary>
public partial class VmPreFilterPage: ViewModelBase, IMk<Ctx>{
	protected VmPreFilterPage(){
		PageBar = VmPageBar.Mk();
		PageBar.PageSize = 10;
		PageBar.FnPrevPage = OnPrevPage;
		PageBar.FnNextPage = OnNextPage;
	}

	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmPreFilterPage(){
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
	public VmPreFilterPage(
		ISvcStudyPlan? SvcStudyPlan
		,IFrontendUserCtxMgr? UserCtxMgr
	):this(){
		this.SvcStudyPlan = SvcStudyPlan;
		this.UserCtxMgr = UserCtxMgr;
	}

	public VmPageBar PageBar{get;set;}

	public str Input{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public ObservableCollection<RowPreFilter> Rows{get;set;} = [];

	/// <summary>
	/// 列表行模型。
	/// </summary>
	public class RowPreFilter{
		public bool IsChecked{get;set;} = false;
		public u64 UiIdx{get;set;}
		public str UiIdxText{get;set;} = "";
		public str Name{get;set;} = "";
		public str Type{get;set;} = "";
		public str ModifiedTime{get;set;} = "";
		public PoPreFilter? Raw{get;set;} = null;
	}

	static str FormatBizTime(PoPreFilter Po){
		var updated = Po.BizUpdatedAt == Tempus.Zero ? Po.BizCreatedAt : Po.BizUpdatedAt;
		if(updated == Tempus.Zero){
			return "-";
		}
		return updated.ToIso();
	}

	public async Task<nil> InitSearch(CT Ct = default){
		PageBar.PageNum = 1;
		return await Search(Ct);
	}

	/// <summary>
	/// 根據輸入條件查詢後端分頁，並回填 UI 列表。
	/// </summary>
	public async Task<nil> Search(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			ShowMsg("Service not ready");
			return NIL;
		}
		try{
			var pageQry = PageBar.ToPageQry();
			pageQry.WantTotCnt = true;
			var req = new ReqPagePreFilter{
				PageQry = pageQry,
				UniqNameSearch = str.IsNullOrWhiteSpace(Input) ? null : Input.Trim(),
			};

			var page = await SvcStudyPlan.PagePreFilter(UserCtxMgr.GetDbUserCtx(), req, Ct);
			PageBar.FromPageResultInfo(page);

			Rows.Clear();
			var startUiIdx = page.PageIdx * page.PageSize;
			var localIdx = 0UL;
			if(page.DataAsyE is not null){
				await foreach(var po in page.DataAsyE){
					localIdx++;
					var uiIdx = startUiIdx + localIdx;
					Rows.Add(new RowPreFilter{
						UiIdx = uiIdx,
						UiIdxText = uiIdx.ToString(),
						Name = po.UniqName ?? "",
						Type = po.Type.ToString(),
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
		var totalPage = pageBar.TotPageCnt ?? 1;
		if(pageBar.PageNum >= totalPage){
			return NIL;
		}
		pageBar.PageNum++;
		return await Search(Ct);
	}

	// TODO 頁面跳轉邏輯不應放在 Vm層。 Vm不應該引用View層的控件
	// 檢查 VmWeightArgPage是否有同樣問題。
	public nil OpenDetail(RowPreFilter? row = null){
		var view = new ViewPreFilterVisualEdit();
		view.Ctx?.FromPoPreFilter(row?.Raw);
		var title = row?.Name ?? "新增預篩選器";
		var titled = ToolView.WithTitle(title, view);
		ViewNavi?.GoTo(titled);
		return NIL;
	}
}
