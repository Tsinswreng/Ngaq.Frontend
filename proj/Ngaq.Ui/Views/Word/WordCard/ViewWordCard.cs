namespace Ngaq.Ui.Views.Word.WordCard;

using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Ui.Converters;
using Ngaq.Ui.StrokeText;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordListCard;
public partial class ViewWordListCard
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordListCard(Ctx Ctx){
		this.Ctx = Ctx;
		Style();
		Render();
	}

	public ViewWordListCard(){
		Ctx = new Ctx();
		//Ctx = Ctx.Samples[0];
		Style();
		Render();
	}

	protected StrokeTextEdit TxtBox(){
		var R = new StrokeTextEdit{
			Fill = Brushes.White,
			Stroke = Brushes.Black,
			StrokeThickness = 5
		};
		R.VerticalAlignment = VAlign.Center;
		R.Styles.Add(new Style().NoMargin().NoPadding());
		return R;
	}

	public partial class Cls_{
		public str InInfoGrid = nameof(InInfoGrid);
	}
	public Cls_ Cls{get;set;} = new Cls_();
	public AutoGrid Root{get;set;} = new AutoGrid(IsRow:true);

	protected nil Style(){
		//Styles.Add(SugarStyle.GridShowLines());

		Styles.AddInit(
			new Style(x=>
				x.Is<TextBlock>()
			)
			,o=>{o.Set(
				EffectProperty
				,new DropShadowDirectionEffect{
					Color = Colors.Black
					,BlurRadius = 4
					,ShadowDepth = 4
					//,Direction = 315 //高度 315是左上角
					,Direction = 330
					,Opacity = 0.5
				});
			}
		);

		// var InfoGridColor = new Style(x=>
		// 	x.Is<TextBlock>()
		// 	.Class(Cls.InInfoGrid)
		// );
		// Styles.Add(InfoGridColor);
		// {var o = InfoGridColor;
		// 	o.Bind(
		// 		TextBlock.ForegroundProperty
		// 		,CBE.Mk<Ctx>(x=>x.LearnedColor,
		// 			Mode: BindingMode.OneWay
		// 			,Source: Ctx
		// 		)
		// 	);
		// }
		return NIL;
	}

	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				new RowDef(4, GUT.Auto),
				new RowDef(8, GUT.Auto),
			]);
		});

		var LangGrid = new AutoGrid(IsRow:false);
		Root.AddInit(LangGrid.Grid, o=>{
			o.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
				new ColDef(0.3, GUT.Star),
				new ColDef(4, GUT.Star),
				new ColDef(13, GUT.Star),
			]);
		});
		{{
			LangGrid
			.AddInit(TxtBox(), o=>{
				o.FontSize = UiCfg.Inst.BaseFontSize*0.8;
				o.Bind(
					o.PropText_()
					,CBE.Mk<Ctx>(x=>x.Index)
				);
			})
			.AddInit(TxtBox(), o=>{
				o.Text = "　";
			})
			.AddInit(TxtBox(), o=>{
				o.VerticalAlignment = VAlign.Center;
				o.Bind(
					o.PropText_()
					,new CBE(CBE.Pth<Ctx>(x=>x.Lang))
				);
				o.Foreground = Brushes.LightGray;
			})
			.AddInit(_InfoGrid());
		}}//~Header


		var HeadBox = new AutoGrid(IsRow:false);
		Root.AddInit(HeadBox.Grid, o=>{
			o.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
			]);
		});

		{{
			HeadBox.AddInit(TxtBox(), o=>{
				o.VerticalAlignment = VAlign.Center;
				o.FontSize = UiCfg.Inst.BaseFontSize+8;
				o.Bind(
					o.PropText_()
					,CBE.Mk<Ctx>(x=>x.Head)
				);
				o.Bind(
					o.PropForeground_()
					,CBE.Mk<Ctx>(x=>x.FontColor)
				);
			});
		}}
		return NIL;
	}

	Control _InfoGrid(){
		var R = new AutoGrid(IsRow:false);
		{var o = R.Grid;
			o.Classes.Add(Cls.InInfoGrid);
			o.ColumnDefinitions.AddRange([
				new ColDef(9, GUT.Star),
				new ColDef(3, GUT.Star),//last review time
				new ColDef(1, GUT.Star),//tab
				new ColDef(7, GUT.Star),//weight
			]);
		}
		{{
			// var LastLearn = new TextBlock();
			// R.Add(LastLearn);
			// {var o = LastLearn;
			// 	o.Bind(
			// 		o.PropText_()
			// 		//LearnRecord不應潙空集合、緣添旹必得'add'
			// 		,CBE.Mk<Ctx>(x=>x.SavedLearnRecords
			// 			,Converter: new ParamFnConvtr<IList<ILearnRecord>, str>((x, p)=>{
			// 				if(x.Count > 0){
			// 					return Ctx.LearnToSymbol(x[^1].Learn);
			// 				}
			// 				return "";
			// 				// var Word = (Ctx)p!;
			// 				// throw new FatalLogicErr("No learn record found. Word:"+Word.Lang+":"+Word.Head);
			// 			})
			// 			,ConverterParameter: Ctx
			// 		)
			// 	);
			// }

			var RecordType = (ELearn Learn)=>{
				var R = new TextBlock{};
				R.Bind(
					TextBlock.TextProperty
					,CBE.Mk<Ctx>(x=>x.Learn_Records
						,Converter: new ConvMultiDictValueCnt<ELearn, ILearnRecord>()
						,ConverterParameter: Learn
					)
				);
				return R;
			};
			var Colon = ()=>new TextBlock(){Text = ":"};

			R.AddInit(TxtBox(), o=>{
				//o.Bind(o.PropText_(), CBE.Mk<Ctx>(x=>x.ToLearnHistoryRepr()));
				//o.Text = Ctx?.ToLearnHistoryRepr();
				o.Bind(
					o.PropText_()
					,CBE.Mk<Ctx>(
						x=>x
						,Converter: new SimpleFnConvtr<Ctx?, str>((x)=>x?.ToLearnHistoryRepr()??"")
					)
				);
			});

			var LastReviewTime = TxtBox();
			R.Add(LastReviewTime);
			{var o = LastReviewTime;
				o.Bind(
					o.PropText_()
					,CBE.Mk<Ctx>(x=>x.LastLearnedTime
						,Converter: new SimpleFnConvtr<i64, str>(x=>{
							var Now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
							var Diff = Now - x;
							return Ctx.FormatUnixMsDiff(Diff);
						})
					)
				);
			}
			R.Add(new TextBlock{Text = "\t"});
			var Weight = TxtBox();
			R.Add(Weight);
			{var o = Weight;
				o.Bind(
					o.PropText_()
					,CBE.Mk<Ctx>(x=>x.Weight
						,Converter: new ParamFnConvtr<f64?,str>((x,p)=>
							Ctx.FmtNum(x??0, 1)
						)
						//,ConverterParameter: "Debug"//t
					)

				);
			}
		}}

		return R.Grid;
	}
}
