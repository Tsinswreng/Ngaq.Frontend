using Avalonia.Controls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Views;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.CsI18n;
namespace Ngaq.Ui.Infra;

public partial class AppViewBase
	:UserControl
	,I_ViewNavi
{
	public IViewNavi? ViewNavi{get;set;} = MgrViewNavi.Inst.ViewNavi;
	public II18n I = AppI18n.Inst;
}


public partial class AppViewBase<TCtx>
	:UserControl
	,I_ViewNavi
	where TCtx:class
{
	public IViewNavi? ViewNavi{get;set;} = MgrViewNavi.Inst.ViewNavi;
	public II18n I = AppI18n.Inst;
	public TCtx? Ctx{
		get{return DataContext as TCtx;}
		set{DataContext = value;}
	}
}


