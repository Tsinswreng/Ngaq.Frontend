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
using Tsinswreng.Avln.StrokeText;
using Ctx = VmWordListCard;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Views.Word.WordEditJsonMap;
using Ngaq.Ui.Infra;

public partial class ViewWordListCard
	:UserControl
{

	public static ContextMenu MkWordCardCtxMenu(
		ViewModelBase? Ctx
		,IJnWord? JnWord
	){
		var R = new ContextMenu();
		R.Items.A(new MenuItem(), o=>{
			Todo.I18n();
			o.Header = Svgs.CreateMD().ToIcon().WithText(" Edit");
			o.Click += (s,e)=>{
				if(AnyNull(JnWord)){
					Todo.I18n();
					MainView.Inst.ShowMsg("No word selected");//TODO
					return;
				}
				var editView = new ViewWordEdit{};

				editView.Ctx?.FromBo(JnWord);
				Ctx?.ViewNavi?.GoTo(ToolView.WithTitle(JnWord.Head, editView));
			};
		});
		return R;
	}

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
		Style();
		Render();
	}

	protected StrokeTextBlock TxtBox(){
		var R = new StrokeTextBlock{
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
		Styles.A(
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
		return NIL;
	}

	protected nil Render(){
		this.InitContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				new RowDef(4, GUT.Auto),
				new RowDef(8, GUT.Auto),
			]);
		});

		var LangGrid = new AutoGrid(IsRow:false);
		Root.A(LangGrid.Grid, o=>{
			o.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
				new ColDef(0.3, GUT.Star),
				new ColDef(4, GUT.Star),
				new ColDef(13, GUT.Star),
			]);
		});
		{{
			LangGrid
			.A(TxtBox(), o=>{
				o.FontSize = UiCfg.Inst.BaseFontSize*0.8;
				o.CBind<Ctx>(
					o.PropText
					,x=>x.Index);
			})
			.A(TxtBox(), o=>{
				o.Text = "　";
			})
			.A(TxtBox(), o=>{
				o.VerticalAlignment = VAlign.Center;
				o.CBind<Ctx>(o.PropText, x=>x.Lang);
				o.Foreground = Brushes.LightGray;
			})
			.A(_InfoGrid());
		}}//~Header


		var HeadBox = new AutoGrid(IsRow:false);
		Root.A(HeadBox.Grid, o=>{
			o.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
			]);
		});

		HeadBox.A(TxtBox(), o=>{
			o.VerticalAlignment = VAlign.Center;
			o.FontSize = UiCfg.Inst.BaseFontSize+8;
			o.CBind<Ctx>(o.PropText, x=>x.Head);
			o.CBind<Ctx>(o.PropForeground,x=>x.FontColor);
		});
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
			var RecordType = (ELearn Learn)=>{
				var R = new TextBlock{};
				R.CBind<Ctx>(
					R.PropText
					,x=>x.Learn_Records
					,Converter: new ConvMultiDictValueCnt<ELearn, ILearnRecord>()
					,ConverterParameter: Learn
				);
				return R;
			};
			var Colon = ()=>new TextBlock(){Text = ":"};

			R.A(TxtBox(), o=>{
				o.CBind<Ctx>(
					o.PropText,x=>x
					,Converter: new SimpleFnConvtr<Ctx?, str>((x)=>x?.ToLearnHistoryRepr()??"")
				);
			});

			//LastReviewTime
			R.A(TxtBox(), o=>{
				o.CBind<Ctx>(
					o.PropText,x=>x.LastLearnedTime
					,Converter: new SimpleFnConvtr<i64, str>(x=>{
						var Now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
						var Diff = Now - x;
						return Ctx.FormatUnixMsDiff(Diff);
					})
				);
			})
			.A(new TextBlock{Text = "\t"})
			.A(TxtBox(), o=>{
				o.CBind<Ctx>(
					o.PropText,x=>x.Weight
					,Converter: new ParamFnConvtr<f64?,str>((x,p)=>
						Ctx.FmtNum(x??0, 1)
					)
				);
			});//Weight
		}}

		return R.Grid;
	}
}
