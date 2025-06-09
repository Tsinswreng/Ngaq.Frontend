namespace Ngaq.Ui.Views.Word.Query;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
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
		//Ctx = Ctx.Samples[0];
		Ctx = App.GetSvc<Ctx>();
		Style();
		Menu = _Menu();
		//Menu.IsVisible = false;
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		//Styles.Add(SugarStyle.GridShowLines());
		return NIL;
	}

	IndexGrid Root = new IndexGrid(IsRow: true);
	Panel Menu;

	protected Panel _Menu(){
		var R = new IndexGrid(IsRow: true);
		var Row1 = new IndexGrid(IsRow: false);
		R.Add(Row1.Grid);
		{var o = Row1.Grid;
			o.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
				new ColDef(1, GUT.Star),
				new ColDef(1, GUT.Star),
				new ColDef(1, GUT.Star),
				new ColDef(1, GUT.Star),
			]);
		}
		{{
			var LoadWord = new SwipeLongPressBtn{Content = "Start"};
			Row1.Add(LoadWord);
			{var o = LoadWord;
				o.Click += (s,e)=>{
					Ctx?.LoadEtStart();
				};
			}

			var Save = new SwipeLongPressBtn{Content = "Save"};
			Row1.Add(Save);
			{var o = Save;
				o.Click += (s,e)=>{
					Ctx?.SaveEtRestart();
				};
			}

			var Reload = new SwipeLongPressBtn{Content = "Reset"};
			Row1.Add(Reload);
			{
				var o = Reload;
				o.Click += (s,e)=>{
					Ctx?.Reset();
				};
			}


		}}

		return R.Grid;
	}


	protected nil Render(){
		Content = Root.Grid;
		{var o = Root.Grid;
			o.RowDefinitions.AddRange([
				new RowDef(1, GUT.Auto),
				//new RowDef(1, GUT.Auto),//可隱藏ʹ菜單
				new RowDef(4, GUT.Star),
				new RowDef(1, GUT.Auto),//GridSplitter
				new RowDef(5, GUT.Star),
			]);
		}
		{{
			var Menu = _Menu();
			Root.Add(Menu);


			// var Canv = new Canvas();
			// Root.Add(Canv);
			// {var o = Canv;
			// 	o.Background = Brushes.Black;
			// 	o.ZIndex = 1;
			// }
			// Canv.Children.Add(this.Menu);
			// {var o = this.Menu;
			// 	o.Background = Brushes.Black;
			// }

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

		return NIL;
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
			var MenuBtn = new SwipeLongPressBtn{Content = "···"};
			Ans.Add(MenuBtn);
			{var o = MenuBtn;
				o.Click += (s,e)=>{
					Menu.IsVisible = !Menu.IsVisible;
				};
			}

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
		Ans.ItemsPanel = new FuncTemplate<Panel?>(()=>{
			return new VirtualizingStackPanel();
		});
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
				// var Index = new TextBlock();
				// Grid.Children.Add(Index);
				// {var o = Index;
				// 	o.Text = Cnt+""; //TODO 虛擬化時此不準
				// 	o.HorizontalAlignment = HoriAlign.Right;
				// 	o.ZIndex = 999;
				// }
				Cnt++;

				var Btn = new SwipeLongPressBtn();
				Grid.Children.Add(Btn);
				{var o = Btn;
					//o.HorizontalContentAlignment = HoriAlign.Left;
					o.HorizontalContentAlignment = HoriAlign.Stretch;
					o.Styles.Add(new Style().NoMargin().NoPadding());
					o.Bind(
						Button.BackgroundProperty
						,CBE.Mk<VmWordListCard>(x=>x.BgColor, Mode: BindingMode.OneWay)
					);
					o.LongPressDurationMs = Ctx?.Cfg.LongPressDurationMs??o.LongPressDurationMs;
					StyBtnWordCard(o.Styles);
				}


				var Card = new ViewWordListCard{};
				Btn.Content = Card;
				{var o = Card;
					o.VerticalAlignment = VertAlign.Stretch;
					o.HorizontalAlignment = HoriAlign.Stretch;
					o.Bind(
						Control.DataContextProperty
						,CBE.Mk<VmWordListCard>(x=>x
							,Mode: BindingMode.OneWay
						)
					);
					Btn.Click += (s,e)=>{
						if(o?.Ctx != null){
							Ctx?.ClickVmWordCard(o.Ctx);
						}
					};
					Btn.OnLongPressed += (s,e)=>{
						if(o?.Ctx != null){
							Ctx?.OnLongPressed(o.Ctx);
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

	Styles StyBtnWordCard(Styles s){
		var PC = PsdCls.Inst;
		var Hover = new Style(x=>
			x.Is<Button>()
			.Class(PC.pointerover)
			.Template()
			.OfType<ContentPresenter>()
		);
		s.Add(Hover);
		{var o = Hover;
			o.Set(
				BackgroundProperty
				,CBE.Mk<VmWordListCard>(x=>x.BgColor)
				//,Brushes.Transparent
			);
		}
		var Pressed = new Style(x=>
			x.Is<Button>()
			.Class(PC.pressed)
			.Template()
			.OfType<ContentPresenter>()
		);
		s.Add(Pressed);
		{var o = Pressed;
			o.Set(
				BackgroundProperty
				//,Brushes.Yellow
				,CBE.Mk<VmWordListCard>(x=>x.BgColor)
			);
		}

		return s;

	}
}


