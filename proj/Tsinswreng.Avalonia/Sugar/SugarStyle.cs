
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using Tsinswreng.Avalonia.Tools;

namespace Tsinswreng.Avalonia.Sugar;

public static class SugarStyle{

	public static Style MkStyForAnyControl(){
		var Ans = new Style(x=>
			x.Is<Control>()
		);
		return Ans;
	}

	public static Style GridShowLines(
		this Style z
	){
		z.Set(
			Grid.ShowGridLinesProperty
			,true
		);
		return z;
	}

	public static Style GridShowLines(
		//this Avalonia.StyledElement.Styles z
	){
		var Ans = new Style(x=>
			x.Is<Grid>()
		);
		Ans.GridShowLines();
		return Ans;
	}



	public static Style NoCornerRadius(
		this Style z
	){
		z.Set(
			ContentControl.CornerRadiusProperty
			,new CornerRadius(0)
		);
		return z;
	}

	public static Style NoPadding(
		this Style z
	){
		z.Set(
			TemplatedControl.PaddingProperty
			,new Thickness(0)
		);
		return z;
	}

	public static Style NoMargin(
		this Style z
	){
		z.Set(
			TemplatedControl.MarginProperty
			,new Thickness(0)
		);
		return z;
	}

	


}
