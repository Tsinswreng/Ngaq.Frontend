namespace Ngaq.Ui.Views.Word.WordManage.SearchWords;

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Ngaq.Core.Word.Models;
using Ngaq.Core.Word.Models.Learn_;
using Ngaq.Ui.Converters;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordManage.SearchWords.SearchedWordCard;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmSearchWords;
public partial class ViewSearchWords
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSearchWords(){
		//Ctx = Ctx.Mk();
		Ctx = App.GetSvc<Ctx>();
		Style();
		Render();
	}

	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	//StackPanel Root = new StackPanel();
	AutoGrid Root = new AutoGrid(IsRow: true);


	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Auto),
			]);
		});
		var SearchGrid = new AutoGrid(IsRow: false);
		Root.AddInit(SearchGrid.Grid);
		{var o = SearchGrid;
			o.Grid.ColumnDefinitions.AddRange([
				ColDef(7, GUT.Star),
				ColDef(1, GUT.Star),
			]);
		}
		{{
			SearchGrid.AddInit(_TextBox(), o=>{
				o.Bind(
					TextBox.TextProperty
					,CBE.Mk<Ctx>(x=>x.Input)
				);
			})
			.AddInit(_Button(), o=>{
				o.Content = "🔍";
				o.Click += (s,e)=>{
					Ctx?.InitSearch();
				};
			});
		}}
		Root.AddInit(_ScrollViewer(), scrl=>{
			scrl.ContentInit(_ListWordCard(), o=>{
				o.Bind(
					ItemsControl.ItemsSourceProperty
					,CBE.Mk<Ctx>(
						x=>x.GotWords
						,Mode: BindingMode.OneWay
					)
				);
			});
		});
		Root.AddInit(_PageBar(), o=>{
			o.HorizontalAlignment = HAlign.Center;
		});



		return NIL;
	}

	Control _ListWordCard(){
		var R = new ItemsControl();
		R.ItemsPanel = new FuncTemplate<Panel?>(()=>{
			return new VirtualizingStackPanel();
		});
		R.ItemTemplate = new FuncDataTemplate<JnWord>((jnWord, b)=>{
			var R = new ViewSearchedWordCard(){Ctx = new VmSearchedWordCard()};
			if(jnWord == null){return R;};
			var WordForLearn = new WordForLearn(jnWord);
			R.Ctx.FromIWordForLearn(WordForLearn);
			return R;
		});
		return R;
	}

	Control _PageBar(){
		var R = new AutoGrid(IsRow: false);
		R.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
		]);
		R.AddInit(_Button(), o=>{
			o.Content = "<";
			o.Click += (s,e)=>{
				Ctx?.PrevPage();
			};
		});
		R.AddInit(_TextBox(), o=>{
			o.Bind(
				TextBox.TextProperty
				,CBE.Mk<Ctx>(
					x=>x.PageIdx
					,Converter: new ParamFnConvtr<u64, str>(
						(idx, arg)=>(idx+1).ToString()
						,(Str, arg)=>{
							if(u64.TryParse(Str, out var R)){
								return R-1;
							}
							return 1;
						}
					)
					,Mode: BindingMode.TwoWay
				)
			);
		});
		R.AddInit(_Button(), o=>{
			o.Content = ">";
			o.Click += (s,e)=>{
				Ctx?.NextPage();
			};
		});
		return R.Grid;

	}


}
