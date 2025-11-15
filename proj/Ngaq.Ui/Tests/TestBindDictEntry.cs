using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.BottomBar;
using Ngaq.Ui.Views.User.Login;
using Ngaq.Ui.Views.User.Register;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Converters;
using Tsinswreng.AvlnTools.Tools;

namespace Ngaq.Ui.Tests;

public partial class TestMainView : UserControl {
	public TestMainView() {
		//InitializeComponent();
		//Content = new ViewWordListCard();
		//Content = new ViewAddWord();
		//Content = new ViewBottomBar();
		// var o = new ConfirmBox();
		// Content = o;
		// o._LeftBtn.Content = "Left";
		// o._RightBtn.Content = "Right";
		//Content = new ViewLogin();
		//Content = new ViewRegister();
		//Content = new ViewWordInfo();
		Content = new ViewTestBindDict();

	}
}



class ViewTestBindDict : UserControl {
	public VmTestBindDict? Ctx {
		get { return DataContext as VmTestBindDict; }
		set { DataContext = value; }
	}


	public ViewTestBindDict() {
		Ctx = new VmTestBindDict();
		var Root = new AutoGrid(IsRow: true);
		Content = Root.Grid;
		{
			var o = Root.Grid;
			o.RowDefinitions.AddRange([
				new RowDef(1,GUT.Auto),
				new RowDef(1,GUT.Auto),
			]);
		}
		{
			{
				var input = new TextBox();
				Root.Add(input);
				{
					var o = input;
					o.Bind(
						o.PropText
						, new CBE(CBE.Pth<VmTestBindDict>(x => x.Dict)) {
							Mode = BindingMode.TwoWay
							,Converter = new ConvDictEntry<str, str>()
							,ConverterParameter = new Dict_Key<str,str>(
								Dict: Ctx.Dict, Key: "Summary"
							)
						}
					);
				}

				var Btn = new Button();
				Root.Add(Btn);
				{
					var o = Btn;
					o.Content = "Click";
					o.Click += (s, e) => {
						System.Console.WriteLine(
							Ctx?.Dict?["Summary"]
						);
					};
				}
			}
		}
	}
}

class VmTestBindDict : ViewModelBase {

	public IDictionary<str, str> Dict { get; set; } = new Dictionary<str, str>() {
		["Summary"] = "Hello"
	};
}
