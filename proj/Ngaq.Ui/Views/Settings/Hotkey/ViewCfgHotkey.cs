namespace Ngaq.Ui.Views.Settings.Hotkey;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;

using Ctx = VmCfgHotkey;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// 快捷鍵配置頁。
public partial class ViewCfgHotkey: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewCfgHotkey(){
		Ctx = App.DiOrMk<Ctx>();
		Render();
		this.Classes.A(App.Cls.ViewPadding);
	}

	public AutoGrid Root = new(IsRow:true);

	/// 渲染配置頁。
	/// <returns>空值。</returns>
	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Auto),
			]);
		});

		Root.A(new ScrollViewer(), sv=>{
			sv.SetContent(new StackPanel(), sp=>{
				sp.Classes.A(App.Cls.SpacedStackPanel);
				sp.A(new TextBlock(), o=>{
					o.Text = I[K.DictionaryLookupHotkeyModifiers_];
				});
				sp.A(new TextBox(), o=>{
					o.CBind<Ctx>(o.PropText, x=>x.DictionaryLookupModifiers);
				});
				sp.A(new TextBlock(), o=>{
					o.Text = I[K.DictionaryLookupHotkeyKey];
				});
				sp.A(new TextBox(), o=>{
					o.CBind<Ctx>(o.PropText, x=>x.DictionaryLookupKey);
				});
			});
		});

		Root.A(new OpBtn(), o=>{
			o._Button.StretchCenter();
			o.VerticalAlignment = VAlign.Bottom;
			o.BtnContent = I[K.Save];
			o._Button.Background = UiCfg.Inst.MainColor;
			o.SetExe(Ct=>Ctx?.Save(Ct));
		});

		return NIL;
	}
}
