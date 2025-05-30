//2025-03-09T21:11:06.192+08:00_W10-7
using System;
using System.Linq.Expressions;
using Avalonia.Data.Converters;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
namespace Tsinswreng.Avalonia.Tools;

/// <summary>
//于c#中用編譯期綁定
// usage:
// using Ctx = MyDataContext;
//,new Binding(nameof(ctx.hasValue)) ->
//,new CBE(CBE.pth<Ctx, bool>(x=>x.hasValue))
//RelativeBinging直用new Binging即可 不必用此
//正則替換:
//new Binding(nameof(ctx\.(.*?)))
//new CBE(CBE.pth<Ctx, object?>(x=>x.$1))
//
/// </summary>
public class CBE : CompiledBindingExtension{
	public CBE(CompiledBindingPath path):base(path){}

	public static CompiledBindingPath Pth<T>(
		Expression<Func<T, object?>> propertySelector
	){
		return Pth<T, object?>(propertySelector);
	}

/// <summary>
/// 除首個參數外 禁止依賴參數定義順序㕥傳參 須用命名參數 如 Mk<Ctx>(x=>x.Foo, Mode:BindingMode.TwoWay ...)
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="PropertySelector"></param>
/// <param name="Mode"></param>
/// <param name="Converter"></param>
/// <param name="Path"></param>
/// <param name="Source"></param>
/// <param name="DataType"></param>
/// <returns></returns>
	public static CompiledBindingExtension Mk<T>(
		Expression<Func<T, object?>> PropertySelector
		,BindingMode Mode = default
		,IValueConverter? Converter = default
		,CompiledBindingPath? Path = default
		,object? Source = default
		,Type? DataType = default
	){
		var r = new CBE(Pth<T, object?>(PropertySelector)){};
		r.Mode = Mode;
		r.Converter = Converter;
		if(Path != null){r.Path = Path;}
		if(Source!= null){r.Source = Source;}
		if(DataType!= null){r.DataType = DataType;}
		return r;
	}



	public static CompiledBindingPath Pth<T, Tar>(
		Expression<Func<T, Tar>> propertySelector
	){
		var builder = new CompiledBindingPathBuilder();
		var body = propertySelector.Body;

		// 处理类型转换表达式（如值类型装箱）
		if (body is UnaryExpression { NodeType: ExpressionType.Convert } unaryExpr){
			body = unaryExpr.Operand;
		}

		switch (body){
			case MemberExpression memberExpr:  // 属性访问模式
				ProcessMemberExpression<T>(builder, memberExpr);
				break;
			case ParameterExpression paramExpr:  // 直接对象绑定模式
				ValidateObjectBinding(typeof(T), typeof(Tar));
				break;
			default:
				throw new ArgumentException("表达式必须为属性访问或对象绑定");
		}

		return builder.Build();
	}

	private static void ValidateObjectBinding(Type sourceType, Type targetType){
		if (!targetType.IsAssignableFrom(sourceType))
			throw new InvalidOperationException($"类型不兼容：{sourceType}无法转换为{targetType}");
	}

	private static void ProcessMemberExpression<T>(
		CompiledBindingPathBuilder builder
		,MemberExpression expr
	){
		var propName = expr.Member.Name;
		var propType = expr.Type;

		var clrProp = new ClrPropertyInfo(
			propName,
			obj => ((T)obj).GetType().GetProperty(propName)?.GetValue(obj),
			(obj, val) => ((T)obj).GetType().GetProperty(propName)?.SetValue(obj, val),
			propType
		);

		builder.Property(clrProp, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor);
	}



	// public static CompiledBindingPath pth<T, Tar>(
	// 	Expression<Func<T, Tar>> propertySelector
	// ){
	// 	var builder = new CompiledBindingPathBuilder();
	// 	var body = propertySelector.Body;

	// 	// 处理类型转换表达式
	// 	if (body is UnaryExpression unaryExpr && unaryExpr.NodeType == ExpressionType.Convert){
	// 		body = unaryExpr.Operand;
	// 	}

	// 	if (!(body is MemberExpression memberExpr)){
	// 		throw new ArgumentException("表达式必须为属性访问");
	// 	}
	// 	//var memberExpr = (MemberExpression)propertySelector.Body;
	// 	var propName = memberExpr.Member.Name;

	// 	var clrProp = new ClrPropertyInfo(
	// 		propName,
	// 		obj => ((T)obj).GetType().GetProperty(propName).GetValue(obj),
	// 		(obj, val) => ((T)obj).GetType().GetProperty(propName).SetValue(obj, val),
	// 		typeof(Tar)
	// 	);
	// 	builder.Property(clrProp, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor);
	// 	var path = builder.Build();
	// 	return path;
	// 	// var ans = new CompiledBindingExtension(path){

	// 	// };
	// 	// return ans;
	// }


}
