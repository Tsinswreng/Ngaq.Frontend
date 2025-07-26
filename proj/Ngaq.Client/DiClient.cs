using Microsoft.Extensions.DependencyInjection;
using Ngaq.Client.Svc;
using Ngaq.Client.Word.Svc;
using Ngaq.Core.FrontendIF;
using Ngaq.Core.Infra;
using Ngaq.Core.Model.UserCtx;
using Ngaq.Core.Sys.Svc;
using Ngaq.Core.Word.Svc;

namespace Ngaq.Client;

public static class DiClient{
	public static IServiceCollection SetUpClient(this IServiceCollection z){
		z.AddScoped<ISvcUser, ClientUser>();
#if false //TODO 移至DiBrowser
		z.AddScoped<ISvcWord, ClientWord>();
		z.AddScoped<IUserCtxMgr, UserCtxMgr>();
		z.AddSingleton<IImgGetter>((s)=>null!);
#endif
		return z;
	}
}
