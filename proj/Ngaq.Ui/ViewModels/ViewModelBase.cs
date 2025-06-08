#define Impl
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Tsinswreng.Avalonia.Navigation;

namespace Ngaq.Ui.ViewModels;

public abstract class ViewModelBase
	:ObservableObject
	,IViewModel
	,IHasViewNavigator
{

	public IViewNavigator? ViewNavigator{get;set;}

	protected ObservableCollection<str> _Msgs = new();
	public ObservableCollection<str> Msgs{
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

	public nil AddMsg(str Msg){
		Msgs.Add(Msg);
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
		OnPropertyChanging(propertyName);
		field = newValue;
		OnPropertyChanged(propertyName);
		return true;
	}

	// void test(){
	// 	ForceSetProperty(ref _IsShowMsg, true);
	// }
}

