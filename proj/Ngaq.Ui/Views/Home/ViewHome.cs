namespace Ngaq.Ui.Views.Home;

using Avalonia.Controls;
using Avalonia.Media;
using Ngaq.Ui.Views.BottomBar;
using Ngaq.Ui.Views.Word.Query;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.AvlnTools.Dsl;
using Ctx = VmHome;
using Ngaq.Ui.Views.Word.WordManage;
using Ngaq.Ui.Views.User.AboutMe;
using Ngaq.Ui.Infra.I18n;

using K = Infra.I18n.ViewHome;
public partial class ViewHome
	:UserControl
{

	public II18n I = I18n.Inst;

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewHome(){
		Ctx = new Ctx();
		Style();
		Render();
		//Ctx.ViewNavi = MgrViewNavi.ViewNavi;
	}

	public  partial class Cls_{

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

			ViewBottomBar.Items.AddInitT(
				new Btn_Control(
					StrBarItem.Inst.BarItem(I[K.Learn], "📖")
					,ViewWordQuery
				),
				o=>{
					ViewBottomBar.Cur.Content = o.Control;
					o.Button.Background = Brushes.Transparent;
				}
			).AddInitT(new Btn_Control(
				StrBarItem.Inst.BarItem(I[K.Library], "📚")
				,new ViewWordManage()
			))
			.AddInitT(new Btn_Control(
				StrBarItem.Inst.BarItem(I[K.Me], "👤")
				,new ViewAboutMe()
			));
		});

		return NIL;
	}


}
