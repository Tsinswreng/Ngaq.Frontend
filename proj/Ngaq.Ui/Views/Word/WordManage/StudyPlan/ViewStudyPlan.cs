namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using Avalonia.Controls;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterPage;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.SetCurStudyPlan;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanPage;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.WeightArgPage;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.WeightCalculatorPage;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ctx = VmStudyPlan;
public partial class ViewStudyPlan
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewStudyPlan(){
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
		Root.A(_S());
		return NIL;
	}

	StackPanel _S(){
		var o = new StackPanel();
		o
		.A(
			MainView.Inst.MkBtnToView(()=>new ViewSetCurStudyPlan(),Todo.I18n("設置當前學習方案"))
		)
		.A(
			MainView.Inst.MkBtnToView(()=>new ViewStudyPlanPage(),Todo.I18n("PoStudyPlan"))
		)
		.A(
			MainView.Inst.MkBtnToView(()=>new ViewWeightCalculatorPage(),Todo.I18n("PoWeightCalculator"))
		)
		.A(
			MainView.Inst.MkBtnToView(()=>new ViewPreFilterPage(),Todo.I18n("PreFilter"))
		)
		.A(
			MainView.Inst.MkBtnToView(()=>new ViewWeightArgPage(),Todo.I18n("Weight Arg"))
		)
		;
		return o;
	}


}
