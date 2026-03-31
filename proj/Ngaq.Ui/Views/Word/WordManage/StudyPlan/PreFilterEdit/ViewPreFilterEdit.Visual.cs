namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmPreFilterEdit;

public partial class ViewPreFilterEdit{
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
		sp.Children.Add(MkInputRow("Version", CBE.Mk<Ctx>(x=>x.PreFilterVersion, Mode: BindingMode.TwoWay)));
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
		root.Children.Add(new TextBlock{Text = Title, FontWeight = FontWeight.SemiBold});
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
			itemList.SetItemsPanel(() => new StackPanel{Spacing = 6});
			itemList.SetItemTemplate<Ctx.VmFilterItemRow>((item, ns2)=>{
				var itemBdr = new Border{
					BorderBrush = Brushes.DimGray,
					BorderThickness = new Thickness(1),
					Padding = new Thickness(6),
				};
				var itemSp = new StackPanel{Spacing = 6};
				itemBdr.Child = itemSp;
				itemSp.Children.Add(MkBoundCombo("Operation", Ctx?.OperationOptions ?? [], CBE.Mk<Ctx.VmFilterItemRow>(x=>x.OperationIndex, Mode: BindingMode.TwoWay)));
				itemSp.Children.Add(MkBoundCombo("Value Type", Ctx?.ValueTypeOptions ?? [], CBE.Mk<Ctx.VmFilterItemRow>(x=>x.ValueTypeIndex, Mode: BindingMode.TwoWay)));
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
}
