using Avalonia.Controls;
using Ngaq.Ui.Views.WordCard;

namespace Ngaq.Ui.Views;

public partial class MainView : UserControl {
	public MainView() {
		//InitializeComponent();
		Content = new View_WordCard();
	}
}
