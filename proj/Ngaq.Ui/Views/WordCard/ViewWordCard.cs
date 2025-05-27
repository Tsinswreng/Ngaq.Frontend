namespace Ngaq.Ui.Views.WordCard;

using Avalonia.Controls;
using Tsinswreng.Avalonia.Sugar;
using Tsinswreng.Avalonia.Tools;
using Ctx = VmWordCard;
public partial class ViewWordCard
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordCard(){
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
		Styles.Add(SugarStyle.GridShowLines());
		return Nil;
	}

	protected nil Render(){

		var RootGrid = Root.Grid;
		Content = RootGrid;
		RootGrid.RowDefinitions.AddRange([
			new RowDef(4, GUT.Star),
			new RowDef(8, GUT.Star),
		]);

		var LangGrid = new IndexGrid(IsRow:true);
		System.Console.WriteLine(Root.Index);//t -> 0
		Root.Add(LangGrid.Grid); // -> 1
		System.Console.WriteLine(Root.Index);//t -> 0
		{var o = LangGrid;
			o.Grid.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
			]);
		}
		{{
			var Lang = new TextBlock();
			LangGrid.Add(Lang);
			{var o = Lang;
				o.Bind(
					TextBlock.TextProperty
					,new CBE(CBE.Pth<Ctx>(x=>x.Lang))
				);
			}
		}}//~Header


		var HeadBox = new IndexGrid(IsRow:false);
		System.Console.WriteLine(Root.Index);//t
		Root.Add(HeadBox.Grid);
		System.Console.WriteLine(Root.Index);//t
		//Grid.SetRow(HeadBox.Grid, 1);//t
		{
			HeadBox.Grid.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
				new ColDef(1, GUT.Star),
			]);
		}
		{{
			var Head = new SelectableTextBlock();
			HeadBox.Add(Head);
			{var o = Head;
				o.Bind(
					TextBlock.TextProperty
					,CBE.Mk<Ctx>(x=>x.Head)
				);
			}

			HeadBox.Add(new Button{Content="123"});
		}}

		return Nil;
	}
}
