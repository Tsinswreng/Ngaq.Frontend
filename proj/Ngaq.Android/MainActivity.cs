using Android.App;
using Android.Util;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Avalonia;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Client;
using Ngaq.Core;
using Ngaq.Core.Infra.Cfg;
using Ngaq.Local;
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
	// 新增：标记初始化是否完成，避免重复执行
	private bool _isInitialized = false;

	// 关键修改1：将 OnCreate 改为同步方法，避免 async void 生命周期问题
	protected override void OnCreate(Bundle? savedInstanceState) {
		Log.Info("abcdefg", "OnCreate");
		base.OnCreate(savedInstanceState);

		// 4. 注册服务（Avalonia 依赖此服务，需在框架初始化前完成）
		var svc = new ServiceCollection();
		svc
			.SetupCore()
			.SetupLocal()
			.SetupLocalFrontend()
			.SetupClient()
			.SetupUi();
		App.SetSvcProvider(svc.BuildServiceProvider());

		// 启动后台线程执行初始化，不阻塞主线程
		//_ = ExecuteInitInBackground(_cts.Token);
	}

	protected override void OnDestroy() {
		_cts.Cancel();
		_cts.Dispose(); // 新增：释放 CTS，避免内存泄漏
		base.OnDestroy();
	}

	// 关键修改2：在后台线程执行耗时初始化
	private async Task ExecuteInitInBackground(CancellationToken ct) {
		if (_isInitialized) return;

		try {
			// 切换到后台线程执行 IO 操作（避免阻塞主线程）
			await Task.Run(async () => await InitializeAppAsync(ct), ct);
			_isInitialized = true;

			// 初始化完成后，通过主线程通知（可选，如需要提示用户）
			RunOnUiThread(() => {
				Toast.MakeText(this, "初始化完成", ToastLength.Short)?.Show();
			});
		}
		catch (TaskCanceledException) {
			// 忽略取消异常（OnDestroy 时触发）
		}
		catch (Exception e) {
			// 关键修改3：捕获异常并通过 Toast 提示，而非仅打印控制台
			RunOnUiThread(() => {
				Toast.MakeText(this, $"初始化失败：{e.Message}", ToastLength.Long)?.Show();
				System.Console.Error.WriteLine("初始化失败: " + e);
			});
		}
	}

	private async Task InitializeAppAsync(CancellationToken ct) {
		// 初始化配置系统
		var dualSrcCfg = AppCfg.Inst;
		var appContext = global::Android.App.Application.Context;

		// 1. 加载只读配置（优先从应用资产目录读取）
		var roCfg = new JsonFileCfgAccessor();
		dualSrcCfg.RoCfg = roCfg;
		var roCfgPath = Path.Combine(appContext.FilesDir.AbsolutePath, "Ngaq.jsonc");

		// 首次启动时从 Assets 复制默认配置（增加 ct 取消支持）
		if (!File.Exists(roCfgPath)) {
			using var assetStream = appContext.Assets.Open("Ngaq.jsonc");
			using var fileStream = new FileStream(roCfgPath, FileMode.CreateNew);
			// 关键修改4：传递 ct 支持取消，避免阻塞
			await assetStream.CopyToAsync(fileStream, ct);
		}
		await roCfg.FromFileAsy(roCfgPath, ct);

		// 2. 加载可读写配置（应用数据目录）
		var guiCfgPath = ItemAppCfg.GuiConfigPath.GetFrom(dualSrcCfg)
					   ?? Path.Combine(appContext.FilesDir.AbsolutePath, "NgaqGui.jsonc");
		// 关键修改5：确保目录存在（原 ToolFile.EnsureFile 可能未处理目录）
		Directory.CreateDirectory(Path.GetDirectoryName(guiCfgPath)!);
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
			var langDir = Path.GetDirectoryName(langFilePath)!;
			Directory.CreateDirectory(langDir); // 确保语言目录存在
			using var langAssetStream = appContext.Assets.Open($"Languages/{lang}.json");
			using var langFileStream = new FileStream(langFilePath, FileMode.CreateNew);
			await langAssetStream.CopyToAsync(langFileStream, ct);
		}
		await i18nCfg.FromFileAsy(langFilePath, ct);
	}

	// 关键修改6：调整 Avalonia 初始化时机，确保服务已注册
	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) {
		return base.CustomizeAppBuilder(builder)
			.WithInterFont()
			.AfterSetup(async b => {
				// 等待服务初始化完成（最多等待 5 秒，避免无限阻塞）
				var initWaitTask = WaitForInitComplete(_cts.Token);
				if (await Task.WhenAny(initWaitTask, Task.Delay(5000)) != initWaitTask) {
					throw new TimeoutException("服务初始化超时");
				}
				// 初始化 AppIniter（等待完成，避免异步遗漏）
				AppIniter.Inst.SvcProvider = App.SvcProvider;
				await AppIniter.Inst.Init(_cts.Token);
			});
	}

	// 辅助方法：等待初始化完成
	private async Task WaitForInitComplete(CancellationToken ct) {
		while (!_isInitialized && !ct.IsCancellationRequested) {
			await Task.Delay(100, ct);
		}
		if (ct.IsCancellationRequested) {
			throw new TaskCanceledException();
		}
	}
}
