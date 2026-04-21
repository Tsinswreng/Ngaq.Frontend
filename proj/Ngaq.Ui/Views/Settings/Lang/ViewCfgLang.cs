namespace Ngaq.Ui.Views.Settings.Lang;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;

using Ctx = VmCfgLang;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

public partial class ViewCfgLang: AppViewBase{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewCfgLang(){
		Ctx = App.DiOrMk<Ctx>();
		Render();
	}

	public AutoGrid Root = new(IsRow:true);

	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Auto),
			]);
		});
		Root.A(new StackPanel(), sp=>{
			sp.A(new TextBlock(), o=>{
				o.Text = Todo.I18n("Lang");
			});
			sp.A(new TextBox(), o=>{
				o.CBind<Ctx>(o.PropText, x=>x.Lang);
			});
		});
		Root.A(new OpBtn(), o=>{
			o._Button.StretchCenter();
			o.VerticalAlignment = VAlign.Bottom;
			o.BtnContent = I[K.Save];
			o.Background = UiCfg.Inst.MainColor;
			o.SetExe(Ct=>Ctx?.Save(Ct));
		});
		return NIL;
	}
}

