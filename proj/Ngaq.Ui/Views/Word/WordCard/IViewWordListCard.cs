namespace Ngaq.Ui.Views.Word.WordCard;

/// 單詞卡片頁面契約。
/// 面向用戶可觀察/可操作語義，不暴露具體控件類型。
public interface IViewWordListCard {
	/// 卡片序號文本。
	str IndexText { get; }

	/// 單詞語言文本。
	str LangText { get; }

	/// 單詞詞頭文本。
	str HeadText { get; }

	/// 學習歷史摘要文本。
	str LearnHistoryText { get; }

	/// 最近一次學習時間文本。
	str LastLearnedTimeText { get; }

	/// 權重文本。
	str WeightText { get; }

	/// 打開卡片菜單。
	void OpenMenu();

	/// 觸發編輯動作。
	void ClickEdit();

	/// 觸發朗讀動作。
	void ClickPronounce();
}
