using Microsoft.Extensions.DependencyInjection;
using Ngaq.Core.Shared.Audio;
using Ngaq.Linux.Domains.Audio;


namespace Ngaq.Linux;

public static class DiLinux{
	public static IServiceCollection SetupLinux(this IServiceCollection z){
		z.AddSingleton<IAudioPlayer, NAudioPlayer>();
		// Linux 专用全局快捷键注册器
		return z;
	}
}
