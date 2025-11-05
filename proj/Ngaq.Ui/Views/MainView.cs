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
	public ILogger Logger{get;set;}
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
		Logger.LogError(Str+"\n"+str.Join("\n",Err.DebugArgs??[]));
		return NIL;
	}

	public nil HandleErr(obj? Ex){
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
    var win = new Window { Width = 400, Height = 300 };

    // 1. 按钮
    var btn = new Button { Content = "开始干活", Margin = new Thickness(10) };

    // 2. 进度条（贴底，宽度跟按钮一致）
    var bar = new ProgressBar
    {
        IsIndeterminate = true,
        IsVisible = false,
        Height = 6,               // 细一点好看
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
    };

    // 3. 用 DockPanel 把按钮放上面，进度条贴底
    var dock = new StackPanel
    {
        Margin = new Thickness(10),
        Children = { btn, bar }
    };
    // DockPanel.SetDock(btn, Dock.Top);
    // DockPanel.SetDock(bar, Dock.Bottom);

    // 4. 根面板（随便什么面板都行，这里用 Grid 方便居中）
    var root = new Grid();
    root.Children.Add(dock);

    win.Content = root;

    // 5. 按钮事件：点一下显示 3 秒
    btn.Click += async (_, __) =>
    {
        bar.IsVisible = true;
        await Task.Delay(3000);
        bar.IsVisible = false;
    };

    win.Show();
    return null;
	}

	public MainView() {
		Test();
		using var loggerFactory = LoggerFactory.Create(b=>{
			b.AddConsole()
			#if DEBUG
			.SetMinimumLevel(LogLevel.Debug)
			#else
			.SetMinimumLevel(LogLevel.Information)
			#endif
			;
		});
		Logger = loggerFactory.CreateLogger("GlobalLogger");

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
