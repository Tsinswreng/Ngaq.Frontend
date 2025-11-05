#define Impl
namespace Ngaq.Ui.Infra;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.CsTools;
using Tsinswreng.CsCore;
using Ngaq.Ui.Views;
using Ngaq.Core.Infra.Errors;
using Ngaq.Ui.Infra.I18n;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;



public class EvtArgMsg:EventArgs{

}

public partial class ViewModelBase
	:ObservableObject
	,IViewModel
	,I_ViewNavi
	,I_Arg
	,I_ForceSetProp
{

	public ViewModelBase(){
		ViewNavi = MgrViewNavi.Inst.ViewNavi;
	}

	[Impl(typeof(I_ViewNavi))]
	public IViewNavi? ViewNavi{get;set;}
	//跳轉傳參
	public ITypedObj? Arg{get;set;}

	//public event EventHandler<EventArgs>? EvtMsg;
	// public nil OnMsg(obj? E = null){
	// 	E??=EventArgs.Empty;
	// 	EvtMsg?.Invoke(this, E);
	// 	return NIL;
	// }

	public nil LogInfo(str? Msg = null){
		MainView.Inst.Logger.LogInformation(Msg??"");
		return NIL;
	}

	public nil LogDebug(str? Msg = null){
		MainView.Inst.Logger.LogDebug(Msg??"");
		return NIL;
	}

	public nil LogWarn(str? Msg=null){
		MainView.Inst.Logger.LogWarning(Msg??"");
		return NIL;
	}

	public nil LogError(str? Msg = null){
		MainView.Inst.Logger.LogError(Msg??"");
		return NIL;
	}

	/// <summary>
	/// 彈窗ʹ抽象。調用方宜無需知其內ʹ叶
	/// </summary>
	/// <param name="Msg"></param>
	/// <returns></returns>
	public nil ShowMsg(str Msg){
		LogInfo(nameof(ShowMsg)+": "+Msg);
		Dispatcher.UIThread.Post(()=>{
			MainView.Inst.ShowMsg(Msg);
		});
		return NIL;
	}
	public II18n I18n{get;set;} = Ngaq.Ui.Infra.I18n.I18n.Inst;
	public nil ShowErr(IAppErr Err){
		return MainView.Inst.ShowErr(Err);
	}

	public nil HandleErr(obj? Ex){
		return MainView.Inst.HandleErr(Ex);
	}

	public nil HandleErr(Task T){
		if(T.IsFaulted){
			if(T.Exception.InnerException is not null){
				HandleErr(T.Exception.InnerException);
			}else{
				HandleErr(T.Exception);
			}
		}
		return NIL;
	}

	/// <summary>
	/// 地址未變但內容ˋ變旹 適用此
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="Field"></param>
	/// <param name="newValue"></param>
	/// <param name="propertyName"></param>
	/// <returns></returns>
	[Impl]
	public bool ForceSetProp<T>(
		[NotNullIfNotNull(nameof(newValue))] ref T Field
		,T newValue
		,[CallerMemberName] string? propertyName = null
	)
#if Impl
	{
		OnPropertyChanging(propertyName);
		Field = newValue;
		OnPropertyChanged(propertyName);
		return true;
	}
#else
	;
#endif

	// void test(){
	// 	ForceSetProperty(ref _IsShowMsg, true);
	// }
}
