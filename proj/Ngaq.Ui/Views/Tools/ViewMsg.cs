namespace Ngaq.Ui.Views.Tools;

using Avalonia.Controls;
using Ctx = VmXxx;
public partial class ViewXxx
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewXxx(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	protected nil Render(){
		return NIL;
	}


}
