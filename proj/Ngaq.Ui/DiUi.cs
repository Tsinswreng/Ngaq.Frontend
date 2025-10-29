using Microsoft.Extensions.DependencyInjection;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Views.User;
using Ngaq.Ui.Views.User.AboutMe;
using Ngaq.Ui.Views.Word.Query;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Ngaq.Ui.Views.Word.WordManage.EditWord;
using Ngaq.Ui.Views.Word.WordManage.SearchWords;
using Ngaq.Ui.Views.Word.WordManage.WordSync;
using Tsinswreng.AvlnTools.Navigation;

namespace Ngaq.Ui;

public static class DiUi{
	public static IServiceCollection SetupUi(
		this IServiceCollection z
	){

		//z.AddSingleton<II18n>(I18n.Inst);

		z.AddTransient<VmAddWord>();
		z.AddTransient<VmWordQuery>();
		z.AddTransient<VmLoginRegister>();
		z.AddTransient<VmSearchWords>();
		z.AddTransient<VmEditWord>();
		z.AddTransient<VmAboutMe>();
		z.AddTransient<VmWordSync>();
		z.AddSingleton<I_GetViewNavi>(MgrViewNavi.Inst);
		return z;
	}
}
