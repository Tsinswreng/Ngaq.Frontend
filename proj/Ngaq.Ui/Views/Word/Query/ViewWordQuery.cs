namespace Ngaq.Ui.Views.Word.Query;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordInfo;
using Tsinswreng.Avalonia.Controls;
using Tsinswreng.Avalonia.Sugar;
using Tsinswreng.Avalonia.Tools;
using Tsinswreng.CsCore.Tools;
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
		Loaded += (s,e)=>{
			var top = TopLevel.GetTopLevel(this);
			var originalBrush = new ImageBrush(
				new Bitmap(@"e:\_\mmf\壁紙頭像等\壁紙\magazine-unlock-hi2964701.jpg")
			){
				Stretch = Stretch.Uniform//改用綁定 否則窗口大小ˋ變旹則比例不對。
			};

    var overlayBrush = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)); // 半透明黑，alpha可调

 var overlayGrid = new Grid
    {
        Width = top.Bounds.Width,
        Height = top.Bounds.Height,
        Children =
        {
            new Border { Background = originalBrush },
            new Border { Background = overlayBrush }
        }
    };

    // 动态监听窗口大小变化，保持同步
    // top.GetObservable(TopLevel.BoundsProperty).Subscribe(bounds => //无法将 lambda 表达式 转换为类型“IObserver<Rect>”，原因是它不是委托类型CS1660
    // {
    //     overlayGrid.Width = bounds.Width;
    //     overlayGrid.Height = bounds.Height;
    // });

    // 用 VisualBrush 包裹 overlayGrid
    var combinedBrush = new VisualBrush
    {
        Visual = overlayGrid,
        Stretch = Stretch.Fill
    };

    top.Background = combinedBrush;

			// top!.Bind(
			// 	BackgroundProperty
			// 	,CBE.Mk<Ctx>(x=>x.BgBrush
			// 		,Source: Ctx
			// 		,Mode: BindingMode.OneWay
			// 	)
			// );
		};
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
				new RowDef(1, GUT.Auto),//overlay
				new RowDef(1, GUT.Auto),
				//new RowDef(1, GUT.Auto),//可隱藏ʹ菜單
				new RowDef(4, GUT.Star),
				new RowDef(1, GUT.Auto),//GridSplitter
				new RowDef(5, GUT.Star),
			]);
		}
		{{
			var overlay = new Border();
			Root.Add(overlay);
			{var o = overlay;
				o.Background = new SolidColorBrush(Color.FromArgb((byte)(255 * 0.35), 0, 0, 0));
				//o.Bind(BoundsProperty, )
				// o.Width = UiCfg.Inst.WindowWidth;
				// o.Height = UiCfg.Inst.WindowHeight;
				o.ZIndex = -1;
			}

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
					o.Background = Brushes.Transparent;
					o.Bind(
						//Button.BackgroundProperty
						BorderBrushProperty
						,CBE.Mk<VmWordListCard>(x=>x.LearnedColor, Mode: BindingMode.OneWay)
					);
					o.BorderThickness = new Thickness(8,0,0,0);
					o.LongPressDurationMs = Ctx?.Cfg.LongPressDurationMs??o.LongPressDurationMs;
					StyBtnWordCard(o.Styles);
				}


				var Card = new ViewWordListCard{};
				Btn.Content = Card;
				{var o = Card;
					o.VerticalAlignment = VertAlign.Stretch;
					o.HorizontalAlignment = HoriAlign.Stretch;
					o.Background = Brushes.Transparent;
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
				BorderBrushProperty
				,CBE.Mk<VmWordListCard>(x=>x.LearnedColor)
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
				BorderBrushProperty
				//,Brushes.Yellow
				,CBE.Mk<VmWordListCard>(x=>x.LearnedColor)
			);
		}

		return s;

	}

	Styles _StyBtnWordCard(Styles s){
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
				,CBE.Mk<VmWordListCard>(x=>x.LearnedColor)
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
				,CBE.Mk<VmWordListCard>(x=>x.LearnedColor)
			);
		}

		return s;

	}
}


