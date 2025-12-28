namespace Ngaq.Ui.Views.User.Profile;

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Ngaq.Ui.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Ngaq.Ui.User;
using Ngaq.Ui.Views.User.Login;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ursa.Controls;
using Ctx = VmXxx;

public partial class ViewUserProfile
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewUserProfile(){
		Ctx = Ctx.Mk();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();
	public AutoGrid Root = new(IsRow: true);

	protected nil Style(){
		return NIL;
	}

	protected TextBox TxtBox(){
		return new TextBox();
	}

	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
			]);
		});
		Root.AddInit(new ScrollViewer(), sv=>{
			var ContentGrid = new AutoGrid(IsRow: true);
			sv.ContentInit(ContentGrid.Grid, o=>{
				o.RowDefinitions.AddRange([
					RowDef(4, GUT.Auto),
					RowDef(4, GUT.Auto),
					RowDef(1, GUT.Auto),
				]);

				ContentGrid
				.AddInit(new CircleAvatar(), o=>{
					try{
						o.Source = DfltAvatar.Img;//TODO
						o.VerticalAlignment = VAlign.Center;
						o.HorizontalAlignment = HAlign.Center;
						o.Width = 150;
						o.Height = 150;
					}catch{}
					//o.Source = SolidImageGenerator.Create(100,100, Colors.Red);
					// o.Bind(
					// 	Avatar.SourceProperty
					// 	,CBE.Mk<Ctx>(x=>x.Avatar)
					// );
				})
				.AddInit(new TextBox(), o=>{

				})
				.AddInit(new StackPanel(), Sp=>{
					Sp
						.AddInit(new Button(), o=>{
						o.StretchCenter();
						o.Content = "Change Account"; //TODO i18n
						o.Click += (s,e)=>{
							Ctx?.ViewNavi?.GoTo(
								ToolView.WithTitle("Change Account", new ViewLoginRegister())
							);
						};
					})
					.AddInit(new OpBtn(), o=>{
						o._Button.StretchCenter();
						o.Background = Brushes.Red;
						o.BtnContent = "Logout"; //TODO i18n
						o.SetExe((Ct)=>Ctx?.LogoutAsy(Ct));
					});
				});
			});
		});
		return NIL;
	}
}
