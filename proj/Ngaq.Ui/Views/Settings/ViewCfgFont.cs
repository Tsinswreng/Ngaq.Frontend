namespace Ngaq.Ui.Views.Settings;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmCfgFont;

public partial class ViewCfgFont
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewCfgFont(){
		Ctx = Ctx.Mk();
		Style();
		Render();
	}

	public  partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();
	public AutoGrid Root = new(IsRow: true);

	protected nil Style(){
		return NIL;
	}

	protected Control InputFontSize(){
		var R = new AutoGrid(IsRow:false);
		R.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Auto),
		]);
		R.AddInit(_TextBox(), o=>{
			o.Bind(
				TextBox.TextProperty
				,CBE.Mk<Ctx>(
					x=>x.InputFontSize
				)
			);
		});
		// .AddInit(new SwipeLongPressBtn(), o=>{

		// });
		return R.Grid;
	}

	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Star),
			]);
		});
		Root.AddInit(_TextBlock(), o=>{
			o.Text = "Base Font Size";
		});
		Root.AddInit(InputFontSize())
		.AddInit(new SwipeLongPressBtn(), o=>{
			o.Content = "Try";
			o.Click += (s,e)=>{
				Ctx?.TryNeoFontSize();
			};
		})
		.AddInit(new SwipeLongPressBtn(), o=>{
			o.Content = "Apply";
			o.Click += (s,e)=>{
				Ctx?.ApplyNeoFontSize();
			};
		})
		.AddInit(_TextBlock(), o=>{
			o.Text = SampleText;
			o.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
			o.Bind(
				TextBlock.FontSizeProperty
				,CBE.Mk<Ctx>(x=>x.FontSize)
			);
		});

		return NIL;
	}

str SampleText =
"""
You need to relaunch the application after changing the font size.
!"#$%&'()*+,-./0123456789:;<=>?@
ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`
abcdefghijklmnopqrstuvwxyz{|}~
 ¡¢£¤¥¦§¨©ª«
一去二三里  煙村亖五家
亭臺六七座  八九十枝花
""";

}
