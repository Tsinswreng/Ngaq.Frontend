namespace Ngaq.Ui.Views.Settings;

using Avalonia.Controls;
using Avalonia.Styling;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.About;
using Ngaq.Ui.Views.Settings.LearnWord;
using Ngaq.Ui.Views.Settings.LlmDictionary;
using Ngaq.Ui.Views.Settings.ServerStorage;
using Ngaq.Ui.Views.Settings.Hotkey;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.CsI18n;
using Ctx = VmSettings;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;
public partial class ViewSettings
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSettings(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}

	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();


	protected nil Style(){
		return NIL;
	}

	protected nil Render(){
		var _Item = FnSettingItem(this.ViewNavi);
		this.SetContent(new StackPanel(), S=>{
			S.A(_Item(I[K.About], new ViewAbout()));
			S.A(_Item(I[K.UIConfig], new ViewCfgUi()));
			S.A(_Item(I[K.LearnWordSettings], new ViewCfgLearnWord()));
			S.A(_Item(Todo.I18n("服務與存儲"), new ViewCfgServerStorage()));
			S.A(_Item(Todo.I18n("LlmDictionary"), new ViewCfgLlmDictionary()));
			S.A(_Item(Todo.I18n("快捷鍵配置"), new ViewCfgHotkey()));
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
			R.SetContent(new TextBlock(), o=>{
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
