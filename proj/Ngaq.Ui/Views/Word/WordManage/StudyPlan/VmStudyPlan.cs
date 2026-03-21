namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;
using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using Ngaq.Core.Tools.JsonMap;
using Ngaq.Ui.Components.KvMap.JsonMap;
using Ngaq.Ui.Infra;
using Tsinswreng.CsTools;

using Ctx = VmStudyPlan;
public partial class VmStudyPlan: ViewModelBase, IMk<Ctx>{
	protected VmStudyPlan(){
		InitSamples();
		ApplyFilter();
		SelectedPlan = FilteredPlans.FirstOrDefault();
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

	public ObservableCollection<PlanDraft> Plans{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=[];

	public ObservableCollection<PlanDraft> FilteredPlans{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=[];

	public PlanDraft? SelectedPlan{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				LoadSelectedToForm();
			}
		}
	}

	public str SearchText{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				ApplyFilter();
			}
		}
	}="";

	public str CurPlanId{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				foreach(var plan in Plans){
					plan.RefreshDisplay(CurPlanId);
				}
			}
		}
	}="";

	public str PlanIdText{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	public str UniqName{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				Touch();
			}
		}
	}="";

	public str Descr{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				Touch();
			}
		}
	}="";

	public str? SelectedPreFilterId{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				Touch();
			}
		}
	}="";

	public str? SelectedWeightCalculatorId{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				Touch();
			}
		}
	}="";

	public str? SelectedWeightArgId{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				Touch();
			}
		}
	}="";

	public ObservableCollection<str> PreFilterOptions{get;set;}=[
		"pf-all",
		"pf-lang-en",
		"pf-lang-ja",
	];

	public ObservableCollection<str> WeightCalculatorOptions{get;set;}=[
		"wcal-default",
		"wcal-js-v1",
	];

	public ObservableCollection<str> WeightArgOptions{get;set;}=[
		"warg-default",
		"warg-hardcore",
	];

	public str PreFilterJson{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				Touch();
			}
		}
	}="{\n  \"lang\": [\"en\"]\n}";

	public str WeightCalculatorJson{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				Touch();
			}
		}
	}="{\n  \"type\": \"js\",\n  \"entry\": \"calc\"\n}";

	public VmUiJsonMap WeightArgMapVm{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=VmUiJsonMap.Mk();

	public str JsonText{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				Touch();
			}
		}
	}="";

	public str LastError{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				OnPropertyChanged(nameof(HasError));
			}
		}
	}="";

	public bool HasError => !str.IsNullOrWhiteSpace(LastError);

	public bool IsDirty{
		get{return field;}
		set{SetProperty(ref field, value);}
	}

	bool _isHydrating = false;
	void Touch(){
		if(_isHydrating){
			return;
		}
		IsDirty = true;
	}

	public nil NewPlan(){
		var plan = new PlanDraft{
			Id = Guid.NewGuid().ToString("N"),
			UniqName = "new-plan",
			Descr = "",
			PreFilterId = PreFilterOptions.FirstOrDefault()??"",
			WeightCalculatorId = WeightCalculatorOptions.FirstOrDefault()??"",
			WeightArgId = WeightArgOptions.FirstOrDefault()??"",
			PreFilterJson = "{\n  \"lang\": [\"en\"]\n}",
			WeightCalculatorJson = "{\n  \"type\": \"js\",\n  \"entry\": \"calc\"\n}",
			WeightArgRaw = MkDefaultWeightArgDict(),
		};
		Plans.Add(plan);
		ApplyFilter();
		SelectedPlan = plan;
		IsDirty = true;
		return NIL;
	}

	public nil CloneSelected(){
		if(SelectedPlan is null){
			ShowMsg("No selected plan.");
			return NIL;
		}
		var neo = SelectedPlan.DeepClone();
		neo.Id = Guid.NewGuid().ToString("N");
		neo.UniqName = $"{SelectedPlan.UniqName}-copy";
		Plans.Add(neo);
		ApplyFilter();
		SelectedPlan = neo;
		IsDirty = true;
		return NIL;
	}

	public nil DeleteSelected(){
		if(SelectedPlan is null){
			ShowMsg("No selected plan.");
			return NIL;
		}
		var id = SelectedPlan.Id;
		Plans.Remove(SelectedPlan);
		ApplyFilter();
		SelectedPlan = FilteredPlans.FirstOrDefault();
		if(CurPlanId == id){
			CurPlanId = "";
		}
		IsDirty = true;
		return NIL;
	}

	public nil SetCurrentSelected(){
		if(SelectedPlan is null){
			ShowMsg("No selected plan.");
			return NIL;
		}
		CurPlanId = SelectedPlan.Id;
		return NIL;
	}

	public nil SaveFormToSelected(){
		if(SelectedPlan is null){
			ShowMsg("No selected plan.");
			return NIL;
		}
		if(!TryBuildDtoFromForm(out var dto, out var err)){
			LastError = err;
			ShowMsg(err);
			return NIL;
		}
		LastError = "";
		SelectedPlan.FromDto(dto!);
		SelectedPlan.RefreshDisplay(CurPlanId);
		SyncJsonFromForm();
		IsDirty = false;
		ShowMsg("Saved in draft.");
		return NIL;
	}

	public nil SyncJsonFromForm(){
		if(!TryBuildDtoFromForm(out var dto, out var err)){
			LastError = err;
			return NIL;
		}
		LastError = "";
		JsonText = JsonSerializer.Serialize(
			dto,
			new JsonSerializerOptions{
				WriteIndented = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			}
		);
		return NIL;
	}

	public nil ApplyJsonToForm(){
		try{
			var dto = JsonSerializer.Deserialize<PlanDto>(JsonText);
			if(dto is null){
				LastError = "Json parse failed.";
				return NIL;
			}
			FillFormFromDto(dto);
			LastError = "";
			IsDirty = true;
		}catch(Exception ex){
			LastError = ex.Message;
		}
		return NIL;
	}

	void LoadSelectedToForm(){
		if(SelectedPlan is null){
			return;
		}
		_isHydrating = true;
		try{
			FillFormFromDto(SelectedPlan.ToDto());
			SyncJsonFromForm();
			IsDirty = false;
			LastError = "";
		}finally{
			_isHydrating = false;
		}
	}

	void FillFormFromDto(PlanDto dto){
		PlanIdText = dto.Id ?? "";
		UniqName = dto.UniqName ?? "";
		Descr = dto.Descr ?? "";
		SelectedPreFilterId = dto.PreFilterId ?? "";
		SelectedWeightCalculatorId = dto.WeightCalculatorId ?? "";
		SelectedWeightArgId = dto.WeightArgId ?? "";
		PreFilterJson = dto.PreFilterJson ?? "{}";
		WeightCalculatorJson = dto.WeightCalculatorJson ?? "{}";
		BindWeightArgMap(dto.WeightArgRaw ?? MkDefaultWeightArgDict());
	}

	bool TryBuildDtoFromForm(out PlanDto? dto, out str err){
		dto = null;
		err = "";
		if(str.IsNullOrWhiteSpace(PlanIdText)){
			err = "PlanId is required.";
			return false;
		}
		if(str.IsNullOrWhiteSpace(UniqName)){
			err = "UniqName is required.";
			return false;
		}

		WeightArgMapVm.UpdData();
		var weightArgRaw = ExtractWeightArgRaw();

		dto = new PlanDto{
			Id = PlanIdText.Trim(),
			UniqName = UniqName.Trim(),
			Descr = Descr,
			PreFilterId = SelectedPreFilterId ?? "",
			WeightCalculatorId = SelectedWeightCalculatorId ?? "",
			WeightArgId = SelectedWeightArgId ?? "",
			PreFilterJson = PreFilterJson,
			WeightCalculatorJson = WeightCalculatorJson,
			WeightArgRaw = weightArgRaw,
		};
		return true;
	}

	Dictionary<str, object?> ExtractWeightArgRaw(){
		var node = WeightArgMapVm.UiJsonMap?.Raw;
		if(node?.ValueObj is IDictionary<str, object?> d){
			return d.ToDictionary(x=>x.Key, x=>x.Value);
		}
		return MkDefaultWeightArgDict();
	}

	void BindWeightArgMap(Dictionary<str, object?> raw){
		var map = new UiJsonMap(){
			Raw = new JsonNode(raw),
		};
		var pathMap = new Dictionary<str, IUiJsonMapItem>();

		pathMap["DfltAddBonus"] = new UiJsonMapItem(map, "DfltAddBonus"){
			JsonValueType = EJsonValueType.Number,
			DisplayName = UiText.FromRawText("Default Add Bonus"),
			Descr = UiText.FromRawText("Base bonus for add event"),
		};
		pathMap["DebuffNumerator"] = new UiJsonMapItem(map, "DebuffNumerator"){
			JsonValueType = EJsonValueType.Number,
			DisplayName = UiText.FromRawText("Debuff Numerator"),
			Descr = UiText.FromRawText("Bigger value means slower decay"),
		};
		pathMap["AddCnt_Bonus[0]"] = new UiJsonMapItem(map, "AddCnt_Bonus[0]"){
			JsonValueType = EJsonValueType.Number,
			DisplayName = UiText.FromRawText("AddCnt Bonus #1"),
			Descr = UiText.FromRawText("Bonus when add-count reaches level 1"),
		};
		pathMap["AddCnt_Bonus[1]"] = new UiJsonMapItem(map, "AddCnt_Bonus[1]"){
			JsonValueType = EJsonValueType.Number,
			DisplayName = UiText.FromRawText("AddCnt Bonus #2"),
			Descr = UiText.FromRawText("Bonus when add-count reaches level 2"),
		};
		map.PathToUiMap = pathMap;
		WeightArgMapVm.FromBo(map);
	}

	static Dictionary<str, object?> MkDefaultWeightArgDict(){
		return new Dictionary<str, object?>(){
			["DfltAddBonus"] = 100d,
			["DebuffNumerator"] = 36d,
			["AddCnt_Bonus"] = new List<object?>(){64d, 128d},
		};
	}

	void ApplyFilter(){
		var q = (SearchText??"").Trim();
		IEnumerable<PlanDraft> seq = Plans;
		if(!str.IsNullOrWhiteSpace(q)){
			seq = seq.Where(x=>
				(x.UniqName?.Contains(q, StringComparison.OrdinalIgnoreCase)??false)
				|| (x.Id?.Contains(q, StringComparison.OrdinalIgnoreCase)??false)
			);
		}
		FilteredPlans = new ObservableCollection<PlanDraft>(seq);
		if(SelectedPlan is not null && !FilteredPlans.Contains(SelectedPlan)){
			SelectedPlan = FilteredPlans.FirstOrDefault();
		}
		foreach(var plan in Plans){
			plan.RefreshDisplay(CurPlanId);
		}
	}

	void InitSamples(){
		var p1 = new PlanDraft{
			Id = "sp-default",
			UniqName = "default",
			Descr = "Default study strategy",
			PreFilterId = "pf-all",
			WeightCalculatorId = "wcal-default",
			WeightArgId = "warg-default",
			PreFilterJson = "{\n  \"lang\": [\"en\", \"ja\"]\n}",
			WeightCalculatorJson = "{\n  \"type\": \"builtin\",\n  \"name\": \"default\"\n}",
			WeightArgRaw = MkDefaultWeightArgDict(),
		};
		var p2 = new PlanDraft{
			Id = "sp-hardcore",
			UniqName = "hardcore",
			Descr = "Faster decay with stricter prefilter",
			PreFilterId = "pf-lang-ja",
			WeightCalculatorId = "wcal-js-v1",
			WeightArgId = "warg-hardcore",
			PreFilterJson = "{\n  \"lang\": [\"ja\"],\n  \"tag\": [\"jlpt\"]\n}",
			WeightCalculatorJson = "{\n  \"type\": \"js\",\n  \"entry\": \"calc_hardcore\"\n}",
			WeightArgRaw = new Dictionary<str, object?>(){
				["DfltAddBonus"] = 80d,
				["DebuffNumerator"] = 18d,
				["AddCnt_Bonus"] = new List<object?>(){32d, 64d},
			},
		};
		Plans = [p1, p2];
		CurPlanId = p1.Id;
		foreach(var p in Plans){
			p.RefreshDisplay(CurPlanId);
		}
	}

	public class PlanDto{
		public str? Id{get;set;}
		public str? UniqName{get;set;}
		public str? Descr{get;set;}
		public str? PreFilterId{get;set;}
		public str? WeightCalculatorId{get;set;}
		public str? WeightArgId{get;set;}
		public str? PreFilterJson{get;set;}
		public str? WeightCalculatorJson{get;set;}
		public Dictionary<str, object?>? WeightArgRaw{get;set;}
	}

	public partial class PlanDraft: ViewModelBase{
		public str Id{
			get{return field;}
			set{SetProperty(ref field, value);}
		}="";
		public str UniqName{
			get{return field;}
			set{
				if(SetProperty(ref field, value)){
					UpdateDisplayName();
				}
			}
		}="";
		public str Descr{
			get{return field;}
			set{SetProperty(ref field, value);}
		}="";
		public str PreFilterId{
			get{return field;}
			set{SetProperty(ref field, value);}
		}="";
		public str WeightCalculatorId{
			get{return field;}
			set{SetProperty(ref field, value);}
		}="";
		public str WeightArgId{
			get{return field;}
			set{SetProperty(ref field, value);}
		}="";
		public str PreFilterJson{
			get{return field;}
			set{SetProperty(ref field, value);}
		}="{}";
		public str WeightCalculatorJson{
			get{return field;}
			set{SetProperty(ref field, value);}
		}="{}";
		public Dictionary<str, object?> WeightArgRaw{get;set;} = [];

		public str DisplayName{
			get{return field;}
			set{SetProperty(ref field, value);}
		}="";

		public void RefreshDisplay(str CurId){
			UpdateDisplayName(CurId);
		}

		void UpdateDisplayName(str CurId = ""){
			var cur = Id == CurId ? "[Current] " : "";
			DisplayName = $"{cur}{UniqName} ({Id})";
		}

		public PlanDto ToDto(){
			return new PlanDto{
				Id = Id,
				UniqName = UniqName,
				Descr = Descr,
				PreFilterId = PreFilterId,
				WeightCalculatorId = WeightCalculatorId,
				WeightArgId = WeightArgId,
				PreFilterJson = PreFilterJson,
				WeightCalculatorJson = WeightCalculatorJson,
				WeightArgRaw = WeightArgRaw.ToDictionary(x=>x.Key, x=>x.Value),
			};
		}

		public nil FromDto(PlanDto dto){
			Id = dto.Id ?? Id;
			UniqName = dto.UniqName ?? "";
			Descr = dto.Descr ?? "";
			PreFilterId = dto.PreFilterId ?? "";
			WeightCalculatorId = dto.WeightCalculatorId ?? "";
			WeightArgId = dto.WeightArgId ?? "";
			PreFilterJson = dto.PreFilterJson ?? "{}";
			WeightCalculatorJson = dto.WeightCalculatorJson ?? "{}";
			WeightArgRaw = dto.WeightArgRaw?.ToDictionary(x=>x.Key, x=>x.Value) ?? [];
			return NIL;
		}

		public PlanDraft DeepClone(){
			var dto = ToDto();
			var neo = new PlanDraft();
			neo.FromDto(dto);
			return neo;
		}
	}
}
