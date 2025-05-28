using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Control = Avalonia.Controls.ContentControl;
namespace Tsinswreng.Avalonia.Navigation;

public class ViewNavigator
	//:UserControl
	:IViewNavigator
{

	public ViewNavigator(Control current){
		Current = current;
		Stack.Add(current);
	}
	public ObservableCollection<Control?> Stack{get;set;} = new(){};
	public Control? Peek{
		get{
			return Stack.Last();
		}
		set{
			Stack[Stack.Count-1] = value;
		}
	}
	public Control Current{get;set;}

	// public Control? Root{
	// 	get{return Stack[0];}
	// }

	public bool Back(){

		if(Stack.Count <= 1){
			return false;
		}
		//stack.count >= 2
		var last = Stack[Stack.Count-1];
		Stack.RemoveAt(Stack.Count-1);
		var target = Stack[Stack.Count -1];
		Current.Content = target;
		return true;

	}

	public void GoTo(Control view){
		Stack.Add(view);
		Current.Content = view;
		if(view is IHasViewNavigator navigatorView){
			navigatorView.ViewNavigator = this;
		}
	}
}

class StackView
	:UserControl
{
	public StackView(){
		var view = new Welcome();
		var navigator = new ViewNavigator(this);
		navigator.GoTo(view);
	}
}

class Welcome
	:UserControl
	,IHasViewNavigator
{
	public IViewNavigator? ViewNavigator{get;set;}
	public Welcome(){
		var btn = new Button();
		Content = btn;
		btn.Click +=(s,e)=>{
			ViewNavigator?.GoTo(
				new Login()
			);
		};
	}
}

class Login
	:UserControl
	,IHasViewNavigator
{
	public IViewNavigator? ViewNavigator{get;set;}
	public Login(){
		var backBtn = new Button();
		Content = backBtn;

		backBtn.Click += (s,e)=>{
			ViewNavigator?.Back();
		};
	}
}
