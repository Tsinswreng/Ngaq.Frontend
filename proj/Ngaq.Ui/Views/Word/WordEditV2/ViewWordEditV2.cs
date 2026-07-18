namespace Ngaq.Ui.Views.Word.WordEditV2;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Views.Word.PoWordEdit;
using Tsinswreng.Avln.Grid;
using Ctx = VmWordEditV2;

/// 單詞編輯主頁：承載三個分頁，並提供全局保存與刪除入口。
public partial class ViewWordEditV2: AppViewBase<Ctx>{
	/// 觸發資料層刪除的按鈕，完成後向宿主發送通知。
	public OpBtn? DeleteBtn{get;set;}
	/// 提交三個分頁草稿的按鈕，完成後向宿主發送通知。
	public OpBtn? SaveBtn{get;set;}
	/// 基本資料分頁的抽象介面，供宿主存取表單資料。
	public ViewPoWordEdit? PoWordEdit{get;set;}
	/// 刪除成功後通知呼叫端關閉或刷新來源頁。
	public event EventHandler? DoneDelete;
	/// 保存成功後通知呼叫端同步來源資料。
	public event EventHandler? DoneSave;
	/// 固定承載分頁、錯誤列與操作列的三列根版面。
	GridStack Root = new(IsRow: true);
	/// 建立頁面、取得 ViewModel，並掛接子頁編輯事件。
	public partial ViewWordEditV2();
	/// 依固定版面順序組裝主畫面。
	partial void Render();
	/// 將列表子頁的單行編輯請求導向對應編輯 View。
	partial void HookSubPageEvents();
	/// 建立基本資料、屬性與學習記錄分頁。
	private partial Control MkTabs();
	/// 建立與 ViewModel 錯誤狀態同步的提示列。
	private partial Control MkErrBar();
	/// 建立刪除及保存操作列。
	private partial Control MkBottomBar();
}
