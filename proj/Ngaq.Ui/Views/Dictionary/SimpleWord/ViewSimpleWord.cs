namespace Ngaq.Ui.Views.Dictionary.SimpleWord;

using Avalonia.Controls;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmSimpleWord;
public partial class ViewSimpleWord
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSimpleWord(){
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

	AutoGrid Root = new (IsRow: true);
	SelectableTextBlock Txt(){
		return new SelectableTextBlock();
	}
	protected nil Render(){
		Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
		]);
		Root
		.AddInit(Txt(), o=>{
			o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.Head));
		})
		.AddInit(Txt(), o=>{
			o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.Pronunciation));
		})
		.AddInit(Txt(), o=>{
			o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.Description));
		})
		;
		return NIL;
	}


}
