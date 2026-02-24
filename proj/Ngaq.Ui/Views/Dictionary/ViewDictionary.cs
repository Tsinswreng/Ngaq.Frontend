namespace Ngaq.Ui.Views.Dictionary;

using Avalonia.Controls;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Dictionary.DictionaryApi;
using Ngaq.Ui.Views.Dictionary.SimpleWord;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmDictionary;
public partial class ViewDictionary
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewDictionary(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls{}

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow:true);
	protected nil Render(){
		this.InitContent(Root.Grid, o=>{
			Root.RowDefs.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),

			]);
		});
		var SearchGrid = new AutoGrid(IsRow: false);
		Root.AddInit(SearchGrid.Grid, o=>{
			SearchGrid.ColDefs.AddRange([
				ColDef(10, GUT.Star),
				ColDef(2, GUT.Star),
			]);
		});
		{{
			SearchGrid
			.AddInit(new TextBox(), o=>{
				o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.Input));
			})
			.AddInit(new OpBtn(), o=>{
				Todo.I18n();
				//o._Button.Content = "Search";
				o._Button.StretchCenter();
				o._Button.Content = Svgs.Search.ToIcon();
				o.SetExe(Ct=>Ctx?.Lookup(Ct));
			})
			;

		}}
		Root.AddInit(new ViewSimpleWord(), o=>{
			o.Bind(o.PropDataContext, CBE.Mk<Ctx>(x=>x.Result));
		})

		;

		return NIL;
	}




}
