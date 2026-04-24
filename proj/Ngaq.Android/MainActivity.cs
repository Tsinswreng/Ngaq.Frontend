using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Avalonia;
using Avalonia.Android;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Backend;
using Ngaq.Backend.Di;
using Ngaq.Client;
using Ngaq.Core;
using Ngaq.Core.Infra.Cfg;
using Ngaq.Core.Infra.Url;
using Ngaq.Ui;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Views.Dictionary;
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
	LaunchMode = LaunchMode.SingleTop,
	ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public partial class MainActivity : AvaloniaMainActivity<App>{
	private const int DictionaryLookupNotificationId = 43001;
	private const int NotificationPermissionRequestCode = 43002;
	private const string DictionaryLookupNotificationChannelId = "ngaq_dictionary_lookup";
	private const string DictionaryLookupNotificationAction = "Tsinswreng.Ngaq.action.DICTIONARY_LOOKUP_FROM_CLIPBOARD";
	private const string DictionaryLookupIntentFlagKey = "dictionary_lookup_from_clipboard";

	private readonly CancellationTokenSource _cts = new();
	private bool _pendingLookupFromNotificationTap = false;

	protected override void OnCreate(Bundle? savedInstanceState){
		Log.Info("Ngaq.Android", "MainActivity.OnCreate");
		Init();
		CaptureDictionaryLookupIntent(Intent);
		base.OnCreate(savedInstanceState);
		EnsureDictionaryLookupNotification();
		TryConsumePendingDictionaryLookupIntent();
	}

	protected override void OnDestroy(){
		_cts.Cancel();
		_cts.Dispose();
		base.OnDestroy();
	}

	protected override void OnNewIntent(Intent? intent){
		base.OnNewIntent(intent);
		CaptureDictionaryLookupIntent(intent);
		TryConsumePendingDictionaryLookupIntent();
	}

	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder){
		return base.CustomizeAppBuilder(builder)
			.WithInterFont()
			.AfterSetup(async _ => {
				AppIniter.Inst.Sp = App.SvcProvider;
				await AppIniter.Inst.Init(_cts.Token);
				EnsureDictionaryLookupNotification();
				TryConsumePendingDictionaryLookupIntent();
			});
	}

	public override void OnRequestPermissionsResult(
		int requestCode, string[]? permissions, Permission[]? grantResults
	){
		base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		if(requestCode != NotificationPermissionRequestCode){
			return;
		}
		if(grantResults is null || grantResults.Length == 0){
			return;
		}
		if(grantResults[0] != Permission.Granted){
			Log.Warn("Ngaq.Android", "POST_NOTIFICATIONS denied, persistent dictionary notification disabled.");
			return;
		}
		EnsureDictionaryLookupNotification();
	}

	private void Init(){
		var appContext = global::Android.App.Application.Context;
		var externalFileDirObj = appContext.GetExternalFilesDir(null)
			?? throw new Exception("Cannot get ExternalFilesDir");
		if(appContext.Assets is null){
			throw new Exception("Cannot get Assets");
		}
		if(appContext.FilesDir is null){
			throw new Exception("Cannot get FilesDir");
		}

		var externalFileDir = externalFileDirObj.AbsolutePath;
		BaseDirMgr.Inst._BaseDir = externalFileDir;
		var baseDir = BaseDirMgr.Inst;

		var roCfgPath = baseDir.Combine("Ngaq.jsonc");
		var dfltRwCfgPath = baseDir.Combine("Ngaq.Rw.jsonc");
		if(!File.Exists(roCfgPath)){
			CopyAssetToDirectory("Ngaq.jsonc", externalFileDir);
			ToolFile.EnsureFile(roCfgPath);
			File.WriteAllText(roCfgPath, "{}");
		}

		var dualSrcCfg = AppCfg.Inst;
		var roCfg = new JsonFileCfgAccessor();
		dualSrcCfg.RoCfg = roCfg;
		roCfg.FromFile(roCfgPath);

		var rwCfgPath = KeysClientCfg.RwCfgPath.GetFrom(dualSrcCfg) ?? dfltRwCfgPath;
		rwCfgPath = baseDir.Combine(rwCfgPath);
		ToolFile.EnsureFile(rwCfgPath);

		var rwCfg = new JsonFileCfgAccessor();
		dualSrcCfg.RwCfg = rwCfg;
		rwCfg.FromFile(rwCfgPath);

		var i18nCfg = new JsonFileCfgAccessor();
		AppI18n.Inst.CfgAccessor = i18nCfg;

		var svc = new ServiceCollection();
		svc
			.SetupCore()
			.SetupLocal()
			.SetupLocalFrontend()
			.SetupClient()
			.SetupUi()
			.SetupAndroid();
		var serviceProvider = svc.BuildServiceProvider(new ServiceProviderOptions{
			ValidateOnBuild = false,
			ValidateScopes = false
		});
		App.SetSvcProvider(serviceProvider);
	}

	private void EnsureDictionaryLookupNotification(){
		if(
			Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu
			&& CheckSelfPermission(Manifest.Permission.PostNotifications) != Permission.Granted
		){
			RequestPermissions([Manifest.Permission.PostNotifications], NotificationPermissionRequestCode);
			return;
		}

		var nm = GetSystemService(NotificationService) as NotificationManager;
		if(nm is null){
			Log.Warn("Ngaq.Android", "NotificationManager unavailable; skip dictionary lookup notification.");
			return;
		}

		if(Build.VERSION.SdkInt >= BuildVersionCodes.O){
			var channel = new NotificationChannel(
				DictionaryLookupNotificationChannelId,
				"Ngaq Dictionary",
				NotificationImportance.Low
			){
				Description = "Tap to lookup clipboard text in Ngaq dictionary."
			};
			nm.CreateNotificationChannel(channel);
		}

		var tapIntent = new Intent(this, typeof(MainActivity));
		tapIntent.SetAction(DictionaryLookupNotificationAction);
		tapIntent.PutExtra(DictionaryLookupIntentFlagKey, true);
		tapIntent.AddFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

		var pendingIntentFlags = PendingIntentFlags.UpdateCurrent;
		if(Build.VERSION.SdkInt >= BuildVersionCodes.M){
			pendingIntentFlags |= PendingIntentFlags.Immutable;
		}
		var tapPendingIntent = PendingIntent.GetActivity(
			this,
			DictionaryLookupNotificationId,
			tapIntent,
			pendingIntentFlags
		);

		var builder = Build.VERSION.SdkInt >= BuildVersionCodes.O
			? new Notification.Builder(this, DictionaryLookupNotificationChannelId)
			: new Notification.Builder(this);
		builder
			.SetContentTitle("Ngaq 查詞")
			.SetContentText("點擊後讀取剪貼板並自動查詞")
			.SetSmallIcon(global::Android.Resource.Drawable.IcMenuSearch)
			.SetOngoing(true)
			.SetOnlyAlertOnce(true)
			.SetAutoCancel(false)
			.SetContentIntent(tapPendingIntent);

		nm.Notify(DictionaryLookupNotificationId, builder.Build());
	}

	private void CaptureDictionaryLookupIntent(Intent? intent){
		if(intent is null){
			return;
		}
		var fromAction = intent.Action == DictionaryLookupNotificationAction;
		var fromExtra = intent.GetBooleanExtra(DictionaryLookupIntentFlagKey, false);
		if(fromAction || fromExtra){
			_pendingLookupFromNotificationTap = true;
		}
	}

	private void TryConsumePendingDictionaryLookupIntent(){
		if(!_pendingLookupFromNotificationTap){
			return;
		}
		if(App.SvcProvider is null){
			return;
		}

		_pendingLookupFromNotificationTap = false;
		Dispatcher.UIThread.Post(async ()=>{
			try{
				var action = App.GetSvc<IHotkeyDictionaryLookupAction>();
				if(action is null){
					Log.Warn("Ngaq.Android", "IHotkeyDictionaryLookupAction unavailable.");
					return;
				}
				await action.Run(_cts.Token);
			}catch(Exception ex){
				Log.Error("Ngaq.Android", "Dictionary lookup from notification failed: " + ex);
			}
		});
	}

	/// 将 Assets 中的文件复制到指定目标路径。
	public static bool CopyAssetToDirectory(string assetFileName, string targetDir){
		try{
			var context = global::Android.App.Application.Context;
			if(context is null){
				throw new InvalidOperationException("Cannot get Android context");
			}

			if(!Directory.Exists(targetDir)){
				Directory.CreateDirectory(targetDir);
			}

			var targetFilePath = Path.Combine(targetDir, Path.GetFileName(assetFileName));
			if(File.Exists(targetFilePath)){
				File.Delete(targetFilePath);
			}

			using var assetStream = context.Assets.Open(assetFileName);
			using var targetStream = new FileStream(targetFilePath, FileMode.CreateNew);
			assetStream.CopyTo(targetStream);
			return true;
		}catch(Exception ex){
			Console.WriteLine($"Copy asset failed: {ex.Message}");
			return false;
		}
	}
}
