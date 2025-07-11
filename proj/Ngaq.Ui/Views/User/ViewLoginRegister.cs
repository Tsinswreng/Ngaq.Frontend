namespace Ngaq.Ui.Views.User;

using Avalonia.Controls;
using Ngaq.Ui.Views.User.Login;
using Ngaq.Ui.Views.User.Register;
using Ctx = VmLoginRegister;
public partial class ViewLoginRegister
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewLoginRegister(){
		//Ctx = new Ctx();
		Ctx=App.GetSvc<Ctx>();
		Style();
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	protected nil Render(){
		var Switch = new TabControl();
		Content = Switch;
		{{
			var Login = new TabItem();
			Switch.Items.Add(Login);
			{var o = Login;
				o.Header = "Login";
				o.Content = new ViewLogin(){Ctx=Ctx};
			}

			var Register = new TabItem();
			Switch.Items.Add(Register);
			{var o = Register;
				o.Header = "Register";
				o.Content = new ViewRegister(){Ctx=Ctx};
			}
		}}
		return NIL;
	}


}
