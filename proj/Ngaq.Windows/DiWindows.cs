using Microsoft.Extensions.DependencyInjection;
using Ngaq.Core.Frontend.Clipboard;
using Ngaq.Core.Shared.Audio;
using Ngaq.Windows.Domains.Audio;
using Ngaq.Windows.Domains.Clipboard;

namespace Ngaq.Windows;

public static class DiWindows{
	public static IServiceCollection SetupWindows(this IServiceCollection z){
		z.AddSingleton<IAudioPlayer, NAudioPlayer>();
		z.AddSingleton<ISvcClipboard, SvcClipboard>();
		return z;
	}
}
