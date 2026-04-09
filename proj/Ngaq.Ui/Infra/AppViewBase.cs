using Avalonia.Controls;
using Ngaq.Ui.Views;
using Tsinswreng.AvlnTools.Navigation;

namespace Ngaq.Ui.Infra;

public partial class AppViewBase
	:UserControl
	,I_ViewNavi
{
	public IViewNavi? ViewNavi{get;set;} = MgrViewNavi.Inst.ViewNavi;
}
