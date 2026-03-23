namespace Ngaq.Ui.Views.User.ChangePassword;

using Avalonia.Controls;
using Avalonia.Styling;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmChangePassword;
public partial class ViewChangePassword
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewChangePassword(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls{

	}


	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);
	public void Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star)
		]);
		Root.A(new StackPanel(), Sp=>{
			Sp.A(new TextBlock(), o=>{
				o.Text = "舊密碼";
				o.VerticalAlignment = VAlign.Center;
			})
			.A(new TextBox(), o=>{
				o.PasswordChar = '*';
				o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.OldPassword));
				o.VerticalAlignment = VAlign.Center;
			})
			.A(new TextBlock(), o=>{
				o.Text = "新密碼";
				o.VerticalAlignment = VAlign.Center;
			})
			.A(new TextBox(), o=>{
				o.PasswordChar = '*';
				o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.NewPassword));
				o.VerticalAlignment = VAlign.Center;
			})
			.A(new TextBlock(), o=>{
				o.Text = "確認新密碼";
				o.VerticalAlignment = VAlign.Center;
			})
			.A(new TextBox(), o=>{
				o.PasswordChar = '*';
				o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.ConfirmPassword));
				o.VerticalAlignment = VAlign.Center;
			})
			.A(new OpBtn(), o=>{
				o._Button.Content = "修改密碼";
				o.SetExe(Ct=>Ctx?.ChangePassword(Ct));
			})
			;
		});
	}
}
