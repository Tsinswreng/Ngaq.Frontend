namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanRow;

using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ctx = VmStudyPlanRow;
public partial class ViewStudyPlanRow
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewStudyPlanRow(){
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

