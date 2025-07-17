using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Tsinswreng.AvlnTools.Tools;

namespace Ngaq.Ui.ViewModels;

public interface IViewModel{}

//棄用 緣lambda中無法推導出IViewModelʹ具體ʹ叶ˡ類ʹ類型
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
// 		return Nil;
// 	}
// }
