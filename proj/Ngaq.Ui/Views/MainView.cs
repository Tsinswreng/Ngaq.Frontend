namespace Ngaq.Ui.Views;

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
using Ngaq.Ui.Controls;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.StrokeText;
using Ngaq.Ui.Tools;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views.Home;
using Ngaq.Ui.Views.User.ChangePassword;
using Ngaq.Ui.Views.Word.WordEditV2;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsErr;


public class ViewNaviBase:UserControl{

}


public partial class MainView : UserControl {
	private static Window? _TreeDemoWnd;
	protected static MainView? _Inst = null;
	public static MainView Inst => _Inst??= new MainView();

	public II18n I18n{get;set;} = Ngaq.Ui.Infra.I18n.I18n.Inst;
	public SvcPopup SvcPopup{get;set;}
	public AutoGrid AutoGrid = new (IsRow: true);
	public Grid Root{get{return AutoGrid.Grid;}}
	public ViewNaviBase ViewNaviBase{get;} = new ();
	public ILogger? Logger{get=>App.Logger;set{}}
	public nil ShowMsg(str Msg){
		Dispatcher.UIThread.Post((Action)(()=>{
			var SvcPopup = MainView.Inst.SvcPopup;
			var msgBox = new MsgBox();
			{var o = msgBox;
				o._CloseBtn.Background = null;
				o._CloseBtn.InitContent(Icon.FromSvg(Svgs.XCircleFill), o=>{
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
			ShowErr(ItemsErr.Common.UnknownErr.ToErr());
			#if DEBUG||true
			ShowMsg(Ex+"");
			#endif
			//TODO log
		}
		return NIL;
	}



Control? Try()
{
	EnsureTreeDataGridTheme();

	var demoItems = new List<TreeRow>{
		new("Root A", "folder", new List<TreeRow>{ new("A-1", "leaf"), new("A-2", "leaf") }),
		new("Root B", "folder", new List<TreeRow>{ new("B-1", "leaf"), new("B-2", "leaf"), new("B-3", "leaf") }),
	};

	var source = new HierarchicalTreeDataGridSource<TreeRow>(demoItems){
		Columns = {
			new HierarchicalExpanderColumn<TreeRow>(new TextColumn<TreeRow, string>("Name", x=>x.Name), x=>x.Children),
			new TextColumn<TreeRow, string>("Type", x=>x.Type),
		},
	};

	var treeDataGrid = new TreeDataGrid{
		Source = source,
		MinHeight = 280,
		Margin = new Thickness(12),
	};

	var host = new Grid{
		RowDefinitions = new RowDefinitions("Auto,*"),
	};
	host.Children.Add(new TextBlock{
		Text = "TreeDataGrid Demo",
		Margin = new Thickness(12, 10, 12, 0),
		Foreground = Brushes.White,
	});
	Grid.SetRow(treeDataGrid, 1);
	host.Children.Add(treeDataGrid);

	Dispatcher.UIThread.Post(()=>{
		_TreeDemoWnd ??= new Window{
			Title = "TreeDataGrid Demo",
			Width = 640,
			Height = 420,
			RequestedThemeVariant = ThemeVariant.Dark,
			Background = new SolidColorBrush(Color.FromRgb(32, 32, 32)),
		};
		_TreeDemoWnd.Content = host;
		_TreeDemoWnd.Show();
	}, DispatcherPriority.Background);

	return treeDataGrid;
}

private static void EnsureTreeDataGridTheme(){
	var app = Application.Current;
	if(app is null){
		return;
	}

	foreach(var style in app.Styles){
		if(style is StyleInclude si && si.Source?.ToString()?.Contains("Avalonia.Controls.TreeDataGrid", StringComparison.OrdinalIgnoreCase) == true){
			return;
		}
	}

	app.Styles.Add(new StyleInclude(new Uri("avares://Avalonia.Controls.TreeDataGrid/")){
		Source = new Uri("avares://Avalonia.Controls.TreeDataGrid/Themes/Fluent.axaml"),
	});
}

private sealed class TreeRow{
	public string Name { get; }
	public string Type { get; }
	public IReadOnlyList<TreeRow> Children { get; }

	public TreeRow(string name, string type, IReadOnlyList<TreeRow>? children = null){
		Name = name;
		Type = type;
		Children = children ?? [];
	}
}

	public MainView() {
		Try();
		DataContext = new MainViewModel();
		SvcPopup = new SvcPopup(Root);
		this.InitContent(AutoGrid.Grid);
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


