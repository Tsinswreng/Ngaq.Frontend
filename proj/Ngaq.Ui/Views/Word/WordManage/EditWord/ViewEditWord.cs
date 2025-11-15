namespace Ngaq.Ui.Views.Word.WordManage.EditWord;

using Avalonia.Controls;
using AvaloniaEdit;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmEditWord;

#if false //TODO 寫到Ui㕥示用戶

改詞旹 需改聚合根ʹ BizUpdatedAt 及 所改ʹ條目ʹBizUpdatedAt、
務必使新BizUpdatedAt值足夠大、任設一值芝大於原者即可。後端自動取當前ʹ時㕥潙其值
于新增ʹ條目 如 Props或Learns、設其Id爲"0"即可、後端自動替換成新生成ʹId

#endif

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
				o.PropText
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
