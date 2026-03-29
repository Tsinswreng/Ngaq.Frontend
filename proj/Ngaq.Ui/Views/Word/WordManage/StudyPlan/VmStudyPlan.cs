namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;
using System.Collections.ObjectModel;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.StudyPlan.Models.Po.WeightArg;
using Ngaq.Ui.Components.PageBar;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanEdit;

using Ctx = VmStudyPlan;
public partial class VmStudyPlan: ViewModelBase, IMk<Ctx>{
	protected VmStudyPlan(){
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
	static VmStudyPlan(){
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

	public ObservableCollection<RowWeightArg> Rows{get;set;} = [];

	protected IList<PoWeightArg> AllWeightArg{get;set;} = [];

	public class RowWeightArg{
		public bool IsChecked{get;set;} = false;
		public u64 UiIdx{get;set;}
		public str UiIdxText{get;set;} = "";
		public str Name{get;set;} = "";
		public str ModifiedTime{get;set;} = "";
		public PoWeightArg? Raw{get;set;} = null;
	}

	protected nil InitDemoData(){
		var now = Tempus.Now();
		var l = new List<PoWeightArg>();
		for(var i = 1; i <= 38; i++){
			var created = (Tempus)(now.Value - i*InMillisecond.Hour);
			var updated = i % 4 == 0 ? Tempus.Zero : (Tempus)(created.Value + 15*InMillisecond.Minute);
			l.Add(new PoWeightArg{
				UniqName = $"WeightArg_{i:000}",
				BizCreatedAt = created,
				BizUpdatedAt = updated,
			});
		}
		AllWeightArg = l;
		return NIL;
	}

	protected static str FormatBizTime(PoWeightArg po){
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
		IEnumerable<PoWeightArg> q = AllWeightArg;
		if(!str.IsNullOrWhiteSpace(Input)){
			q = q.Where(x=>(x.UniqName??"").Contains(Input, StringComparison.OrdinalIgnoreCase));
		}
		var pageNum = PageBar.PageNum <= 1 ? 1 : PageBar.PageNum;
		var pageSize = PageBar.PageSize == 0 ? 10 : PageBar.PageSize;
		var start = (pageNum - 1) * pageSize;
		var end = start + pageSize;
		u64 idx = 0;
		var onePage = new List<PoWeightArg>();
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
			Rows.Add(new RowWeightArg{
				UiIdx = uiIdx,
				UiIdxText = uiIdx.ToString(),
				Name = po.UniqName ?? "",
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

	protected async Task<nil> OnGoPage(VmPageBar pageBar, CT Ct){
		var totalPage = pageBar.TotPageCnt ?? 1;
		var target = pageBar.PageNum <= 1 ? 1 : pageBar.PageNum;
		if(target > totalPage){
			target = totalPage;
		}
		pageBar.PageNum = target;
		return await Search(Ct);
	}

	public nil OpenDetail(RowWeightArg? row = null){
		var view = new ViewStudyPlanEdit();
		var title = row?.Name ?? "新增權重參數";
		var titled = ToolView.WithTitle(title, view);
		ViewNavi?.GoTo(titled);
		return NIL;
	}
}
