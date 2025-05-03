using Avalonia.Controls;
using Ngaq.Ui.Views.WordCard;
using Ngaq.Ui.Views.WordManage.AddWord;

namespace Ngaq.Ui.Views;

public partial class MainView : UserControl {
	public MainView() {
		//InitializeComponent();
		//Content = new View_WordCard();
		Content = new View_AddWord();
	}
}
