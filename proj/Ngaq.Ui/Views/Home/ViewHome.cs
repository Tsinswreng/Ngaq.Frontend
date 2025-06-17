namespace Ngaq.Ui.Views.Home;

using Avalonia.Controls;
using Avalonia.Media;
using Ngaq.Ui.Views.BottomBar;
using Ngaq.Ui.Views.Word.Query;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Tsinswreng.Avalonia.Tools;
using Ctx = VmHome;
public partial class ViewHome
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewHome(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	IndexGrid Root = new(IsRow:true);

	protected nil Render(){
		Content = Root.Grid;
		{var o = Root.Grid;
			o.RowDefinitions.AddRange([
				new RowDef(20, GUT.Star),
				//new RowDef(1, GUT.Star),
			]);
		}

		var ViewBottomBar = new ViewBottomBar();
		Root.Add(ViewBottomBar);
		{var o = ViewBottomBar;
			//o.ItemsControl.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
		}
		{{
			var Learn = new Btn_Control(
				StrBarItem.Inst.BarItem("Learn", "📖")
				,new ViewWordQuery()
			);
			ViewBottomBar.Items.Add(Learn);
			{var o = Learn;
				ViewBottomBar.Cur.Content = Learn.Control;
				o.Button.Background = Brushes.Transparent;
			}


			var Lib = new Btn_Control(
				StrBarItem.Inst.BarItem("Lib", "📚")
				,new ViewAddWord()
			);
			ViewBottomBar.Items.Add(Lib);

			var Me = new Btn_Control(
				StrBarItem.Inst.BarItem("Me", "👤")
				,new Control()
			);
			ViewBottomBar.Items.Add(Me);
		}}

		return NIL;
	}


}
