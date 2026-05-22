namespace Ngaq.Ui.Views.Word.WordManage.SearchWords.SearchedWordCard;

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Core.Shared.Base.Models.Po;
using Ngaq.Core.Shared.User.Models.Po;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Tools;
using Ngaq.Ui.Converters;
using Ngaq.Ui.Infra;
using Tsinswreng.Avln.Grid;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmSearchedWordCard;
public partial class ViewSearchedWordCard
	:AppViewBase<Ctx>
{
	public ViewSearchedWordCard(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public partial class Cls{
		public static str InInfoGrid = nameof(InInfoGrid);
	}

	public GridStack Root{get;set;} = new GridStack(IsRow:true);

	protected nil Style(){
		Styles.A(
			Sty.Is<TextBlock>()
			.Set(
				x=>x.Effect
				,new DropShadowDirectionEffect{
					Color = Colors.Black
					,BlurRadius = 4
					,ShadowDepth = 4
					//,Direction = 315 //高度 315是左上角
					,Direction = 330
					,Opacity = 0.5
				}
			)
		);
		return NIL;
	}

	protected nil Render(){
		this.SetContent(Root.Grid);
		Root.SetRowDefs([
			new(4, GUT.Auto),
			new(8, GUT.Auto),
		]);


		var LangGrid = new GridStack(IsRow:false);
		Root.A(LangGrid.Grid, lg=>{
			lg.SetColDefs([
				new(1, GUT.Star),
				new(2, GUT.Star),
			]).A(new TextBlock(), o=>{
				o.VAlign(x=>x.Center);
				o.CBind<Ctx>(o.PropText,x=>x.Lang);
				o.Foreground = Brushes.LightGray;
			}).A(_InfoGrid())
			;
		});

		var HeadBox = new GridStack(IsRow:false);
		Root.A(HeadBox.Grid, hb=>{
			hb.SetColDefs([
				new(1, GUT.Star),
			]);
			hb.A(new TextBlock(), o=>{
				o.VAlign(x=>x.Center);
				o.FontSize = UiCfg.Inst.BaseFontSize+8;
				Ctx.Bind(
					o,x=>x.TextDecorations,x=>x.DelAt
					,Converter: new FnConvtr<IdDel?, TextDecorationCollection?>((delAt)=>{
						return delAt.IsNullOrDefault()
						? null : TextDecorations.Strikethrough;
					})
				);
				Ctx.Bind(o, o=>o.Text,x=>x.Head);
				Ctx.Bind(o, o=>o.Foreground,x=>x.FontColor);
			});
		});
		return NIL;
	}

	Control _InfoGrid(){
		var R = new GridStack(IsRow:false);
		{
			var o = R.Grid;
			o.Classes.Add(Cls.InInfoGrid);
			o.SetColDefs([
				new(1, GUT.Auto),//上次學習記錄
				new(1, GUT.Auto),//add
				new(1, GUT.Auto),//:
				new(1, GUT.Auto),//rmb
				new(1, GUT.Auto),//:
				new(1, GUT.Auto),//fgt
				new(1, GUT.Auto),//tab
				new(1, GUT.Auto),//last review time
				new(1, GUT.Auto),//tab
				new(1, GUT.Auto),//weight
			]);
		}

		R.A(new TextBlock(), o=>{
			//LearnRecord不應潙空集合、緣添旹必得'add'
			Ctx.Bind(
				o,o=>o.Text,x=>x.SavedLearnRecords
				,Converter: new FnConvtr<IList<ILearnRecord>, str>((x, p)=>{
					if(x.Count > 0){
						return Ctx.LearnToSymbol(x[^1].Learn);
					}
					return "";
					// var Word = (Ctx)p!;
					// throw new FatalLogicErr("No learn record found. Word:"+Word.Lang+":"+Word.Head);
				})
				,ConverterParameter: Ctx
			);
		});

		var RecordType = (ELearn Learn)=>{
			var R = new TextBlock{};
			Ctx.Bind(
				R,x=>x.Text,x=>x.Learn_Records
				,Converter: new ConvMultiDictValueCnt<ELearn, ILearnRecord>()
				,ConverterParameter: Learn
			);
			return R;
		};
		var Colon = ()=>new TextBlock(){Text = ":"};

		R.A(RecordType(ELearn.Add))
		.A(Colon())
		.A(RecordType(ELearn.Rmb))
		.A(Colon())
		.A(RecordType(ELearn.Fgt))
		.A(new TextBlock{Text = "\t"})

		//LastReviewTime
		.A(new TextBlock(), o=>{
			Ctx.Bind(
				o, o=>o.Text,x=>x.LastLearnedTime
				,Converter: new FnConvtr<i64, str>(x=>{
					var Now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
					var Diff = Now - x;
					return Ctx.FormatUnixMsDiff(Diff);
				})
			);
		})
		.A(new TextBlock{Text = "\t"})
		.A(new TextBlock(), o=>{
			Ctx.Bind(
				o, o=>o.Text,x=>x.Weight
				,Converter: new FnConvtr<f64?,str>((x,p)=>
					Ctx.FmtNum(x??0, 2)
				)
			);
		});
		return R.Grid;
	}
}
