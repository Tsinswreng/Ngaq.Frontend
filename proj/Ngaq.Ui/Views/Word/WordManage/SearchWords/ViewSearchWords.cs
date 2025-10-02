namespace Ngaq.Ui.Views.Word.WordManage.SearchWords;

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Ngaq.Core.Word.Models;
using Ngaq.Core.Word.Models.Learn_;
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
					Ctx?.Search();
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


}
