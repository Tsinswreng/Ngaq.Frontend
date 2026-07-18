namespace Ngaq.Ui.Views.Word.WordEditV2.WordPropEdit;

using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordEditV2.WordPropPage;

/// 單行屬性編輯頁 ViewModel。
public partial class VmWordPropEdit: ViewModelBase{
	/// 刪除回調由父頁注入。
	public Func<CT, Task<bool>>? OnDelete{get;set;}
	/// 正在編輯的屬性行。
	public VmWordPropRow Row{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = VmWordPropRow.NewRow();
	/// 轉發父頁提供的刪除操作。
	public partial Task<bool> Delete(CT Ct);
}
