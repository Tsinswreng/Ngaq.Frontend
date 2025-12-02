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

using K = Ngaq.Ui.Infra.I18n.ItemsUiI18n.Home;
using Ngaq.Ui.Icons;
using Avalonia;

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

		var BarItem = (str Title, Control Icon)=>{
			var R = StrBarItem.Inst.BarItem(Title, Icon);
			//不效
			// if (R.Content is Grid grid && grid.Children.Count >= 2 && grid.Children[1] is Control ctrl){
			// 	ctrl.Margin = new Thickness(0, 0, 0, 10); // 👈 關鍵：往下留少許空間，視覺上往上移
			// }
			return R;
		};

		Root.AddInit(new ViewBottomBar(), ViewBottomBar=>{
			var ViewWordQuery = new ViewLearnWords();

			ViewBottomBar.Items.AddInitT(
				new Btn_Control(
					BarItem(I[K.Learn], Svgs.BookOpenTextFill.ToIcon())//📖
					,ViewWordQuery
				),
				o=>{
					ViewBottomBar.Cur.Content = o.Control;
					o.Button.Background = Brushes.Transparent;
				}
			).AddInitT(new Btn_Control(
				BarItem(I[K.Library], Svgs.BookBookmarkFill.ToIcon())//📚
				,new ViewWordManage()
			))
			.AddInitT(new Btn_Control(
				BarItem(I[K.Me], Svgs.UserCircleFill.ToIcon())//👤
				,new ViewAboutMe()
			));
		});

		return NIL;
	}


}
