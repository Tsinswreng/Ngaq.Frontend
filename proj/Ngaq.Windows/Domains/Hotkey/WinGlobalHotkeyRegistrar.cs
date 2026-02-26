namespace Ngaq.Windows.Domains.Hotkey;

using System;
using System.Threading.Tasks;
using Ngaq.Core.Frontend.Hotkey;
using Microsoft.Extensions.Logging;
using Ngaq.Ui.Infra.Hotkey;

/// <summary>
/// Windows 专用的全局快捷键注册器
/// 负责在 Windows 平台启动时为应用统一注册所有需要的快捷键
/// </summary>
public class WinGlobalHotkeyRegistrar : I_RegisterGlobalHotKeys{
	private readonly IHotkeyListener _hotkeyListener;
	private readonly ILogger _logger;

	public WinGlobalHotkeyRegistrar(
		IHotkeyListener hotkeyListener,
		ILogger logger
	){
		_hotkeyListener = hotkeyListener;
		_logger = logger;
	}

	public nil RegisterGlobalHotKeys(){
		try{
			// 示例：注册测试快捷键 Ctrl+Shift+T
			_ = _hotkeyListener.Register(
				HotkeyId: "test_hotkey_1",
				Modifiers: EHotkeyModifiers.Ctrl | EHotkeyModifiers.Shift,
				Key: EHotkeyKey.T,
				OnHotkey: async (Ct) => {
					System.Console.WriteLine("🎉 [Global Hotkey] Ctrl+Shift+T triggered! 全局快捷键被触发了!");
					await Task.CompletedTask;
				},
				Ct: default
			).ConfigureAwait(false);

			_logger?.LogInformation("Windows global hotkeys registered successfully");
		}catch(Exception ex){
			_logger?.LogError(ex, "Failed to register Windows global hotkeys");
		}
		return NIL;
	}
}
