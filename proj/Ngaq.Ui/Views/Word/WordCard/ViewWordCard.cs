namespace Ngaq.Ui.Views.Word.WordCard;

using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Ngaq.Core.Infra.Core;
using Ngaq.Core.Service.Word.Learn_.Models;
using Ngaq.Ui.Converters;
using Tsinswreng.Avalonia.Sugar;
using Tsinswreng.Avalonia.Tools;
using Ctx = VmWordListCard;
public partial class ViewWordListCard
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordListCard(){
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
		//Styles.Add(SugarStyle.GridShowLines());
		return Nil;
	}

	protected nil Render(){

		var RootGrid = Root.Grid;
		Content = RootGrid;
		RootGrid.RowDefinitions.AddRange([
			new RowDef(4, GUT.Auto),
			new RowDef(8, GUT.Auto),
		]);

		var LangGrid = new IndexGrid(IsRow:false);
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
				o.VerticalAlignment = VertAlign.Center;
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


		var HeadBox = new IndexGrid(IsRow:false);
		Root.Add(HeadBox.Grid);
		{
			HeadBox.Grid.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Star),
			]);
		}
		{{
			var Head = new SelectableTextBlock();
			HeadBox.Add(Head);
			{var o = Head;
				o.VerticalAlignment = VertAlign.Center;
				o.Bind(
					TextBlock.TextProperty
					,CBE.Mk<Ctx>(x=>x.Head)
				);
				o.FontSize = UiCfg.Inst.BaseFontSize+8;
			}
		}}

		return Nil;
	}

	Control _InfoGrid(){
		var R = new IndexGrid(IsRow:false);
		{var o = R.Grid;
			o.ColumnDefinitions.AddRange([
				new ColDef(1, GUT.Auto),//上次學習記錄
				new ColDef(1, GUT.Auto),//add
				new ColDef(1, GUT.Auto),//:
				new ColDef(1, GUT.Auto),//rmb
				new ColDef(1, GUT.Auto),//:
				new ColDef(1, GUT.Auto),//fgt
				new ColDef(1, GUT.Auto),//tab
				new ColDef(1, GUT.Auto),//last review time
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
						,Converter: new SimpleFnConvtr<IList<ILearnRecord>, str>(x=>{
							if(x.Count > 0){
								return Ctx.LearnToSymbol(x[^1].Value);
							}
							throw new FatalLogicErr("No learn record found.");
						})
					)
				);
			}

			var RecordType = (Learn Learn)=>{
				var R = new TextBlock{};
				R.Bind(
					TextBlock.TextProperty
					,CBE.Mk<Ctx>(x=>x.Learn_Records
						,Converter: new ConvMultiDictValueCnt<Learn, ILearnRecord>()
						,ConverterParameter: Learn
					)
				);
				return R;
			};
			var Colon = ()=>new TextBlock(){Text = ":"};

			var Add = RecordType(ELearn.Inst.Add);
			R.Add(Add);

			R.Add(Colon());
			var Rmb = RecordType(ELearn.Inst.Rmb);
			R.Add(Rmb);
			R.Add(Colon());
			var Fgt = RecordType(ELearn.Inst.Fgt);
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
		}}

		return R.Grid;
	}
}
