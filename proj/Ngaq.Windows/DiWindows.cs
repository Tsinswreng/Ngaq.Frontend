using Microsoft.Extensions.DependencyInjection;
using Ngaq.Ui.Views.Word.Query;
using Ngaq.Ui.Views.Word.WordManage.AddWord;

namespace Ngaq.Windows;

public static class DiWindows{
	public static IServiceCollection SetupWindows(
		this IServiceCollection z
	){
		z.AddTransient<VmAddWord>();
		z.AddTransient<VmWordQuery>();
		return z;
	}
}
