using System;
using System.Data;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Core.Infra.Db;
using Ngaq.Core.Model.Po.Kv;
using Ngaq.Core.Model.Po.Learn;
using Ngaq.Core.Model.Po.Word;
using Ngaq.Core.Model.UserCtx;
using Ngaq.Core.Service.Word;

//using Microsoft.CodeAnalysis.Text;
using Ngaq.Core.Tools;
using Ngaq.Db;
using Ngaq.Local.Dao;
using Ngaq.Local.Db;
using Ngaq.Local.Service.Word;
using Ngaq.Ui;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Tsinswreng.SqlHelper;
using Tsinswreng.SqlHelper.Cmd;

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
		svc.AddDbContext<LocalDbCtx>();
		svc.AddScoped<DaoWord, DaoWord>();
		svc.AddScoped<DaoSqlWord, DaoSqlWord>();
		svc.AddScoped<ISqlCmdMkr, SqlCmdMkr>();
		svc.AddSingleton<IDbConnection>(AppTblInfo.Inst.DbConnection);
		svc.AddSingleton<ITableMgr>(AppTableMgr.Inst);
		// svc.AddScoped(
		// 	typeof(RepoSql<>)
		// 	,typeof(RepoSql<>)
		// );
		//<RepoSql, RepoSql>

svc.AddScoped<RepoSql<PoWord,	IdWord>>();
svc.AddScoped<RepoSql<PoKv,	IdKv>>();
svc.AddScoped<RepoSql<PoLearn,	IdLearn>>();

		//svc.AddScoped<I_TxnRunnerAsy, EfTxnRunner>();
		svc.AddScoped<IRunInTxn, SqlTxnRunner>();
		svc.AddScoped<ITxnRunner, SqlTxnRunner>();
		svc.AddScoped<ISvcParseWordList, SvcParseWordList>();
		svc.AddScoped<ISvcWord, SvcWord>();
		svc.AddScoped<IUserCtxMgr, UserCtxMgr>();
		svc.AddScoped<IGetTxn, SqlCmdMkr>();
		svc.AddTransient<VmAddWord>();

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
