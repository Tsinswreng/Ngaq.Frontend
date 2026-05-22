using System;
using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.Avln.Grid;
using Tsinswreng.CsCore;
namespace Ngaq.Ui;

public static class Sty{
	public static StyleBuilder<TCtrl> Is<TCtrl>(
		Func<Selector, Selector>? selector = null
	)
		where TCtrl : StyledElement
	{
		return new StyleBuilder<TCtrl>(new Style(x=>ApplySelector(selector, x.Is<TCtrl>())));
	}

	public static StyleBuilder<TCtrl> OfType<TCtrl>(
		Func<Selector, Selector>? selector = null
	)
		where TCtrl : StyledElement
	{
		return new StyleBuilder<TCtrl>(new Style(x=>ApplySelector(selector, x.OfType<TCtrl>())));
	}

	static Selector ApplySelector(
		Func<Selector, Selector>? selector
		,Selector root
	){
		return selector?.Invoke(root) ?? root;
	}
}


public class StyleBuilder<TCtrl>
	where TCtrl : StyledElement
{
	public Style Style{get;}

	public StyleBuilder(Style style){
		Style = style;
	}

	public Style Build(){
		return Style;
	}

	public static implicit operator Style(StyleBuilder<TCtrl> builder){
		return builder.Style;
	}

	public StyleBuilder<TCtrl> Set<T>(
		Expression<Func<TCtrl, T?>> ScltProp, T? V
	){
		var prop = Tsinswreng.Avln.Dsl.Extn.Prop<TCtrl>(null, ScltProp);
		Style.Setters.Add(new Setter(prop, V));
		return this;
	}
}


public class Test{
	public void T(Styles S){
		S.A(
			Sty.Is<TextBox>(x=>x.Class(App.Cls.RoTextBox))
			.Set(x=>x.IsReadOnly, true)
			.Set(x=>x.AcceptsReturn, true)
			.Set(x=>x.TextWrapping, TextWrapping.Wrap)
			.Set(x=>x.Focusable, false)
			.Set(x=>x.IsTabStop, false)
		)
		.A(
			Sty.Is<Button>()
			.Set(x=>x.HorizontalAlignment, HAlign.Stretch)
			.Set(x=>x.Background, new SolidColorBrush(Color.FromArgb(255, 32, 32, 32)))
		)
		.A(
			Sty.Is<Button>(x=>
				x.Class(":pointerover")
				.Template()
				.OfType<ContentPresenter>()
			)
			.Set(x=>x.BorderBrush, UiCfg.Inst.MainColor)
		)
		.A(
			Sty.OfType<TreeDataGrid>()
			.Set(x=>x.HorizontalAlignment, HAlign.Stretch)
			.Set(x=>x.VerticalAlignment, VAlign.Stretch)
		)
		.A(
			Sty.Is<Control>(x=>
				x.Class(App.Cls.ViewPadding)
				.Or()
				.Class(App.Cls.CenterBtn)
			)
			.Set(x=>x.Focusable, false)
		);
	}
}
