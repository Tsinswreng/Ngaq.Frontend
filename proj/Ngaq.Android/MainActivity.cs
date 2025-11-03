using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Client;
using Ngaq.Core;
using Ngaq.Core.Infra.Cfg;
using Ngaq.Local.Di;
using Ngaq.Ui;
using Ngaq.Ui.Infra.I18n;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tsinswreng.CsCfg;
using Tsinswreng.CsTools;

namespace Ngaq.Android;

[Activity(
	Label = "Ngaq",
	Theme = "@style/MyTheme.NoActionBar",
	Icon = "@drawable/icon",
	MainLauncher = true,
	ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public partial class MainActivity : AvaloniaMainActivity<App> {
	private CancellationTokenSource _cts = new();

	protected override async void OnCreate(Bundle? savedInstanceState) {
		base.OnCreate(savedInstanceState);
		await InitializeAppAsync(_cts.Token);
	}

	protected override void OnDestroy() {
		_cts.Cancel();
		base.OnDestroy();
	}

	private async Task InitializeAppAsync(CancellationToken ct) {
		try {
			// 初始化配置系统
			var dualSrcCfg = AppCfg.Inst;

			// 1. 加载只读配置（优先从应用资产目录读取）
			var roCfg = new JsonFileCfgAccessor();
			dualSrcCfg.RoCfg = roCfg;

			// 修复：通过类型名访问静态成员 Application.Context
			var appContext = global::Android.App.Application.Context;
			var roCfgPath = Path.Combine(appContext.FilesDir.AbsolutePath, "Ngaq.jsonc");

			// 首次启动时从Assets复制默认配置
			if (!File.Exists(roCfgPath)) {
				using var assetStream = appContext.Assets.Open("Ngaq.jsonc");
				using var fileStream = new FileStream(roCfgPath, FileMode.CreateNew);
				await assetStream.CopyToAsync(fileStream, ct);
			}
			await roCfg.FromFileAsy(roCfgPath, ct);

			// 2. 加载可读写配置（应用数据目录）
			var guiCfgPath = ItemAppCfg.GuiConfigPath.GetFrom(dualSrcCfg)
						   ?? Path.Combine(appContext.FilesDir.AbsolutePath, "NgaqGui.jsonc");
			ToolFile.EnsureFile(guiCfgPath);
			var guiCfg = new JsonFileCfgAccessor();
			dualSrcCfg.RwCfg = guiCfg;
			await guiCfg.FromFileAsy(guiCfgPath, ct);

			// 3. 初始化国际化配置
			var lang = ItemAppCfg.Lang.GetFrom(AppCfg.Inst) ?? "default";
			var i18nCfg = new JsonFileCfgAccessor();
			I18n.Inst.CfgAccessor = i18nCfg;

			var langFilePath = Path.Combine(appContext.FilesDir.AbsolutePath, $"Languages/{lang}.json");
			if (!File.Exists(langFilePath)) {
				Directory.CreateDirectory(Path.GetDirectoryName(langFilePath)!);
				using var langAssetStream = appContext.Assets.Open($"Languages/{lang}.json");
				using var langFileStream = new FileStream(langFilePath, FileMode.CreateNew);
				await langAssetStream.CopyToAsync(langFileStream, ct);
			}
			await i18nCfg.FromFileAsy(langFilePath, ct);

			// 4. 注册服务
			var svc = new ServiceCollection();
			svc
				.SetupCore()
				.SetupLocal()
				.SetupLocalFrontend()
				.SetupClient()
				.SetupUi()
			;
			App.SetSvcProvider(svc.BuildServiceProvider());
		} catch (Exception e) {
			System.Console.Error.WriteLine("初始化失败: " + e);
		}
	}

	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) {
		return base.CustomizeAppBuilder(builder)
			.WithInterFont()
			.AfterSetup(_ => {
				_ = AppIniter.Inst.Init(default);
			});
	}
}
