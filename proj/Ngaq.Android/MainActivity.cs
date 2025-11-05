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
using Ngaq.Core.Infra.Url;


using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;

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
		var cwd = Directory.GetCurrentDirectory();
		Log.Info("abcdefg", "cwd: "+cwd);
		Log.Error("abcdefg", "abcdefg OnCreate");

		// 启动后台线程执行初始化，不阻塞主线程
		//_ = ExecuteInitInBackground(_cts.Token);
		//base.OnCreate璫置于末
		Init();
		base.OnCreate(savedInstanceState);
	}

	protected override void OnDestroy() {
		_cts.Cancel();
		_cts.Dispose(); // 新增：释放 CTS，避免内存泄漏
		base.OnDestroy();
	}

	// 关键修改2：在后台线程执行耗时初始化
	private async Task ExecuteInitInBackground(CT Ct) {
		if (_isInitialized) return;

		try {
			// 切换到后台线程执行 IO 操作（避免阻塞主线程）
			//await Task.Run(async () => await Init(Ct), Ct);
			await Task.Run(async () => Init(), Ct);
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

	void Init() {
		var appContext = global::Android.App.Application.Context;
		var externalFileDirObj = appContext.GetExternalFilesDir(null)
			?? throw new Exception("Cannot get ExternalFilesDir");
		;
		if(appContext.Assets is null){
			throw new Exception("Cannot get Assets");
		}
		if(appContext.FilesDir is null){
			throw new Exception("Cannot get FilesDir");
		}

		var externalFileDir = externalFileDirObj.AbsolutePath; // /sdcard/Android/data/<>/files
		BaseDirMgr.Inst._BaseDir = externalFileDir;
		var BaseDir = BaseDirMgr.Inst;

		var roCfgPath = BaseDir.Combine("Ngaq.jsonc");
		var dfltRwCfgPath = BaseDir.Combine("Ngaq.Rw.jsonc");
		if(!File.Exists(roCfgPath)){
			CopyAssetToDirectory("Ngaq.jsonc", externalFileDir);
			ToolFile.EnsureFile(roCfgPath);
			File.WriteAllText(roCfgPath, "{}");
		}

		var dualSrcCfg = AppCfg.Inst;
		// 1. 加载只读配置（优先从应用资产目录读取）

		var roCfg = new JsonFileCfgAccessor();
		dualSrcCfg.RoCfg = roCfg;
		roCfg.FromFile(roCfgPath);

		// 2. 加载可读写配置
		var rwCfgPath = ItemsClientCfg.RwCfgPath.GetFrom(dualSrcCfg)??dfltRwCfgPath;
		rwCfgPath = BaseDir.Combine(rwCfgPath);
		ToolFile.EnsureFile(rwCfgPath);

		var rwCfg = new JsonFileCfgAccessor();
		dualSrcCfg.RwCfg = rwCfg;
		rwCfg.FromFile(rwCfgPath);

		// 3. 初始化国际化配置
		var lang = ItemsClientCfg.Lang.GetFrom(AppCfg.Inst) ?? "default";
		var i18nCfg = new JsonFileCfgAccessor();
		I18n.Inst.CfgAccessor = i18nCfg;

		// var langFilePath = Path.Combine(appContext.FilesDir.AbsolutePath, $"Languages/{lang}.json");
		// if (!File.Exists(langFilePath)) {
		// 	var langDir = Path.GetDirectoryName(langFilePath)!;
		// 	Directory.CreateDirectory(langDir); // 确保语言目录存在
		// 	using var langAssetStream = appContext.Assets.Open($"Languages/{lang}.json");
		// 	using var langFileStream = new FileStream(langFilePath, FileMode.CreateNew);
		// 	langAssetStream.CopyTo(langFileStream);
		// }
		// i18nCfg.FromFile(langFilePath);

		var svc = new ServiceCollection();
		svc
			.SetupCore()
			.SetupLocal()
			.SetupLocalFrontend()
			.SetupClient()
			.SetupUi();
		//App.SetSvcProvider(svc.BuildServiceProvider());
		var serviceProvider = svc.BuildServiceProvider(new ServiceProviderOptions
		{
			ValidateOnBuild = false,
			ValidateScopes = false
		});

		App.SetSvcProvider(serviceProvider);
	}

	// 关键修改6：调整 Avalonia 初始化时机，确保服务已注册
	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) {
		return base.CustomizeAppBuilder(builder)
			.WithInterFont()
			.AfterSetup(async b => {
				// 等待服务初始化完成（最多等待 5 秒，避免无限阻塞）
				// var initWaitTask = WaitForInitComplete(_cts.Token);
				// if (await Task.WhenAny(initWaitTask, Task.Delay(5000)) != initWaitTask) {
				// 	throw new TimeoutException("服务初始化超时");
				// }
				// 初始化 AppIniter（等待完成，避免异步遗漏）
				AppIniter.Inst.SvcProvider = App.SvcProvider;
				await AppIniter.Inst.Init(_cts.Token);
			});
	}

	// 辅助方法：等待初始化完成
	private async Task WaitForInitComplete(CT Ct) {
		while (!_isInitialized && !Ct.IsCancellationRequested) {
			await Task.Delay(100, Ct);
		}
		if (Ct.IsCancellationRequested) {
			throw new TaskCanceledException();
		}
	}




/// <summary>
	/// 将 Assets 中的文件复制到指定目标路径
	/// </summary>
	/// <param name="assetFileName">Assets 中的文件名（含子路径，如 "configs/setting.json"）</param>
	/// <param name="targetDir">目标目录绝对路径（如外部存储的应用私有目录）</param>
	/// <returns>是否复制成功</returns>
	public static bool CopyAssetToDirectory(
		string assetFileName, string targetDir
	) {
		try {
			// 1. 获取 Android 上下文（访问 Assets 和文件系统）
			var context = global::Android.App.Application.Context;
			if (context == null) {
				throw new InvalidOperationException("无法获取 Android 上下文");
			}

			// 2. 确保目标目录存在
			if (!Directory.Exists(targetDir)) {
				Directory.CreateDirectory(targetDir);
			}

			// 3. 构建目标文件路径（目标目录 + 原文件名）
			string targetFilePath = Path.Combine(targetDir, Path.GetFileName(assetFileName));

			// 4. 若目标文件已存在，可选择跳过或覆盖（这里选择覆盖）
			if (File.Exists(targetFilePath)) {
				File.Delete(targetFilePath); // 覆盖现有文件
			}

			// 5. 从 Assets 打开文件流，并复制到目标路径
			using (Stream assetStream = context.Assets.Open(assetFileName)) // 关键：通过 AssetManager 打开 Assets 流
			using (FileStream targetStream = new FileStream(targetFilePath, FileMode.CreateNew)) {
				assetStream.CopyTo(targetStream); // 异步复制流
			}

			return true;
		} catch (Exception ex) {
			// 处理异常（如文件不存在、权限问题等）
			Console.WriteLine($"复制 Assets 文件失败：{ex.Message}");
			return false;
		}
	}



}



#if false
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
public class MainActivity : AvaloniaMainActivity<App> {
	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) {
		return base.CustomizeAppBuilder(builder)
			.WithInterFont();
	}
}

#endif
