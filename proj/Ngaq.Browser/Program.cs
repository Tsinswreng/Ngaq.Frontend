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
		System.Console.WriteLine(111);//t
		svc
			.SetUpCore()
			.SetupUi()
			.SetupBrowser()
			//.SetUpLocal()//TODO 改成按需API調用
			.SetUpClient()
		;
		System.Console.WriteLine(123);//t
		var servicesProvider = svc.BuildServiceProvider();

		return BuildAvaloniaApp()
		.AfterSetup(e=>{
			App.ConfigureServices(servicesProvider);
		})
		.WithInterFont()
		.StartBrowserAppAsync("out");//avalonia窗口 ʃʰ掛載ʹhtml元素

	}

	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>();
}
