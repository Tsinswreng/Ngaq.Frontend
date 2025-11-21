using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Core;
using Ngaq.Ui;
using Ngaq.Client;
namespace Ngaq.Browser;
internal sealed partial class Program {
	private static Task Main(string[] args){

		var svc = new ServiceCollection();
		svc
			.SetupCore()
			.SetupUi()
			.SetupBrowser()
			//.SetUpLocal()//TODO 改成按需API調用
			.SetupClient()
		;
		var servicesProvider = svc.BuildServiceProvider();

		return BuildAvaloniaApp()
		.AfterSetup(e=>{
			App.SetSvcProvider(servicesProvider);
		})
		.WithInterFont()
		.StartBrowserAppAsync("out");//avalonia窗口 ʃʰ掛載ʹhtml元素

	}

	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>();
}
