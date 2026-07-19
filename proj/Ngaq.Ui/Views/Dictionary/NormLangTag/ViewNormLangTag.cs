namespace Ngaq.Ui.Views.Dictionary.NormLangTag;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Ctx = VmNormLangTag;

/// 按內容最小寬度顯示的詞典源語言快捷標籤。
public partial class ViewNormLangTag: AppViewBase<Ctx>{
	/// 建立 Tag 控件並初始化樣式與綁定。
	public partial ViewNormLangTag();

	/// 用戶點擊 Tag 時觸發；由詞典頁決定如何切換源語言。
	public event Action<Ctx>? OnSelected;
	/// 承載短標籤文字與點擊行為的按鈕。
	public Button? TagButton;

	/// 建立按內容自適應寬度的 Tag 按鈕。
	private partial void Render();
	/// 初始化普通狀態與當前源語言狀態的視覺樣式。
	private partial void InitStyle();
	/// 將點擊事件轉發給詞典頁。
	private partial void Select();
}
