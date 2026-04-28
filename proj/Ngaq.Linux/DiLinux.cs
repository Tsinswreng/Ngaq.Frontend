using Microsoft.Extensions.DependencyInjection;
using Ngaq.Core.Frontend;
using Ngaq.Linux.Domains.Audio;
using Ngaq.Linux.Domains.Hotkey;

namespace Ngaq.Linux;

public static class DiLinux{
	public static IServiceCollection SetupLinux(this IServiceCollection Z){
		// Linux 平台: 音頻使用 LinuxAudioPlayer，熱鍵使用 Linux 專用註冊器與監聽器。
		return Z.SetupPlatformFrontend<LinuxAudioPlayer, LinuxHotkeyListener, LinuxGlobalHotkeyRegistrar>();
	}
}
