namespace Ngaq.Ui.Views.User.Profile;

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Ngaq.Ui.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Ngaq.Ui.User;
using Ngaq.Ui.Views.User.Login;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ursa.Controls;
using Ctx = VmXxx;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

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
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
			]);
		});
		Root.A(new ScrollViewer(), sv=>{
			var ContentGrid = new AutoGrid(IsRow: true);
			sv.SetContent(ContentGrid.Grid, o=>{
				o.RowDefinitions.AddRange([
					RowDef(4, GUT.Auto),
					RowDef(4, GUT.Auto),
					RowDef(1, GUT.Auto),
				]);

				ContentGrid
				.A(new CircleAvatar(), o=>{
					try{
						o.Source = DfltAvatar.Img;//TODO
						o.VerticalAlignment = VAlign.Center;
						o.HorizontalAlignment = HAlign.Center;
						o.Width = 150;
						o.Height = 150;
					}catch{}
					//o.Source = SolidImageGenerator.Create(100,100, Colors.Red);
					// o.CBind(
					// 	Avatar.SourceProperty
					// 	,CBE.Mk<Ctx>(x=>x.Avatar)
					// );
				})
				.A(new TextBox(), o=>{

				})
				.A(new StackPanel(), Sp=>{
					Sp
						.A(new Button(), o=>{
						o.StretchCenter();
						o.Content = I[K.ChangeAccount];
						o.Click += (s,e)=>{
							ViewNavi?.GoTo(
								ToolView.WithTitle(I[K.ChangeAccount], new ViewLoginRegister())
							);
						};
					})
					.A(new OpBtn(), o=>{
						o._Button.StretchCenter();
						o.Background = Brushes.Red;
						o.BtnContent = I[K.Logout];
						o.SetExe((Ct)=>Ctx?.LogoutAsy(Ct));
					});
				});
			});
		});
		return NIL;
	}
}
