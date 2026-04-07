namespace Ngaq.Ui.Views;

using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using Ngaq.Core.Infra.Errors;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views.Home;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsErr;


public class ViewNaviBase:UserControl{

}


public partial class MainView : UserControl {
	public partial Button MkBtnToView(
		Control Target
		,str? Title = null
	){
		var RealTarget = Target;
		var btn = new Button();
		if(Title is not null){
			RealTarget = ToolView.WithTitle(Title, Target);
			btn.Content = Title;
		}

		btn.Click += (s,e)=>{
			MgrViewNavi.Inst.ViewNavi?.GoTo(RealTarget);
		};
		return btn;
	}

	public partial Button MkBtnToView(
		Func<Control> MkTarget
		,str? Title = null
	){
		Control? RealTarget = null;
		var btn = new Button();
		if(Title is not null){
			btn.Content = Title;
		}

		btn.Click += (s,e)=>{
			RealTarget ??= Title is not null
				? ToolView.WithTitle(Title, MkTarget())
				: MkTarget()
			;
			MgrViewNavi.Inst.ViewNavi?.GoTo(RealTarget);
		};
		return btn;
	}

	public partial nil ShowMsg(str Msg){
		Dispatcher.UIThread.Post((Action)(()=>{
			var SvcPopup = this.SvcPopup;
			var msgBox = new MsgBox();
			{var o = msgBox;
				o._CloseBtn.Background = null;
				o._CloseBtn.SetContent(Icon.FromSvg(Svgs.XCircleFill()), o=>{
					o.Fill = Brushes.Red;
				});
				o.MinHeight = UiCfg.Inst.WindowHeight*0.2;
				o.MinWidth = UiCfg.Inst.WindowWidth*0.5;
				o._Border.BorderThickness = new Avalonia.Thickness(1);
				o._Border.BorderBrush = Brushes.White;
				o._BdrTitle.Background = new SolidColorBrush(new Color(255, 40,40,40));
				o._BdrBody.MaxHeight = UiCfg.Inst.WindowHeight*0.8;

				o._CloseBtn.Click+=(object? s,global::Avalonia.Interactivity.RoutedEventArgs e)=>{
					SvcPopup.ClosePopup();
				};
				o.HorizontalAlignment = HAlign.Center;
				o.VerticalAlignment = VAlign.Center;
				// msgBox._Title.Content = new TextBlock{
				// 	Text = "TestTitle"
				// 	,FontSize = 26
				// };
				o._Body.Content = new SelectableTextBlock {
					Text= Msg,
					HorizontalAlignment = HAlign.Center,
					VerticalAlignment = VAlign.Center,
					TextWrapping = TextWrapping.Wrap,
				};
				o.Background = Brushes.Black;
				var Bdr = new Border();
				Bdr.Child = o;
				Bdr.Padding = new Avalonia.Thickness(40);
				SvcPopup.ShowPopup(Bdr);
			}
		}));
		return NIL;
	}

	public partial nil ShowMsg(str Msg, IList<Button> Operations){
		Dispatcher.UIThread.Post((()=>{
			var SvcPopup = this.SvcPopup;
			var msgBox = new MsgBox();
			{var o = msgBox;
				o._CloseBtn.Background = null;
				o._CloseBtn.SetContent(Icon.FromSvg(Svgs.XCircleFill()), o=>{
					o.Fill = Brushes.Red;
				});
				o.MinHeight = UiCfg.Inst.WindowHeight*0.2;
				o.MinWidth = UiCfg.Inst.WindowWidth*0.5;
				o._Border.BorderThickness = new Avalonia.Thickness(1);
				o._Border.BorderBrush = Brushes.White;
				o._BdrTitle.Background = new SolidColorBrush(new Color(255, 40,40,40));
				o._BdrBody.MaxHeight = UiCfg.Inst.WindowHeight*0.8;

				o._CloseBtn.Click+=(object? s,global::Avalonia.Interactivity.RoutedEventArgs e)=>{
					SvcPopup.ClosePopup();
				};
				o.HorizontalAlignment = HAlign.Center;
				o.VerticalAlignment = VAlign.Center;

				// 先展示提示文本，再縱向渲染操作按鈕列。
				var Body = new StackPanel{
					Orientation = Orientation.Vertical,
					Spacing = UiCfg.Inst.BaseFontSize*0.35,
					Margin = new Avalonia.Thickness(
						UiCfg.Inst.BaseFontSize*0.7,
						UiCfg.Inst.BaseFontSize*0.45,
						UiCfg.Inst.BaseFontSize*0.7,
						UiCfg.Inst.BaseFontSize*0.45
					),
					HorizontalAlignment = HAlign.Stretch,
					VerticalAlignment = VAlign.Center,
				};
				Body.A(new SelectableTextBlock{
					Text = Msg,
					TextWrapping = TextWrapping.Wrap,
					HorizontalAlignment = HAlign.Center,
				});

				for(i32 Idx = 0; Idx < Operations.Count; Idx++){
					var Btn = Operations[Idx];
					Btn.HorizontalAlignment = HAlign.Stretch;
					// 點擊操作按鈕時統一關閉彈窗；其他業務行爲由調用方在按鈕自身事件中處理。
					Btn.Click += (s,e)=>{
						SvcPopup.ClosePopup();
					};
					Body.A(Btn);
				}

				o._Body.Content = Body;
				o.Background = Brushes.Black;
				var Bdr = new Border();
				Bdr.Child = o;
				Bdr.Padding = new Avalonia.Thickness(40);
				SvcPopup.ShowPopup(Bdr);
			}
		}));
		return NIL;
	}

	public partial nil HandleErr(obj? Ex){
		str? toLog = null;
		if(Ex is IAppErr Err){
			if(Err.Type is not null){
				var Str = I18n.Get(Err.Type.ToI18nKey(), Err.Args??[]);
				ShowMsg(Str);
				toLog += "\n"+Str+"\n"+str.Join("\n",Err.DebugArgs??[]);
			}
			toLog += "\n"+Err;
		}
		else if(Ex is Exception Exception){
			toLog = Exception+"";
			ShowMsg(Todo.I18n("Unknown Error"));
		}
		else{//非Exception 非 IAppErr
			toLog??="";
			toLog += "\n"+Ex+"";
			var err = ItemsErr.Common.UnknownErr.ToErr();
			if(err.Type is not null){
				var Str = I18n.Get(err.Type.ToI18nKey(), err.Args??[]);
				ShowMsg(Str);
			}
		}
		Logger?.LogError(toLog);
		return NIL;
	}

	void Try(){
	}

	public MainView() {
		Inst ??= this;
		Try();
		DataContext = new MainViewModel();
		SvcPopup = new SvcPopup(Root);
		this.SetContent(AutoGrid.Grid);
		AutoGrid.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
		]);
		AutoGrid.Add(ViewNaviBase);
		InputElement.KeyDownEvent.AddClassHandler<TopLevel>(
			(s,e)=>{
				if(e.Key == Avalonia.Input.Key.Escape){
					if(SvcPopup.Popups.Count > 0){
						SvcPopup.ClosePopup();
					}else{
						MgrViewNavi.Inst.GetViewNavi().Back();
					}
				}
			}
			,handledEventsToo: true
		);

		MgrViewNavi.Inst.ViewNavi = new ViewNavi(ViewNaviBase);
		var Navi = MgrViewNavi.Inst.ViewNavi;


		var Home = new ViewHome();
		// var Home = new ViewWordEditV2();

		Navi.GoTo(Home);
	}
}


