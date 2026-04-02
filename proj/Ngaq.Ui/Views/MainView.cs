namespace Ngaq.Ui.Views;

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using Ngaq.Core.Infra.Errors;
using Ngaq.Ui.CodeTemplate.Sample;
using Ngaq.Ui.Components.KvMap;
using Ngaq.Ui.Components.KvMap.JsonMap;
using Ngaq.Ui.Components.PageBar;
using Ngaq.Ui.Controls;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.StrokeText;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Try;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views.Home;
using Ngaq.Ui.Views.User.ChangePassword;
using Ngaq.Ui.Views.Word.WordEditV2;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsCore;
using Tsinswreng.CsErr;


public class ViewNaviBase:UserControl{

}


public partial class MainView : UserControl {
	public static MainView Inst{get;protected set;} =new();

	[Doc(@$"造按鈕、點後跳到目標視圖")]
	public Button MkBtnToView(
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

	[Doc(@$"造按鈕、點後跳到目標視圖（延遲建構）")]
	public Button MkBtnToView(
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
	public II18n I18n{get;set;} = Ngaq.Ui.Infra.I18n.I18n.Inst;
	public SvcPopup SvcPopup{get;set;}
	public AutoGrid AutoGrid = new (IsRow: true);
	public Grid Root{get{return AutoGrid.Grid;}}
	public ViewNaviBase ViewNaviBase{get;} = new ();
	public ILogger? Logger{get=>App.Logger;set{}}
	[Doc(@$"可關閉彈窗")]
	public nil ShowMsg(str Msg){
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


	[Doc(@$"前端拿到異常後處理之")]
	public nil HandleErr(obj? Ex){
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


