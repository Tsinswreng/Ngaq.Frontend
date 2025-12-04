namespace Ngaq.Ui.Tools;

using Avalonia.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

public class ToolIcon{
	public static Panel IconWithTitle(
		Control Icon, str Title
	){
		var R = new AutoGrid(IsRow: false);
		R.ColDefs.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(UiCfg.Inst.BaseFontSize, GUT.Pixel),
			ColDef(1, GUT.Auto),
		]);
		R.AddInit(Icon);
		R.Add();
		R.AddInit(new TextBlock(), t=>{t.Text = Title;});
		return R.Grid;
	}
}
