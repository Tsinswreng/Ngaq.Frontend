using System;
using Avalonia;
using Ngaq.Core.Util;

namespace Ngaq.Ui.Desktop;

sealed class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args){
		for(var i = 0; i < 1000; i++){
			var id = IdUtil.NewUInt128();
			var base64Url = IdUtil.ToBase64Url(id);
			var id2 = IdUtil.FromBase64Url(base64Url);
			System.Console.WriteLine(
				id+"  "+base64Url
			);
			if(id!= id2){
				throw new Exception("IdUtil test failed");
			}
		}
		BuildAvaloniaApp()
		.StartWithClassicDesktopLifetime(args);
	}

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace();
}
