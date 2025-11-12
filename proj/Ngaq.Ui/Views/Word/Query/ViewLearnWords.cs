namespace Ngaq.Ui.Views.Word.Query;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Settings.LearnWord;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordInfo;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using static Tsinswreng.AvlnTools.Dsl.DslFactory;
using Ctx = VmLearnWords;
using K = Ngaq.Ui.Infra.I18n.ItemsUiI18n.LearnWord;
public partial class ViewLearnWords
	:UserControl
{

	public II18n I = I18n.Inst;

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewLearnWords(){
		//Ctx = new Ctx();
		//Ctx = Ctx.Samples[0];
		Ctx = App.GetSvc<Ctx>();
		Style();
		Menu = _Menu();
		//Menu.IsVisible = false;
		Render();
		Loaded += (s,e)=>{
			_OnLoad();
		};
	}

	public partial class Cls_{
		public str MenuBtn = nameof(MenuBtn);
	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		Styles.AddInit(new Style(x=>
			x.Is<Control>()
			.Class(Cls.MenuBtn)
		).Set(
			VerticalAlignmentProperty
			, VAlign.Stretch
		));
		return NIL;
	}

	AutoGrid Root = new AutoGrid(IsRow: true);
	Panel Menu;

	protected IBrush Shade(IBrush originalBrush, ContentControl top){
		var overlayBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)); // 半透明黑，alpha可调
		var overlayGrid = new Grid{
			Width = top.Bounds.Width,
			Height = top.Bounds.Height,
			Children ={
				new Border { Background = originalBrush },
				new Border { Background = overlayBrush }
			}
		};
		// 动态监听窗口大小变化，保持同步
		// top.GetObservable(TopLevel.BoundsProperty).Subscribe(bounds => //无法将 lambda 表达式 转换为类型“IObserver<Rect>”，原因是它不是委托类型CS1660
		// {
		// 	originalBrush.Stretch = Stretch.Uniform;
		// 	overlayGrid.Width = bounds.Width;
		// 	overlayGrid.Height = bounds.Height;
		// });

		// 用 VisualBrush 包裹 overlayGrid
		var combinedBrush = new VisualBrush{
			Visual = overlayGrid,
			Stretch = Stretch.Fill
		};
		return combinedBrush;

	}

	protected nil _OnLoad(){
		var top = TopLevel.GetTopLevel(this);
		if(top == null){return NIL;}
		top.Bind(
			BackgroundProperty
			,CBE.Mk<Ctx>(x=>x.BgBrush
				,Source: Ctx
				,Mode: BindingMode.OneWay
				,Converter: new ParamFnConvtr<IBrush, nil>((oldBrush, arg)=>{
					return Shade(oldBrush, top);
				})
			)
		);
		top.GetObservable(TopLevel.BoundsProperty).Subscribe(_ =>{
			// 触发 Background 的刷新或重设，保证ImageBrush用UniformToFill渲染
			if(Ctx==null){return;}
			if(top.Background is TileBrush tb){
				tb.Stretch = Stretch.UniformToFill; //TODO與Ctx中ʹ配置 持一
			}
			//Background = Shade(Ctx.BgBrush, top);
			//top.Background.Stretch = Stretch.Uniform;
		});
		return NIL;
	}

	protected Panel _Menu(){
		var R = new AutoGrid(IsRow: true);
		var Row1 = new AutoGrid(IsRow: false);
		R.AddInit(Row1.Grid, (o)=>{
			o.ColumnDefinitions.AddRange([
				ColDef(100, GUT.Star),
				ColDef(100, GUT.Star),
				ColDef(100, GUT.Star),
				ColDef(100, GUT.Star),
			]);
		});
		{{
			Row1.AddInit(new OpBtn{}, (o)=>{
				o.Classes.Add(Cls.MenuBtn);
				o._Button.ContentInit(_TextBlock(), t=>{
					t.Text = "▶️"+I[K.Start];
				});
				o.SetExt((Ct)=>Ctx?.LoadEtStartAsy(Ct));
			}).AddInit(new OpBtn{}, o=>{ //📁
				o.Classes.Add(Cls.MenuBtn);
				o._Button.ContentInit(_TextBlock(), t=>{
					t.Text = "💾"+I[K.Save];
				});
				o.SetExt((Ct)=>Ctx?.SaveEtRestartAsy(Ct));
			}).AddInit(new OpBtn{}, o=>{
				o.Classes.Add(Cls.MenuBtn);
				o._Button.Content = "🔄"+I[K.Clear];
				o.SetExt((Ct)=>Ctx?.ResetAsy(Ct));
			})
			.AddInit(new OpBtn{}, o=>{
				o.Classes.Add(Cls.MenuBtn);
				o._Button.Content = "🔄"+I[K.Settings];
				o._Button.Click += (s,e)=>{
					Ctx?.ViewNavi?.GoTo(
						ToolView.WithTitle(
							I[K.LearnWordSettings]
							,new ViewCfgLearnWord()
						)
					);
				};
				//o.SetExt((Ct)=>Ctx?.ResetAsy(Ct));
			})

			;
		}}

		return R.Grid;
	}


	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),//overlay
				RowDef(1, GUT.Auto),
				RowDef(4, GUT.Star),
				RowDef(1, GUT.Auto),//GridSplitter
				RowDef(5, GUT.Star),
			]);
		});
		{{
			Root.AddInit(_Border(), o=>{
				o.Background = new SolidColorBrush(Color.FromArgb((byte)(255 * 0.35), 0, 0, 0));
				o.ZIndex = -1;
			})
			.AddInit(_Menu())
			.AddInit(_ScrollViewer(), Scr=>{
				Scr.ContentInit(_ListWordCard(), o=>{
					o.Bind(
						ItemsControl.ItemsSourceProperty
						,CBE.Mk<Ctx>(x=>x.WordCards, Mode: BindingMode.TwoWay)
					);
				});
				Root.AddInit(_GridSplitter(), o=>{
					o.Background = Brushes.Black;
					o.BorderThickness = new Thickness(1);
					o.BorderBrush = Brushes.LightGray;
				}).AddInit(_WordInfo(), o=>{
					o.Bind(o.PropDataContext_()
						,CBE.Mk<Ctx>(x=>x.CurWordInfo, Mode: BindingMode.TwoWay)
					);
				});
			});//~ScrollViewer
		}}
		return NIL;
	}

	Control _RowSearch(){
		var Ans = new AutoGrid(IsRow: false);
		{var o = Ans.Grid;
			o.ColumnDefinitions.AddRange([
				ColDef(1, GUT.Star),
				ColDef(7, GUT.Star),
				ColDef(2, GUT.Star),
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

	//TODO 分頁加載以代虛擬化
	Control _ListWordCard(){
		var Ans = new ItemsControl();
		{var o = Ans;
		}
		var Cnt = 1;
		Ans.ItemsPanel = new FuncTemplate<Panel?>(()=>{
			return new VirtualizingStackPanel();
			//return new StackPanel();
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
				Cnt++;
				var Btn = new SwipeLongPressBtn();
				Grid.Children.Add(Btn);
				{var o = Btn;
					//o.HorizontalContentAlignment = HoriAlign.Left;
					o.HorizontalContentAlignment = HAlign.Stretch;
					o.Styles.Add(new Style().NoMargin().NoPadding());
					o.Background = Brushes.Transparent;
					o.Bind(
						//Button.BackgroundProperty
						BorderBrushProperty
						,CBE.Mk<VmWordListCard>(x=>x.LearnedColor, Mode: BindingMode.OneWay)
					);
					o.BorderThickness = new Thickness(4,0,0,0);
					o.LongPressDurationMs = Ctx?.CfgUi.LongPressDurationMs??o.LongPressDurationMs;
					StyBtnWordCard(o.Styles);
				}

				Btn.ContentInit(new ViewWordListCard(VmWordCard), o=>{
					o.VAlign(VAlign.Stretch).HAlign(HAlign.Stretch);
					o.Background = Brushes.Transparent;
					o.Bind(
						o.PropDataContext_()
						,CBE.Mk<VmWordListCard>(x=>x
							,Mode: BindingMode.OneWay
						)
					);
					Btn.Click += (s,e)=>{
						if(o?.Ctx != null){
							Ctx?.ClickWordCard(o.Ctx);
						}
					};
					//觸屏旹 長按ʹ效ˋ不佳。scrollViewer上下滑動旹亦祘。璫只在靜止旹長按纔生效
					// Btn.OnLongPressed += (s,e)=>{
					// 	if(o?.Ctx != null){
					// 		Ctx?.OnLongPressed(o.Ctx);
					// 	}
					// };
					// Btn.OnSwipe += (s,e)=>{
					// 	if(e.Direction == Tsinswreng.AvlnTools.Controls.IF.ISwipeBtn.SwipeDirection.Right){
					// 		if(o?.Ctx != null){
					// 			Ctx?.OnLongPressed(o.Ctx);
					// 		}
					// 	}
					// };
				});
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
		).Set(
			BorderBrushProperty
			,CBE.Mk<VmWordListCard>(x=>x.LearnedColor)
			//,Brushes.Transparent
		).Attach(s);


		var Pressed = new Style(x=>
			x.Is<Button>()
			.Class(PC.pressed)
			.Template()
			.OfType<ContentPresenter>()
		).Set(
			BorderBrushProperty
			//,Brushes.Yellow
			,CBE.Mk<VmWordListCard>(x=>x.LearnedColor)
		).Attach(s);


		return s;

	}
}


