using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordInfo;
using Tsinswreng.CsCore;

namespace Ngaq.Ui.Views.Word.Learn;

public interface IViewLearnWord:IViewBase{

	[Doc(@$"
	點擊開始按鈕。
	界面應出現單詞卡片，並開始學習流程。
	")]
	public Task<nil> ClickStart(CT Ct);

	[Doc(@$"
	點擊保存按鈕後
	當前一輪的學習記錄會被持久化到數據庫中、
	然後重新計算權重排序 開始新一輪學習
	")]
	public Task<nil> ClickSave(CT Ct);

	[Doc(@$"整個還原到初始狀態。
	清除背景和背單詞狀態、
	{nameof(WordInfo)}也還原到初始狀態
	")]
	public Task<nil> ClickReset(CT Ct);

	[Doc(@$"跳轉到設置頁")]
	public Task<nil> ClickSettings(CT Ct);

	[Doc(@$"點擊一次單詞卡片即標記該單詞潙{nameof(ELearn.Rmb)}、
	在詞基礎上再擊則標記潙{nameof(ELearn.Fgt)}、
	再擊則回到空狀態。
	{nameof(IViewWordListCard.LearnedColor)}隨自狀態而變。
	")]
	public Task<nil> ClickWordCard(u64 Pos, CT Ct);

	[Doc(@$"所有單詞卡。顯示在上方。")]
	public IList<IViewWordListCard>? WordListCards{get;}

	[Doc(@$"當前單詞信息。顯示在下方。
	每{nameof(ClickWordCard)}時、此則被改潙該詞之訊。
	初始狀態下 {nameof(WordInfo.Descrs)}[0] 當潙
	{nameof(KeysUiI18nCommon.PressStartButtonToBeginLearning)};
	
	當用戶點擊{nameof(ClickStart)}之後、此當示潙{nameof(KeysUiI18nCommon.WordLearningHelpText_)}
	")]
	public IViewWordInfo? WordInfo{get;}
}
