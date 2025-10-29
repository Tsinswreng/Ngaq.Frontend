namespace Ngaq.Ui.Views.User.AboutMe;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Settings;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmAboutMe;
public partial class ViewAboutMe
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewAboutMe(){
		Ctx = App.GetSvc<Ctx>();
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
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(2, GUT.Auto),
			]);
		});
		Root.AddInit(_ToolBar(), o=>{});
		Root.AddInit(_UserCard());
		return NIL;
	}

	protected Control _UserCard(){
		var R = new AutoGrid(IsRow: false);
		R.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Star),
		]);
		// R.Grid.RowDefinitions.AddRange([
		// 	RowDef(1, GUT.Auto)
		// ]);

		R.AddInit(new SwipeLongPressBtn(), o=>{
			o.VAlign(VAlign.Stretch);
			o.Content = "👤";
		});
		R.AddInit(new SwipeLongPressBtn(), o=>{
			o.VAlign(VAlign.Stretch);
			o.ContentInit(_TextBlock(), t=>{
				t.FontSize = UiCfg.Inst.BaseFontSize*1.2;
				t.Bind(
					t.PropText_()
					,CBE.Mk<Ctx>(x=>x.UserIdRepr)
				);
			});
			o.Click += (s,e)=>{
				Ctx?.ViewNavi?.GoTo(
					ToolView.WithTitle("", new ViewLoginRegister())
				);
			};
		});
		return R.Grid;
	}

	protected Control _ToolBar(){
		// var R = new AutoGrid(IsRow:false);
		// R.Grid.ColumnDefinitions.AddRange([
		// 	ColDef()
		// ]);
		var R = new StackPanel();
		R.Orientation = Orientation.Horizontal;
		{{
			R.AddInit(_Button(), o=>{
				o.Content = "⚙️";
				o.Click += (s,e)=>{
					Ctx?.ViewNavi?.GoTo(
						ToolView.WithTitle("Settings", new ViewSettings())
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
