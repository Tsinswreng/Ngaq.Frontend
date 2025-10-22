using Microsoft.Extensions.DependencyInjection;
using Ngaq.Client.Svc;
using Ngaq.Core.Shared.User.Svc;

namespace Ngaq.Client;

public static class DiClient{
	public static IServiceCollection SetupClient(this IServiceCollection z){
		z.AddScoped<ISvcUser, ClientUser>();
#if false //TODO 移至DiBrowser
		z.AddScoped<ISvcWord, ClientWord>();
		z.AddScoped<IUserCtxMgr, UserCtxMgr>();
		z.AddSingleton<IImgGetter>((s)=>null!);
#endif
		return z;
	}
}
