namespace Ngaq.Ui.Views.Word.WordManage.SearchWords.SearchedWordCard;

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Core.Word.Models.Learn_;
using Ngaq.Ui.Converters;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmSearchedWordCard;
public partial class ViewSearchedWordCard
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSearchedWordCard(){
		Ctx = new Ctx();
		//Ctx = Ctx.Samples[0];
		Style();
		Render();
	}

	public  partial class Cls_{
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

		var RootGrid = Root.Grid;
		Content = RootGrid;
		RootGrid.RowDefinitions.AddRange([
			new RowDef(4, GUT.Auto),
			new RowDef(8, GUT.Auto),
		]);

		var LangGrid = new AutoGrid(IsRow:false);
		Root.Add(LangGrid.Grid);
		{var o = LangGrid;
			o.Grid.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
				new ColDef(2, GUT.Star),
			]);
		}
		{{
			var Lang = new TextBlock();
			LangGrid.Add(Lang);
			{var o = Lang;
				o.VerticalAlignment = VAlign.Center;
				o.Bind(
					TextBlock.TextProperty
					,new CBE(CBE.Pth<Ctx>(x=>x.Lang))
				);
				o.Foreground = Brushes.LightGray;
			}

			var InfoGrid = _InfoGrid();
			LangGrid.Add(InfoGrid);
			{var o = InfoGrid;
				//o.HorizontalAlignment = HoriAlign.Right;
			}
		}}//~Header


		var HeadBox = new AutoGrid(IsRow:false);
		Root.Add(HeadBox.Grid);
		{
			HeadBox.Grid.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
			]);
		}
		{{
			var Head = new TextBlock();
			HeadBox.Add(Head);
			{var o = Head;
				o.VerticalAlignment = VAlign.Center;
				o.FontSize = UiCfg.Inst.BaseFontSize+8;
				o.Bind(
					TextBlock.TextProperty
					,CBE.Mk<Ctx>(x=>x.Head)
				);
				o.Bind(
					TextBlock.ForegroundProperty
					,CBE.Mk<Ctx>(x=>x.FontColor)
				);

			}
		}}

		return NIL;
	}

	Control _InfoGrid(){
		var R = new AutoGrid(IsRow:false);
		{var o = R.Grid;
			o.Classes.Add(Cls.InInfoGrid);
			o.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Auto),//上次學習記錄
				new ColDef(1, GUT.Auto),//add
				new ColDef(1, GUT.Auto),//:
				new ColDef(1, GUT.Auto),//rmb
				new ColDef(1, GUT.Auto),//:
				new ColDef(1, GUT.Auto),//fgt
				new ColDef(1, GUT.Auto),//tab
				new ColDef(1, GUT.Auto),//last review time
				new ColDef(1, GUT.Auto),//tab
				new ColDef(1, GUT.Auto),//weight
			]);
		}
		{{
			var LastLearn = new TextBlock();
			R.Add(LastLearn);
			{var o = LastLearn;
				o.Bind(
					TextBlock.TextProperty
					//LearnRecord不應潙空集合、緣添旹必得'add'
					,CBE.Mk<Ctx>(x=>x.SavedLearnRecords
						,Converter: new ParamFnConvtr<IList<ILearnRecord>, str>((x, p)=>{
							if(x.Count > 0){
								return Ctx.LearnToSymbol(x[^1].Learn);
							}
							return "";
							// var Word = (Ctx)p!;
							// throw new FatalLogicErr("No learn record found. Word:"+Word.Lang+":"+Word.Head);
						})
						,ConverterParameter: Ctx
					)
				);
			}

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

			var Add = RecordType(ELearn.Add);
			R.Add(Add);

			R.Add(Colon());
			var Rmb = RecordType(ELearn.Rmb);
			R.Add(Rmb);
			R.Add(Colon());
			var Fgt = RecordType(ELearn.Fgt);
			R.Add(Fgt);

			R.Add(new TextBlock{Text = "\t"});

			var LastReviewTime = new TextBlock();
			R.Add(LastReviewTime);
			{var o = LastReviewTime;
				o.Bind(
					TextBlock.TextProperty
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
			var Weight = new TextBlock();
			R.Add(Weight);
			{var o = Weight;
				o.Bind(
					TextBlock.TextProperty
					,CBE.Mk<Ctx>(x=>x.Weight
						,Converter: new ParamFnConvtr<f64?,str>((x,p)=>
							Ctx.FmtNum(x??0, 2)
						)
						//,ConverterParameter: "Debug"//t
					)

				);
			}
		}}

		return R.Grid;
	}
}
