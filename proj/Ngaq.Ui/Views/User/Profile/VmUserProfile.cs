namespace Ngaq.Ui.Views.User.Profile;
using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Models.Sys.Req;
using Ngaq.Core.Shared.User.Svc;
using Ngaq.Ui.Infra;

using Ctx = VmUserProfile;
public partial class VmUserProfile: ViewModelBase{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmUserProfile(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmUserProfile(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	ISvcUser? SvcUser;
	IFrontendUserCtxMgr? UserCtxMgr;
	/// 登出成功後通知View層執行頁面跳轉。
	public event EventHandler<EvtArgMsg>? OnLogoutSucceeded;
	public VmUserProfile(
		ISvcUser? SvcUser
		,IFrontendUserCtxMgr? UserCtxMgr
	){
		this.SvcUser = SvcUser;
		this.UserCtxMgr = UserCtxMgr;
	}


	

/*
	public str YYY{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";
 */

	public async Task<nil> LogoutAsy(CT Ct){
		if(AnyNull(SvcUser, UserCtxMgr)){
			return NIL;
		}
		var User = UserCtxMgr.GetUserCtx();
		await Task.Run(async()=>{
			await SvcUser.Logout(User, new ReqLogout{}, Ct);
		});
		OnLogoutSucceeded?.Invoke(this, new EvtArgMsg());
		return NIL;
	}
}

