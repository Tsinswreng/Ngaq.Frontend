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
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Star),
			]);
		});
		Root.A(new ScrollViewer(), sc=>{
			sc.SetContent(new StackPanel(), sp=>{
				sp
				.A(new CheckBox(), o=>{
					//o.Tag = new TextBlock{Text = "Enable Random Background"};
					o.Content = "Enable Random Background";
					o.CBind<Ctx>(o.PropIsChecked,x=>x.EnableRandomBackground);
				})
				.A(new CheckBox(), o=>{
					o.Content = "Enable Auto Pronounce";
					o.CBind<Ctx>(o.PropIsChecked,x=>x.EnableAutoPronounce);
				})

				;
			});
		})
		.A(new OpBtn(), o=>{
			DockPanel.SetDock(o, Dock.Bottom);
			o._Button.StretchCenter();
			o.VerticalAlignment = VAlign.Bottom;
			o.BtnContent = Todo.I18n("Save");
			o.SetExe((Ct)=>Ctx?.Save(Ct));
		});


		;

		return NIL;
	}


}

