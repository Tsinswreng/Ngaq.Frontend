namespace Ngaq.Ui.Views.Settings.LearnWord;

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ngaq.Ui.Infra.I18n;
using Ctx = VmCfgLearnWord;
using Tsinswreng.AvlnTools.Controls;

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
				.A(new SwipeLongPressBtn(), o=>{
					// 在背詞設置中直接提供學習計劃入口，避免多層跳轉。
					o.Content = I[KeysUiI18nCommon.StudyPlan];
					o.Click += (s,e)=>{
						ViewNavi?.GoTo(
							ToolView.WithTitle(I[KeysUiI18nCommon.StudyPlan], new ViewStudyPlan())
						);
					};
				})
				.A(new CheckBox(), o=>{
					//o.Tag = new TextBlock{Text = "Enable Random Background"};
					o.Content = I[KeysUiI18nCommon.EnableRandomBackground];
					o.CBind<Ctx>(o.PropIsChecked,x=>x.EnableRandomBackground);
				})
				.A(new CheckBox(), o=>{
					o.Content = I[KeysUiI18nCommon.EnableAutoPronounce];
					o.CBind<Ctx>(o.PropIsChecked,x=>x.EnableAutoPronounce);
				})
				.A(new TextBlock(), o=>{
					o.Text = I[KeysUiI18nCommon.MaxDisplayedWordCount];
				})
				.A(new TextBox(), o=>{
					o.CBind<Ctx>(o.PropText, x=>x.MaxDisplayedWordCount);
				})

				;
			});
		})
		.A(new OpBtn(), o=>{
			DockPanel.SetDock(o, Dock.Bottom);
			o._Button.StretchCenter();
			o.VerticalAlignment = VAlign.Bottom;
			o.BtnContent = I[KeysUiI18nCommon.Save];
			o._Button.Background = UiCfg.Inst.MainColor;
			o.SetExe((Ct)=>Ctx?.Save(Ct));
		});


		;

		return NIL;
	}


}

