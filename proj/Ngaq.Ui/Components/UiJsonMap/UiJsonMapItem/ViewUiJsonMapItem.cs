namespace Ngaq.Ui.Components.KvMap;

using Avalonia.Controls;
using Avalonia.Media;
using Ngaq.Core.Tools.JsonMap;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using ScottPlot.Plottables;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmJsonMapItem;
public partial class ViewJsonMapItem
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewJsonMapItem(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls{

	}


	protected nil Style(){
		return NIL;
	}

	TextBlock Txt(){
		return new();
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
		]);
		Root
		.A(Txt(), o=>{
			o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.DisplayName));
		})
		.A(Txt(), o=>{
			o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.Descr));
			o.FontSize = UiCfg.Inst.BaseFontSize * 0.8;
			o.Foreground = Brushes.LightGray;
			//TODO 處理文字過多
		})
		.A(new TextBox(), o=>{
			o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.RawInput));

		})
		//TODO 增一詳情頁、即把只一項置全屏㕥編輯、中ʸ用大ʹ可換行ʹTextBox作輸入框。


		;
		return NIL;
	}
}

