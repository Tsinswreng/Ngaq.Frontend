namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

using Ctx = VmFieldsFilterCardEdit;

public class ViewFieldsFilterCardEdit: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewFieldsFilterCardEdit(){
		Ctx = App.DiOrMk<Ctx>();
		Render();
	}

	AutoGrid Root = new(IsRow: true);
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
		var sv = new ScrollViewer();
		var root = new StackPanel{
			Spacing = 10,
			Margin = new Thickness(10),
		};
		sv.Content = root;

		root.Children.Add(MkInputRow(
			"Fields (comma/newline separated)",
			CBE.Mk<Ctx>(x=>x.FieldsText, Mode: BindingMode.TwoWay),
			AcceptsReturn: true
		));

		var btnAdd = new Button{
			Content = Svgs.Add().ToIcon().WithText(" Add Item"),
			HorizontalAlignment = HAlign.Left,
		};
		btnAdd.Click += (s,e)=>Ctx?.AddItem();
		root.Children.Add(btnAdd);

		var list = new ItemsControl();
		list.Bind(ItemsControl.ItemsSourceProperty, CBE.Mk<Ctx>(x=>x.Items, Mode: BindingMode.OneWay));
		list.SetItemsPanel(()=>new StackPanel{Spacing = 8});
		list.SetItemTemplate<VmPreFilterVisualEdit.VmFilterItemRow>((item, ns)=>{
			var bdr = new Border{
				BorderBrush = Brushes.DimGray,
				BorderThickness = new Thickness(1),
				Padding = new Thickness(8),
			};
			var sp = new StackPanel{Spacing = 6};
			bdr.Child = sp;
			sp.Children.Add(MkComboRow("Operation", Ctx?.OperationOptions ?? [], CBE.Mk<VmPreFilterVisualEdit.VmFilterItemRow>(x=>x.OperationIndex, Mode: BindingMode.TwoWay)));
			sp.Children.Add(MkComboRow("Value Type", Ctx?.ValueTypeOptions ?? [], CBE.Mk<VmPreFilterVisualEdit.VmFilterItemRow>(x=>x.ValueTypeIndex, Mode: BindingMode.TwoWay)));
			sp.Children.Add(MkInputRow("Values (comma/newline separated)", CBE.Mk<VmPreFilterVisualEdit.VmFilterItemRow>(x=>x.ValuesText, Mode: BindingMode.TwoWay), AcceptsReturn: true));
			var rm = new Button{
				Content = Svgs.DeleteForeverSharp().ToIcon().WithText(" Remove Item"),
				Background = new SolidColorBrush(Color.FromRgb(210, 56, 56)),
				HorizontalAlignment = HAlign.Right,
			};
			rm.Click += (s,e)=>Ctx?.RemoveItem(item);
			sp.Children.Add(rm);
			return bdr;
		});
		root.Children.Add(list);
		return sv;
	}

	Control MkBottomBar(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		g.A(new Button(), o=>{
			o.Content = "Back";
			o.Click += (s,e)=>Ctx?.ViewNavi?.Back();
		});
		g.A(new Button(), o=>{
			o.Content = Svgs.FloppyDiskBackFill().ToIcon().WithText(" Save Item");
			o.Background = UiCfg.Inst.MainColor;
			o.Click += (s,e)=>Ctx?.Save();
		});
		return g.Grid;
	}

	Control MkInputRow(str Label, IBinding Binding, bool AcceptsReturn = false){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		var tb = new TextBox{
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
}
