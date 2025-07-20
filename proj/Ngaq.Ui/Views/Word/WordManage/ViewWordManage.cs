using Ngaq.Ui.Infra;

namespace Ngaq.Ui.Views.Word.WordManage;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = ViewModelBase;
public partial class ViewWordManage
	:UserControl
{


	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordManage(){
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

	public AutoGrid Root = new(IsRow:true);

	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(9999, GUT.Star),
				RowDef(1, GUT.Star),
			]);
		});
		Root.AddInit(_StackPanel(), stk=>{
			stk.AddInit(_Item("Add Words", new ViewAddWord()));
			stk.AddInit(_Item("Backup & Sync", new ViewAddWord()));
		});


		return NIL;
	}

	protected Control _Item(str Title, ContentControl Target){
		var R = new SwipeLongPressBtn();
		var titled = new ViewTitle();
		titled.Body.Content = Target;
		//titled.Title.Content = new TextBlock(){Text = Title};
		titled.Title.ContentInit(_TextBlock(), o=>{
			o.Text = Title;
			o.VerticalAlignment = VertAlign.Center;
			o.HorizontalAlignment = HoriAlign.Center;
			o.FontSize = UiCfg.Inst.BaseFontSize*1.2;
		});
		titled.BdrTitle.Background = new SolidColorBrush(Color.FromRgb(32, 32, 32));
		// titled.BdrTitle.BorderThickness = new Thickness(0,0,0,1);
		// titled.BdrTitle.BorderBrush = Brushes.Aqua;
		R.Click += (s,e)=>{
			Ctx?.ViewNavi?.GoTo(titled);
		};
		R.HorizontalContentAlignment = HoriAlign.Left;
		R.ContentInit(_TextBlock(), o=>{
			o.Text = Title;
		});
		return R;
	}


}
