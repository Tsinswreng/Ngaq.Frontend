namespace Ngaq.Ui.Views.Home;

using Avalonia.Controls;
using Avalonia.Media;
using Ngaq.Ui.Views.BottomBar;
using Ngaq.Ui.Views.User;
using Ngaq.Ui.Views.User.Register;
using Ngaq.Ui.Views.Word.Query;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.AvlnTools.Dsl;
using Ctx = VmHome;
using Ngaq.Ui.Views.Word.WordManage;
using Tsinswreng.AvlnTools.Navigation;
using Ngaq.Ui.Infra;

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
		Ctx.ViewNavi = MgrViewNavi.ViewNavi;
		Ctx.ViewNavi.Current = this;
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow:true);

	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(20, GUT.Star),
			]);
		});

		Root.AddInit(new ViewBottomBar(), ViewBottomBar=>{
			var ViewWordQuery = new ViewWordQuery();

			ViewBottomBar.Items.AddInit(
				new Btn_Control(
					StrBarItem.Inst.BarItem("Learn", "📖")
					,ViewWordQuery
				),
				o=>{
					ViewBottomBar.Cur.Content = o.Control;
					o.Button.Background = Brushes.Transparent;
				}
			).AddInit(new Btn_Control(
				StrBarItem.Inst.BarItem("Library", "📚")
				,new ViewWordManage()
				//,new ViewAddWord()
			))
			.AddInit(new Btn_Control(
				StrBarItem.Inst.BarItem("Me", "👤")
				,new ViewLoginRegister()
			));
		});

		return NIL;
	}


}
