namespace Ngaq.Ui.Views.WordCard;

using Avalonia.Controls;
using Tsinswreng.Avalonia.Util;
using Ctx = Vm_WordCard;
public partial class View_WordCard
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public View_WordCard(){
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
		return null!;
	}

	protected nil Render(){
		
		var RootGrid = Root.Grid;
		Content = RootGrid;
		RootGrid.RowDefinitions.AddRange([
			new RowDefinition(1, GridUnitType.Star),
			new RowDefinition(8, GridUnitType.Star),
			new RowDefinition(1, GridUnitType.Star)
		]);
		Root.Add();

		var Body = new IndexGrid(IsRow:false);
		Root.Add(Body.Grid);
		{
			Body.Grid.ColumnDefinitions.AddRange([
				new ColumnDefinition(1, GridUnitType.Star),
				new ColumnDefinition(16, GridUnitType.Star),
				new ColumnDefinition(1, GridUnitType.Star),
			]);
		}
		{{
			Body.Add();

			var WordText = new SelectableTextBlock();
			Body.Add(WordText);
			{
				var o = WordText;
				o.Bind(
					TextBlock.TextProperty
					,CBE.Mk<Ctx>(x=>x.WordText)
				);
			}

			Body.Add();
		}}
		Root.Add();

		return null!;
	}
}
