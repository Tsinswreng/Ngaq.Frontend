namespace Ngaq.Ui.Views.About;

using Avalonia.Controls;
using Ngaq.Core.Infra;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

//using Ctx = VmXxx;
using K = Ngaq.Ui.Infra.I18n.ItemsUiI18n.About;
public partial class ViewAbout
	:AppViewBase
{

	// public Ctx? Ctx{
	// 	get{return DataContext as Ctx;}
	// 	set{DataContext = value;}
	// }
	public II18n I = I18n.Inst;
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
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
			]);
		});
		Root.AddInit(_StackPanel(), sp=>{
			sp.AddInit(new SelectableTextBlock(), o=>{
				o.Text = I[K.AppVersion];
			})
			.AddInit(new SelectableTextBlock(), o=>{
				o.Text = AppVer.Inst.Ver+"";
			})
			.AddInit(new Separator())
			.AddInit(new SelectableTextBlock(), o=>{
				o.Text = I[K.Website];
			})
			.AddInit(new SelectableTextBlock(), o=>{
				o.Text = "https://github.com/Tsinswreng/CsNgaq";
			});
		});
		return NIL;
	}


}
