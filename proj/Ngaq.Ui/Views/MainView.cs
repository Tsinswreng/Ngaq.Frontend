namespace Ngaq.Ui.Views;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using Ngaq.Core.Infra.Errors;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.StrokeText;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views.Home;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.AvlnTools.Tools;

public class ViewNaviBase:UserControl{

}


public partial class MainView : UserControl {
	protected static MainView? _Inst = null;
	public static MainView Inst => _Inst??= new MainView();

	public II18n I18n{get;set;} = Ngaq.Ui.Infra.I18n.I18n.Inst;
	public SvcPopup SvcPopup{get;set;}
	public AutoGrid AutoGrid = new (IsRow: true);
	public Grid Root{get{return AutoGrid.Grid;}}
	public ViewNaviBase ViewNaviBase{get;} = new ();
	public ILogger? Logger{get=>App.Logger;set{}}
	public nil ShowMsg(str Msg){
		Dispatcher.UIThread.Post(()=>{
			var msgBox = new MsgBox();
			msgBox.MinHeight = UiCfg.Inst.WindowHeight*0.2;
			msgBox.MinWidth = UiCfg.Inst.WindowWidth*0.5;
			msgBox._Border.BorderThickness = new Avalonia.Thickness(1);
			msgBox._Border.BorderBrush = Brushes.White;
			msgBox._BdrTitle.Background = new SolidColorBrush(new Color(255, 40,40,40));
			msgBox._BdrBody.MaxHeight = UiCfg.Inst.WindowHeight*0.8;
			var SvcPopup = MainView.Inst.SvcPopup;
			msgBox._CloseBtn.Click+=(s,e)=>{
				SvcPopup.ClosePopup();
			};
			msgBox.HorizontalAlignment = HAlign.Center;
			msgBox.VerticalAlignment = VAlign.Center;
			// msgBox._Title.Content = new TextBlock{
			// 	Text = "TestTitle"
			// 	,FontSize = 26
			// };
			msgBox._Body.Content = new SelectableTextBlock{
				Text=Msg,
				HorizontalAlignment = HAlign.Center,
				VerticalAlignment = VAlign.Center,
				TextWrapping = TextWrapping.Wrap,
			};
			msgBox.Background = Brushes.Black;
			var Bdr = new Border();
			Bdr.Child = msgBox;
			Bdr.Padding = new Avalonia.Thickness(40);
			SvcPopup.ShowPopup(Bdr);
		});
		return NIL;
	}

	public nil ShowErr(IAppErr Err){
		if(Err.Type is null){
			return NIL;
		}
		var Str = I18n.Get(Err.Type.ToI18nKey(), Err.Args??[]);
		ShowMsg(Str);
		Logger?.LogError(Str+"\n"+str.Join("\n",Err.DebugArgs??[]));
		return NIL;
	}

	public nil HandleErr(obj? Ex){
		Logger?.LogError(Ex+"");
		if(Ex is IAppErr Err){
			ShowErr(Err);
			return NIL;
		}else{
			ShowMsg("Unknown Error.");//TODO i18n
			#if DEBUG||true
			ShowMsg(Ex+"");
			#endif
			//TODO log
		}
		return NIL;
	}



Control? Test()
{
	return null;
}

	public MainView() {
		Test();
		DataContext = new MainViewModel();
		SvcPopup = new SvcPopup(Root);
		this.ContentInit(AutoGrid.Grid);
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
		//var Home = new ViewEditWord();
		Navi.GoTo(Home);

	}
}


