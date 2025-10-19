namespace Ngaq.Ui.Views.Word.WordManage.EditWord;

using Avalonia.Controls;
using AvaloniaEdit;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmEditWord;
public partial class ViewEditWord
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewEditWord(){
		Ctx = Ctx.Mk();
		Style();
		Render();
	}

	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Auto),
			]);
		});
		Root.AddInit(_TextBox(), o=>{
			o.AcceptsReturn = true;
			o.Bind(
				o.PropText_()
				,CBE.Mk<Ctx>(x=>x.Json)
			);
		});
		var BottomBtnGrid = new AutoGrid(IsRow: false);
		Root.Add(BottomBtnGrid.Grid);
		{
			BottomBtnGrid.Grid.ColumnDefinitions.AddRange([
				ColDef(1,GUT.Star),
				ColDef(1,GUT.Star),
			]);
		}
		{{
			BottomBtnGrid
			.AddInit(_Button(), o=>{
				o.HorizontalContentAlignment = HAlign.Center;
				o.Content = "Save";
				o.Click += (s,e)=>{
					Ctx?.Save();
				};
			})
			.AddInit(_Button(), o=>{
				o.HorizontalContentAlignment = HAlign.Center;
				o.Content = "Delete";
				o.Click += (s,e)=>{
					Ctx?.Delete();
				};
			})
			;
		}}

		return NIL;
	}
}
