namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmPreFilterEdit;
public partial class ViewPreFilterEdit
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewPreFilterEdit(){
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
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Auto),
		]);
		Root.A(MkTabs());
		Root.A(MkBottomBar());
		return NIL;
	}

	protected Control MkTabs(){
		var tab = new TabControl();
		tab.CBind<Ctx>(
			tab.PropSelectedIndex
			,x=>x.TabIndex
		);
		tab.Items.Add(MkVisualTab());
		tab.Items.Add(MkPoTab());
		tab.Items.Add(MkPreFilterTab());
		return tab;
	}

	protected TabItem MkVisualTab(){
		var tab = new TabItem{
			Header = "Visual",
		};
		tab.Content = MkVisualEditor();
		return tab;
	}

	protected Control MkVisualEditor(){
		var sv = new ScrollViewer();
		var root = new StackPanel{
			Spacing = 10,
			Margin = new Thickness(10),
		};
		sv.Content = root;

		root.Children.Add(MkErrorBar());
		root.Children.Add(MkPoSection());
		root.Children.Add(MkPreFilterSection());
		root.Children.Add(MkSwitchToJsonBar());
		return sv;
	}

	protected Control MkErrorBar(){
		var b = new Border{
			Background = new SolidColorBrush(Color.FromArgb(80, 180, 30, 30)),
			Padding = new Thickness(10, 6),
			IsVisible = false,
		};
		b.CBind<Ctx>(IsVisibleProperty, x=>x.HasError, Mode: BindingMode.OneWay);
		b.Child = new TextBlock{
			Foreground = Brushes.White,
			Init = o=>{
				o.CBind<Ctx>(TextBlock.TextProperty, x=>x.LastError, Mode: BindingMode.OneWay);
			}
		};
		return b;
	}

	protected Control MkPoSection(){
		var bdr = new Border{
			BorderBrush = Brushes.DimGray,
			BorderThickness = new Thickness(1),
			Padding = new Thickness(10),
		};
		var sp = new StackPanel{Spacing = 8};
		bdr.Child = sp;

		sp.Children.Add(new TextBlock{
			Text = "PoPreFilter",
			FontSize = UiCfg.Inst.BaseFontSize * 1.1,
			FontWeight = FontWeight.SemiBold,
		});

		sp.Children.Add(MkInputRow(
			"Id",
			CBE.Mk<Ctx>(x=>x.PoIdText, Mode: BindingMode.OneWay),
			ReadOnly: true
		));
		sp.Children.Add(MkInputRow(
			"Name",
			CBE.Mk<Ctx>(x=>x.PoUniqName, Mode: BindingMode.TwoWay)
		));
		sp.Children.Add(MkInputRow(
			"Description",
			CBE.Mk<Ctx>(x=>x.PoDescr, Mode: BindingMode.TwoWay),
			AcceptsReturn: true
		));
		sp.Children.Add(MkComboRow(
			"Type",
			Ctx?.PoTypeOptions ?? [],
			CBE.Mk<Ctx>(x=>x.PoTypeIndex, Mode: BindingMode.TwoWay)
		));
		return bdr;
	}

	protected Control MkPreFilterSection(){
		var bdr = new Border{
			BorderBrush = Brushes.DimGray,
			BorderThickness = new Thickness(1),
			Padding = new Thickness(10),
		};
		var sp = new StackPanel{Spacing = 10};
		bdr.Child = sp;

		sp.Children.Add(new TextBlock{
			Text = "PreFilter",
			FontSize = UiCfg.Inst.BaseFontSize * 1.1,
			FontWeight = FontWeight.SemiBold,
		});
		sp.Children.Add(MkInputRow(
			"Version",
			CBE.Mk<Ctx>(x=>x.PreFilterVersion, Mode: BindingMode.TwoWay)
		));

		sp.Children.Add(MkFieldsFilterSection(
			"CoreFilter",
			CBE.Mk<Ctx>(x=>x.CoreFilterRows, Mode: BindingMode.OneWay),
			()=>Ctx?.AddCoreGroup(),
			row=>Ctx?.RemoveCoreGroup(row)
		));

		sp.Children.Add(MkFieldsFilterSection(
			"PropFilter",
			CBE.Mk<Ctx>(x=>x.PropFilterRows, Mode: BindingMode.OneWay),
			()=>Ctx?.AddPropGroup(),
			row=>Ctx?.RemovePropGroup(row)
		));
		return bdr;
	}

	protected Control MkFieldsFilterSection(
		str Title,
		IBinding RowsBinding,
		Action OnAddGroup,
		Action<Ctx.VmFieldsFilterRow> OnRemoveGroup
	){
		var root = new StackPanel{Spacing = 8};
		root.Children.Add(new TextBlock{
			Text = Title,
			FontWeight = FontWeight.SemiBold,
		});

		var btnAdd = new Button{
			Content = Svgs.Add().ToIcon().WithText(" Add Group"),
			HorizontalAlignment = HAlign.Left,
		};
		btnAdd.Click += (s,e)=>OnAddGroup();
		root.Children.Add(btnAdd);

		var list = new ItemsControl();
		list.Bind(ItemsControl.ItemsSourceProperty, RowsBinding);
		list.SetItemsPanel(() => new StackPanel{Spacing = 8});
		list.SetItemTemplate<Ctx.VmFieldsFilterRow>((row, ns)=>{
			var bdr = new Border{
				BorderBrush = Brushes.Gray,
				BorderThickness = new Thickness(1),
				Padding = new Thickness(8),
			};
			var sp = new StackPanel{Spacing = 8};
			bdr.Child = sp;

			sp.Children.Add(MkBoundInput(
				"Fields (comma/newline separated)",
				CBE.Mk<Ctx.VmFieldsFilterRow>(x=>x.FieldsText, Mode: BindingMode.TwoWay),
				AcceptsReturn: true
			));

			var itemList = new ItemsControl();
			itemList.Bind(ItemsControl.ItemsSourceProperty, CBE.Mk<Ctx.VmFieldsFilterRow>(x=>x.Items, Mode: BindingMode.OneWay));
			itemList.SetItemsPanel(()=>new StackPanel{Spacing = 6});
			itemList.SetItemTemplate<Ctx.VmFilterItemRow>((item, ns2)=>{
				var itemBdr = new Border{
					BorderBrush = Brushes.DimGray,
					BorderThickness = new Thickness(1),
					Padding = new Thickness(6),
				};
				var itemSp = new StackPanel{Spacing = 6};
				itemBdr.Child = itemSp;

				itemSp.Children.Add(MkBoundCombo(
					"Operation",
					Ctx?.OperationOptions ?? [],
					CBE.Mk<Ctx.VmFilterItemRow>(x=>x.OperationIndex, Mode: BindingMode.TwoWay)
				));
				itemSp.Children.Add(MkBoundCombo(
					"Value Type",
					Ctx?.ValueTypeOptions ?? [],
					CBE.Mk<Ctx.VmFilterItemRow>(x=>x.ValueTypeIndex, Mode: BindingMode.TwoWay)
				));
				itemSp.Children.Add(MkBoundInput(
					"Values (comma/newline separated)",
					CBE.Mk<Ctx.VmFilterItemRow>(x=>x.ValuesText, Mode: BindingMode.TwoWay),
					AcceptsReturn: true
				));
				var rmItemBtn = new Button{
					Content = Svgs.DeleteForeverSharp().ToIcon().WithText(" Remove Item"),
					Background = new SolidColorBrush(Color.FromRgb(210, 56, 56)),
					HorizontalAlignment = HAlign.Right,
				};
				rmItemBtn.Click += (s,e)=>Ctx?.RemoveFilterItem(row, item);
				itemSp.Children.Add(rmItemBtn);
				return itemBdr;
			});
			sp.Children.Add(itemList);

			var ops = new AutoGrid(IsRow: false);
			ops.Grid.ColumnDefinitions.AddRange([
				ColDef(1, GUT.Star),
				ColDef(1, GUT.Star),
			]);
			ops.A(new Button(), o=>{
				o.Content = Svgs.Add().ToIcon().WithText(" Add Item");
				o.Click += (s,e)=>Ctx?.AddFilterItem(row);
			});
			ops.A(new Button(), o=>{
				o.Content = Svgs.DeleteForeverSharp().ToIcon().WithText(" Remove Group");
				o.Background = new SolidColorBrush(Color.FromRgb(210, 56, 56));
				o.Click += (s,e)=>OnRemoveGroup(row);
			});
			sp.Children.Add(ops.Grid);
			return bdr;
		});
		root.Children.Add(list);
		return root;
	}

	protected Control MkSwitchToJsonBar(){
		var g = new AutoGrid(IsRow: false);
		g.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		g.A(new Button(), o=>{
			o.Content = "Open PoPreFilter JSON";
			o.Click += (s,e)=>Ctx?.GoToPoJson();
		});
		g.A(new Button(), o=>{
			o.Content = "Open PreFilter JSON";
			o.Click += (s,e)=>Ctx?.GoToPreFilterJson();
		});
		return g.Grid;
	}

	protected TabItem MkPoTab(){
		var tab = new TabItem{
			Header = "PoPreFilter JSON",
		};
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
		]);
		root.A(MkJsonOpsBar(), o=>{});
		root.A(JsonText(), o=>{
			o.CBind<Ctx>(o.PropText, x=>x.PoPreFilterJson);
		});
		tab.Content = root.Grid;
		return tab;
	}

	protected TabItem MkPreFilterTab(){
		var tab = new TabItem{
			Header = "PreFilter JSON",
		};
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
		]);
		root.A(MkJsonOpsBar(), o=>{});
		root.A(JsonText(), o=>{
			o.CBind<Ctx>(o.PropText, x=>x.PreFilterJson);
		});
		tab.Content = root.Grid;
		return tab;
	}

	protected Control MkJsonOpsBar(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.Margin = new Thickness(10, 10, 10, 4);
		g.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		g.A(new Button(), o=>{
			o.Content = "Apply JSON -> Visual";
			o.Click += (s,e)=>Ctx?.ApplyJsonToVisual();
		});
		g.A(new Button(), o=>{
			o.Content = "Back To Visual";
			o.Click += (s,e)=>Ctx?.GoToVisual();
		});
		return g.Grid;
	}

	TextBox JsonText(){
		var box = new TextBox{
			AcceptsReturn = true,
			AcceptsTab = true,
			TextWrapping = TextWrapping.Wrap,
			HorizontalAlignment = HAlign.Stretch,
			VerticalAlignment = VAlign.Stretch,
			Margin = new Thickness(10, 4, 10, 10),
		};
		box.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
		return box;
	}

	protected Control MkBottomBar(){
		var bar = new AutoGrid(IsRow:false);
		bar.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		bar.A(_Button(), o=>{
			o.HorizontalContentAlignment = HAlign.Center;
			o.Content = "Sync Visual -> JSON";
			o.Click += (s,e)=>Ctx?.SyncJsonFromVisual();
		})
		.A(_Button(), o=>{
			o.Background = UiCfg.Inst.MainColor;
			o.HorizontalContentAlignment = HAlign.Center;
			o.Content = Svgs.FloppyDiskBackFill().ToIcon().WithText(" Save");
			o.Click += async (s,e)=>{
				if(Ctx is null){
					return;
				}
				await Ctx.Save();
			};
		})
		.A(_Button(), o=>{
			o.Background = new SolidColorBrush(Color.FromRgb(210, 56, 56));
			o.HorizontalContentAlignment = HAlign.Center;
			o.Content = Svgs.DeleteForeverSharp().ToIcon().WithText(" Delete");
			o.Click += async (s,e)=>{
				if(Ctx is null){
					return;
				}
				await Ctx.Delete();
			};
		});
		return bar.Grid;
	}

	Control MkInputRow(str Label, IBinding Binding, bool ReadOnly = false, bool AcceptsReturn = false){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		var tb = new TextBox{
			IsReadOnly = ReadOnly,
			AcceptsReturn = AcceptsReturn,
			TextWrapping = AcceptsReturn ? TextWrapping.Wrap : TextWrapping.NoWrap,
		};
		tb.Bind(TextBox.TextProperty, Binding);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkComboRow(str Label, IEnumerable<str> Items, IBinding Binding){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		var cb = new ComboBox();
		foreach(var item in Items){
			cb.Items.Add(item);
		}
		cb.Bind(ComboBox.SelectedIndexProperty, Binding);
		sp.Children.Add(cb);
		return sp;
	}

	Control MkBoundInput(str Label, IBinding Binding, bool AcceptsReturn = false){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label, FontSize = UiCfg.Inst.BaseFontSize * 0.9});
		var tb = new TextBox{
			AcceptsReturn = AcceptsReturn,
			TextWrapping = AcceptsReturn ? TextWrapping.Wrap : TextWrapping.NoWrap,
		};
		tb.Bind(TextBox.TextProperty, Binding);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkBoundCombo(str Label, IEnumerable<str> Items, IBinding Binding){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label, FontSize = UiCfg.Inst.BaseFontSize * 0.9});
		var cb = new ComboBox();
		foreach(var item in Items){
			cb.Items.Add(item);
		}
		cb.Bind(ComboBox.SelectedIndexProperty, Binding);
		sp.Children.Add(cb);
		return sp;
	}
}
