namespace Ngaq.Ui.Views.Dictionary;

using System.Linq;
using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Tsinswreng.AvlnTools.Navigation;

/// <summary>
/// 熱鍵用字典導航器：
/// 1) 重用同一個字典頁實例；
/// 2) 避免在導航棧中重複壓入同一頁。
/// </summary>
public class SvcDictionaryHotkeyNavigator : ISvcDictionaryHotkeyNavigator{
	readonly ViewDictionary DictionaryView = new();
	readonly Control DictionaryPage;

	public SvcDictionaryHotkeyNavigator(){
		DictionaryPage = ToolView.WithTitle("Dictionary", DictionaryView);
	}

	/// <summary>
	/// 打開字典頁，並可選擇直接查詢詞條。
	/// </summary>
	/// <param name="Term">待查詞條。</param>
	/// <returns>空值。</returns>
	public nil OpenDictionary(str? Term = null){
		var navi = MgrViewNavi.Inst.GetViewNavi();
		if(navi is ViewNavi concrete){
			if(object.ReferenceEquals(concrete.Peek, DictionaryPage)){
				LookupIfNeeded(Term);
				return NIL;
			}

			// 若字典頁已在棧中，直接回退到該頁，避免重複壓棧。
			if(concrete.Stack.Any(x=>object.ReferenceEquals(x, DictionaryPage))){
				while(!object.ReferenceEquals(concrete.Peek, DictionaryPage) && concrete.Back()){
				}
				LookupIfNeeded(Term);
				return NIL;
			}
		}

		navi.GoTo(DictionaryPage);
		LookupIfNeeded(Term);
		return NIL;
	}

	/// <summary>
	/// 當輸入有效時觸發字典查詢。
	/// </summary>
	/// <param name="Term">詞條。</param>
	/// <returns>空值。</returns>
	nil LookupIfNeeded(str? Term){
		if(str.IsNullOrWhiteSpace(Term)){
			return NIL;
		}
		DictionaryView.ClickLookupBtn(Term);
		return NIL;
	}
}
