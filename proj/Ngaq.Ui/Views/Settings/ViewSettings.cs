namespace Ngaq.Ui.Views.Settings;

using Avalonia.Controls;
using Avalonia.Styling;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.About;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Navigation;
using Ctx = VmSettings;
using K = Ngaq.Ui.Infra.I18n.ItemsUiI18n.Settings;
public partial class ViewSettings
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSettings(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();
	public II18n I = I18n.Inst;

	protected nil Style(){
		return NIL;
	}

	protected nil Render(){
		var _Item = FnSettingItem(Ctx?.ViewNavi);
		this.ContentInit(_StackPanel(), S=>{
			S.AddInit(_Item(I[K.About], new ViewAbout()));
			S.AddInit(_Item(I[K.UIConfig], new ViewCfgUi()));
		});
		return NIL;
	}

	public static Func<
		str
		,Control
		,SwipeLongPressBtn
	> FnSettingItem(
		IViewNavi? ViewNavi
	){
		var Fn = (str Title, Control Target)=>{
			var R = new SwipeLongPressBtn();
			var titled = ToolView.WithTitle(Title, Target);
			R.HorizontalContentAlignment = HAlign.Left;
			R.ContentInit(_TextBlock(), o=>{
				o.Text = Title;
				o.FontSize = UiCfg.Inst.BaseFontSize*1.2;
			});
			R.Click += (s,e)=>{
				ViewNavi?.GoTo(titled);
			};
			R.Styles.Add(
				new Style(x=>x.Is<Control>())
				.BgTrnsp()
			);
			return R;
		};
		return Fn;
	}


}
