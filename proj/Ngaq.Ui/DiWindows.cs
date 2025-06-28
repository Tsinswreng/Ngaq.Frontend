using Microsoft.Extensions.DependencyInjection;
using Ngaq.Ui.Views.User;
using Ngaq.Ui.Views.Word.Query;
using Ngaq.Ui.Views.Word.WordManage.AddWord;

namespace Ngaq.Ui;

public static class DiUi{
	public static IServiceCollection SetupUi(
		this IServiceCollection z
	){
		z.AddTransient<VmAddWord>();
		z.AddTransient<VmWordQuery>();
		z.AddTransient<VmLoginRegister>();
		
		return z;
	}
}
