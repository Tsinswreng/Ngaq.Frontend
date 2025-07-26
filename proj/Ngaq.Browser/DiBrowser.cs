using Microsoft.Extensions.DependencyInjection;
using Ngaq.Browser.Infra;
using Ngaq.Core.Infra;

namespace Ngaq.Browser;
public static class DiUi{
	public static IServiceCollection SetupBrowser(
		this IServiceCollection z
	){
		z.AddSingleton<I_GetBaseUrl, BaseUrl>();
		return z;
	}
}
