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
using Tsinswreng.Avalonia.Sugar;

namespace Ngaq.Ui;

public partial class App : Application {


	public static IServiceProvider ServiceProvider { get; private set; } = null!;
	public static void ConfigureServices(IServiceProvider serviceProvider){
		ServiceProvider = serviceProvider;
	}


	public override void Initialize() {
		//AvaloniaXamlLoader.Load(this);
		Styles.Add(new FluentTheme());
		Styles.Add(SugarStyle.NoCornerRadius());
#if DEBUG
		this.AttachDevTools();
#endif
	}

	public override void OnFrameworkInitializationCompleted() {
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
			// Avoid duplicate validations from both Avalonia and the CommunityToolkit.
			// More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
			DisableAvaloniaDataAnnotationValidation();
			desktop.MainWindow = new MainWindow {
				DataContext = new MainViewModel()
				,Width = 400
				,Height = 700
			};
		} else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
			singleViewPlatform.MainView = new MainView {
				DataContext = new MainViewModel()
			};
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
}
