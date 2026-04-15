namespace Xxx;

using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ctx = VmXxx;
public partial class ViewXxx
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewXxx(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}
	public partial class Cls{

	}


	protected nil Style(){
		return NIL;
	}


	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
		]);
		return NIL;
	}
}

