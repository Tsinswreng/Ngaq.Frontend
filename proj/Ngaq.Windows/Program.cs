using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Local;
using Ngaq.Ui;
using Ngaq.Ui.Views.Word.Query;
using Ngaq.Ui.Views.Word.WordManage.AddWord;

namespace Ngaq.Windows;

sealed class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args){
		var svc = new ServiceCollection();
		DiLocal.SetUpLocal(svc);
		//new Setup().ConfigureServices(services);
		// services.AddSingleton<I_SeekFullWordKVByIdAsy, WordSeeker>();
		// services.AddTransient<WordCrudVm>();

		//svc.AddScoped<DbCtx, DbCtx>();

		// svc.AddScoped(
		// 	typeof(RepoSql<>)
		// 	,typeof(RepoSql<>)
		// );
		//<RepoSql, RepoSql>

		//svc.AddScoped<I_TxnRunnerAsy, EfTxnRunner>();
		svc.AddTransient<VmAddWord>();
		svc.AddTransient<VmWordQuery>();

		var servicesProvider = svc.BuildServiceProvider();
		BuildAvaloniaApp()
			.AfterSetup(e=>App.ConfigureServices(servicesProvider))
			.StartWithClassicDesktopLifetime(args)
		;
	}

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace();
}
