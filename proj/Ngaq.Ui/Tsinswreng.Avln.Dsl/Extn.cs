using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.Avln.Grid;
using Tsinswreng.CsCore;
namespace Ngaq.Ui;

// public static class Extn{


// }


public class StyleBuilder<TCtrl>
	where TCtrl : StyledElement
{
	public IList<SetterBase> Setters{get;set;} = new List<SetterBase>();
	public Style Build(){

	}

	public StyleBuilder<TCtrl> Set(
		Expression<Func<TCtrl, obj?>> ScltProp, obj? V
	){
		var prop = Tsinswreng.Avln.Dsl.Extn.Prop<TCtrl>(null, ScltProp);
		Setters.Add(new Setter(prop, V));
		return this;
	}
}


public class Test{
	public void T(Styles S){
		S.A(
			new StyleBuilder<TextBox>()
			.Set(x=>x.IsReadOnly, true)
			.Set(x=>x.AcceptsReturn, true)
			.Set(x=>x.TextWrapping, TextWrapping.Wrap)
			.Set(x=>x.Focusable, false)
			.Set(x=>x.IsTabStop, false)
			.Build()
		);
	}
}
