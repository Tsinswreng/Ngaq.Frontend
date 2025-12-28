namespace Ngaq.Ui.Views.Settings.LearnWord;

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
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
		Ctx = App.DiOrMk<Ctx>();
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
				sp
				.AddInit(new CheckBox(), o=>{
					//o.Tag = new TextBlock{Text = "Enable Random Background"};
					o.Content = "Enable Random Background";
					o.Bind(
						o.PropIsChecked
						,CBE.Mk<Ctx>(x=>x.EnableRandomBackground)
					);
				})
				.AddInit(new TextBlock(), o=>{
					o.Text = "Language Filter(One per line)";// TODO i18n
				})
				.AddInit(new TextBox(), o=>{
					o.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
					o.AcceptsReturn = true;
					o.Height = 100;
					o.Bind(
						o.PropText
						,CBE.Mk<Ctx>(x=>x.LanguageFilterExpr)
					);
				})
				.AddInit(new TextBlock{
					Text = "Lua Filter",// TODO i18n
				})
				.AddInit(new TextBox{
					TextWrapping = Avalonia.Media.TextWrapping.Wrap,
					AcceptsReturn = true,
					Height = 100,
					//Bind = (o.PropText, CBE.Mk<Ctx>(x=>x.LuaFilterExpr)),
					Init=o=>{o.Bind(
						o.PropText
						,CBE.Mk<Ctx>(x=>x.LuaFilterExpr)
					);},
				})

				// .AddInit(new TextBox(), o=>{
				// 	o.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
				// 	o.AcceptsReturn = true;
				// 	o.Height = 100;
				// 	o.Bind(
				// 		o.PropText
				// 		,CBE.Mk<Ctx>(x=>x.LuaFilterExpr)
				// 	);
				// })
				;
			});
		})
		.AddInit(new OpBtn(), o=>{
			DockPanel.SetDock(o, Dock.Bottom);
			o._Button.StretchCenter();
			o.VerticalAlignment = VAlign.Bottom;
			o.BtnContent = "Save";//TODO i18n
			o.SetExe((Ct)=>Ctx?.SaveAsy(Ct));
		});


		;

		return NIL;
	}


}

