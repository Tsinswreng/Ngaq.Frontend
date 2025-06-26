using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views.BottomBar;
using Ngaq.Ui.Views.Home;
using Ngaq.Ui.Views.User.Login;
using Ngaq.Ui.Views.User.Register;
using Ngaq.Ui.Views.Word.Query;
using Ngaq.Ui.Views.Word.WordCard;
using Tsinswreng.Avalonia.Controls;
using Tsinswreng.Avalonia.Converters;
using Tsinswreng.Avalonia.Tools;

namespace Ngaq.Ui.Views;

public partial class MainView : UserControl {
	public MainView() {
		//Content = new ViewWordInfo();
		//Content = new ViewWordListCard();
		//Content = new ViewWordQuery();
		//Content = new ViewBottomBar();
		// Content = new ViewRegister();
		Content = new ViewHome();
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
