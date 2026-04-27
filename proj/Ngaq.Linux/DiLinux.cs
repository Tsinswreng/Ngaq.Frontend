using Microsoft.Extensions.DependencyInjection;
using Ngaq.Core.Frontend.Hotkey;
using Ngaq.Core.Shared.Audio;
using Ngaq.Linux.Domains.Audio;




namespace Ngaq.Linux;

public static class DiLinux{
	public static IServiceCollection SetupLinux(this IServiceCollection z){
		z.AddSingleton<IHotkeyListener, LinuxHotkeyListener>();
		z.AddSingleton<I_RegisterGlobalHotKeys, LinuxGlobalHotkeyRegistrar>();

		// Linux 端使用 LinuxAudioPlayer，避免依賴 Windows 專用庫。
		z.AddSingleton<IAudioPlayer, LinuxAudioPlayer>();
		return z;
	}
}
