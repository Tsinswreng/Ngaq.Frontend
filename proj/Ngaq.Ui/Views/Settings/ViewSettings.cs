namespace Ngaq.Ui.Views.Settings;

using Avalonia.Controls;
using Avalonia.Styling;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.About;
using Ngaq.Ui.Views.Settings.LearnWord;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Navigation;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ctx = VmSettings;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;
public partial class ViewSettings
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSettings(){
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

	protected nil Render(){
		var _Item = FnSettingItem(this.ViewNavi);
		this.SetContent(new AutoGrid(IsRow:true).Grid, Root=>{
			Root.RowDefinitions.AddRange([
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Auto),
			]);

			Root.A(new ScrollViewer(), Sv=>{
				Sv.SetContent(new StackPanel(), S=>{
					S.A(_Item(I[K.About], new ViewAbout()));
					S.A(_Item(I[K.UIConfig], new ViewCfgUi()));
					S.A(_Item(I[K.LearnWordSettings], new ViewCfgLearnWord()));

					S.A(MkTextInputRow("Lang", x=>x.Lang));
					S.A(MkTextInputRow("ServerBaseUrl", x=>x.ServerBaseUrl));
					S.A(MkTextInputRow("SqlitePath", x=>x.SqlitePath));
					S.A(MkTextInputRow("MaxDisplayedWordCount", x=>x.MaxDisplayedWordCount));

					S.A(new TextBlock(), o=>{
						o.Text = "LlmDictionary";
						o.FontSize = UiCfg.Inst.BaseFontSize*1.05;
					});
					S.A(MkTextInputRow("ApiUrl", x=>x.LlmDictionaryApiUrl));
					S.A(MkTextInputRow("ApiKey", x=>x.LlmDictionaryApiKey));
					S.A(MkTextInputRow("Model", x=>x.LlmDictionaryModel));
					S.A(MkTextInputRow("Prompt", x=>x.LlmDictionaryPrompt, AcceptsReturn:true));
				});
			});

			Root.A(new OpBtn(), o=>{
				// 以底部固定按鈕統一提交所有文本配置。
				o._Button.StretchCenter();
				o.VerticalAlignment = VAlign.Bottom;
				o.BtnContent = I[K.Save];
				o.Background = UiCfg.Inst.MainColor;
				o.SetExe(Ct=>Ctx?.Save(Ct));
			});
		});
		return NIL;
	}

	protected Control MkTextInputRow(
		str Title
		,System.Linq.Expressions.Expression<Func<Ctx, str>> Prop
		,bool AcceptsReturn = false
	){
		var row = new AutoGrid(IsRow:true);
		row.RowDefs.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
		]);
		row.A(new TextBlock(), o=>{
			o.Text = Title;
			o.FontSize = UiCfg.Inst.BaseFontSize;
		});
		row.A(new TextBox(), o=>{
			o.AcceptsReturn = AcceptsReturn;
			if(AcceptsReturn){
				o.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
				o.MinHeight = UiCfg.Inst.BaseFontSize*5.5;
			}
			o.CBind<Ctx>(o.PropText, x=>Prop);
		});
		return row.Grid;
	}

	public static Func<
		str
		,Control
		,SwipeLongPressBtn
	> FnSettingItem(
		IViewNavi? ViewNavi
	){
		var Fn = (str Title, Control Target)=>{
			var R = new SwipeLongPressBtn();
			var titled = ToolView.WithTitle(Title, Target);
			R.HorizontalContentAlignment = HAlign.Left;
			R.SetContent(new TextBlock(), o=>{
				o.Text = Title;
				o.FontSize = UiCfg.Inst.BaseFontSize*1.2;
			});
			R.Click += (s,e)=>{
				ViewNavi?.GoTo(titled);
			};
			R.Styles.Add(
				new Style(x=>x.Is<Control>())
				.BgTrnsp()
			);
			return R;
		};
		return Fn;
	}


}
