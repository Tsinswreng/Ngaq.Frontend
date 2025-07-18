using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsTools.Tools;

namespace Ngaq.Ui.ViewModels;

public interface I_Arg{
	public ITypedObj? Arg{get;set;}
}

public interface IViewModel{}



// public static class ExtnIViewModel{
// 	public static nil Bind(
// 		this IViewModel z
// 		,Control Ctrl
// 		,AvaloniaProperty Property
// 		,Expression<Func<IViewModel, object?>> propertySelector

// 	){
// 		Ctrl.Bind(
// 			Property
// 			,new CBE(CBE.Pth<IViewModel>(propertySelector)){
// 				Mode = BindingMode.TwoWay
// 			}
// 		);
// 		return NIL;
// 	}
// }
