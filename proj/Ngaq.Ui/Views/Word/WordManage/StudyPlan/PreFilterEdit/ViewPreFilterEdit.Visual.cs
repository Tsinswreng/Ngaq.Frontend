namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmPreFilterEdit;

public partial class ViewPreFilterEdit{
	TreeDataGrid? CoreGrid;
	TreeDataGrid? PropGrid;
	FlatTreeDataGridSource<Ctx.RowFieldsFilterCard>? CoreGridSource;
	FlatTreeDataGridSource<Ctx.RowFieldsFilterCard>? PropGridSource;

	protected TabItem MkVisualTab(){
		var tab = new TabItem{
			Header = "Visual",
		};
		tab.Content = MkVisualEditor();
		InitVisualGridSource();
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
		root.Children.Add(MkPreFilterMetaSection());
		root.Children.Add(MkFieldsFilterGridSection(true));
		root.Children.Add(MkFieldsFilterGridSection(false));
		root.Children.Add(MkSwitchToJsonBar());
		return sv;
	}

	void InitVisualGridSource(){
		if(Ctx is null){
			return;
		}
		if(CoreGrid is not null){
			CoreGridSource = MkGridSource(Ctx.CoreFilterCards);
			CoreGrid.Source = CoreGridSource;
		}
		if(PropGrid is not null){
			PropGridSource = MkGridSource(Ctx.PropFilterCards);
			PropGrid.Source = PropGridSource;
		}
	}

	FlatTreeDataGridSource<Ctx.RowFieldsFilterCard> MkGridSource(ObservableCollection<Ctx.RowFieldsFilterCard> Rows){
		return new FlatTreeDataGridSource<Ctx.RowFieldsFilterCard>(Rows){
			Columns = {
				new TextColumn<Ctx.RowFieldsFilterCard, str>("#", x=>x.UiIdxText),
				new TextColumn<Ctx.RowFieldsFilterCard, str>("Fields", x=>x.FieldsPreview),
				new TextColumn<Ctx.RowFieldsFilterCard, str>("Items", x=>x.FilterCountText),
			},
		};
	}

	protected Control MkErrorBar(){
		var b = new Border{
			Background = new SolidColorBrush(Color.FromArgb(80, 180, 30, 30)),
			Padding = new Thickness(10, 6),
			IsVisible = false,
		};
		b.CBind<Ctx>(IsVisibleProperty, x=>x.HasError, Mode: BindingMode.OneWay);
		var txt = new TextBlock{
			Foreground = Brushes.White,
		};
		txt.CBind<Ctx>(TextBlock.TextProperty, x=>x.LastError, Mode: BindingMode.OneWay);
		b.Child = txt;
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
		sp.Children.Add(MkInputRow("Id", CBE.Mk<Ctx>(x=>x.PoIdText, Mode: BindingMode.OneWay), ReadOnly: true));
		sp.Children.Add(MkInputRow("Name", CBE.Mk<Ctx>(x=>x.PoUniqName, Mode: BindingMode.TwoWay)));
		sp.Children.Add(MkInputRow("Description", CBE.Mk<Ctx>(x=>x.PoDescr, Mode: BindingMode.TwoWay), AcceptsReturn: true));
		sp.Children.Add(MkComboRow("Type", Ctx?.PoTypeOptions ?? [], CBE.Mk<Ctx>(x=>x.PoTypeIndex, Mode: BindingMode.TwoWay)));
		return bdr;
	}

	protected Control MkPreFilterMetaSection(){
		var bdr = new Border{
			BorderBrush = Brushes.DimGray,
			BorderThickness = new Thickness(1),
			Padding = new Thickness(10),
		};
		var sp = new StackPanel{Spacing = 8};
		bdr.Child = sp;
		sp.Children.Add(new TextBlock{
			Text = "PreFilter",
			FontSize = UiCfg.Inst.BaseFontSize * 1.1,
			FontWeight = FontWeight.SemiBold,
		});
		sp.Children.Add(MkInputRow("Version", CBE.Mk<Ctx>(x=>x.PreFilterVersion, Mode: BindingMode.TwoWay)));
		return bdr;
	}

	protected Control MkFieldsFilterGridSection(bool IsCore){
		var box = new Border{
			BorderBrush = Brushes.DimGray,
			BorderThickness = new Thickness(1),
			Padding = new Thickness(10),
		};
		var root = new AutoGrid(IsRow:true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
		]);
		box.Child = root.Grid;

		var title = IsCore ? "CoreFilter" : "PropFilter";
		var top = new AutoGrid(IsRow:false);
		top.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Auto),
		]);
		top.A(new TextBlock(), o=>{
			o.Text = title;
			o.FontWeight = FontWeight.SemiBold;
			o.VerticalAlignment = VAlign.Center;
		});
		top.A(new Button(), o=>{
			o.Content = Svgs.Add().ToIcon().WithText(" Add Group");
			o.HorizontalAlignment = HAlign.Right;
			o.Click += (s,e)=>{
				if(IsCore){
					Ctx?.AddCoreGroup();
				}else{
					Ctx?.AddPropGroup();
				}
			};
		});
		root.A(top.Grid);

		var grid = new TreeDataGrid{
			MinHeight = 180,
		};
		grid.Styles.Add(
			new Style(x=>x.OfType<TreeDataGridRow>().Class(":pointerover"))
			.Set(TemplatedControl.BackgroundProperty, new SolidColorBrush(Color.FromRgb(46, 46, 46)))
		);
		grid.Styles.Add(
			new Style(x=>x.OfType<TreeDataGridRow>().Class(":pressed"))
			.Set(TemplatedControl.BackgroundProperty, new SolidColorBrush(Color.FromRgb(70, 70, 70)))
		);
		if(IsCore){
			CoreGrid = grid;
			grid.AddHandler(InputElement.TappedEvent, OnCoreGridTapped, RoutingStrategies.Bubble, true);
		}else{
			PropGrid = grid;
			grid.AddHandler(InputElement.TappedEvent, OnPropGridTapped, RoutingStrategies.Bubble, true);
		}
		root.A(grid);
		return box;
	}

	void OnCoreGridTapped(object? sender, TappedEventArgs e){
		OnFieldsGridTapped(e, IsCore: true);
	}

	void OnPropGridTapped(object? sender, TappedEventArgs e){
		OnFieldsGridTapped(e, IsCore: false);
	}

	void OnFieldsGridTapped(TappedEventArgs e, bool IsCore){
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
				if(row.DataContext is Ctx.RowFieldsFilterCard vmRow){
					if(IsCore){
						Ctx.OpenCoreFilterCard(vmRow);
					}else{
						Ctx.OpenPropFilterCard(vmRow);
					}
					e.Handled = true;
				}
				return;
			}
		}
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
}
