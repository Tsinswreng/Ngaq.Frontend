namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.FieldsFilterCardEdit;

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.PreFilterVisualEdit;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

using Ctx = VmFieldsFilterCardEdit;`r`nusing K = Ngaq.Ui.Infra.I18n.KeysUiI18n.FieldsFilterCardEdit;

/// <summary>
/// Editor view for a single fields-filter group.
/// </summary>
public class ViewFieldsFilterCardEdit: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewFieldsFilterCardEdit(){
		Ctx = App.DiOrMk<Ctx>();
		Render();
		InitGridSource();
	}

	AutoGrid Root = new(IsRow: true);
	TreeDataGrid? ItemGrid;
	FlatTreeDataGridSource<Ctx.RowFilterItemCard>? ItemGridSource;

	protected nil Render(){
		Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Auto),
		]);
		Root.A(MkBody());
		Root.A(MkBottomBar());
		return NIL;
	}

	Control MkBody(){
		var tabs = new TabControl();
		tabs.Items.Add(new TabItem{
			Header = I[K.Fields],
			Content = MkFieldsTab(),
		});
		tabs.Items.Add(new TabItem{
			Header = I[K.FilterItems],
			Content = MkFilterItemsTab(),
		});
		return tabs;
	}

	Control MkFieldsTab(){
		var sv = new ScrollViewer();
		var root = new StackPanel{
			Spacing = 10,
			Margin = new Thickness(10),
		};
		sv.Content = root;

		var addBtn = new Button{
			Content = Svgs.Add().ToIcon().WithText(I[K.AddField]),
			HorizontalAlignment = HAlign.Left,
		};
		addBtn.Click += (s,e)=>Ctx?.AddField();
		root.Children.Add(addBtn);

		var list = new ItemsControl();
		list.Bind(ItemsControl.ItemsSourceProperty, CBE.Mk<Ctx>(x=>x.Fields, Mode: BindingMode.OneWay));
		list.SetItemsPanel(()=>new StackPanel{Spacing = 8});
		list.SetItemTemplate<VmPreFilterVisualEdit.VmFieldValueRow>((item, ns)=>{
			var row = new AutoGrid(IsRow:false);
			row.Grid.ColumnDefinitions.AddRange([
				ColDef(1, GUT.Star),
				ColDef(1, GUT.Auto),
			]);

			var cb = new ComboBox{
				IsEditable = true,
				HorizontalAlignment = HAlign.Stretch,
			};
			cb.Bind(ComboBox.ItemsSourceProperty, CBE.Mk<Ctx>(x=>x.FieldOptions, Source: Ctx, Mode: BindingMode.OneWay));
			cb.Bind(ComboBox.TextProperty, CBE.Mk<VmPreFilterVisualEdit.VmFieldValueRow>(x=>x.Value, Mode: BindingMode.TwoWay));
			row.A(cb);

			var rm = new Button{
				Content = Svgs.DeleteForeverSharp().ToIcon().WithText(I[K.Remove]),
				Background = new SolidColorBrush(Color.FromRgb(210, 56, 56)),
			};
			rm.Click += (s,e)=>Ctx?.RemoveField(item);
			row.A(rm);
			return row.Grid;
		});
		root.Children.Add(list);
		return sv;
	}

	Control MkFilterItemsTab(){
		var root = new AutoGrid(IsRow:true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
		]);

		var top = new AutoGrid(IsRow:false);
		top.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Auto),
		]);
		top.A(new TextBlock(), o=>{
			o.Text = I[K.TapRowToEditOneFilterItem];
			o.VerticalAlignment = VAlign.Center;
		});
		top.A(new Button(), o=>{
			o.Content = Svgs.Add().ToIcon().WithText(I[K.AddItem]);
			o.Click += (s,e)=>Ctx?.AddItem();
		});
		root.A(top.Grid);

		ItemGrid = new TreeDataGrid{
			MinHeight = 220,
		};
		ItemGrid.Styles.A(
			new Style(x=>x.OfType<TreeDataGridRow>().Class(":pointerover"))
			.Set(TemplatedControl.BackgroundProperty, new SolidColorBrush(Color.FromRgb(46, 46, 46)))
		).A(
			new Style(x=>x.OfType<TreeDataGridRow>().Class(":pressed"))
			.Set(TemplatedControl.BackgroundProperty, new SolidColorBrush(Color.FromRgb(70, 70, 70)))
		);
		ItemGrid.AddHandler(InputElement.TappedEvent, OnGridTapped, RoutingStrategies.Bubble, true);
		root.A(ItemGrid);
		return root.Grid;
	}

	void InitGridSource(){
		if(Ctx is null || ItemGrid is null){
			return;
		}
		ItemGridSource = new FlatTreeDataGridSource<Ctx.RowFilterItemCard>(Ctx.ItemCards){
			Columns = {
				new TextColumn<Ctx.RowFilterItemCard, str>(I[K.NumberSign], x=>x.UiIdxText),
				new TextColumn<Ctx.RowFilterItemCard, str>(I[K.Operation], x=>x.Operation),
				new TextColumn<Ctx.RowFilterItemCard, str>(I[K.ValueType], x=>x.ValueType),
				new TextColumn<Ctx.RowFilterItemCard, str>(I[K.Values], x=>x.ValuesPreview),
			},
		};
		ItemGrid.Source = ItemGridSource;
	}

	void OnGridTapped(object? sender, TappedEventArgs e){
		if(Ctx is null){
			return;
		}
		if(e.Source is not StyledElement src){
			return;
		}
		for(StyledElement? cur = src; cur is not null; cur = cur.Parent){
			if(cur is ToggleButton){
				return;
			}
			if(cur is TreeDataGridRow row){
				if(row.DataContext is Ctx.RowFilterItemCard vmRow){
					Ctx.OpenFilterItem(vmRow);
					e.Handled = true;
				}
				return;
			}
		}
	}

	Control MkBottomBar(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		g.A(new Button(), o=>{
			o.Content = I[K.Back];
			o.Click += (s,e)=>ViewNavi?.Back();
		});
		g.A(new Button(), o=>{
			o.Content = Svgs.FloppyDiskBackFill().ToIcon().WithText(I[K.SaveGroup]);
			o.Background = UiCfg.Inst.MainColor;
			o.Click += (s,e)=>Ctx?.Save();
		});
		return g.Grid;
	}
}

