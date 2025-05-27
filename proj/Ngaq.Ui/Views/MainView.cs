using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Ngaq.Ui.Views.BottomBar;
using Ngaq.Ui.Views.WordCard;
using Ngaq.Ui.Views.WordManage.AddWord;
using Tsinswreng.Avalonia.Controls;

namespace Ngaq.Ui.Views;

public partial class MainView : UserControl {
	Button Btn = new();
	public MainView() {
		//InitializeComponent();
		Content = new ViewWordCard();
		//Content = new ViewAddWord();
		//Content = new ViewBottomBar();
		// var o = new ConfirmBox();
		// Content = o;
		// o._LeftBtn.Content = "Left";
		// o._RightBtn.Content = "Right";

	}
}
