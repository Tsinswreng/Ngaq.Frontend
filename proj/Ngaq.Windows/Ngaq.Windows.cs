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
		CT Ct = new();
		try{
			System.Console.WriteLine(
				"pwd: "+Directory.GetCurrentDirectory()
			);
			var CfgPath = GetCfgFilePath(args);
			var localCfg = LocalCfg.Inst;
			await localCfg.FromFileAsy(CfgPath, Ct);

			var MergedCfgPath = LocalCfgItems.MergedConfigPath.GetFrom(localCfg)??"";
			ToolFile.EnsureFile(MergedCfgPath);
			var MergedCfg = new JsonFileCfgAccessor();
			localCfg.FnReLoadAsy = async(z, Ct)=>{
				await MergedCfg.FromFileAsy(MergedCfgPath, Ct);
				z.CfgDict = MergedCfg.CfgDict.ToDeepMerge(z.CfgDict);
				MergedCfg.CfgDict = z.CfgDict;
				return NIL;
			};
			await localCfg.ReLoadAsy(Ct);
			localCfg.FnSaveAsy = async(z,Ct)=>{
				MergedCfg.CfgDict = z.CfgDict;
				await MergedCfg._SaveAsy(Ct);
				return NIL;
			};
			//TODO 緟構多源配置框架
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
