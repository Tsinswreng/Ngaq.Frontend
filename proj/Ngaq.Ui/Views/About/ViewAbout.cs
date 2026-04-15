namespace Ngaq.Ui.Views.About;

using Avalonia.Controls;
using Ngaq.Core.Infra;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;

//using Ctx = VmXxx;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18n.About;
public partial class ViewAbout
	:AppViewBase
{

	// public Ctx? Ctx{
	// 	get{return DataContext as Ctx;}
	// 	set{DataContext = value;}
	// }
	public ViewAbout(){
		//Ctx = Ctx.Mk();
		Style();
		Render();
	}

	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}
	AutoGrid Root = new (IsRow: true);

	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
			]);
		});
		Root.A(new StackPanel(), sp=>{
			sp.A(new SelectableTextBlock(), o=>{
				o.Text = I[K.AppVersion];
			})
			.A(new SelectableTextBlock(), o=>{
				o.Text = AppVer.Inst.Ver+"";
			})
			.A(new Separator())
			.A(new SelectableTextBlock(), o=>{
				o.Text = I[K.Website];
			})
			.A(new SelectableTextBlock(), o=>{
				o.Text = "https://github.com/Tsinswreng/CsNgaq";
			});
		});
		return NIL;
	}


}
