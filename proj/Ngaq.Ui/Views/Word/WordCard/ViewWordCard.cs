namespace Ngaq.Ui.Views.Word.WordCard;

using Avalonia.Controls;
using Avalonia.Media;
using Tsinswreng.Avalonia.Sugar;
using Tsinswreng.Avalonia.Tools;
using Ctx = VmWordListCard;
public partial class ViewWordListCard
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordListCard(){
		//Ctx = new Ctx();
		Ctx = Ctx.Samples[0];
		Style();
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();
	public IndexGrid Root{get;set;} = new IndexGrid(IsRow:true);

	protected nil Style(){
		//Styles.Add(SugarStyle.GridShowLines());
		return Nil;
	}

	protected nil Render(){

		var RootGrid = Root.Grid;
		Content = RootGrid;
		RootGrid.RowDefinitions.AddRange([
			new RowDef(4, GUT.Auto),
			new RowDef(8, GUT.Auto),
		]);

		var LangGrid = new IndexGrid(IsRow:true);
		Root.Add(LangGrid.Grid);
		{var o = LangGrid;
			o.Grid.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
			]);
		}
		{{
			var Lang = new TextBlock();
			LangGrid.Add(Lang);
			{var o = Lang;
				o.VerticalAlignment = VertAlign.Center;
				o.Bind(
					TextBlock.TextProperty
					,new CBE(CBE.Pth<Ctx>(x=>x.Lang))
				);
				o.Foreground = Brushes.LightGray;
			}
		}}//~Header


		var HeadBox = new IndexGrid(IsRow:false);
		Root.Add(HeadBox.Grid);
		{
			HeadBox.Grid.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
			]);
		}
		{{
			var Head = new SelectableTextBlock();
			HeadBox.Add(Head);
			{var o = Head;
				o.VerticalAlignment = VertAlign.Center;
				o.Bind(
					TextBlock.TextProperty
					,CBE.Mk<Ctx>(x=>x.Head)
				);
				o.FontSize = UiCfg.Inst.BaseFontSize+8;
			}
		}}

		return Nil;
	}
}
