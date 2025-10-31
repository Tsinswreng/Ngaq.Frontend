namespace Ngaq.Ui.Views;

using Avalonia.Controls;
using Avalonia.Input;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Home;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.AvlnTools.Tools;

public class ViewNaviBase:UserControl{

}


public partial class MainView : UserControl {
	public AutoGrid AutoGrid = new (IsRow: true);
	public Grid Root{get{return AutoGrid.Grid;}}
	public ViewNaviBase ViewNaviBase{get;} = new ();
	public MainView() {
		this.ContentInit(AutoGrid.Grid);
		AutoGrid.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
		]);
		AutoGrid.Add(ViewNaviBase);

		InputElement.KeyDownEvent.AddClassHandler<TopLevel>(
			(s,e)=>{
				if(e.Key == Avalonia.Input.Key.Escape){
					MgrViewNavi.Inst.GetViewNavi().Back();
				}
			}
			,handledEventsToo: true
		);

		MgrViewNavi.Inst.ViewNavi = new ViewNavi(ViewNaviBase);
		var Navi = MgrViewNavi.Inst.ViewNavi;

		var Home = new ViewHome();
		//var Home = new ViewEditWord();
		Navi.GoTo(Home);

	}
}
