namespace Xxx;

using Ngaq.Ui.Infra;
using Ctx = VmXxx;
public partial class ViewXxx
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewXxx(){
		Ctx = Ctx.Mk();
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
