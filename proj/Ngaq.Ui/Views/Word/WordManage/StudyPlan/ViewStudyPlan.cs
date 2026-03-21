namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Components.KvMap.JsonMap;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmStudyPlan;

public partial class ViewStudyPlan
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewStudyPlan(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls{

	}


	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
			RowDef(8, GUT.Star),
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
		]);

		Root
		.AddInit(MkTopBar(), o=>{})
		.AddInit(MkCurrentBar(), o=>{})
		.AddInit(MkBody(), o=>{})
		.AddInit(MkErrBar(), o=>{})
		.AddInit(MkBottomBar(), o=>{})
		;

		return NIL;
	}

	Control MkTopBar(){
		var sp = new StackPanel{
			Orientation = Orientation.Vertical,
			Spacing = 6,
			Margin = new Thickness(8, 8, 8, 4),
		};
		sp.Children.Add(MkSearch());
		sp.Children.Add(MkActionWrap());
		return sp;
	}

	Control MkActionWrap(){
		var wrap = new WrapPanel{
			Orientation = Orientation.Horizontal,
			HorizontalAlignment = HAlign.Left,
		};
		var b1 = MkBtn("New", true); b1.Click += (s,e)=>Ctx?.NewPlan(); wrap.Children.Add(b1);
		var b2 = MkBtn("Clone", true); b2.Click += (s,e)=>Ctx?.CloneSelected(); wrap.Children.Add(b2);
		var b3 = MkBtn("Delete", true); b3.Click += (s,e)=>Ctx?.DeleteSelected(); wrap.Children.Add(b3);
		var b4 = MkBtn("Set Current", true); b4.Click += (s,e)=>Ctx?.SetCurrentSelected(); wrap.Children.Add(b4);
		var b5 = MkBtn("Save Draft", true); b5.Click += (s,e)=>Ctx?.SaveFormToSelected(); wrap.Children.Add(b5);
		return wrap;
	}

	Control MkSearch(){
		var tb = new TextBox();
		tb.Watermark = "Search by name or id";
		tb.Bind(tb.PropText, CBE.Mk<Ctx>(x=>x.SearchText, Mode:BindingMode.TwoWay));
		return tb;
	}

	Control MkCurrentBar(){
		var b = new Border{
			BorderBrush = Brushes.Gray,
			BorderThickness = new Thickness(1),
			Padding = new Thickness(8,4),
			Margin = new Thickness(8,0,8,4),
		};
		var txt = new TextBlock();
		txt.Bind(txt.PropText, CBE.Mk<Ctx>(x=>x.CurPlanId, Converter: new SimpleFnConvtr<str, str>(v=>$"Current Plan: {v}")));
		b.Child = txt;
		return b;
	}

	Control MkBody(){
		var g = new AutoGrid(IsRow:true);
		g.Grid.Margin = new Thickness(8, 2, 8, 2);
		g.Grid.RowDefinitions.AddRange([
			RowDef(3, GUT.Star),
			RowDef(7, GUT.Star),
		]);
		g
		.AddInit(MkPlanList(), o=>{})
		.AddInit(MkDetailPanel(), o=>{})
		;
		return g.Grid;
	}

	Control MkPlanList(){
		var b = new Border{
			BorderBrush = Brushes.Gray,
			BorderThickness = new Thickness(1),
			Padding = new Thickness(6),
		};
		var root = new StackPanel{ Spacing = 4 };
		root.Children.Add(new TextBlock{ Text = "Plans" });
		var list = new ListBox{ MaxHeight = 200 };
		list.Bind(list.PropItemsSource, CBE.Mk<Ctx>(x=>x.FilteredPlans));
		list.Bind(list.PropSelectedItem, CBE.Mk<Ctx>(x=>x.SelectedPlan, Mode:BindingMode.TwoWay));
		list.SetItemTemplate<Ctx.PlanDraft>((item, ns)=>{
			var txt = new TextBlock{ TextWrapping = TextWrapping.Wrap };
			txt.Bind(txt.PropText, CBE.Mk<Ctx.PlanDraft>(x=>x.DisplayName));
			return txt;
		});
		root.Children.Add(list);
		b.Child = root;
		return b;
	}

	Control MkDetailPanel(){
		var tab = new TabControl();
		tab.Items.AddInit(new TabItem(), o=>{
			o.Header = "Form";
			o.Content = MkFormTab();
		}).AddInit(new TabItem(), o=>{
			o.Header = "Weight Arg";
			o.Content = MkWeightArgTab();
		}).AddInit(new TabItem(), o=>{
			o.Header = "JSON";
			o.Content = MkJsonTab();
		});
		return tab;
	}

	Control MkFormTab(){
		var sv = new ScrollViewer();
		var sp = new StackPanel{
			Spacing = 8,
			Margin = new Thickness(8),
		};
		sv.Content = sp;

		sp
		.AddInit(MkReadOnlyRow("PlanId", CBE.Mk<Ctx>(x=>x.PlanIdText, Mode:BindingMode.TwoWay)))
		.AddInit(MkInputRow("UniqName", CBE.Mk<Ctx>(x=>x.UniqName, Mode:BindingMode.TwoWay)))
		.AddInit(MkInputRow("Descr", CBE.Mk<Ctx>(x=>x.Descr, Mode:BindingMode.TwoWay)))
		.AddInit(MkComboRow("PreFilter", CBE.Mk<Ctx>(x=>x.PreFilterOptions), CBE.Mk<Ctx>(x=>x.SelectedPreFilterId, Mode:BindingMode.TwoWay)))
		.AddInit(MkComboRow("WeightCalculator", CBE.Mk<Ctx>(x=>x.WeightCalculatorOptions), CBE.Mk<Ctx>(x=>x.SelectedWeightCalculatorId, Mode:BindingMode.TwoWay)))
		.AddInit(MkComboRow("WeightArg", CBE.Mk<Ctx>(x=>x.WeightArgOptions), CBE.Mk<Ctx>(x=>x.SelectedWeightArgId, Mode:BindingMode.TwoWay)))
		.AddInit(MkJsonTextRow("PreFilter Json", CBE.Mk<Ctx>(x=>x.PreFilterJson, Mode:BindingMode.TwoWay), 120))
		.AddInit(MkJsonTextRow("WeightCalculator Json", CBE.Mk<Ctx>(x=>x.WeightCalculatorJson, Mode:BindingMode.TwoWay), 120))
		;

		return sv;
	}

	Control MkWeightArgTab(){
		var g = new AutoGrid(IsRow:true);
		g.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(9, GUT.Star),
		]);
		g.AddInit(new TextBlock(), o=>{
			o.Margin = new Thickness(8,8,8,4);
			o.Text = "Edit WeightArg via UiJsonMap. Save/Sync will collect these values.";
			o.Foreground = Brushes.LightGray;
		});
		g.AddInit(new ViewUiJsonMap(){
			Ctx = Ctx?.WeightArgMapVm
		}, o=>{
			o.Margin = new Thickness(8,0,8,8);
		});
		return g.Grid;
	}

	Control MkJsonTab(){
		var g = new AutoGrid(IsRow:true);
		g.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(9, GUT.Star),
		]);
		g.AddInit(MkJsonOps(), o=>{});
		g.AddInit(new TextBox(), o=>{
			o.Margin = new Thickness(8,4,8,8);
			o.AcceptsReturn = true;
			o.TextWrapping = TextWrapping.Wrap;
			o.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
			o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.JsonText, Mode:BindingMode.TwoWay));
		});
		return g.Grid;
	}

	Control MkJsonOps(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.Margin = new Thickness(8,8,8,2);
		g.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		g
		.AddInit(MkBtn("Sync Form -> Json", true), o=>{ o.Click += (s,e)=>Ctx?.SyncJsonFromForm(); })
		.AddInit(MkBtn("Apply Json -> Form", true), o=>{ o.Click += (s,e)=>Ctx?.ApplyJsonToForm(); })
		;
		return g.Grid;
	}

	Control MkErrBar(){
		var b = new Border{
			Background = new SolidColorBrush(Color.FromArgb(70, 180, 20, 20)),
			Padding = new Thickness(8, 4),
			Margin = new Thickness(8, 2, 8, 2),
			IsVisible = false,
		};
		b.Bind(IsVisibleProperty, CBE.Mk<Ctx>(x=>x.HasError, Mode:BindingMode.OneWay));
		var txt = new TextBlock{ Foreground = Brushes.White, TextWrapping = TextWrapping.Wrap };
		txt.Bind(txt.PropText, CBE.Mk<Ctx>(x=>x.LastError, Mode:BindingMode.OneWay));
		b.Child = txt;
		return b;
	}

	Control MkBottomBar(){
		var sp = new StackPanel{
			Orientation = Orientation.Vertical,
			Spacing = 6,
			Margin = new Thickness(8, 2, 8, 8),
		};
		var action = new WrapPanel{
			Orientation = Orientation.Horizontal,
			HorizontalAlignment = HAlign.Left,
		};
		var b1 = MkBtn("Save Form", true); b1.Click += (s,e)=>Ctx?.SaveFormToSelected(); action.Children.Add(b1);
		var b2 = MkBtn("Sync Form -> Json", true); b2.Click += (s,e)=>Ctx?.SyncJsonFromForm(); action.Children.Add(b2);
		var b3 = MkBtn("Apply Json -> Form", true); b3.Click += (s,e)=>Ctx?.ApplyJsonToForm(); action.Children.Add(b3);
		sp.Children.Add(action);
		sp.AddInit(new TextBlock(), o=>{
			o.VerticalAlignment = VAlign.Center;
			o.Text = "Dirty";
			o.Foreground = Brushes.Orange;
			o.Bind(IsVisibleProperty, CBE.Mk<Ctx>(x=>x.IsDirty, Mode:BindingMode.OneWay));
		});
		return sp;
	}

	Control MkInputRow(str label, IBinding binding){
		var sp = new StackPanel{ Orientation = Orientation.Vertical, Spacing = 2 };
		sp.Children.Add(new TextBlock{ Text = label });
		var tb = new TextBox();
		tb.Bind(tb.PropText, binding);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkReadOnlyRow(str label, IBinding binding){
		var sp = new StackPanel{ Orientation = Orientation.Vertical, Spacing = 2 };
		sp.Children.Add(new TextBlock{ Text = label });
		var tb = new TextBox{ IsReadOnly = true };
		tb.Bind(tb.PropText, binding);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkComboRow(str label, IBinding itemsBinding, IBinding selectedBinding){
		var sp = new StackPanel{ Orientation = Orientation.Vertical, Spacing = 2 };
		sp.Children.Add(new TextBlock{ Text = label });
		var cb = new ComboBox();
		cb.Bind(cb.PropItemsSource, itemsBinding);
		cb.Bind(cb.PropSelectedItem, selectedBinding);
		sp.Children.Add(cb);
		return sp;
	}

	Control MkJsonTextRow(str label, IBinding binding, double height){
		var sp = new StackPanel{ Orientation = Orientation.Vertical, Spacing = 2 };
		sp.Children.Add(new TextBlock{ Text = label });
		var tb = new TextBox{
			Height = height,
			AcceptsReturn = true,
			TextWrapping = TextWrapping.Wrap,
		};
		tb.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
		tb.Bind(tb.PropText, binding);
		sp.Children.Add(tb);
		return sp;
	}

	Button MkBtn(str text, bool mobile = false){
		return new Button{
			Content = text,
			Margin = new Thickness(0,0,8,6),
			MinHeight = mobile ? 36 : 0,
			MinWidth = mobile ? 96 : 0,
		};
	}
}
