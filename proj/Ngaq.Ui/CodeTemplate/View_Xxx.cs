namespace Xxx;

using Avalonia.Controls;
using Ctx = Vm_Xxx;
public partial class Template
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public Template(){
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
