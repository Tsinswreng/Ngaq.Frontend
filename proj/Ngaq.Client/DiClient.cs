using Microsoft.Extensions.DependencyInjection;
using Ngaq.Client.Svc;
using Ngaq.Core.Sys.Svc;

namespace Ngaq.Client;

public static class DiClient{
	public static IServiceCollection SetUpClient(this IServiceCollection z){
		z.AddScoped<ISvcUser, ClientUser>();
		return z;
	}
}
