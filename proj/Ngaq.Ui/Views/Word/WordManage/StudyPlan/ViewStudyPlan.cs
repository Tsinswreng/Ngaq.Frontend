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
using Ctx = VmStudyPlan;using K = Ngaq.Ui.Infra.I18n.KeysUiI18n.StudyPlan;
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
			MainView.Inst.MkBtnToView(()=>new ViewSetCurStudyPlan(),I[K.SetCurrentStudyPlan])
		)
		.A(
			MainView.Inst.MkBtnToView(()=>new ViewStudyPlanPage(),I[K.PoStudyPlan])
		)
		.A(
			MainView.Inst.MkBtnToView(()=>new ViewWeightCalculatorPage(),I[K.PoWeightCalculator])
		)
		.A(
			MainView.Inst.MkBtnToView(()=>new ViewPreFilterPage(),I[K.PreFilter])
		)
		.A(
			MainView.Inst.MkBtnToView(()=>new ViewWeightArgPage(),I[K.WeightArgWithSpace])
		)
		;
		return o;
	}


}

