namespace Ngaq.Windows;

using Avalonia;
using Avalonia.Markup.Declarative;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Client;
using Ngaq.Core;
using Ngaq.Core.Infra.Cfg;
using Ngaq.Core.Infra.Url;
using Ngaq.Local;
using Ngaq.Local.Di;
using Ngaq.Local.Sql;
using Ngaq.Ui;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.CsCfg;
using Tsinswreng.CsTools;



sealed class Program {

	static str GetCfgFilePath(string[] args) {
		var CfgFilePath = "";
		if (args.Length > 0) {
			CfgFilePath = args[0];
		} else {
#if DEBUG
			CfgFilePath = "Ngaq.jsonc";
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
	public static async Task Main(string[] args) {
		if(args.Length > 1 && args[0] == "--version"){
			System.Console.WriteLine(1757779054280);
		}
		BaseDirMgr.Inst._BaseDir = Directory.GetCurrentDirectory();
		CT Ct = new();
		try {
/*
改潙勿合併配置字典
緣: 初始化旹各從用戶配置文件與Gui配置文件讀、汶合併。保存旹 併ᵣ後ˌʹ配置對象ˋ被寫入Gui配置文件。
下次初始化旹又各取ⁿ合併、則列表合併旹有褈、益益多矣
當改作 設多源配置、訪問配置項時 優先讀用戶配置、尋不見汶尋Gui配置。
保存旹只寫入Gui配置、緣用戶配置潙只讀。
 */
			System.Console.WriteLine(
				"pwd: " + Directory.GetCurrentDirectory()
			);
			var DualSrcCfg = AppCfg.Inst;
			var CfgPath = GetCfgFilePath(args);
			var RoCfg = new JsonFileCfgAccessor();
			DualSrcCfg.RoCfg = RoCfg;
			await RoCfg.FromFileAsy(CfgPath, Ct);

			var GuiCfgPath = ItemAppCfg.GuiConfigPath.GetFrom(DualSrcCfg) ?? "";
			ToolFile.EnsureFile(GuiCfgPath);
			var GuiCfg = new JsonFileCfgAccessor();
			DualSrcCfg.RwCfg = GuiCfg;
			await GuiCfg.FromFileAsy(GuiCfgPath, Ct);

			var Lang = ItemAppCfg.Lang.GetFrom(AppCfg.Inst)??"default";

			var I18nCfg = new JsonFileCfgAccessor();
			I18n.Inst.CfgAccessor = I18nCfg;
			await I18nCfg.FromFileAsy($"Languages/{Lang}.json", Ct);

		} catch (System.Exception e) {
			System.Console.Error.WriteLine("Failed to load config file: " + e);
		}

		var svc = new ServiceCollection();
		svc
			.SetupCore()
			.SetupLocal()//TODO 改成按需API調用
			.SetupLocalFrontend()
			.SetupClient()
			.SetupUi()
		;
		var svcProvider = svc.BuildServiceProvider();
		BuildAvaloniaApp()
#if DEBUG
			.UseHotReload()
			.UseRiderHotReload()
#endif
			.AfterSetup(e => {
				App.SetSvcProvider(svcProvider);
				AppIniter.Inst.SvcProvider = svcProvider;
				_ = AppIniter.Inst.Init(default).Result;
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
