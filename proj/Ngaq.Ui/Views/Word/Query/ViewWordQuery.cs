namespace Ngaq.Ui.Views.Word.Query;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordInfo;
using Tsinswreng.Avalonia.Controls;
using Tsinswreng.Avalonia.Sugar;
using Tsinswreng.Avalonia.Tools;
using Ctx = VmWordQuery;
public partial class ViewWordQuery
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordQuery(){
		//Ctx = new Ctx();
		Ctx = Ctx.Samples[0];
		Style();
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		//Styles.Add(SugarStyle.GridShowLines());
		return Nil;
	}

	IndexGrid Root = new IndexGrid(IsRow: true);


	protected nil Render(){
		Content = Root.Grid;
		{var o = Root.Grid;
			o.RowDefinitions.AddRange([
				new RowDef(1, GUT.Auto),
				new RowDef(4, GUT.Star),
				new RowDef(1, GUT.Auto),//GridSplitter
				new RowDef(5, GUT.Star),
			]);
		}
		{{
			var RowSearch = _RowSearch();
			Root.Add(RowSearch);

			var Scr = new ScrollViewer();
			Root.Add(Scr);

			var ListWordCard = _ListWordCard();
			Scr.Content = ListWordCard;
			{var o = ListWordCard;
				o.Bind(
					ItemsControl.ItemsSourceProperty
					,CBE.Mk<Ctx>(x=>x.WordCards, Mode: BindingMode.TwoWay)
				);
			}

			var Spl = new GridSplitter();
			Root.Add(Spl);
			{var o = Spl;
				o.Background = Brushes.Black;
				o.BorderThickness = new Thickness(1);
				o.BorderBrush = Brushes.LightGray;
			}

			var WordInfo = _WordInfo();
			Root.Add(WordInfo);
			{var o = WordInfo;
				o.Bind(Control.DataContextProperty
					,CBE.Mk<Ctx>(x=>x.CurWordInfo, Mode: BindingMode.TwoWay)
				);
			}
		}}

		return Nil;
	}

	Control _RowSearch(){
		var Ans = new IndexGrid(IsRow: false);
		{var o = Ans.Grid;
			o.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
				new ColDef(7, GUT.Star),
				new ColDef(2, GUT.Star),
			]);
		}
		{{
			Ans.Add();

			var SearchBox = new AutoCompleteBox();
			{var o = SearchBox;Ans.Add(o);

			}

			var SearchButton = new SwipeLongPressBtn();
			{var o = SearchButton;Ans.Add(o);
				o.Content = "Search";
			}
		}}
		return Ans.Grid;
	}

	Control _ListWordCard(){
		var Ans = new ItemsControl();
		{var o = Ans;
		}
		var Cnt = 1;
		Ans.ItemTemplate = new FuncDataTemplate<VmWordListCard>((VmWordCard,b)=>{
			var Grid = new Grid();
			{var o = Grid;
				o.RowDefinitions.AddRange([
					new RowDef(1,GUT.Auto)
				]);
			}
			{{
				if(Cnt > Ctx?.WordCards.Count){
					Cnt = 1;
				}
				var Index = new TextBlock();
				Grid.Children.Add(Index);
				{var o = Index;
					o.Text = Cnt+"";
					o.HorizontalAlignment = HoriAlign.Right;
				}
				Cnt++;

				var Btn = new SwipeLongPressBtn();
				{var o = Btn;
					o.HorizontalContentAlignment = HoriAlign.Left;
					o.Styles.Add(new Style().NoMargin().NoPadding());
				}
				Grid.Children.Add(Btn);

				var Card = new ViewWordListCard{};
				Btn.Content = Card;
				{var o = Card;
					o.Bind(
						Control.DataContextProperty
						,CBE.Mk<VmWordListCard>(x=>x
							,Mode: BindingMode.OneWay
						)
					);
					Btn.Click += (s,e)=>{
						if(o?.Ctx?.BoWord != null){
							Ctx?.SetCurBoWord(o.Ctx.BoWord);
						}
					};
				}
			}}//~Grid

			return Grid;
		});
		return Ans;
	}

	Control _WordInfo(){
		var R = new ViewWordInfo();
		return R;
	}
}
