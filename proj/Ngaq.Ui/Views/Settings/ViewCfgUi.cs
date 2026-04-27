namespace Ngaq.Ui.Views.Settings;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Settings.Lang;
using Tsinswreng.AvlnTools.Dsl;
using Ctx = Ngaq.Ui.Infra.ViewModelBase;
using Tsinswreng.AvlnTools.Tools;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Icons;

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
			S.A(_Item(I[KeysUiI18nCommon.FontSize], new ViewCfgFont(), Icons.FontOutline()));
			S.A(_Item(Todo.I18n("語言"), new ViewCfgLang(), Icons.EarthAmericasSolid()));
		});
		return NIL;
	}


}
