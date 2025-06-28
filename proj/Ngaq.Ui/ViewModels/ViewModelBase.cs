#define Impl
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Tsinswreng.Avalonia.Navigation;
using Tsinswreng.CsTools;
using Tsinswreng.CsCore;

namespace Ngaq.Ui.ViewModels;

public abstract class ViewModelBase
	:ObservableObject
	,IViewModel
	,I_ViewNavigator
{

	[Impl(typeof(I_ViewNavigator))]
	public IViewNavigator? ViewNavigator{get;set;}

	protected ObservableCollection<object?> _Msgs = new();
	public ObservableCollection<object?> Msgs{
		get{return _Msgs;}
		set{SetProperty(ref _Msgs, value);}
	}


/// <summary>
/// 在Popup被關閉旹 須手動于回調中把isShowMsg設潙false
/// </summary>
	protected bool _IsShowMsg = false;
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

	public nil AddMsg(object? Msg){
		Msgs.Add(Msg);
#if DEBUG
		System.Console.WriteLine(Msg);
#endif
		return NIL;
	}

	public nil ShowMsg(){

		var Old = Msgs;
		Msgs = new();
		Msgs = Old;

		IsShowMsg = true;
		return NIL;
	}

	public nil ClearErr(){
		Msgs.Clear();
		return NIL;
	}

/// <summary>
/// 地址未變但內容ˋ變旹 適用此
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="field"></param>
/// <param name="newValue"></param>
/// <param name="propertyName"></param>
/// <returns></returns>
	public bool ForceSetProp<T>(
		[NotNullIfNotNull(nameof(newValue))] ref T field
		,T newValue
		,[CallerMemberName] string? propertyName = null
	){
		base.OnPropertyChanging(propertyName);
		field = newValue;
		base.OnPropertyChanged(propertyName);
		return true;
	}

	// void test(){
	// 	ForceSetProperty(ref _IsShowMsg, true);
	// }
}

