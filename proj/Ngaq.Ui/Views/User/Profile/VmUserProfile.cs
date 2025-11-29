namespace Ngaq.Ui.Views.User.Profile;
using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Models.Sys.Req;
using Ngaq.Core.Shared.User.Svc;
using Ngaq.Ui.Infra;

using Ctx = VmXxx;
public partial class VmXxx: ViewModelBase{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmXxx(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmXxx(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	ISvcUser? SvcUser;
	IFrontendUserCtxMgr? UserCtxMgr;
	public VmXxx(
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
		return NIL;
	}
}

