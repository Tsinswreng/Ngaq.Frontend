#define Impl
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Ngaq.Ui.Infra;

public class EvtArgMsg:EventArgs{

}

public  partial class ViewModelBase
	:ObservableObject
	,IViewModel
	,I_ViewNavi
	,I_Arg
	,I_ForceSetProp
	,IMsgViewModel
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

	protected ICollection<object?> _Msgs = new ObservableCollection<object?>();

	[Impl(typeof(IMsgViewModel))]
	public ICollection<object?> Msgs{
		get{return _Msgs;}
		set{SetProperty(ref _Msgs, value);}
	}


	/// <summary>
	/// 在Popup被關閉旹 須手動于回調中把isShowMsg設潙false
	/// </summary>
	protected bool _IsShowMsg = false;

	[Impl(typeof(IMsgViewModel))]
	public bool IsShowMsg{
		get{return _IsShowMsg;}
		set{
			// OnPropertyChanging(nameof(IsShowMsg));
			// _IsShowMsg = value;
			// OnPropertyChanged(nameof(IsShowMsg));
			SetProperty(ref _IsShowMsg, value);
			//OnPropertyChanged(nameof(IsShowMsg));
		}
	}

	// public bool HasErr{
	// 	get{return Errors.Count > 0;}
	// }

	[Impl(typeof(IMsgViewModel))]
	[Obsolete]
	public ViewModelBase AddMsg(object? Msg){
		Msgs.Add(Msg);
#if DEBUG
		Console.WriteLine(Msg);
#endif
		return this;
	}

	[Impl(typeof(IMsgViewModel))]
	[Obsolete]
	public nil ShowMsg(){
		var Old = Msgs;
		Msgs = new ObservableCollection<object?>();
		Msgs = Old;
		IsShowMsg = true;
		return NIL;
	}


	[Impl(typeof(IMsgViewModel))]
	[Obsolete]
	public nil ClearMsg(){
		Msgs.Clear();
		return NIL;
	}

	/// <summary>
	/// 彈窗ʹ抽象。調用方宜無需知其內ʹ叶
	/// </summary>
	/// <param name="Msg"></param>
	/// <returns></returns>
	public nil ShowMsg(str Msg){
		Dispatcher.UIThread.Post(()=>{
			MainView.Inst.ShowMsg(Msg);
		});
		return NIL;
	}
	public II18n I18n{get;set;} = Ngaq.Ui.Infra.I18n.I18n.Inst;
	public nil ShowMsg(IAppErr Err){
		if(Err.Type is null){
			return NIL;
		}
		var Str = I18n.Get(Err.Type.ToI18nKey(), Err.Args??[]);
		ShowMsg(Str);
		return NIL;
	}

	public nil HandleErr(Exception Ex){
		if(Ex is IAppErr Err){
			ShowMsg(Err);
			return NIL;
		}else{
			ShowMsg("Unknown Error.");//TODO i18n
			#if DEBUG
			ShowMsg(Ex+"");
			#endif
			//TODO log
		}
		return NIL;
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

