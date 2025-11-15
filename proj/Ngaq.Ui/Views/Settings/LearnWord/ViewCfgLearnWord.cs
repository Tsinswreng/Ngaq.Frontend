namespace Ngaq.Ui.Views.Settings.LearnWord;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmCfgLearnWord;
public partial class ViewCfgLearnWord
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewCfgLearnWord(){
		Ctx = Ctx.Mk();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new AutoGrid(IsRow:true);
	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Star),
			]);
		});
		Root.AddInit(new ScrollViewer(), sc=>{
			sc.ContentInit(new StackPanel(), sp=>{
				sp.AddInit(new TextBlock(), o=>{
					o.Text = "Lua Filter";// TODO i18n
				})
				.AddInit(new TextBox(), o=>{
					o.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
					o.Height = 100;
					o.Bind(
						o.PropText
						,CBE.Mk<Ctx>(x=>x.LuaFilterExpr)
					);
				})
				;
			});
		})
		.AddInit(new Button(), o=>{
			o.VerticalAlignment = VAlign.Bottom;
			o.Content = "Save";//TODO i18n
			DockPanel.SetDock(o, Dock.Bottom);
		});

		;

		return NIL;
	}


}
