using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Styling;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views.BottomBar;
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
		Content = new ViewWordQuery();
	}
}
