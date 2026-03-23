namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
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
		return NIL;
	}
}

/*

StudyPlan:
表頭:
`序號(在UI中顯示的序號、從1開始、不是id)	名稱	`
ScrollViewer{
	StudyPlanRow{}
}

 */
