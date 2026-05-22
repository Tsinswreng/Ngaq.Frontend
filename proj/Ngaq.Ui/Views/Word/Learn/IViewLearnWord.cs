using Tsinswreng.CsCore;

namespace Ngaq.Ui.Views.Word.Learn;

public interface IViewLearnWord{

	[Doc(@$"
	點擊開始按鈕。
	界面應出現單詞卡片，並開始學習流程。
	")]
	public Task<nil> ClickStart(CT Ct);
	public Task<nil> ClickSave(CT Ct);
	public Task<nil> ClickReset(CT Ct);
	public Task<nil> ClickSettings(CT Ct);

	public Task<nil> ClickWordCard(u64 Pos, CT Ct);
}
