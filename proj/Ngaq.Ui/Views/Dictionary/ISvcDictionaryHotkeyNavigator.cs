namespace Ngaq.Ui.Views.Dictionary;

/// <summary>
/// 字典頁導航服務（供熱鍵與其他入口復用）。
/// </summary>
public interface ISvcDictionaryHotkeyNavigator{
	/// <summary>
	/// 打開字典頁。若提供詞條，則自動觸發查詢。
	/// </summary>
	/// <param name="Term">待查詞條；為空時僅打開頁面。</param>
	/// <returns>空值。</returns>
	public nil OpenDictionary(str? Term = null);
}
