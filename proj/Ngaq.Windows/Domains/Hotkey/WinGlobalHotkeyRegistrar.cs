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
			// 示例：注册快捷键 Alt+W
			var success = _hotkeyListener.Register(
				HotkeyId: "alt_w",
				Modifiers: EHotkeyModifiers.Alt,
				Key: EHotkeyKey.W,
				OnHotkey: async (Req, Ct) => {
					System.Console.WriteLine("🎉 [Global Hotkey] Alt+W triggered! 快捷键 Alt+W 触发。");
					return null;
				}
			);
			if(!success){
				_logger?.LogWarning("Alt+W registration failed (可能被系统占用)");
			}else{
				_logger?.LogInformation("Alt+W registered successfully");
			}

			_logger?.LogInformation("Windows global hotkeys registered successfully");
		}catch(Exception ex){
			_logger?.LogError(ex, "Failed to register Windows global hotkeys");
		}
		return NIL;
	}
}
