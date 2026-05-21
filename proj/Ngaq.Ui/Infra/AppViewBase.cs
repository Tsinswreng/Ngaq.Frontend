using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Views;
using Tsinswreng.Avln.Navi;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.CsI18n;
namespace Ngaq.Ui.Infra;

public partial class AppViewBase
	:UserControl
	,IViewNaviHolder
{
	public IViewNavi? ViewNavi{get;set;} = MgrViewNavi.Inst.ViewNavi;
	public II18n I = AppI18n.Inst;
}


public partial class AppViewBase<TCtx>
	:UserControl
	,IView<TCtx>
	,IViewNaviHolder
	where TCtx:class
{
	public IViewNavi? ViewNavi{get;set;} = MgrViewNavi.Inst.ViewNavi;
	public II18n I = AppI18n.Inst;
	public TCtx? Ctx{
		get{return DataContext as TCtx;}
		set{DataContext = value;}
	}

}


