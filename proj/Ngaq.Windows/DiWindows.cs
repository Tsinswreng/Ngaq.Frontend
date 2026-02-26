using Microsoft.Extensions.DependencyInjection;
using Ngaq.Core.Frontend.Clipboard;
using Ngaq.Core.Frontend.Hotkey;
using Ngaq.Core.Shared.Audio;
using Ngaq.Ui.Infra.Hotkey;
using Ngaq.Windows.Domains.Audio;
using Ngaq.Windows.Domains.Clipboard;
using Ngaq.Windows.Domains.Hotkey;

namespace Ngaq.Windows;

public static class DiWindows{
	public static IServiceCollection SetupWindows(this IServiceCollection z){
		z.AddSingleton<IAudioPlayer, NAudioPlayer>();
		z.AddSingleton<ISvcClipboard, SvcClipboard>();
		z.AddSingleton<IHotkeyListener, WinHotkeyListener>();
		// Windows 专用全局快捷键注册器
		z.AddSingleton<I_RegisterGlobalHotKeys, WinGlobalHotkeyRegistrar>();
		return z;
	}
}
