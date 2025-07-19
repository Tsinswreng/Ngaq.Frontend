using Avalonia.Controls;
using Avalonia.Input;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Home;
using Tsinswreng.AvlnTools.Navigation;

namespace Ngaq.Ui.Views;

public partial class MainView : UserControl {
	public MainView() {
		InputElement.KeyDownEvent.AddClassHandler<TopLevel>(
			(s,e)=>{
				if(e.Key == Avalonia.Input.Key.Escape){
					MgrViewNavi.Inst.GetViewNavi().Back();//t
				}
			}
			,handledEventsToo: true
		);

		MgrViewNavi.Inst.ViewNavi = new ViewNavi(this);
		var Navi = MgrViewNavi.Inst.ViewNavi;

		var Home = new ViewHome();
		Navi.GoTo(Home);
		//Content = Home;
		//Content = new ViewLoginRegister();
		// var Btn = new Button { Content = "Hello World!" };
		// Content = Btn;
		// var Pressed = new Style(x=>
		// 	x.Is<Button>()
		// 	.Class(":pressed")
		// 	.Template()
		// 	.OfType<ContentPresenter>()
		// 	//.Name("PART_ContentPresenter")
		// 	//"PART_ContentPresenter"
		// );
		// //var Pressed = new Style(x => x.OfType<Button>().Class(":pressed").Template().Name("PART_ContentPresenter"));
		// Btn.Styles.Add(Pressed);
		// {var o = Pressed;
		// 	o.Set(
		// 		BackgroundProperty
		// 		,Brushes.Yellow
		// 	);
		// }

	}
}
