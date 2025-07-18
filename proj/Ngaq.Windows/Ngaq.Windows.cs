using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Client;
using Ngaq.Core;
using Ngaq.Core.Infra.Cfg;
using Ngaq.Local.Di;
using Ngaq.Local.Sql;
using Ngaq.Ui;
using Tsinswreng.AvlnTools.Navigation;

namespace Ngaq.Windows;

sealed class Program
{

	static str GetCfgFilePath(string[] args){
		var CfgFilePath = "";
		if(args.Length > 0){
			CfgFilePath = args[0];
		}else{
#if DEBUG
			CfgFilePath = "Ngaq.dev.jsonc";
#else
			CfgFilePath = "Ngaq.jsonc";
#endif
		}
		return CfgFilePath;
	}

	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args){
		try{
			System.Console.WriteLine(
				"pwd: "+Directory.GetCurrentDirectory()
			);
			var CfgPath = GetCfgFilePath(args);
			var CfgText = File.ReadAllText(CfgPath);
			LocalCfg.Inst.FromJson(CfgText);
			//AppCfg.Inst = AppCfgParser.Inst.FromYaml(GetCfgFilePath(args));
		}
		catch (System.Exception e){
			System.Console.Error.WriteLine("Failed to load config file: "+e);
		}

		var svc = new ServiceCollection();
		svc
			.SetUpCore()
			.SetUpLocal()//TODO 改成按需API調用
			.SetUpClient()
			.SetupUi()
		;
		var servicesProvider = svc.BuildServiceProvider();
		BuildAvaloniaApp()
			.AfterSetup(e=>{
				App.ConfigureServices(servicesProvider);
				var DbIniter = App.GetSvc<DbIniter>();
				_ = DbIniter.Init(default).Result;
			})
			.StartWithClassicDesktopLifetime(args)
		;
	}

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			//.WithInterFont()
			.LogToTrace();
}
