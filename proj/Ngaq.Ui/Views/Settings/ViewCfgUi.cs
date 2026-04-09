namespace Ngaq.Ui.Views.Settings;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Ctx = Ngaq.Ui.Infra.ViewModelBase;

public partial class ViewCfgUi
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewCfgUi(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public  partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	protected nil Render(){
		var _Item = ViewSettings.FnSettingItem(ViewNavi);
		this.SetContent(new StackPanel(), S=>{
			S.A(_Item(Todo.I18n("Font Size"), new ViewCfgFont()));
		});
		return NIL;
	}


}
