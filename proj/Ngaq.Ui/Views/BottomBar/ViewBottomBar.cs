namespace Ngaq.Ui.Views.BottomBar;

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.Avalonia.Controls;
using Tsinswreng.Avalonia.Tools;
using Ctx = Vm_BottomBar;
public partial class ViewBottomBar
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewBottomBar(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public class Cls_{
		public str BarItem = nameof(BarItem);
	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		var ShowGrid = new Style(x=>
			x.Is<Grid>()
		);
		Styles.Add(ShowGrid);
		{
			var o = ShowGrid;
			o.Set(
				Grid.ShowGridLinesProperty
				,true
			);
		}
		return Nil;
	}

	IndexGrid Root = new(IsRow:false);
	protected nil Render(){
		Content = Root.Grid;
		{
			var o = Root.Grid;
			o.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
				new ColDef(4, GUT.Star),
				new ColDef(4, GUT.Star),
				new ColDef(4, GUT.Star),
				new ColDef(1, GUT.Star),
			]);
		}
		{{
			Root.Add();

			var Learn = BarItem("Learn", "📖");
			Root.Add(Learn);

			var Settings = BarItem("Lib", "📚");
			Root.Add(Settings);

			var Me = BarItem("Me", "👤");
			Root.Add(Me);

			Root.Add();
		}}
		return Nil;
	}

	protected Control BarItem(
		str Title
		,str IconStr
	){
		var Ans = new SwipeLongPressBtn();
		{
			var o = Ans;
			o.VerticalAlignment = VertAlign.Stretch;
			o.HorizontalAlignment = HoriAlign.Stretch;
		}
		{{
			var Grid = new IndexGrid(IsRow:true);
			Ans.Content = Grid.Grid;

			Grid.Grid.Classes.Add(Cls.BarItem);
			Grid.Grid.RowDefinitions.AddRange([
				new RowDef(1, GUT.Star),
				new RowDef(6, GUT.Star),
				new RowDef(2, GUT.Star),
				new RowDef(1, GUT.Star),
			]);
			{{
				Grid.Add();

				var Icon = new TextBlock{};
				Grid.Add(Icon);
				{
					var o = Icon;
					o.Text = IconStr;
					o.TextAlignment = TxtAlign.Center;
					o.VerticalAlignment = VertAlign.Center;
					o.FontSize = 24.0;
				}

				var Title_ = new TextBlock{};
				Grid.Add(Title_);
				{
					var o = Title_;
					o.Text = Title;
					o.TextAlignment = TxtAlign.Center;
					o.VerticalAlignment = VertAlign.Center;
				}
				Grid.Add();
			}}//~Grid
		}}//~Button
		return Ans;
	}


}
