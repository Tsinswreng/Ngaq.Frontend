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

using Ctx = VmNormLangPage;using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

public partial class VmNormLangPage: ViewModelBase, IMk<Ctx>{
	void Init(){
		PageBar = VmPageBar.Mk();
		PageBar.PageSize = 20;
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

	public bool CanCreate => true;

	public bool IsSelectMode{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = false;

	Action<PoNormLang>? FnOnSelected{get;set;}

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
		if(IsSelectMode){
			if(Row?.Raw is not null){
				FnOnSelected?.Invoke(Row.Raw);
			}
			return NIL;
		}
		OnOpenDetailRequested?.Invoke(Row);
		return NIL;
	}

	public nil SetSelectMode(Action<PoNormLang> FnOnSelected){
		IsSelectMode = true;
		this.FnOnSelected = FnOnSelected;
		return NIL;
	}

	public async Task<nil> InitBuiltinNormLang(CT Ct = default){
		if(AnyNull(SvcNormLang, UserCtxMgr)){
			return NIL;
		}
		try{
			await SvcNormLang.InitBuiltinNormLang(UserCtxMgr.GetDbUserCtx(), Ct);
			ShowDialog(I18n[K.InitializedBuiltinNormLang]);
			return await InitSearch(Ct);
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}

	public event Action<RowNormLang?>? OnOpenDetailRequested;
}

