namespace Ngaq.Windows.Domains.Hotkey;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using Ngaq.Core.Frontend.Clipboard;
using Ngaq.Core.Frontend.Hotkey;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Dictionary;
using Tsinswreng.CsErr;

/// Windows 专用的全局快捷键注册器
/// TODO 把查詞快捷键之註冊邏輯放到新文件中、然後在此文件中調用
public class WinGlobalHotkeyRegistrar : I_RegisterGlobalHotKeys{
	private readonly IHotkeyListener _hotkeyListener;
	private readonly ISvcClipboard _svcClipboard;
	private readonly ILogger _logger;

	public WinGlobalHotkeyRegistrar(
		IHotkeyListener hotkeyListener,
		ISvcClipboard svcClipboard,
		ILogger logger
	){
		_hotkeyListener = hotkeyListener;
		_svcClipboard = svcClipboard;
		_logger = logger;
	}

	public IAnswer<obj?> RegisterGlobalHotKeys(){
		var R = new Answer<obj?>();
		try{
			var hotkey = new HotKey{
				Id = "alt_e",
				Modifiers = EHotkeyModifiers.Alt,
				Key = EHotkeyKey.E,
				OnHotkey = async (Req, Ct) => {
					Dispatcher.UIThread.Post(async () => {
						try{
							ShowMainWindow();

							var view = new ViewDictionary();
							MgrViewNavi.Inst.GetViewNavi().GoTo(ToolView.WithTitle("Dictionary", view));

							var clipText = _svcClipboard.GetText(Ct)?.Trim();
							if(!string.IsNullOrWhiteSpace(clipText)){
								view.ClickLookupBtn();
							}
						}catch(Exception ex){
							_logger?.LogError(ex, "Alt+E hotkey handler failed");
						}
					});
					return null;
				}
			};

			var result = _hotkeyListener.Register(hotkey);
			if(!result.Ok){
				_logger?.LogWarning("Alt+E registration failed\n"+string.Join(";", result.Errors ?? new[]{""}));
			}else{
				_logger?.LogInformation("Alt+E registered successfully");
			}

			_logger?.LogInformation("Windows global hotkeys registered successfully");
			return R.OkWith(NIL);
		}catch(Exception ex){
			return R.AddErr(ex);
		}
	}

	private static nil ShowMainWindow(){
		var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
		var mainWindow = desktop?.MainWindow;
		if(mainWindow == null){
			return NIL;
		}

		if(!mainWindow.IsVisible){
			mainWindow.Show();
		}
		if(mainWindow.WindowState == WindowState.Minimized){
			mainWindow.WindowState = WindowState.Normal;
		}
		mainWindow.Activate();
		return NIL;
	}
}
