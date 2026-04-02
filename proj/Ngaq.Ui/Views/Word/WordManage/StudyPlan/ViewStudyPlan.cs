namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using Avalonia.Controls;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterPage;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.WeightArgPage;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
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
	public II18n I = I18n.Inst;
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
		o.A(
			MainView.Inst.MkBtnToView(()=>new ViewPreFilterPage(),Todo.I18n("PreFilter"))
		).A(MainView.Inst.MkBtnToView(()=>new ViewWeightArgPage(),Todo.I18n("Weight Arg")))
		;
		return o;
	}


}

