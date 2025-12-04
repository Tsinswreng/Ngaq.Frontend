namespace Ngaq.Ui;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views;
using Avalonia.Themes.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Semi.Avalonia;
using Avalonia.Styling;
using Avalonia.Controls;
using Tsinswreng.AvlnTools.Tools;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Ngaq.Core.Infra.Errors;
using Avalonia.Media;
using Tsinswreng.AvlnTools.Dsl;
using Live.Avalonia;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

public partial class App :Application
#if DEBUG
	,ILiveView
#endif
{

	public static ILogger? Logger{get;protected set;} = null!;
	public static T GetSvc<T>()
		where T : class
	{
		Logger??=App.SvcProvider.GetRequiredService<ILogger>();
		Logger?.LogInformation("GetSvc: "+typeof(T));
		return App.SvcProvider.GetRequiredService<T>();
	}

	public static IServiceProvider SvcProvider { get; private set; } = null!;
	public static void SetSvcProvider(IServiceProvider SvcProvider){
		App.SvcProvider = SvcProvider;
	}

	public override void Initialize() {
		//AvaloniaXamlLoader.Load(this);
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
this.AttachDevTools();
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
		).Attach(Styles);
		// System.Console.WriteLine(
		// 	TextElement.FontFamilyProperty == TextBlock.FontFamilyProperty
		// );


		//按鈕舒展
		var Button = new Style(x=>
			x.Is<Button>()
		).Set(
			TemplatedControl.HorizontalAlignmentProperty
			, HAlign.Stretch
		);
		//.Set(
		// 	ContentControl.HorizontalContentAlignmentProperty
		// 	, HAlign.Center
		// );

		Styles.Add(Button);

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
		} else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
			singleViewPlatform.MainView = MainView.Inst;
		}

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
