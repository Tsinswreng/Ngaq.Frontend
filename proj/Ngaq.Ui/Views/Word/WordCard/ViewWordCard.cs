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
using Ngaq.Ui.Views.Word.WordEditV2;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordManage.UserLang.UserLangPage;

public partial class ViewWordListCard
	:UserControl
{

	public static ContextMenu MkWordCardCtxMenu(
		ViewModelBase? Ctx
		,IJnWord? JnWord
	){
		var R = new ContextMenu();
		R.Items.A(new MenuItem(), o=>{

			o.Header = Svgs.CreateMD().ToIcon().WithText(Todo.I18n("Edit"));
			o.Click += (s,e)=>{
				if(AnyNull(JnWord)){

					MainView.Inst.ShowDialog(Todo.I18n("No word selected"));
					return;
				}
                var editView = new ViewWordEditV2();
                if(editView.Ctx is null){
                                    return;
                }
                editView.Ctx.FromJnWord(JnWord);
                ViewNavi?.GoTo(ToolView.WithTitle(JnWord.Head, editView));
			};
		})
		.A(new MenuItem(), o=>{
			o.Header = Svgs.VolHigh().ToIcon().WithText(Todo.I18n("朗讀"));
			o.Click += async(s,e)=>{
				if(Ctx is IWordCardMenuAction Action){
					var R = await Action.PronounceWord(JnWord, default);
					HandlePronounceResult(Ctx, R);
					return;
				}
				MainView.Inst.ShowDialog(Todo.I18n("Current page does not support word pronounce action"));
			};
		});
		return R;
	}

	public static nil HandlePronounceResult(
		ViewModelBase? Ctx,
		DtoWordCardPronounceResult? R
	){
		if(R is null){
			MainView.Inst.ShowDialog(Todo.I18n("Pronounce failed"));
			return NIL;
		}
		if(R.Status == EWordCardPronounceStatus.Played){
			return NIL;
		}
		if(R.Status == EWordCardPronounceStatus.UserLangNotMapped){
			var BtnGoCfg = new Button{
				Content = Todo.I18n("去配置 UserLang"),
			};
			BtnGoCfg.Click += (bs,be)=>{
				var View = new ViewUserLangPage();
				if(View.Ctx is not null){
					View.Ctx.Input = R.WordLang;
					_ = View.Ctx.InitSearch(default);
				}
				ViewNavi?.GoTo(ToolView.WithTitle(Todo.I18n("UserLang"), View));
			};
			MainView.Inst.ShowDialog(
				Todo.I18n("當前單詞語言未映射到標準語言，無法朗讀。"),
				[BtnGoCfg]
			);
			return NIL;
		}
		if(R.Status == EWordCardPronounceStatus.NoWordSelected){
			MainView.Inst.ShowDialog(Todo.I18n("No word selected"));
			return NIL;
		}
		if(R.Status == EWordCardPronounceStatus.WordLangEmpty){
			MainView.Inst.ShowDialog(Todo.I18n("Word lang is empty"));
			return NIL;
		}
		if(R.Status == EWordCardPronounceStatus.ServiceUnavailable){
			MainView.Inst.ShowDialog(Todo.I18n("Service unavailable"));
			return NIL;
		}
		if(R.Status == EWordCardPronounceStatus.Failed){
			if(R.Error is not null){
				Ctx?.HandleErr(R.Error);
			}else{
				MainView.Inst.ShowDialog(Todo.I18n("Pronounce failed"));
			}
			return NIL;
		}
		return NIL;
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
		this.SetContent(Root.Grid, o=>{
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
				o.CBind<Ctx>(o.PropText,x=>x.Index);
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
			o.FontSize = UiCfg.Inst.BaseFontSize*1.2;
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



