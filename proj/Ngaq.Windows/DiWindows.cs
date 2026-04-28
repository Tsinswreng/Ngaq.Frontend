using Microsoft.Extensions.DependencyInjection;
using Ngaq.Core.Frontend;
using Ngaq.Windows.Domains.Audio;
using Ngaq.Windows.Domains.Hotkey;

namespace Ngaq.Windows;

public static class DiWindows{
	public static IServiceCollection SetupWindows(this IServiceCollection Z){
		// Windows 平台: 音頻使用 NAudio，熱鍵使用 Win32 全局熱鍵實現。
		return Z.SetupPlatformFrontend<NAudioPlayer, WinHotkeyListener, WinGlobalHotkeyRegistrar>();
	}
}
