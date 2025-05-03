namespace Ngaq.Ui.Views.BottomBar;

using Avalonia.Controls;
using Ctx = Vm_BottomBar;
public partial class View_BottomBar
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public View_BottomBar(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return null!;
	}

	protected nil Render(){
		return null!;
	}


}
