namespace Ngaq.Ui.Views.Dictionary.SimpleWord;

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Views.Word.Pronunciation_;
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

	AutoGrid Root = new (IsRow: true);
	SelectableTextBlock Txt(){
		return new SelectableTextBlock();
	}
	protected nil Render(){
		Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
		]);
		Root
		.AddInit(Txt(), o=>{
			o.FontSize = UiCfg.Inst.BaseFontSize*1.5;
			o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.Head));
		})
		.AddInit(PronunciationList(), o=>{

		})
		.AddInit(Txt(), o=>{
			o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.Description));
		})
		;
		return NIL;
	}


	Control PronunciationList(){
		var R = new ItemsControl();
		R.Bind(
			R.PropItemsSource, CBE.Mk<Ctx>(x=>x.Pronunciations)
		);


		R.SetItemTemplate<Pronunciation>((p,b)=>{
			var R = new ViewPronunciation();
			R.Ctx!.FromPronunciation(p);
			return R;
		});
		R.SetItemsPanel(()=>{
			return new StackPanel{Orientation = Orientation.Horizontal};
		});

		return R;
	}


}
