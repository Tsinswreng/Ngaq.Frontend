using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;

namespace Ngaq.Ui.Views.Word.WordPropPage;

public interface IViewWordPropPage{
	public IBtn BtnAddProp{get;}
	public TreeDataGrid? Rows{get;}
	
}
