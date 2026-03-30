namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterPage;
using System.Collections.ObjectModel;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.StudyPlan.Models.Po.PreFilter;
using Ngaq.Ui.Components.PageBar;
using Ngaq.Ui.Infra;

using Ctx = VmPreFilterPage;
public partial class VmPreFilterPage: ViewModelBase, IMk<Ctx>{
	protected VmPreFilterPage(){
		PageBar = VmPageBar.Mk();
		PageBar.PageSize = 10;
		PageBar.FnPrevPage = OnPrevPage;
		PageBar.FnNextPage = OnNextPage;
		InitDemoData();
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

	public VmPageBar PageBar{get;set;}

	public str Input{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	public ObservableCollection<RowPreFilter> Rows{get;set;} = [];

	protected IList<PoPreFilter> AllPreFilter{get;set;} = [];

	public class RowPreFilter{
		public bool IsChecked{get;set;} = false;
		public u64 UiIdx{get;set;}
		public str UiIdxText{get;set;} = "";
		public str Name{get;set;} = "";
		public str Type{get;set;} = "";
		public str ModifiedTime{get;set;} = "";
		public PoPreFilter? Raw{get;set;} = null;
	}

	protected nil InitDemoData(){
		var now = Tempus.Now();
		var l = new List<PoPreFilter>();
		for(var i = 1; i <= 45; i++){
			var created = (Tempus)(now.Value - i*InMillisecond.Hour);
			var updated = i % 3 == 0 ? Tempus.Zero : (Tempus)(created.Value + 10*InMillisecond.Minute);
			l.Add(new PoPreFilter{
				UniqName = $"PreFilter_{i:000}",
				Type = EPreFilterType.Json,
				BizCreatedAt = created,
				BizUpdatedAt = updated,
			});
		}
		AllPreFilter = l;
		return NIL;
	}

	protected static str FormatBizTime(PoPreFilter po){
		var updated = po.BizUpdatedAt == Tempus.Zero ? po.BizCreatedAt : po.BizUpdatedAt;
		if(updated == Tempus.Zero){
			return "-";
		}
		return updated.ToIso();
	}

	protected nil CalcTotalPage(u64 totalCount){
		var pageSize = PageBar.PageSize == 0 ? 10 : PageBar.PageSize;
		var totalPage = totalCount == 0 ? 1 : (totalCount + pageSize - 1) / pageSize;
		PageBar.TotPageCnt = totalPage;
		return NIL;
	}

	public async Task<nil> InitSearch(CT Ct = default){
		PageBar.PageNum = 1;
		return await Search(Ct);
	}

	public async Task<nil> Search(CT Ct = default){
		await Task.Yield();
		IEnumerable<PoPreFilter> q = AllPreFilter;
		if(!str.IsNullOrWhiteSpace(Input)){
			q = q.Where(x=>(x.UniqName??"").Contains(Input, StringComparison.OrdinalIgnoreCase));
		}
		var pageNum = PageBar.PageNum <= 1 ? 1 : PageBar.PageNum;
		var pageSize = PageBar.PageSize == 0 ? 10 : PageBar.PageSize;
		var start = (pageNum - 1) * pageSize;
		var end = start + pageSize;
		u64 idx = 0;
		var onePage = new List<PoPreFilter>();
		foreach(var po in q){
			if(idx >= start && idx < end){
				onePage.Add(po);
			}
			idx++;
		}
		CalcTotalPage(idx);
		var totalPage = PageBar.TotPageCnt ?? 1;
		if(pageNum > totalPage){
			PageBar.PageNum = totalPage;
			return await Search(Ct);
		}
		Rows.Clear();
		for(var i = 0; i < onePage.Count; i++){
			var po = onePage[i];
			var uiIdx = start + (u64)i + 1;
			Rows.Add(new RowPreFilter{
				UiIdx = uiIdx,
				UiIdxText = uiIdx.ToString(),
				Name = po.UniqName ?? "",
				Type = po.Type.ToString(),
				ModifiedTime = FormatBizTime(po),
				Raw = po,
			});
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
}