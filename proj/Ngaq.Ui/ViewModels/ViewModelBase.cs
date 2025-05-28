#define Impl
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Tsinswreng.Avalonia.Navigation;

namespace Ngaq.Ui.ViewModels;

public abstract class ViewModelBase
	:ObservableObject
	,IViewModel
	,IHasViewNavigator
{

	public IViewNavigator? ViewNavigator{get;set;}

	protected ObservableCollection<str> _Errors = new();
	public ObservableCollection<str> Errors{
		get{return _Errors;}
		set{SetProperty(ref _Errors, value);}
	}

/// <summary>
/// 無法綁定
/// </summary>
	public bool HasErr{
		get{return Errors.Count > 0;}
	}

	public nil AddErr(str Err){
		Errors.Add(Err);
		return Nil;
	}

	public nil ClearErr(){
		Errors.Clear();
		return Nil;
	}


}
