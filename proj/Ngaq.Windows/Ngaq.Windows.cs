using Avalonia;
using Avalonia.Markup.Declarative;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Client;
using Ngaq.Core;
using Ngaq.Core.Infra.Cfg;
using Ngaq.Local.Di;
using Ngaq.Local.Sql;
using Ngaq.Ui;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.CsCfg;
using Tsinswreng.CsTools;

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
	public static async Task Main(string[] args){
		try{
			System.Console.WriteLine(
				"pwd: "+Directory.GetCurrentDirectory()
			);
			var CfgPath = GetCfgFilePath(args);
			var CfgText = File.ReadAllText(CfgPath);
			var localCfg = LocalCfg.Inst;
			localCfg.FromJson(CfgText);

			var MergedCfgPath = LocalCfgItems.Inst.MergedConfigPath.GetFrom(localCfg)??"";
			ToolFile.EnsureFile(MergedCfgPath);
			var MergedCfg = new JsonFileCfgAccessor();
			await MergedCfg.FromFileAsy(MergedCfgPath, default);
			localCfg.CfgDict = MergedCfg.CfgDict.ToDeepMerge(localCfg.CfgDict);
			localCfg.FnSaveAsy = MergedCfg._SaveAsy;
			localCfg.FnReLoadAsy = MergedCfg._ReLoadAsy;

			// var MergedCfg = new JsonFileCfgAccessor().FromJson(
			// 	File.ReadAllText(MergedCfgPath)
			// );
			// MergedCfg.CfgDict = MergedCfg.CfgDict.ToDeepMerge(
			// 	MergedCfg.CfgDict
			// ).ToDeepMerge(
			// 	LocalCfg.Inst.CfgDict
			// );


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
#if DEBUG
			.UseHotReload()
			.UseRiderHotReload()
#endif
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
