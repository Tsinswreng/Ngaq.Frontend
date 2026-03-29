namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;
using System.Collections.ObjectModel;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.StudyPlan.Models.Po.WeightArg;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanEdit;

using Ctx = VmStudyPlan;
public partial class VmStudyPlan: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmStudyPlan(){
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


	public str Input{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	public str CurPageInput{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "1";

	public str TotalPageText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "1";

	public u64 PageSize{get;set;} = 10;

	public u64 PageIdx{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				OnPropertyChanged(nameof(PageDisplay));
			}
		}
	}

	public str PageDisplay => (PageIdx+1).ToString();

	public ObservableCollection<RowWeightArg> Rows{get;set;} = [];

	protected IList<PoWeightArg> AllWeightArg{get;set;} = [];

	public class RowWeightArg{
		public bool IsChecked{get;set;} = false;
		public u64 UiIdx{get;set;}
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
		var totalPage = totalCount == 0 ? 1 : (totalCount + PageSize - 1) / PageSize;
		TotalPageText = totalPage.ToString();
		return NIL;
	}

	public async Task<nil> InitSearchAsy(CT Ct = default){
		PageIdx = 0;
		return await SearchAsy(Ct);
	}

	public async Task<nil> SearchAsy(CT Ct = default){
		await Task.Yield();
		IEnumerable<PoWeightArg> q = AllWeightArg;
		if(!str.IsNullOrWhiteSpace(Input)){
			q = q.Where(x=>(x.UniqName??"").Contains(Input, StringComparison.OrdinalIgnoreCase));
		}
		var start = PageIdx*PageSize;
		var end = start + PageSize;
		u64 idx = 0;
		var onePage = new List<PoWeightArg>();
		foreach(var po in q){
			if(idx >= start && idx < end){
				onePage.Add(po);
			}
			idx++;
		}
		CalcTotalPage(idx);
		var totalPage = u64.Parse(TotalPageText);
		if(PageIdx >= totalPage){
			PageIdx = totalPage - 1;
			return await SearchAsy(Ct);
		}
		Rows.Clear();
		for(var i = 0; i < onePage.Count; i++){
			var po = onePage[i];
			Rows.Add(new RowWeightArg{
				UiIdx = start + (u64)i + 1,
				Name = po.UniqName ?? "",
				ModifiedTime = FormatBizTime(po),
				Raw = po,
			});
		}
		CurPageInput = (PageIdx+1).ToString();
		return NIL;
	}

	public nil PrevPage(){
		if(PageIdx == 0){
			return NIL;
		}
		PageIdx--;
		_ = SearchAsy();
		return NIL;
	}

	public nil NextPage(){
		var totalPage = u64.TryParse(TotalPageText, out var parsed) ? parsed : 1;
		if(PageIdx + 1 >= totalPage){
			return NIL;
		}
		PageIdx++;
		_ = SearchAsy();
		return NIL;
	}

	public nil GoInputPage(){
		if(!u64.TryParse(CurPageInput, out var pageNo)){
			return NIL;
		}
		if(pageNo <= 1){
			PageIdx = 0;
			_ = SearchAsy();
			return NIL;
		}
		PageIdx = pageNo - 1;
		_ = SearchAsy();
		return NIL;
	}

	public nil OpenDetail(RowWeightArg? row = null){
		var view = new ViewStudyPlanEdit();
		var title = row?.Name ?? "新增權重參數";
		var titled = ToolView.WithTitle(title, view);
		ViewNavi?.GoTo(titled);
		return NIL;
	}


}
