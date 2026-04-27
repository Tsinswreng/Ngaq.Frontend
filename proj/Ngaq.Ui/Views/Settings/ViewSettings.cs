namespace Ngaq.Ui.Views.Settings;

using Avalonia.Controls;
using Avalonia.Layout;
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
using Ngaq.Ui.Icons;
using Avalonia.Media;
using Tsinswreng.AvlnTools.Tools;

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

	public partial class Cls{

	}



	protected nil Style(){
		Styles.A(new Style(
			x=>x.Is<TextBox>()
		).Set(
				BackgroundProperty
				,Brushes.Gray
			)
		);
		return NIL;
	}

	protected nil Render(){
		var _Item = FnSettingItem(this.ViewNavi);

		this.SetContent(new StackPanel(), S=>{
			S.A(_Item(I[K.About], new ViewAbout(), Svgs.Info()));
			S.A(_Item(I[K.UIConfig], new ViewCfgUi(), Svgs.SolidWindowAlt()));
			S.A(_Item(I[K.LearnWordSettings], new ViewCfgLearnWord(), Svgs.BookOpenTextFill()));
			S.A(_Item(Todo.I18n("服務與存儲"), new ViewCfgServerStorage(), Svgs.Server()));
			S.A(_Item(Todo.I18n("LlmDictionary"), new ViewCfgLlmDictionary(), Svgs.BookAlphabet()));
			S.A(_Item(Todo.I18n("快捷鍵配置"), new ViewCfgHotkey(), Svgs.KeyboardAltSharp()));
		});
		return NIL;
	}

	public static Func<
		str
		,Control
		,Svg?
		,SwipeLongPressBtn
	> FnSettingItem(
		IViewNavi? ViewNavi
	){
		var Fn = (str Title, Control Target, Svg? Icon)=>{
			var R = new SwipeLongPressBtn();
			var titled = ToolView.WithTitle(Title, Target);
			R.HorizontalContentAlignment = HAlign.Left;
			R.SetContent(new StackPanel(), o=>{
				o.Orientation = Orientation.Horizontal;
				o.Spacing = UiCfg.Inst.BaseFontSize * 0.35;
				if(Icon is not null){
					o.A(Icon.Value.ToIcon(), iconCtrl=>{
						iconCtrl.Width = UiCfg.Inst.BaseFontSize * 1.2;
						iconCtrl.Height = UiCfg.Inst.BaseFontSize * 1.2;
					});
				}
				o.A(new TextBlock(), txt=>{
					txt.Text = Title;
					txt.FontSize = UiCfg.Inst.BaseFontSize*1.2;
				});
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
