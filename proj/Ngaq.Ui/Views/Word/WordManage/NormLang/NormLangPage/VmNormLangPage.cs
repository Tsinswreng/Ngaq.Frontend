namespace Ngaq.Ui.Views.Word.WordManage.NormLang.NormLangPage;

using System;
using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.Dictionary.Models.Po.NormLang;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Ui.Components.PageBar;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using Ctx = VmNormLangPage;

/// NormLang 列表頁 ViewModel。
/// 負責分頁查詢與打開編輯頁事件派發。
public partial class VmNormLangPage: ViewModelBase, IMk<Ctx>{
	void Init(){
		PageBar = VmPageBar.Mk();
		PageBar.PageSize = 10;
		PageBar.FnPrevPage = OnPrevPage;
		PageBar.FnNextPage = OnNextPage;
	}

	protected VmNormLangPage(){
		Init();
	}

	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmNormLangPage(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	ISvcNormLang? SvcNormLang{get;set;}
	IFrontendUserCtxMgr? UserCtxMgr{get;set;}

	public VmNormLangPage(
		ISvcNormLang? SvcNormLang
		,IFrontendUserCtxMgr? UserCtxMgr
	):this(){
		this.SvcNormLang = SvcNormLang;
		this.UserCtxMgr = UserCtxMgr;
		Init();
	}

	public VmPageBar PageBar{get;set;} = null!;

	public str Input{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public ObservableCollection<RowNormLang> Rows{get;set;} = [];

	/// 當前頁允許新增。
	public bool CanCreate => true;

	public class RowNormLang{
		public u64 UiIdx{get;set;}
		public str UiIdxText{get;set;} = "";
		public str Type{get;set;} = "";
		public str Code{get;set;} = "";
		public str NativeName{get;set;} = "";
		public str ModifiedTime{get;set;} = "";
		public PoNormLang? Raw{get;set;} = null;
	}

	public async Task<nil> InitSearch(CT Ct){
		PageBar.PageNum = 1;
		return await Search(Ct);
	}

	/// 按 Code 模糊查詢 NormLang 分頁。
	public async Task<nil> Search(CT Ct = default){
		if(AnyNull(SvcNormLang, UserCtxMgr)){
			return NIL;
		}
		try{
			var pageQry = PageBar.ToPageQry();
			pageQry.WantTotCnt = true;
			var req = new ReqPageNormLang{
				PageQry = pageQry,
				Code = str.IsNullOrWhiteSpace(Input) ? null : Input.Trim(),
			};

			var page = await SvcNormLang.PageNormLang(UserCtxMgr.GetDbUserCtx(), req, Ct);
			PageBar.FromPageResultInfo(page);

			Rows.Clear();
			var startUiIdx = page.PageIdx * page.PageSize;
			var localIdx = 0UL;
			if(page.DataAsyE is not null){
				await foreach(var po in page.DataAsyE){
					localIdx++;
					var uiIdx = startUiIdx + localIdx;
					Rows.Add(new RowNormLang{
						UiIdx = uiIdx,
						UiIdxText = uiIdx.ToString(),
						Type = po.Type.ToString(),
						Code = po.Code ?? "",
						NativeName = po.NativeName ?? "",
						ModifiedTime = ToolStudyPlanView.FormatUpdatedDateShort(po.BizUpdatedAt, po.BizCreatedAt),
						Raw = po,
					});
				}
			}
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}

	protected async Task<nil> OnPrevPage(VmPageBar PageBar, CT Ct){
		if(PageBar.PageNum <= 1){
			PageBar.PageNum = 1;
			return NIL;
		}
		PageBar.PageNum--;
		return await Search(Ct);
	}

	protected async Task<nil> OnNextPage(VmPageBar PageBar, CT Ct){
		if(PageBar.TotPageCnt is u64 TotalPage && TotalPage > 0 && PageBar.PageNum >= TotalPage){
			return NIL;
		}
		PageBar.PageNum++;
		return await Search(Ct);
	}

	public nil OpenDetail(RowNormLang? Row = null){
		OnOpenDetailRequested?.Invoke(Row);
		return NIL;
	}

	/// 初始化內置語言，完成後刷新當前分頁。
	public async Task<nil> InitBuiltinNormLang(CT Ct = default){
		if(AnyNull(SvcNormLang, UserCtxMgr)){
			return NIL;
		}
		try{
			await SvcNormLang.InitBuiltinNormLang(UserCtxMgr.GetDbUserCtx(), Ct);
			ShowMsg(Todo.I18n("Initialized builtin norm lang"));
			return await InitSearch(Ct);
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}

	/// 由 View 層監聽，收到後在 View 中完成導航。
	public event Action<RowNormLang?>? OnOpenDetailRequested;
}
