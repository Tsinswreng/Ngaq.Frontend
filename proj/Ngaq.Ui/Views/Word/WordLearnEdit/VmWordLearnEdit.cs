namespace Ngaq.Ui.Views.Word.WordLearnEdit;

using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordLearnPage;

/// 單行學習記錄編輯頁 ViewModel。
public partial class VmWordLearnEdit: ViewModelBase{
	public Action<VmWordLearnRow>? OnRemove{get;set;}

	public VmWordLearnRow Row{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = VmWordLearnRow.NewRow();
}
