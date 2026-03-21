namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

public partial class VmPreFilterQuery: ViewModelBase, IMk<VmPreFilterQuery>{
	protected VmPreFilterQuery(){
		StudyPlanUiStore.EnsureInit();
		Refresh();
	}
	public static VmPreFilterQuery Mk()=>new();

	public str SearchText{
		get=>field;
		set{ if(SetProperty(ref field, value)){ PageIdx = 0; Refresh(); } }
	}="";
	public i32 PageIdx{
		get=>field;
		set{ if(SetProperty(ref field, value)){ Refresh(); OnPropertyChanged(nameof(PageText)); } }
	}=0;
	public i32 PageSize{get;set;} = 8;

	public ObservableCollection<UiPreFilter> PageItems{ get=>field; set=>SetProperty(ref field, value);} = [];
	public i32 Total{ get=>field; set=>SetProperty(ref field, value);} = 0;
	public str PageText => $"Page {PageIdx+1}";

	public nil Prev(){ if(PageIdx>0){ PageIdx--; } return NIL; }
	public nil Next(){ if((PageIdx+1)*PageSize < Total){ PageIdx++; } return NIL; }

	public nil Refresh(){
		IEnumerable<UiPreFilter> seq = StudyPlanUiStore.PreFilters;
		var q = (SearchText??"").Trim();
		if(!str.IsNullOrWhiteSpace(q)){
			seq = seq.Where(x=>
				x.Id.Contains(q, StringComparison.OrdinalIgnoreCase)
				|| x.UniqName.Contains(q, StringComparison.OrdinalIgnoreCase)
				|| x.Descr.Contains(q, StringComparison.OrdinalIgnoreCase)
			);
		}
		var list = seq.ToList();
		Total = list.Count;
		PageItems = new ObservableCollection<UiPreFilter>(list.Skip(PageIdx*PageSize).Take(PageSize));
		OnPropertyChanged(nameof(PageText));
		return NIL;
	}
}

public partial class VmPreFilterEdit: ViewModelBase, IMk<VmPreFilterEdit>{
	protected VmPreFilterEdit(){
		StudyPlanUiStore.EnsureInit();
	}
	public static VmPreFilterEdit Mk()=>new();

	public bool IsNew{ get=>field; set=>SetProperty(ref field, value); }
	public str Id{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str UniqName{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str Descr{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str JsonText{ get=>field; set=>SetProperty(ref field, value);} = "{}";

	public nil Load(UiPreFilter? src){
		if(src is null){
			IsNew = true;
			Id = Guid.NewGuid().ToString("N");
			UniqName = "";
			Descr = "";
			JsonText = "{}";
			return NIL;
		}
		IsNew = false;
		Id = src.Id;
		UniqName = src.UniqName;
		Descr = src.Descr;
		JsonText = src.JsonText;
		return NIL;
	}

	public nil Save(){
		if(str.IsNullOrWhiteSpace(Id) || str.IsNullOrWhiteSpace(UniqName)){
			ShowMsg("Id / UniqName 不能為空");
			return NIL;
		}
		var got = StudyPlanUiStore.PreFilters.FirstOrDefault(x=>x.Id == Id);
		if(got is null){
			StudyPlanUiStore.PreFilters.Add(new UiPreFilter{ Id = Id.Trim(), UniqName = UniqName.Trim(), Descr = Descr, JsonText = JsonText });
		}else{
			got.UniqName = UniqName.Trim();
			got.Descr = Descr;
			got.JsonText = JsonText;
		}
		ShowMsg("PreFilter 已保存");
		return NIL;
	}

	public nil Delete(){
		var got = StudyPlanUiStore.PreFilters.FirstOrDefault(x=>x.Id == Id);
		if(got is not null){
			StudyPlanUiStore.PreFilters.Remove(got);
			ShowMsg("PreFilter 已刪除");
		}
		return NIL;
	}
}

public partial class ViewPreFilterQuery: AppViewBase{
	public VmPreFilterQuery? Ctx{ get=>DataContext as VmPreFilterQuery; set=>DataContext = value; }
	public ViewPreFilterQuery(){ Ctx = VmPreFilterQuery.Mk(); Render(); }

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
		g.AddInit(new TextBox{ Watermark = "Search PreFilter" }, o=>{
			o.Bind(o.PropText, CBE.Mk<VmPreFilterQuery>(x=>x.SearchText, Mode:BindingMode.TwoWay));
		});
		g.AddInit(new Button{ Content = "New", MinHeight = 36 }, o=>{
			o.Click += (s,e)=>OpenEdit(null);
		});
		return g.Grid;
	}

	Control MkList(){
		var items = new ItemsControl();
		items.Bind(items.PropItemsSource, CBE.Mk<VmPreFilterQuery>(x=>x.PageItems));
		items.SetItemsPanel(()=>new StackPanel{ Spacing = 6 });
		items.SetItemTemplate<UiPreFilter>((row, ns)=>{
			var b = new Border{ BorderThickness = new Thickness(1), Padding = new Thickness(8) };
			var g = new AutoGrid(IsRow:true);
			g.Grid.RowDefinitions.AddRange([ RowDef(1,GUT.Auto), RowDef(1,GUT.Auto), RowDef(1,GUT.Auto) ]);
			g.AddInit(new TextBlock{ Text = $"{row.UniqName} ({row.Id})" }, o=>{});
			g.AddInit(new TextBlock{ Text = row.Descr, TextWrapping = TextWrapping.Wrap }, o=>{});
			g.AddInit(new Button{ Content = "Edit", MinHeight = 34 }, o=>{ o.Click += (s,e)=>OpenEdit(row); });
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
		g.AddInit(new TextBlock{ VerticalAlignment = VAlign.Center }, o=>o.Bind(o.PropText, CBE.Mk<VmPreFilterQuery>(x=>x.PageText)));
		g.AddInit(new Button{ Content=">", MinHeight = 34 }, o=>o.Click+=(s,e)=>Ctx?.Next());
		return g.Grid;
	}

	void OpenEdit(UiPreFilter? row){
		var v = new ViewPreFilterEdit();
		v.Ctx?.Load(row);
		Ctx?.ViewNavi?.GoTo(ToolView.WithTitle("PreFilter Edit", v));
	}
}

public partial class ViewPreFilterEdit: AppViewBase{
	public VmPreFilterEdit? Ctx{ get=>DataContext as VmPreFilterEdit; set=>DataContext = value; }
	public ViewPreFilterEdit(){ Ctx = VmPreFilterEdit.Mk(); Render(); }

	protected nil Render(){
		var root = new AutoGrid(IsRow:true);
		root.Grid.RowDefinitions.AddRange([ RowDef(8,GUT.Star), RowDef(1,GUT.Auto) ]);
		this.Content = root.Grid;

		root.AddInit(new ScrollViewer{ Content = MkForm(), Margin = new Thickness(8,8,8,4) }, o=>{});
		root.AddInit(MkOps(), o=>{});
		return NIL;
	}

	Control MkForm(){
		var sp = new StackPanel{ Spacing = 8 };
		sp.Children.Add(MkInput("Id", CBE.Mk<VmPreFilterEdit>(x=>x.Id, Mode:BindingMode.TwoWay), true));
		sp.Children.Add(MkInput("UniqName", CBE.Mk<VmPreFilterEdit>(x=>x.UniqName, Mode:BindingMode.TwoWay)));
		sp.Children.Add(MkInput("Descr", CBE.Mk<VmPreFilterEdit>(x=>x.Descr, Mode:BindingMode.TwoWay)));
		var tb = new TextBox{ AcceptsReturn = true, Height = 240, TextWrapping = TextWrapping.Wrap };
		tb.Bind(tb.PropText, CBE.Mk<VmPreFilterEdit>(x=>x.JsonText, Mode:BindingMode.TwoWay));
		sp.Children.Add(new TextBlock{ Text = "Data(Json)" });
		sp.Children.Add(tb);
		return sp;
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
