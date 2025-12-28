namespace Ngaq.Ui.Views.Word.Pronunciation_;

using Avalonia.Controls;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmPronunciation;
public partial class ViewPronunciation
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewPronunciation(){
		//Ctx = App.DiOrMk<Ctx>();
		Ctx = Ctx.Samples[0];
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls{

	}


	protected nil Style(){
		return NIL;
	}

	SelectableTextBlock Txt(){
		return new SelectableTextBlock();
	}
	AutoGrid Root = new(IsRow: false);
	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
		]);
		Root.AddInit(new OpBtn(), o=>{
			o.BtnContent = Svgs.VolHigh.ToIcon();
			o.SetExt(Ct=>Ctx?.Play(Ct));
		})
		.AddInit(Txt(), o=>{
			o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.Text));
		})
		;
		return NIL;
	}
}

