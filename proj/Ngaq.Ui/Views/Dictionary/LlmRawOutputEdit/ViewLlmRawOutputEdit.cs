namespace Ngaq.Ui.Views.Dictionary.LlmRawOutputEdit;

using Avalonia.Controls;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

using Ctx = VmLlmRawOutputEdit;

/// LLM 原始輸出查看/編輯頁。
public partial class ViewLlmRawOutputEdit: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewLlmRawOutputEdit(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}

	public II18n I = I18n.Inst;
	public partial class Cls{}

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);

	protected nil Render(){
		this.Content = Root.Grid;
		Root.RowDefs.AddRange([
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Auto),
		]);
		Root.A(new TextBox(), o=>{
			o.CBind<Ctx>(o.PropText, x=>x.RawOutput);
			o.AcceptsReturn = true;
			o.TextWrapping = TextWrapping.Wrap;
			o.MinHeight = 200;
		})
		.A(new OpBtn(), o=>{
			o.Background = UiCfg.Inst.MainColor;
			o._Button.HorizontalAlignment = HAlign.Stretch;
			o._Button.VerticalAlignment = VAlign.Stretch;
			o._Button.HorizontalContentAlignment = HAlign.Center;
			o._Button.VerticalContentAlignment = VAlign.Center;
			o._Button.FontWeight = FontWeight.Bold;
			o.BtnContent = Todo.I18n("確認更改");
			o.SetExe(Ct=>Ctx?.Confirm(Ct));
		});
		return NIL;
	}
}
