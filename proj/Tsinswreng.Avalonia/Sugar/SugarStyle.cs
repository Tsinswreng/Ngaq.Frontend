
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Tsinswreng.Avalonia.Tools;

namespace Tsinswreng.Avalonia.Sugar;

public static class SugarStyle{
	public static Style GridShowLines(
		//this Avalonia.StyledElement.Styles z
	){
		var Ans = new Style(x=>
			x.Is<Grid>()
		);
		{
			var o = Ans;
			o.Set(
				Grid.ShowGridLinesProperty
				,true
			);
		}
		return Ans;
	}

	public static Style NoCornerRadius(){
		var o = new Style(x=>
			x.Is<Control>()
		);
		o.Set(
			ContentControl.CornerRadiusProperty
			,new CornerRadius(0)
		);
		return o;
	}
}
