using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Live.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ngaq.Core.Frontend.Hotkey;
using Ngaq.Ui.Infra;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

namespace Ngaq.Ui;


public partial class App :Application
#if DEBUG
	,ILiveView
#endif
{
	public static ILogger? Logger{get;protected set;} = null!;
	public static T GetRSvc<T>()
		where T : class
	{
		Logger??=App.SvcProvider.GetRequiredService<ILogger>();
		Logger?.LogInformation("GetRSvc: "+typeof(T));
		return App.SvcProvider.GetRequiredService<T>();
	}
	public static T? GetSvc<T>()
		where T : class
	{
		Logger??=App.SvcProvider.GetRequiredService<ILogger>();
		Logger?.LogInformation("GetRSvc: "+typeof(T));
		return App.SvcProvider.GetService<T>();
	}

	public static T DiOrNew<T>()
		where T : class, new()
	{
		var Svc = App.SvcProvider?.GetService<T>();
		return Svc ?? new T();
	}

	public static T DiOrMk<T>()
		where T : class, IMk<T>
	{
		var Svc = App.SvcProvider?.GetService<T>();
		return Svc ?? T.Mk();
	}

	public static IServiceProvider SvcProvider { get; private set; } = null!;
	public static void SetSvcProvider(IServiceProvider SvcProvider){
		App.SvcProvider = SvcProvider;
	}

	public override void Initialize() {
		AvaloniaXamlLoader.Load(this);
		var Sty = MkrStyle.MkStyForAnyControl();
		Styles.Add(new FluentTheme());
		this.RequestedThemeVariant = ThemeVariant.Dark;
		Styles.Add(MkrStyle.NoCornerRadius(Sty));
		// 在 App 初始化时添加资源（如 App.axaml.cs 的构造函数）
		App.Current?.Resources.Add(KeysRsrc.ControlContentThemeFontSize, UiCfg.Inst.BaseFontSize);
		_Style();

		//只在windows下可用。在安卓中debug旹需手動註釋掉此。
		//不能運行旹判斷平臺、緣無此符號則編譯不通
		//增 && WINDOWS亦不效 緣Ngaq.Ui中無斯預處理ˉ符號
		#if DEBUG
			if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
				DevToolsExtensions.AttachDevTools(this);
			}
		#endif

	}

	protected nil _Style(){
		// 如下設置後 在局部覆蓋全局字體旹 TextBlock生效洏TextBox不效、不效者 字體大小恆不變
		// var StyBaseFontSize = new Style(x=>
		// 	x.Is<Control>()
		// ).Set(
		// 	TextElement.FontSizeProperty
		// 	,UiCfg.Inst.BaseFontSize
		// );
		// Styles.Add(StyBaseFontSize);

		var StyBaseFontSize = new Style(x=>
			x.Is<Control>()
		).Set(
			TextElement.FontFamilyProperty
			//,new FontFamily("Times New Roman, STSong")
			//,new FontFamily("Times New Roman")
			,FontFamily.Default//不顯式指定Default則珩于android恐缺漢字字體
		).AddTo(Styles);
		//按鈕舒展

		Styles.A(new Style(x=>
				x.Is<Button>()
			).Set(
				TemplatedControl.HorizontalAlignmentProperty
				, HAlign.Stretch
			).Set(
				Button.BackgroundProperty
				,new SolidColorBrush(Color.FromArgb(255, 32,32,32))
			)
		);

		/*
我想把按鈕的邊框顏色綁定到和他自己的背景顏色一樣、並把這當成一種優先級最低的默認行爲
如果 在 局部 顯示指定了按鈕的邊框 再不再使用默認行爲。
		 */
		TemplatedControl.BackgroundProperty.Changed.AddClassHandler<Button>((btn, e) => {
			btn.SetValue(
				TemplatedControl.BorderBrushProperty
				,e.NewValue as IBrush
				,BindingPriority.Style
			);
		});

		return NIL;
	}

	public override void OnFrameworkInitializationCompleted() {
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			// Avoid duplicate validations from both Avalonia and the CommunityToolkit.
			// More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
			DisableAvaloniaDataAnnotationValidation();

			#region 全局基字體 2025-05-29T17:14:54.155+08:00_W22-4
			// 创建资源字典并添加资源
			var resources = new ResourceDictionary();
			resources.Add("ControlContentThemeFontSize", 14.0);
			// 确保主资源字典存在
			if (App.Current != null && App.Current.Resources == null){
				App.Current.Resources = new ResourceDictionary();
			}
			// 合并新资源到全局字典
			App.Current?.Resources.MergedDictionaries.Add(resources);
			#endregion #region 全局基字體 2025-05-29T17:14:54.155+08:00_W22-4
			desktop.MainWindow = MkWindow();
			// var liveViewHost = new LiveViewHost(this, Console.WriteLine);
			// liveViewHost.StartWatchingSourceFilesForHotReloading();
			// liveViewHost.Show();

			var RegisterGlobalHotkeys = App.GetSvc<I_RegisterGlobalHotKeys>();
			RegisterGlobalHotkeys?.RegisterGlobalHotKeys();
		} else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
			singleViewPlatform.MainView = MainView.Inst;
		}

 		// var overrideStyle = new Style();
		// overrideStyle.Resources.Add("ButtonBackground", new SolidColorBrush(Colors.Black));
		// // overrideStyle.Resources.Add("ButtonBackgroundPointerOver", new SolidColorBrush(Colors.DarkGray));

		// // 3. 将这个 Style 添加到 Application.Styles 集合中，并确保它在 FluentTheme 之后
		// //    注意：如果 App.Styles 中已有 FluentTheme，直接 Add 即可
		// Application.Current?.Styles.Add(overrideStyle);


		base.OnFrameworkInitializationCompleted();
	}



	private void DisableAvaloniaDataAnnotationValidation() {
		// Get an array of plugins to remove
		var dataValidationPluginsToRemove =
			BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

		// remove each entry found
		foreach (var plugin in dataValidationPluginsToRemove) {
			BindingPlugins.DataValidators.Remove(plugin);
		}
	}


	public Window MkWindow(){
		var Cfg = UiCfg.Inst;
		return new MainWindow{
			DataContext = new MainViewModel()
			,Title= "ŋaʔ"
			,Width = Cfg.WindowWidth
			,Height = Cfg.WindowHeight
			,Foreground = Brushes.White
		};
	}

	public object CreateView(Window window) {
		return MkWindow();
	}
}
