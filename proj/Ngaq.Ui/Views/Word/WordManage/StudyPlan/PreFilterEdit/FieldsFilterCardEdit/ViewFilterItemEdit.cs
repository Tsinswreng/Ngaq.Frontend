namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.FieldsFilterCardEdit;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

using Ctx = VmFilterItemEdit;

/// <summary>
/// View for editing a single FilterItem.
/// </summary>
public class ViewFilterItemEdit: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewFilterItemEdit(){
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

		var top = new AutoGrid(IsRow:false);
		top.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		top.A(MkComboRow(
			Todo.I18n("Operation"),
			CBE.Mk<Ctx>(x=>x.OperationOptions, Mode: BindingMode.OneWay),
			CBE.Mk<Ctx>(x=>x.SelectedOperationIndex, Mode: BindingMode.TwoWay)
		));
		top.A(MkComboRow(
			Todo.I18n("Value Type"),
			CBE.Mk<Ctx>(x=>x.ValueTypeOptions, Mode: BindingMode.OneWay),
			CBE.Mk<Ctx>(x=>x.SelectedValueTypeIndex, Mode: BindingMode.TwoWay)
		));
		root.Children.Add(top.Grid);
		root.Children.Add(MkInputRow(Todo.I18n("Values (newline separated)"), CBE.Mk<Ctx>(x=>x.ValuesText, Mode: BindingMode.TwoWay), AcceptsReturn: true, MaxHeight: 180));
		return sv;
	}

	Control MkBottomBar(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		g.A(new Button(), o=>{
			o.Content = Todo.I18n("Back");
			o.Click += (s,e)=>Ctx?.ViewNavi?.Back();
		});
		g.A(new Button(), o=>{
			o.Content = Svgs.DeleteForeverSharp().ToIcon().WithText(Todo.I18n("Delete"));
			o.Background = new SolidColorBrush(Color.FromRgb(210, 56, 56));
			o.Click += (s,e)=>Ctx?.Delete();
		});
		g.A(new Button(), o=>{
			o.Content = Svgs.FloppyDiskBackFill().ToIcon().WithText(Todo.I18n("Save"));
			o.Background = UiCfg.Inst.MainColor;
			o.Click += (s,e)=>Ctx?.Save();
		});
		return g.Grid;
	}

	Control MkInputRow(str Label, IBinding Binding, bool AcceptsReturn = false, double MaxHeight = double.PositiveInfinity){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		var tb = new TextBox{
			AcceptsReturn = AcceptsReturn,
			TextWrapping = AcceptsReturn ? TextWrapping.Wrap : TextWrapping.NoWrap,
			MaxHeight = MaxHeight,
		};
		tb.Bind(TextBox.TextProperty, Binding);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkComboRow(str Label, IBinding ItemsBinding, IBinding SelectedIdxBinding){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		var cb = new ComboBox();
		cb.Bind(ComboBox.ItemsSourceProperty, ItemsBinding);
		cb.Bind(ComboBox.SelectedIndexProperty, SelectedIdxBinding);
		sp.Children.Add(cb);
		return sp;
	}
}
