using System;
using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
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
		return new StyleBuilder<TCtrl>(x=>ApplySelector(selector, x.Is<TCtrl>()));
	}

	public static StyleBuilder<TCtrl> OfType<TCtrl>(
		Func<Selector, Selector>? selector = null
	)
		where TCtrl : StyledElement
	{
		return new StyleBuilder<TCtrl>(x=>ApplySelector(selector, x.OfType<TCtrl>()));
	}

	static Selector ApplySelector(
		Func<Selector, Selector>? selector
		,Selector root
	){
		return selector?.Invoke(root) ?? root;
	}
}


public class StyleBuilder<TCtrl>
	: Style
	where TCtrl : StyledElement
{
	public StyleBuilder(Func<Selector, Selector> selector)
		: base(selector)
	{
	}

	public Style Build(){
		return this;
	}


	public StyleBuilder<TCtrl> Set<T>(
		Expression<Func<TCtrl, T?>> ScltProp, T? V
	){
		Expression body = ScltProp.Body;
		if(body.Type.IsValueType){
			body = Expression.Convert(body, typeof(object));
		}
		var scltObj = Expression.Lambda<Func<TCtrl, object?>>(body, ScltProp.Parameters);
		var prop = Tsinswreng.Avln.Dsl.Extn.Prop<TCtrl>(null, scltObj);
		Setters.Add(new Setter(prop, V));
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
		).A(
			Sty.Is<Button>()
			.Set(x=>x.HorizontalAlignment, HAlign.Stretch)
			.Set(x=>x.Background, new SolidColorBrush(Color.FromArgb(255, 32, 32, 32)))
		).A(
			Sty.Is<Button>(x=>
				x.Class(":pointerover")
				.Template()
				.OfType<ContentPresenter>()
			)
			.Set(x=>x.BorderBrush, UiCfg.Inst.MainColor)
		).A(
			Sty.OfType<TreeDataGrid>()
			.Set(x=>x.HorizontalAlignment, HAlign.Stretch)
			.Set(x=>x.VerticalAlignment, VAlign.Stretch)
		).A(
			Sty.Is<StackPanel>(x=>x.Class(App.Cls.SpacedStackPanel))
			.Set(x=>x.Spacing, UiCfg.Inst.BaseFontSize * 0.5)
		).A(
			Sty.Is<Control>(x=>
				x.Class(App.Cls.ViewPadding)
				//再附加一個 selector 條件：只匹配 IsVisible == true 的元素
				.PropertyEquals(Layoutable.IsVisibleProperty, true)
			)
			.Set(x=>x.Focusable, false)
		)
		;
	}
	
	void TryOr(){
// 组合选择器：匹配所有带 "primary" 或 "secondary" 类的 Button 控件
var combinedSelector = Selectors.Or(
    new Style(x => x.OfType<Button>().Class("primary")).Selector,
    new Style(x => x.OfType<Button>().Class("secondary")).Selector
);

var style = new Style
{
    Selector = combinedSelector,
    Setters =
    {
        new Setter(Button.BackgroundProperty, Brushes.Green),
        new Setter(Button.ForegroundProperty, Brushes.White)
    }
};

// 将样式添加到全局样式表或控件的样式集合中
Application.Current?.Styles.Add(style);
	}
}
