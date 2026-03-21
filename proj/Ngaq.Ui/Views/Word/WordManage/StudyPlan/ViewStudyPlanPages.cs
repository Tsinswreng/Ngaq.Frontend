namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

public partial class VmStudyPlanQuery: ViewModelBase, IMk<VmStudyPlanQuery>{
	protected VmStudyPlanQuery(){ StudyPlanUiStore.EnsureInit(); Refresh(); }
	public static VmStudyPlanQuery Mk()=>new();

	public str SearchText{ get=>field; set{ if(SetProperty(ref field,value)){ PageIdx=0; Refresh(); } } }="";
	public i32 PageIdx{ get=>field; set{ if(SetProperty(ref field,value)){ Refresh(); OnPropertyChanged(nameof(PageText)); } } }=0;
	public i32 PageSize{get;set;} = 8;
	public ObservableCollection<UiStudyPlan> PageItems{ get=>field; set=>SetProperty(ref field,value);}=[];
	public i32 Total{ get=>field; set=>SetProperty(ref field,value);} = 0;
	public str PageText => $"Page {PageIdx+1}";

	public nil Prev(){ if(PageIdx>0){ PageIdx--; } return NIL; }
	public nil Next(){ if((PageIdx+1)*PageSize < Total){ PageIdx++; } return NIL; }

	public nil Refresh(){
		IEnumerable<UiStudyPlan> seq = StudyPlanUiStore.StudyPlans;
		var q = (SearchText??"").Trim();
		if(!str.IsNullOrWhiteSpace(q)){
			seq = seq.Where(x=>x.Id.Contains(q, StringComparison.OrdinalIgnoreCase)
				|| x.UniqName.Contains(q, StringComparison.OrdinalIgnoreCase)
				|| x.Descr.Contains(q, StringComparison.OrdinalIgnoreCase));
		}
		var list = seq.ToList();
		Total = list.Count;
		PageItems = new ObservableCollection<UiStudyPlan>(list.Skip(PageIdx*PageSize).Take(PageSize));
		OnPropertyChanged(nameof(PageText));
		return NIL;
	}
}

public partial class VmStudyPlanEdit: ViewModelBase, IMk<VmStudyPlanEdit>{
	protected VmStudyPlanEdit(){
		StudyPlanUiStore.EnsureInit();
		RefreshOptions();
	}
	public static VmStudyPlanEdit Mk()=>new();

	public str Id{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str UniqName{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str Descr{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str? SelectedPreFilterId{ get=>field; set{ if(SetProperty(ref field, value)){ RefreshAssemblyPreview(); } } } = "";
	public str? SelectedWeightCalculatorId{ get=>field; set{ if(SetProperty(ref field, value)){ RefreshAssemblyPreview(); } } } = "";
	public str? SelectedWeightArgId{ get=>field; set{ if(SetProperty(ref field, value)){ RefreshAssemblyPreview(); } } } = "";

	public ObservableCollection<str> PreFilterOptions{ get=>field; set=>SetProperty(ref field, value);}=[];
	public ObservableCollection<str> WeightCalculatorOptions{ get=>field; set=>SetProperty(ref field, value);}=[];
	public ObservableCollection<str> WeightArgOptions{ get=>field; set=>SetProperty(ref field, value);}=[];

	public str AssemblyPreview{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str LastError{ get=>field; set{ if(SetProperty(ref field, value)){ OnPropertyChanged(nameof(HasError)); } } } = "";
	public bool HasError => !str.IsNullOrWhiteSpace(LastError);
	public str JsonText{ get=>field; set=>SetProperty(ref field, value);} = "{}";

	public nil Load(UiStudyPlan? src){
		RefreshOptions();
		if(src is null){
			Id = Guid.NewGuid().ToString("N");
			UniqName = "";
			Descr = "";
			SelectedPreFilterId = PreFilterOptions.FirstOrDefault();
			SelectedWeightCalculatorId = WeightCalculatorOptions.FirstOrDefault();
			SelectedWeightArgId = WeightArgOptions.FirstOrDefault();
			SyncJsonFromForm();
			RefreshAssemblyPreview();
			return NIL;
		}
		Id = src.Id;
		UniqName = src.UniqName;
		Descr = src.Descr;
		SelectedPreFilterId = src.PreFilterId;
		SelectedWeightCalculatorId = src.WeightCalculatorId;
		SelectedWeightArgId = src.WeightArgId;
		SyncJsonFromForm();
		RefreshAssemblyPreview();
		return NIL;
	}

	public nil Save(){
		if(!Validate(out var err)){
			LastError = err;
			ShowMsg(err);
			return NIL;
		}
		var got = StudyPlanUiStore.StudyPlans.FirstOrDefault(x=>x.Id == Id);
		if(got is null){
			StudyPlanUiStore.StudyPlans.Add(new UiStudyPlan{
				Id = Id.Trim(),
				UniqName = UniqName.Trim(),
				Descr = Descr,
				PreFilterId = SelectedPreFilterId??"",
				WeightCalculatorId = SelectedWeightCalculatorId??"",
				WeightArgId = SelectedWeightArgId??"",
			});
		}else{
			got.UniqName = UniqName.Trim();
			got.Descr = Descr;
			got.PreFilterId = SelectedPreFilterId??"";
			got.WeightCalculatorId = SelectedWeightCalculatorId??"";
			got.WeightArgId = SelectedWeightArgId??"";
		}
		LastError = "";
		SyncJsonFromForm();
		RefreshAssemblyPreview();
		ShowMsg("StudyPlan 已保存");
		return NIL;
	}

	public nil Delete(){
		var got = StudyPlanUiStore.StudyPlans.FirstOrDefault(x=>x.Id == Id);
		if(got is not null){
			StudyPlanUiStore.StudyPlans.Remove(got);
			ShowMsg("StudyPlan 已刪除");
		}
		return NIL;
	}

	public nil SyncJsonFromForm(){
		var dto = new Dto{
			Id = Id,
			UniqName = UniqName,
			Descr = Descr,
			PreFilterId = SelectedPreFilterId,
			WeightCalculatorId = SelectedWeightCalculatorId,
			WeightArgId = SelectedWeightArgId,
		};
		JsonText = JsonSerializer.Serialize(dto, new JsonSerializerOptions{ WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
		return NIL;
	}

	public nil ApplyJsonToForm(){
		try{
			var dto = JsonSerializer.Deserialize<Dto>(JsonText);
			if(dto is null){
				LastError = "Json parse failed.";
				return NIL;
			}
			Id = dto.Id ?? "";
			UniqName = dto.UniqName ?? "";
			Descr = dto.Descr ?? "";
			SelectedPreFilterId = dto.PreFilterId ?? "";
			SelectedWeightCalculatorId = dto.WeightCalculatorId ?? "";
			SelectedWeightArgId = dto.WeightArgId ?? "";
			RefreshAssemblyPreview();
			LastError = "";
		}catch(Exception ex){
			LastError = ex.Message;
		}
		return NIL;
	}

	public nil RefreshOptions(){
		PreFilterOptions = new ObservableCollection<str>(StudyPlanUiStore.PreFilters.Select(x=>x.Id));
		WeightCalculatorOptions = new ObservableCollection<str>(StudyPlanUiStore.WeightCalculators.Select(x=>x.Id));
		WeightArgOptions = new ObservableCollection<str>(StudyPlanUiStore.WeightArgs.Select(x=>x.Id));
		return NIL;
	}

	public nil RefreshAssemblyPreview(){
		var pf = StudyPlanUiStore.PreFilters.FirstOrDefault(x=>x.Id == (SelectedPreFilterId??""));
		var wc = StudyPlanUiStore.WeightCalculators.FirstOrDefault(x=>x.Id == (SelectedWeightCalculatorId??""));
		var wa = StudyPlanUiStore.WeightArgs.FirstOrDefault(x=>x.Id == (SelectedWeightArgId??""));
		AssemblyPreview =
			$"PreFilter: {(pf?.UniqName ?? "(missing)")}\n"
			+ $"WeightCalculator: {(wc?.UniqName ?? "(missing)")}\n"
			+ $"WeightArg: {(wa?.UniqName ?? "(missing)")}";
		return NIL;
	}

	bool Validate(out str err){
		err = "";
		if(str.IsNullOrWhiteSpace(Id) || str.IsNullOrWhiteSpace(UniqName)){
			err = "Id / UniqName 不能為空";
			return false;
		}
		if(StudyPlanUiStore.PreFilters.All(x=>x.Id != (SelectedPreFilterId??""))){
			err = "PreFilter 不存在，請先建立或重新選擇";
			return false;
		}
		if(StudyPlanUiStore.WeightCalculators.All(x=>x.Id != (SelectedWeightCalculatorId??""))){
			err = "WeightCalculator 不存在，請先建立或重新選擇";
			return false;
		}
		if(StudyPlanUiStore.WeightArgs.All(x=>x.Id != (SelectedWeightArgId??""))){
			err = "WeightArg 不存在，請先建立或重新選擇";
			return false;
		}
		return true;
	}

	class Dto{
		public str? Id{get;set;}
		public str? UniqName{get;set;}
		public str? Descr{get;set;}
		public str? PreFilterId{get;set;}
		public str? WeightCalculatorId{get;set;}
		public str? WeightArgId{get;set;}
	}
}

public partial class ViewStudyPlanQuery: AppViewBase{
	public VmStudyPlanQuery? Ctx{ get=>DataContext as VmStudyPlanQuery; set=>DataContext = value; }
	public ViewStudyPlanQuery(){ Ctx = VmStudyPlanQuery.Mk(); Render(); }

	protected nil Render(){
		var root = new AutoGrid(IsRow:true);
		root.Grid.RowDefinitions.AddRange([ RowDef(1,GUT.Auto), RowDef(8,GUT.Star), RowDef(1,GUT.Auto) ]);
		this.Content = root.Grid;
		root.AddInit(MkTop(), o=>{});
		root.AddInit(new ScrollViewer{ Content = MkList(), Margin = new Thickness(8,0,8,4) }, o=>{});
		root.AddInit(MkPageBar(), o=>{});
		return NIL;
	}

	Control MkTop(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.Margin = new Thickness(8,8,8,4);
		g.Grid.ColumnDefinitions.AddRange([ ColDef(7,GUT.Star), ColDef(3,GUT.Star) ]);
		g.AddInit(new TextBox{ Watermark = "Search StudyPlan" }, o=>o.Bind(o.PropText, CBE.Mk<VmStudyPlanQuery>(x=>x.SearchText, Mode:BindingMode.TwoWay)));
		g.AddInit(new Button{ Content = "New", MinHeight = 36 }, o=>o.Click += (s,e)=>OpenEdit(null));
		return g.Grid;
	}

	Control MkList(){
		var items = new ItemsControl();
		items.Bind(items.PropItemsSource, CBE.Mk<VmStudyPlanQuery>(x=>x.PageItems));
		items.SetItemsPanel(()=>new StackPanel{ Spacing = 6 });
		items.SetItemTemplate<UiStudyPlan>((row, ns)=>{
			var b = new Border{ BorderThickness = new Thickness(1), Padding = new Thickness(8) };
			var g = new AutoGrid(IsRow:true);
			g.Grid.RowDefinitions.AddRange([ RowDef(1,GUT.Auto), RowDef(1,GUT.Auto), RowDef(1,GUT.Auto), RowDef(1,GUT.Auto) ]);
			g.AddInit(new TextBlock{ Text = $"{row.UniqName} ({row.Id})" }, o=>{});
			g.AddInit(new TextBlock{ Text = row.Descr, TextWrapping = TextWrapping.Wrap }, o=>{});
			g.AddInit(new TextBlock{ Text = $"Assemble: {row.PreFilterId} / {row.WeightCalculatorId} / {row.WeightArgId}", Foreground = Brushes.LightGray, TextWrapping = TextWrapping.Wrap }, o=>{});
			g.AddInit(new Button{ Content = "Edit", MinHeight = 34 }, o=>o.Click += (s,e)=>OpenEdit(row));
			b.Child = g.Grid;
			return b;
		});
		return items;
	}

	Control MkPageBar(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.Margin = new Thickness(8,0,8,8);
		g.Grid.ColumnDefinitions.AddRange([ ColDef(1,GUT.Auto), ColDef(1,GUT.Auto), ColDef(1,GUT.Auto) ]);
		g.AddInit(new Button{ Content="<", MinHeight = 34 }, o=>o.Click+=(s,e)=>Ctx?.Prev());
		g.AddInit(new TextBlock{ VerticalAlignment = VAlign.Center }, o=>o.Bind(o.PropText, CBE.Mk<VmStudyPlanQuery>(x=>x.PageText)));
		g.AddInit(new Button{ Content=">", MinHeight = 34 }, o=>o.Click+=(s,e)=>Ctx?.Next());
		return g.Grid;
	}

	void OpenEdit(UiStudyPlan? row){
		var v = new ViewStudyPlanEdit();
		v.Ctx?.Load(row);
		Ctx?.ViewNavi?.GoTo(ToolView.WithTitle("StudyPlan Edit", v));
	}
}

public partial class ViewStudyPlanEdit: AppViewBase{
	public VmStudyPlanEdit? Ctx{ get=>DataContext as VmStudyPlanEdit; set=>DataContext = value; }
	public ViewStudyPlanEdit(){ Ctx = VmStudyPlanEdit.Mk(); Render(); }

	protected nil Render(){
		var root = new AutoGrid(IsRow:true);
		root.Grid.RowDefinitions.AddRange([ RowDef(8,GUT.Star), RowDef(1,GUT.Auto), RowDef(1,GUT.Auto) ]);
		this.Content = root.Grid;

		var tab = new TabControl();
		tab.Items.AddInit(new TabItem(), o=>{ o.Header = "Form"; o.Content = new ScrollViewer{ Content = MkForm(), Margin = new Thickness(8,8,8,4) }; });
		tab.Items.AddInit(new TabItem(), o=>{ o.Header = "Json"; o.Content = MkJsonTab(); });

		root.AddInit(tab, o=>{});
		root.AddInit(MkErrBar(), o=>{});
		root.AddInit(MkOps(), o=>{});
		return NIL;
	}

	Control MkForm(){
		var sp = new StackPanel{ Spacing = 8 };
		sp.Children.Add(MkInput("Id", CBE.Mk<VmStudyPlanEdit>(x=>x.Id, Mode:BindingMode.TwoWay), true));
		sp.Children.Add(MkInput("UniqName", CBE.Mk<VmStudyPlanEdit>(x=>x.UniqName, Mode:BindingMode.TwoWay)));
		sp.Children.Add(MkInput("Descr", CBE.Mk<VmStudyPlanEdit>(x=>x.Descr, Mode:BindingMode.TwoWay)));
		sp.Children.Add(MkCombo("PreFilter", CBE.Mk<VmStudyPlanEdit>(x=>x.PreFilterOptions), CBE.Mk<VmStudyPlanEdit>(x=>x.SelectedPreFilterId, Mode:BindingMode.TwoWay)));
		sp.Children.Add(MkCombo("WeightCalculator", CBE.Mk<VmStudyPlanEdit>(x=>x.WeightCalculatorOptions), CBE.Mk<VmStudyPlanEdit>(x=>x.SelectedWeightCalculatorId, Mode:BindingMode.TwoWay)));
		sp.Children.Add(MkCombo("WeightArg", CBE.Mk<VmStudyPlanEdit>(x=>x.WeightArgOptions), CBE.Mk<VmStudyPlanEdit>(x=>x.SelectedWeightArgId, Mode:BindingMode.TwoWay)));
		sp.Children.Add(new TextBlock{ Text = "Assembly Preview" });
		sp.AddInit(new TextBox{ IsReadOnly = true, AcceptsReturn = true, Height = 90, TextWrapping = TextWrapping.Wrap }, o=>o.Bind(o.PropText, CBE.Mk<VmStudyPlanEdit>(x=>x.AssemblyPreview, Mode:BindingMode.OneWay)));
		return sp;
	}

	Control MkCombo(str label, IBinding items, IBinding selected){
		var sp = new StackPanel{ Spacing = 2 };
		sp.Children.Add(new TextBlock{ Text = label });
		var cb = new ComboBox();
		cb.Bind(cb.PropItemsSource, items);
		cb.Bind(cb.PropSelectedItem, selected);
		sp.Children.Add(cb);
		return sp;
	}

	Control MkJsonTab(){
		var g = new AutoGrid(IsRow:true);
		g.Grid.RowDefinitions.AddRange([ RowDef(1,GUT.Auto), RowDef(8,GUT.Star) ]);
		var op = new AutoGrid(IsRow:false);
		op.Grid.ColumnDefinitions.AddRange([ ColDef(1,GUT.Star), ColDef(1,GUT.Star), ColDef(1,GUT.Star) ]);
		op.AddInit(new Button{ Content = "Refresh Options", MinHeight = 34 }, o=>o.Click += (s,e)=>Ctx?.RefreshOptions());
		op.AddInit(new Button{ Content = "Form -> Json", MinHeight = 34 }, o=>o.Click += (s,e)=>Ctx?.SyncJsonFromForm());
		op.AddInit(new Button{ Content = "Json -> Form", MinHeight = 34 }, o=>o.Click += (s,e)=>Ctx?.ApplyJsonToForm());
		g.AddInit(op.Grid, o=>o.Margin = new Thickness(8,8,8,4));
		g.AddInit(new TextBox{ AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(8,0,8,8) }, o=>{
			o.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
			o.Bind(o.PropText, CBE.Mk<VmStudyPlanEdit>(x=>x.JsonText, Mode:BindingMode.TwoWay));
		});
		return g.Grid;
	}

	Control MkErrBar(){
		var b = new Border{
			Background = new SolidColorBrush(Color.FromArgb(70, 180, 20, 20)),
			Padding = new Thickness(8, 4),
			Margin = new Thickness(8, 0, 8, 2),
			IsVisible = false,
		};
		b.Bind(IsVisibleProperty, CBE.Mk<VmStudyPlanEdit>(x=>x.HasError, Mode:BindingMode.OneWay));
		var txt = new TextBlock{ Foreground = Brushes.White, TextWrapping = TextWrapping.Wrap };
		txt.Bind(txt.PropText, CBE.Mk<VmStudyPlanEdit>(x=>x.LastError, Mode:BindingMode.OneWay));
		b.Child = txt;
		return b;
	}

	Control MkInput(str label, IBinding bind, bool readOnly = false){
		var sp = new StackPanel{ Spacing = 2 };
		sp.Children.Add(new TextBlock{ Text = label });
		var tb = new TextBox{ IsReadOnly = readOnly };
		tb.Bind(tb.PropText, bind);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkOps(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.Margin = new Thickness(8,0,8,8);
		g.Grid.ColumnDefinitions.AddRange([ ColDef(1,GUT.Star), ColDef(1,GUT.Star), ColDef(1,GUT.Star) ]);
		g.AddInit(new Button{ Content = "Save", MinHeight = 36 }, o=>o.Click+=(s,e)=>Ctx?.Save());
		g.AddInit(new Button{ Content = "Delete", MinHeight = 36 }, o=>o.Click+=(s,e)=>Ctx?.Delete());
		g.AddInit(new Button{ Content = "Back", MinHeight = 36 }, o=>o.Click+=(s,e)=>Ctx?.ViewNavi?.Back());
		return g.Grid;
	}
}
