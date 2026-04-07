using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsCore;

namespace Ngaq.Ui.Views;

public partial class MainView : UserControl {
	public static MainView Inst{get;protected set;} =new();

	[Doc(@$"造按鈕、點後跳到目標視圖")]
	public partial Button MkBtnToView(
		Control Target
		,str? Title = null
	);

	[Doc(@$"造按鈕、點後跳到目標視圖（延遲建構）")]
	public partial Button MkBtnToView(
		Func<Control> MkTarget
		,str? Title = null
	);
	public II18n I18n{get;set;} = Ngaq.Ui.Infra.I18n.I18n.Inst;
	public SvcPopup SvcPopup{get;set;}
	public AutoGrid AutoGrid = new (IsRow: true);
	public Grid Root{get{return AutoGrid.Grid;}}
	public ViewNaviBase ViewNaviBase{get;} = new ();
	public ILogger? Logger{get=>App.Logger;set{}}
	[Doc(@$"可關閉彈窗")]
	public partial nil ShowMsg(str Msg);

	[Doc(@$"可關閉彈窗、執行操作。
	#Params([],[縱向生成按鈕列])
	")]
	public partial nil ShowMsg(str Msg, IList<Button> Operations);

	[Doc(@$"前端拿到異常後處理之")]
	public partial nil HandleErr(obj? Ex);


}
