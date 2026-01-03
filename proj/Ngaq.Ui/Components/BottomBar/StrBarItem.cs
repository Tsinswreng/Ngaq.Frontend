namespace Ngaq.Ui.Views.BottomBar;

using Avalonia.Controls;
using Avalonia.Media;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;


public partial class StrBarItem{
	protected static StrBarItem? _Inst = null;
	public static StrBarItem Inst => _Inst??= new StrBarItem();

	public SwipeLongPressBtn BarItem(
		str Title
		,Control Icon
	){
		var Ans = new SwipeLongPressBtn();
		{
			var o = Ans;
			o.VerticalAlignment = VAlign.Stretch;
			o.HorizontalAlignment = HAlign.Stretch;
			//o.Background = Brushes.Black;
			o.Background = Brushes.Transparent;
			o.Styles.Add(new Style().NoMargin().NoPadding());
		}
		{{
			var Grid = new AutoGrid(IsRow:true);
			Ans.Content = Grid.Grid;
			//Grid.Grid.Classes.Add(Cls.BarItem);
			Grid.Grid.RowDefinitions.AddRange([
				// new RowDef(12, GUT.Star),
				// new RowDef(4, GUT.Star),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
			]);
			{{
				Grid.Add(Icon);{
					var o = Icon;
					o.VerticalAlignment = VAlign.Center;
				}

				var Title_ = new TextBlock{};
				Grid.Add(Title_);
				{
					var o = Title_;
					o.Text = Title;
					o.TextAlignment = TxtAlign.Center;
					o.VerticalAlignment = VAlign.Center;
					o.FontSize = UiCfg.Inst.BaseFontSize*0.75;
				}
			}}//~Grid
		}}//~Button
		return Ans;
	}
}
