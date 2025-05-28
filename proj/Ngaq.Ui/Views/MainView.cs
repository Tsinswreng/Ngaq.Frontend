using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Ngaq.Ui.Views.BottomBar;
using Ngaq.Ui.Views.User.Login;
using Ngaq.Ui.Views.User.Register;
using Ngaq.Ui.Views.WordCard;
using Ngaq.Ui.Views.WordManage.AddWord;
using Tsinswreng.Avalonia.Controls;

namespace Ngaq.Ui.Views;

public partial class MainView : UserControl {
	public MainView() {
		//InitializeComponent();
		//Content = new ViewWordListCard();
		//Content = new ViewAddWord();
		//Content = new ViewBottomBar();
		// var o = new ConfirmBox();
		// Content = o;
		// o._LeftBtn.Content = "Left";
		// o._RightBtn.Content = "Right";
		//Content = new ViewLogin();
		Content = new ViewRegister();

	}
}
