namespace Ngaq.Ui.Views.Word.Learn;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Settings.LearnWord;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordInfo;
using Tsinswreng.Avln.StrokeText;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using static Tsinswreng.AvlnTools.Dsl.DslFactory;
using Ctx = VmLearnWords;
using K = Infra.I18n.ItemsUiI18n.LearnWord;
public partial class ViewLearnWords
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewLearnWords(){
		Ctx = App.GetRSvc<Ctx>();
		if(Ctx is not null){
			Ctx.OnAutoPronounceResult += OnAutoPronounceResult;
		}
		Style();
		Menu = _Menu();
		//Menu.IsVisible = false;
		Render();
		Loaded += (s,e)=>{
			_OnLoad();
		};
	}

	void OnAutoPronounceResult(DtoWordCardPronounceResult Result){
		ViewWordListCard.HandlePronounceResult(Ctx, Result);
	}

	public partial class Cls_{
		public str MenuBtn = nameof(MenuBtn);
	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		Styles.A(new Style(x=>
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
		// top.GetObservable(TopLevel.BoundsProperty).Subscribe(bounds => //无法?lambda 表达?转换为类型“IObserver<Rect>”，原因是它不是委托类型CS1660
		// {
		// 	originalBrush.Stretch = Stretch.Uniform;
		// 	overlayGrid.Width = bounds.Width;
		// 	overlayGrid.Height = bounds.Height;
		// });

		// ?VisualBrush 包裹 overlayGrid
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
				tb.Stretch = Stretch.UniformToFill; //TODO與Ctx中ʹ配?持一
			}
			//Background = Shade(Ctx.BgBrush, top);
			//top.Background.Stretch = Stretch.Uniform;
		});
		return NIL;
	}

	StrokeTextBlock _Txt(){
		var R = new StrokeTextBlock();
		R.Foreground = Brushes.White;
		return R;
	}

	protected Panel _Menu(){
		var R = new AutoGrid(IsRow: true);
		var Row1 = new AutoGrid(IsRow: false);
		R.A(Row1.Grid, (o)=>{
			o.ColumnDefinitions.AddRange([
				ColDef(100, GUT.Star),
				ColDef(100, GUT.Star),
				ColDef(100, GUT.Star),
				ColDef(100, GUT.Star),
			]);
		});
		{{
			var T = (str t)=>{
				var R = new StrokeTextBlock{
					Text = " "+t,
					FontSize = UiCfg.Inst.BaseFontSize*0.8,
					StrokeThickness = 2.0,

					//Foreground = Brushes.White,
				};
				return R;
			};
			var Ic = (Svg s)=>{
				var R = s.ToIcon();
				R.Stroke = Brushes.Black;
				R.StrokeThickness = 0.9;
				return R;
			};
			var Hc = (params Control[] Ctrls)=>{
				return HoriCloseCtrls.Mk(Ctrls);
			};
			var Btn = ()=>{
				var R = new OpBtn{};
				R.Classes.Add(Cls.MenuBtn);
				R._Button.VerticalAlignment = VAlign.Stretch;
				R._Button.VerticalContentAlignment = VAlign.Stretch;
				R.Padding = R._Button.Margin = new Thickness(0);
				return R;
			};
			Row1.A(Btn(), (o)=>{
				o._Button.SetContent(Hc(
					Ic(Svgs.PlayCircleFill())//▶️
					,T(I[K.Start])
				));
				//o._Button.ContentInit(_Txt(), t=>{t.Text = "▶️"+I[K.Start];});
				o.SetExe((Ct)=>Ctx?.LoadEtStartAsy(Ct));
			}).A(Btn(), o=>{ //📁"💾"
				o._Button.SetContent(Hc(
					Ic(Svgs.FloppyDiskBackFill())
					,T(I[K.Save])
				));

				o.SetExe((Ct)=>Ctx?.SaveEtRestart(Ct));
 				o._Button.Styles.Add(
					new Style(
						//x=>x.OfType<Button>().Template().OfType<ContentPresenter>() //改僞類樣式旹纔需?
						x=>x.Is<Button>()
					).Set(
						BackgroundProperty
						,CBE.Mk<Ctx>(x=>x.IsSaved
							,Converter: new SimpleFnConvtr<bool, IBrush?>((b)=>{
								if(!b){
									return UiCfg.Inst.MainColor;
								}
								return o._Button.Background;
							})
						)
					)
				);
			}).A(Btn(), o=>{
				o._Button.SetContent(Hc(//🔄
					Ic(Svgs.RotateCw())
					,T(I[K.Reset])
				));
				o.SetExe((Ct)=>Ctx?.ResetAsy(Ct));
			})
			.A(Btn(), o=>{
				o._Button.SetContent(Hc(//?
					Ic(Svgs.GearFill())
					,T(I[K.Settings])
				));
				o._Button.Click += (s,e)=>{
					ViewNavi?.GoTo(
						ToolView.WithTitle(
							I[K.LearnWordSettings]
							,new ViewCfgLearnWord()
						)
					);
				};
			})
			;
		}}

		return R.Grid;
	}


	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),//overlay
				RowDef(1, GUT.Auto),
				RowDef(4, GUT.Star),
				RowDef(1, GUT.Auto),//GridSplitter
				RowDef(5, GUT.Star),
			]);
		});
		{{
			Root
			.A(new Border(), o=>{
				//背景圖遮?
				o.Background = new SolidColorBrush(Color.FromArgb((byte)(255 * 0.35), 0, 0, 0));
				o.ZIndex = -1;
			})
			.A(_Menu())
			.A(new ScrollViewer(), Scr=>{
				Scr.SetContent(_ListWordCard(), o=>{
					o.CBind<Ctx>(
						ItemsControl.ItemsSourceProperty
						,x=>x.WordCards, Mode: BindingMode.TwoWay);
				});
			})//~ScrollViewer
			.A(new GridSplitter(), o=>{
				o.GrayBarWith3Dots();
			})
			.A(_WordInfo(), o=>{
				o.CBind<Ctx>(o.PropDataContext
					,x=>x.CurWordInfo, Mode: BindingMode.TwoWay);
			});
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

	//TODO 分頁加載以代虛擬?
	Control _ListWordCard(){
		var Ans = new ItemsControl();
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
					o.CBind<VmWordListCard>(
						//Button.BackgroundProperty
						BorderBrushProperty
						,x=>x.LearnedColor, Mode: BindingMode.OneWay);
					o.BorderThickness = new Thickness(4,0,0,0);
					o.LongPressDurationMs = Ctx?.CfgUi.LongPressDurationMs??o.LongPressDurationMs;
					o.ContextMenu = ViewWordListCard.MkWordCardCtxMenu(Ctx, VmWordCard?.WordForLearn?.JnWord);
					o.OnLongPressed += (s,e)=>{
						o.ContextMenu.Open();
					};
					o.LongPressDurationMs = 500;
					StyBtnWordCard(o.Styles);
				}

				Btn.SetContent(new ViewWordListCard(VmWordCard), o=>{
					o.VAlign(VAlign.Stretch).HAlign(HAlign.Stretch);
					o.Background = Brushes.Transparent;
					o.CBind<VmWordListCard>(
						o.PropDataContext_()
						,x=>x
							,Mode: BindingMode.OneWay
						);
					Btn.Click += (s,e)=>{
						if(o?.Ctx != null){
							Ctx?.ClickWordCard(o.Ctx);
						}
					};
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
		).AddTo(s);


		var Pressed = new Style(x=>
			x.Is<Button>()
			.Class(PC.pressed)
			.Template()
			.OfType<ContentPresenter>()
		).Set(
			BorderBrushProperty
			//,Brushes.Yellow
			,CBE.Mk<VmWordListCard>(x=>x.LearnedColor)
		).AddTo(s);


		return s;

	}
}


