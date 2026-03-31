namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmPreFilterEdit;

public partial class ViewPreFilterEdit{
	protected TabItem MkPoTab(){
		var tab = new TabItem{Header = "PoPreFilter JSON"};
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
		]);
		root.A(MkJsonOpsBar());
		root.A(JsonText(), o=>{
			o.CBind<Ctx>(o.PropText, x=>x.PoPreFilterJson);
		});
		tab.Content = root.Grid;
		return tab;
	}

	protected TabItem MkPreFilterTab(){
		var tab = new TabItem{Header = "PreFilter JSON"};
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
		]);
		root.A(MkJsonOpsBar());
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
}
