namespace Ngaq.Ui.Views.WordCard;

using Avalonia.Controls;
using Tsinswreng.Avalonia.Util;
using Ctx = Vm_WordCard;
public partial class View_WordCard
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public View_WordCard(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();
	public IndexGrid

	protected nil Style(){
		return null!;
	}

	protected nil Render(){
		return null!;
	}


}
