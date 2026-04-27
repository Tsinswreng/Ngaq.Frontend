namespace Ngaq.Ui.Views.User.AboutMe;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Settings;
using Ngaq.Ui.Views.User.Profile;
using Ngaq.Ui.Views.Word.WordManage;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ngaq.Ui.Infra.I18n;
using Ctx = VmAboutMe;
using Ngaq.Ui.Components;

public partial class ViewAboutMe
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewAboutMe(){
		Ctx = App.GetRSvc<Ctx>();
		Style();
		Render();
	}

	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();
	public AutoGrid Root = new(IsRow: true);
	protected nil Style(){
		return NIL;
	}

	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(2, GUT.Auto),
				RowDef(1, GUT.Star),
			]);
		});
		Root.A(_ToolBar(), o=>{})
		.A(_UserCard())
		.A(new ScrollViewer(), sv=>{
			sv.SetContent(new ViewWordManage());
		});
		return NIL;
	}

	protected Control _UserCard(){
		var R = new AutoGrid(IsRow: false);
		R.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Star),
		]);

		R.A(new SwipeLongPressBtn(), o=>{
			o.VAlign(x=>x.Stretch);
			o.SetContent(new CircleAvatar(), o=>{
				o.Width = UiCfg.Inst.BaseFontSize*1.5;
				o.Height = UiCfg.Inst.BaseFontSize*1.5;
				o.CBind<Ctx>(o.PropSource,x=>x.AvatarImg);
			});
			o.Click += (o, e) => {
				ViewNavi?.GoTo(
					ToolView.WithTitle(I[KeysUiI18nCommon.UserProfile], new ViewUserProfile())
				);
			};
		});
		R.A(new SwipeLongPressBtn(), o=>{
			o.VAlign(VAlign.Stretch);
			o.SetContent(new TextBlock(), t=>{
				t.FontSize = UiCfg.Inst.BaseFontSize*1.2;
				t.Bind(
					t.PropText_()
					,CBE.Mk<Ctx>(x=>x.UserIdRepr)
				);
			});
			o.Click += (s,e)=>{
				ViewNavi?.GoTo(
					ToolView.WithTitle("", new ViewLoginRegister())
				);
			};
		});
		return R.Grid;
	}

	protected Control _ToolBar(){
		var R = new StackPanel();
		R.Orientation = Orientation.Horizontal;
		{{
			R.A(new Button(), o=>{
				//o.Content = "⚙️";
				o.Content = Svgs.Setting().ToIcon();
				o.Click += (s,e)=>{
					ViewNavi?.GoTo(
					ToolView.WithTitle(I[KeysUiI18nCommon.SettingsTitle], new ViewSettings())
					);
				};
				o.Styles.Add(new Style(
					x=>x.Is<Control>()
				).BgTrnsp());
			});
		}}
		return R;
	}


}
