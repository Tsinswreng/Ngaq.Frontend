namespace Ngaq.Ui.Views.User;

using System.Collections.ObjectModel;
using Ngaq.Core.Shared.User.Models.Req;
using Ngaq.Core.Shared.User.Svc;
using Ngaq.Ui.Infra;

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
			this.AddMsg("Password and Confirm Password must be the same.");
			return NIL;
		}
		try{
			var reqAddUser = new ReqAddUser{
				Email = Email
				,Password = Password
			};
			SvcUser?.AddUser(reqAddUser, default).ContinueWith(t=>{
				if(t.IsFaulted){
					//this.Msgs.Add(t?.Exception);//TODO
					this.AddMsg(t?.Exception);
				}
			});
		}
		catch (System.Exception e){
			this.AddMsg(e.Message);//TODO
			//throw;
		}
		return NIL;
	}
	CancellationTokenSource Cts = new();
	public nil Login(){
		try{
			var reqLogin = new ReqLogin{
				Email = Email
				,Password = Password
				,KeepLogin = true
				,UserIdentityMode = ReqLogin.EUserIdentityMode.Email
			};
			SvcUser?.Login(reqLogin, Cts.Token).ContinueWith(t=>{
				if(t.IsFaulted){
					//this.Msgs.Add(t?.Exception);//TODO
					this.AddMsg(t?.Exception);
				}
			});
		}catch(Exception e){
			this.AddMsg(e.Message);//TODO
		}
		return NIL;
	}






}

