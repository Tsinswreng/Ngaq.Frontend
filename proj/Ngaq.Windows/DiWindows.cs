using Microsoft.Extensions.DependencyInjection;
using Ngaq.Core.Shared.Audio;
using Ngaq.Windows.Domains.Audio;

namespace Ngaq.Windows;

public static class DiWindows{
	public static IServiceCollection SetupWindows(this IServiceCollection z){
		z.AddSingleton<IAudioPlayer, NAudioPlayer>();
		return z;
	}
}
