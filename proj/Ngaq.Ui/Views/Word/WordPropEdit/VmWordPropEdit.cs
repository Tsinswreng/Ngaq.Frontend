namespace Ngaq.Ui.Views.Word.WordPropEdit;

using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordPropPage;

/// 單行屬性編輯頁 ViewModel。
public partial class VmWordPropEdit: ViewModelBase{
	public Action<VmWordPropRow>? OnRemove{get;set;}

	public VmWordPropRow Row{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = VmWordPropRow.NewRow();
}
