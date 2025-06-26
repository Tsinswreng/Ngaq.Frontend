namespace Ngaq.Ui.Views.User;

using System.Collections.ObjectModel;
using Ngaq.Core.Model.Sys.Req;
using Ngaq.Core.Sys.Svc;
using Ngaq.Ui.ViewModels;

using Ctx = VmLoginRegister;
public partial class VmLoginRegister: ViewModelBase{

	public VmLoginRegister(){}
	protected ISvcUser? SvcUser;
	public VmLoginRegister(
		ISvcUser SvcUser
	){
		this.SvcUser = SvcUser;
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmLoginRegister(){
		{
			var o = new Ctx();
			Samples.Add(o);
		}
	}

	protected str _Email="";
	public str Email{
		get{return _Email;}
		set{SetProperty(ref _Email, value);}
	}

	protected str _Password="";
	public str Password{
		get{return _Password;}
		set{SetProperty(ref _Password, value);}
	}

	protected str _ConfirmPassword = "";
	public str ConfirmPassword{
		get{return _ConfirmPassword;}
		set{SetProperty(ref _ConfirmPassword, value);}
	}


	public nil Register(){
		if(Password != ConfirmPassword){
			//TODO
			this.Msgs.Add("Password and Confirm Password must be the same.");
			return NIL;
		}
		try{
			var ReqAddUser = new ReqAddUser{
				Email = Email
				,Password = Password
			};
			SvcUser?.AddUser(ReqAddUser, default).ContinueWith(t=>{
				if(t.IsFaulted){
					this.Msgs.Add(t?.Exception);//TODO
				}
			});
		}
		catch (System.Exception e){
			this.Msgs.Add(e.Message);//TODO
			//throw;
		}
		return NIL;
	}




}
