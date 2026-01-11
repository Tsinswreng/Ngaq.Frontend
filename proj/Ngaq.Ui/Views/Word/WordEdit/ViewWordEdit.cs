namespace Ngaq.Ui.Views.Word.WordEdit;

using Ngaq.Core.Tools.JsonMap;
using Ngaq.Ui;
using Ngaq.Ui.Components.KvMap.JsonMap;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordEdit;
public partial class ViewWordEdit
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordEdit(){
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
			RowDef(1, GUT.Star),
		]);
		Root.AddInit(
			new ViewUiJsonMap()
		);
		return NIL;
	}


}
