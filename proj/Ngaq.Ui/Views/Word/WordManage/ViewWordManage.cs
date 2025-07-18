namespace Ngaq.Ui.Views.Word.WordManage;

using Avalonia.Controls;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = Ngaq.Ui.ViewModels.ViewModelBase;
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

		Root.AddInit(_Item("Add Words", new ViewAddWord()));
		return NIL;
	}

	protected Control _Item(str Title, ContentControl Target){
		var R = new SwipeLongPressBtn();
		R.Click += (s,e)=>{
			Ctx?.ViewNavi?.GoTo(Target);
		};
		R.HorizontalContentAlignment = HoriAlign.Left;
		R.ContentInit(_TextBlock(), o=>{
			o.Text = Title;
		});
		return R;
	}


}
