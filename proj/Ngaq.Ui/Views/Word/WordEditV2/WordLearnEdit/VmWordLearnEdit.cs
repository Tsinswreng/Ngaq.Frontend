namespace Ngaq.Ui.Views.Word.WordEditV2.WordLearnEdit;

using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordEditV2.WordLearnPage;

/// 單行學習記錄編輯頁 ViewModel。
public partial class VmWordLearnEdit: ViewModelBase{
	/// 刪除回調由父頁注入。
	public Func<CT, Task<bool>>? OnDelete{get;set;}
	/// 正在編輯的學習記錄行。
	public VmWordLearnRow Row{
		get;
		set{SetProperty(ref field, value);}
	} = VmWordLearnRow.NewRow();
	/// 轉發父頁提供的刪除操作。
	public partial Task<bool> Delete(CT Ct);
}
