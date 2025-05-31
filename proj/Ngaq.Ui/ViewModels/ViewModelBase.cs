#define Impl
using System.Collections.ObjectModel;
using System.ComponentModel;
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
		return Nil;
	}

	public nil ShowMsg(){
		//IsShowMsg = false;
		IsShowMsg = true;
		return Nil;
	}

	public nil ClearErr(){
		Msgs.Clear();
		return Nil;
	}


}
