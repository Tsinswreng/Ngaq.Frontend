namespace Ngaq.Ui.Views.Settings.LlmDictionary;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;

using Ctx = VmCfgLlmDictionary;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

public partial class ViewCfgLlmDictionary: AppViewBase{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewCfgLlmDictionary(){
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
		Root.A(new ScrollViewer(), sv=>{
			sv.SetContent(new StackPanel(), sp=>{
				sp.Spacing = UiCfg.Inst.BaseFontSize/2;
				sp.A(new TextBlock(), o=>{o.Text = I[K.ApiUrl];});
				sp.A(new TextBox(), o=>{o.CBind<Ctx>(o.PropText, x=>x.ApiUrl);});
				sp.A(new TextBlock(), o=>{o.Text = I[K.ApiKey];});
				sp.A(new TextBox(), o=>{o.CBind<Ctx>(o.PropText, x=>x.ApiKey);});
				sp.A(new TextBlock(), o=>{o.Text = I[K.Model];});
				sp.A(new TextBox(), o=>{o.CBind<Ctx>(o.PropText, x=>x.Model);});
				sp.A(new TextBlock(), o=>{o.Text = I[K.Prompt];});
				sp.A(new TextBox(), o=>{
					o.AcceptsReturn = true;
					o.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
					o.MinHeight = UiCfg.Inst.BaseFontSize*6;
					o.CBind<Ctx>(o.PropText, x=>x.Prompt);
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

