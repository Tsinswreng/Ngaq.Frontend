namespace Ngaq.Ui.Views.Word.WordManage.WordSync;

using Avalonia.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordSync;
public partial class ViewWordSync
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordSync(){
		Ctx = Ctx.Mk();
		Style();
		Render();
	}

	public  partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	public AutoGrid Root = new(IsRow:true);
	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{

		});
		return NIL;
	}


}
