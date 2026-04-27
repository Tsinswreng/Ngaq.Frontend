using Microsoft.Extensions.DependencyInjection;
using Ngaq.Core.Frontend.Hotkey;
using Ngaq.Core.Shared.Audio;
using Ngaq.Linux.Domains.Audio;
using Ngaq.Linux.Domains.Hotkey;

namespace Ngaq.Linux;

public static class DiLinux{
	public static IServiceCollection SetupLinux(this IServiceCollection z){
		z.AddSingleton<IAudioPlayer, NAudioPlayer>();
		z.AddSingleton<IHotkeyListener, WinHotkeyListener>();
		// Linux 专用全局快捷键注册器
		z.AddSingleton<I_RegisterGlobalHotKeys, WinGlobalHotkeyRegistrar>();
		return z;
	}
}
