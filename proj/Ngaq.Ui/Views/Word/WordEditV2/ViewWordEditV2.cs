namespace Ngaq.Ui.Views.Word.WordEditV2;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Views.Word.PoWordEdit;
using Tsinswreng.Avln.Grid;
using Ctx = VmWordEditV2;

/// 單詞編輯主頁：承載三個分頁，並提供全局保存與刪除入口。
public partial class ViewWordEditV2: AppViewBase<Ctx>{
	public OpBtn? DeleteBtn{get;set;}
	public OpBtn? SaveBtn{get;set;}
	public IViewPoWordEdit? PoWordEdit{get;set;}
	public event EventHandler? DoneDelete;
	public event EventHandler? DoneSave;
	GridStack Root = new(IsRow: true);
	public partial ViewWordEditV2();
	partial void Render();
	partial void HookSubPageEvents();
	private partial Control MkTabs();
	private partial Control MkErrBar();
	private partial Control MkBottomBar();
}
