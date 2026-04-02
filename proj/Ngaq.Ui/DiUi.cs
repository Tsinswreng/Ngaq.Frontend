using Microsoft.Extensions.DependencyInjection;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Views.Dictionary;
using Ngaq.Ui.Views.Settings.LearnWord;
using Ngaq.Ui.Views.User;
using Ngaq.Ui.Views.User.AboutMe;
using Ngaq.Ui.Views.Word.Learn;
using Ngaq.Ui.Views.Word.Pronunciation;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Ngaq.Ui.Views.Word.WordManage.EditWord;
using Ngaq.Ui.Views.Word.WordManage.SearchWords;
using Ngaq.Ui.Views.Word.WordManage.Statistics;
using Ngaq.Ui.Views.Word.WordManage.WordSync;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;
using Tsinswreng.AvlnTools.Navigation;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterPage;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.PreFilterVisualEdit;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.PreFilterJsonEdit;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanPage;

namespace Ngaq.Ui;

public static class DiUi{
	public static IServiceCollection SetupUi(
		this IServiceCollection z
	){

		//z.AddSingleton<II18n>(I18n.Inst);

		z.AddTransient<VmAddWord>();
		z.AddTransient<VmLearnWords>();
		z.AddTransient<VmLoginRegister>();
		z.AddTransient<VmSearchWords>();
		z.AddTransient<VmEditJsonWord>();
		z.AddTransient<VmAboutMe>();
		z.AddTransient<VmWordSync>();
		z.AddTransient<VmStatistics>();
		z.AddTransient<VmCfgLearnWord>();
		z.AddTransient<VmPronunciation>();
		z.AddTransient<VmDictionary>();
		z.AddTransient<VmPreFilterVisualEdit>();
		z.AddTransient<VmPreFilterJsonEdit>();
		z.AddTransient<VmPreFilterPage>();
		z.AddTransient<VmStudyPlanPage>();
		z.AddSingleton<I_GetViewNavi>(MgrViewNavi.Inst);
		return z;
	}
}
