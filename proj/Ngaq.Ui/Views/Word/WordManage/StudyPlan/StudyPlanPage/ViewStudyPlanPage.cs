namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanPage;

using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmStudyPlanPage;
public partial class ViewStudyPlanPage
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewStudyPlanPage(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls{

	}


	protected nil Style(){
		return NIL;
	}


	AutoGrid Root = new(IsRow: true);
	public nil Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
		]);
		Root.A(new OpBtn(), o=>{
			o.BtnContent = Todo.I18n("RestoreStudyPlan");
			o.SetExe(Ct=>Ctx?.RestoreStudyPlan(Ct));
		});
		return NIL;
	}
}

