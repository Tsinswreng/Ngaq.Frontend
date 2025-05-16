using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Core.Infra.Db;
using Ngaq.Core.Model.UserCtx;
using Ngaq.Core.Service.Word;

//using Microsoft.CodeAnalysis.Text;
using Ngaq.Core.Tools;
using Ngaq.Db;
using Ngaq.Local.Dao;
using Ngaq.Local.Db;
using Ngaq.Local.Service.Word;
using Ngaq.Ui;
using Ngaq.Ui.Views.WordManage.AddWord;

namespace Ngaq.Windows;

sealed class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args){
		var svc = new ServiceCollection();
		//new Setup().ConfigureServices(services);
		// services.AddSingleton<I_SeekFullWordKVByIdAsy, WordSeeker>();
		// services.AddTransient<WordCrudVm>();

		//svc.AddScoped<DbCtx, DbCtx>();
		svc.AddDbContext<DbCtx>();
		svc.AddScoped<Dao_Word, Dao_Word>();
		svc.AddScoped<I_TxnAsyFnRunner, TxnAsyFnRunner>();
		svc.AddScoped<I_Svc_ParseWordList, Svc_ParseWordList>();
		svc.AddScoped<I_Svc_Word, Svc_Word>();
		svc.AddScoped<I_UserCtxMgr, UserCtxMgr>();
		svc.AddTransient<Vm_AddWord>();

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
