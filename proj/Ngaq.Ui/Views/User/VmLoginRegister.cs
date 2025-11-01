namespace Ngaq.Ui.Views.User;

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Ngaq.Core.Frontend.Kv;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra.Core;
using Ngaq.Core.Infra.Errors;
using Ngaq.Core.Shared.Kv.Svc;
using Ngaq.Core.Shared.User.Models.Po.Device;
using Ngaq.Core.Shared.User.Models.Po.User;
using Ngaq.Core.Shared.User.Models.Req;
using Ngaq.Core.Shared.User.Svc;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Ui.Infra;

using Ctx = VmLoginRegister;
public partial class VmLoginRegister: ViewModelBase{
	protected VmLoginRegister(){}
	public static Ctx Mk(){
		return new Ctx();
	}
	protected ISvcUser? SvcUser;
	IFrontendUserCtxMgr? UserCtxMgr;
	ISvcKv SvcKv;
	public VmLoginRegister(
		ISvcUser SvcUser
		,IFrontendUserCtxMgr? UserCtxMgr
		,ISvcKv SvcKv
	){
		this.SvcUser = SvcUser;
		this.UserCtxMgr = UserCtxMgr;
		this.SvcKv = SvcKv;
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

	public bool CheckRegister(){
		//TODO 一次校驗多條
		this.ClearMsg();
		if(str.IsNullOrEmpty(Password)){
			this.AddMsg("Password is empty.");
			return false;
		}
		if(Password != ConfirmPassword){
			this.AddMsg("Password and Confirm Password must be the same.");
			return false;
		}
		if(ValidateEmail(Email) == false){
			this.AddMsg("Email is not valid.");
			return false;
		}
		return true;
	}

	public nil Register(){
		if(!CheckRegister()){
			return NIL;
		}
		RegisterAsy(Cts.Token).ContinueWith(t=>{
			HandleErr(t);
		});
		return NIL;
	}

	public async Task<nil> RegisterAsy(CT Ct){
		if(SvcUser is null|| UserCtxMgr is null){
			return NIL;
		}
		var User = UserCtxMgr.GetUserCtx();
		var reqAddUser = new ReqAddUser{
			Email = Email
			,Password = Password
		};
		await SvcUser.AddUser(User, reqAddUser, Ct);
		return NIL;
	}
	CancellationTokenSource Cts = new();
	public nil Login(){
		LoginAsy(Cts.Token).ContinueWith(t=>{
			HandleErr(t);
		});
		return NIL;
	}
	public async Task<nil> LoginAsy(CT Ct){
		var z = this;
		if(z.UserCtxMgr is null
			|| z.SvcUser is null || z.SvcKv is null
		){
			return NIL;
		}
		var User = z.UserCtxMgr.GetUserCtx();

		var ClientId = await SvcKv.GetByOwnerEtKeyAsy(IdUser.Zero, KeysClientKv.ClientId, Ct);
		if(ClientId is null){
			throw new FatalLogicErr("ClientId is null. ClientId should be in Db when App is launched");
		}
		var reqLogin = new ReqLogin{
			Email = Email
			,Password = Password
			,KeepLogin = true
			,UserIdentityMode = ReqLogin.EUserIdentityMode.Email
			,CliendId = IdClient.FromLow64Base(ClientId.GetVStr()??"")
		};
		await z.SvcUser.Login(User, reqLogin, Cts.Token);

		return NIL;
	}


	//校驗郵箱格式
	public static bool ValidateEmail(str Email){
		var R = MyRegex();
		return R.IsMatch(Email);
	}

	[System.Text.RegularExpressions.GeneratedRegex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")]
	private static partial System.Text.RegularExpressions.Regex MyRegex();
}

