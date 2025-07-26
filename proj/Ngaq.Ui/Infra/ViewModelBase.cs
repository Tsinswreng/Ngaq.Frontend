#define Impl
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.CsTools;
using Tsinswreng.CsCore;

namespace Ngaq.Ui.Infra;

public class ViewModelBase
	:ObservableObject
	,IViewModel
	,I_ViewNavi
	,I_Arg
{

	public ViewModelBase(){
		ViewNavi = MgrViewNavi.Inst.ViewNavi;
	}

	[Impl(typeof(I_ViewNavi))]
	public IViewNavi? ViewNavi{get;set;}
	//跳轉傳參
	public ITypedObj? Arg{get;set;}

	protected ICollection<object?> _Msgs = new ObservableCollection<object?>();
	public ICollection<object?> Msgs{
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

	public ViewModelBase AddMsg(object? Msg){
		Msgs.Add(Msg);
#if DEBUG
		Console.WriteLine(Msg);
#endif
		return this;
	}

	public nil ShowMsg(){
		var Old = Msgs;
		Msgs = new ObservableCollection<object?>();
		Msgs = Old;
		IsShowMsg = true;
		return NIL;
	}

	public nil ClearMsg(){
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

