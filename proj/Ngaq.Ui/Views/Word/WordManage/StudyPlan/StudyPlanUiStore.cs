namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;

public class UiPreFilter: ViewModelBase{
	public str Id{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str UniqName{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str Descr{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str JsonText{ get=>field; set=>SetProperty(ref field, value);} = "{}";
	public UiPreFilter DeepClone(){
		return new UiPreFilter{ Id = Id, UniqName = UniqName, Descr = Descr, JsonText = JsonText };
	}
}

public class UiWeightCalculator: ViewModelBase{
	public str Id{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str UniqName{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str Descr{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str JsonText{ get=>field; set=>SetProperty(ref field, value);} = "{}";
	public UiWeightCalculator DeepClone(){
		return new UiWeightCalculator{ Id = Id, UniqName = UniqName, Descr = Descr, JsonText = JsonText };
	}
}

public class UiWeightArg: ViewModelBase{
	public str Id{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str UniqName{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str Descr{ get=>field; set=>SetProperty(ref field, value);} = "";
	public Dictionary<str, object?> Raw{ get; set; } = [];
	public UiWeightArg DeepClone(){
		return new UiWeightArg{
			Id = Id,
			UniqName = UniqName,
			Descr = Descr,
			Raw = Raw.ToDictionary(x=>x.Key, x=>x.Value),
		};
	}
}

public class UiStudyPlan: ViewModelBase{
	public str Id{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str UniqName{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str Descr{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str PreFilterId{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str WeightCalculatorId{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str WeightArgId{ get=>field; set=>SetProperty(ref field, value);} = "";
	public UiStudyPlan DeepClone(){
		return new UiStudyPlan{
			Id = Id,
			UniqName = UniqName,
			Descr = Descr,
			PreFilterId = PreFilterId,
			WeightCalculatorId = WeightCalculatorId,
			WeightArgId = WeightArgId,
		};
	}
}

public static class StudyPlanUiStore{
	static bool _inited = false;

	public static ObservableCollection<UiPreFilter> PreFilters {get;} = [];
	public static ObservableCollection<UiWeightCalculator> WeightCalculators {get;} = [];
	public static ObservableCollection<UiWeightArg> WeightArgs {get;} = [];
	public static ObservableCollection<UiStudyPlan> StudyPlans {get;} = [];

	public static nil EnsureInit(){
		if(_inited){ return NIL; }
		_inited = true;

		PreFilters.Add(new UiPreFilter{
			Id = "pf-all",
			UniqName = "all",
			Descr = "All words",
			JsonText = "{\n  \"lang\": [\"en\", \"ja\"]\n}",
		});
		PreFilters.Add(new UiPreFilter{
			Id = "pf-jlpt",
			UniqName = "jlpt-only",
			Descr = "Only JLPT-tagged words",
			JsonText = "{\n  \"tag\": [\"jlpt\"]\n}",
		});

		WeightCalculators.Add(new UiWeightCalculator{
			Id = "wc-default",
			UniqName = "default",
			Descr = "Built-in calculator",
			JsonText = "{\n  \"type\": \"builtin\",\n  \"name\": \"default\"\n}",
		});
		WeightCalculators.Add(new UiWeightCalculator{
			Id = "wc-js-v1",
			UniqName = "js-v1",
			Descr = "JavaScript calculator",
			JsonText = "{\n  \"type\": \"js\",\n  \"entry\": \"calc\"\n}",
		});

		WeightArgs.Add(new UiWeightArg{
			Id = "wa-default",
			UniqName = "default",
			Descr = "Default args",
			Raw = new Dictionary<str, object?>{
				["DfltAddBonus"] = 100d,
				["DebuffNumerator"] = 36d,
				["AddCnt_Bonus"] = new List<object?>{ 64d, 128d },
			},
		});

		StudyPlans.Add(new UiStudyPlan{
			Id = "sp-default",
			UniqName = "default",
			Descr = "Default plan",
			PreFilterId = "pf-all",
			WeightCalculatorId = "wc-default",
			WeightArgId = "wa-default",
		});
		return NIL;
	}
}
