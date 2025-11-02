namespace Ngaq.Ui.Views.User;

using Avalonia.Controls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Views.User.Login;
using Ngaq.Ui.Views.User.Register;
using Ctx = VmLoginRegister;
using K = Ngaq.Ui.Infra.I18n.ItemsUiI18n.LoginRegister;
public partial class ViewLoginRegister
	:UserControl
{
	public II18n I = I18n.Inst;

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

	public  partial class Cls_{

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
				o.Header = I[K.Login];
				o.Content = new ViewLogin(){Ctx=Ctx};
			}

			var Register = new TabItem();
			Switch.Items.Add(Register);
			{var o = Register;
				o.Header = I[K.Register];
				o.Content = new ViewRegister(){Ctx=Ctx};
			}
		}}
		return NIL;
	}
}
