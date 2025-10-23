namespace Ngaq.Client.Svc;

using Tsinswreng.CsCore;
using Ngaq.Core.Models.Sys.Req;
using Ngaq.Core.Infra.Url;
using Ngaq.Core.Shared.User.Models.Req;
using Ngaq.Core.Shared.User.Models.Resp;
using Ngaq.Core.Shared.User.Svc;
using Ngaq.Core.Shared.User.UserCtx;
using Ngaq.Core.Shared.User.Models.Po.User;
using Ngaq.Core.Shared.Kv.Svc;
using Ngaq.Core.Frontend.User.Svc;
using Ngaq.Core.Frontend.User;

public partial class ClientUser
	:ISvcUser
{
	ISvcKv SvcKv;
	IHttpCaller HttpCaller;
	IFrontendUserCtxMgr UserCtxMgr;
	ISvcTokenStorage SvcTokenStorage;

	public ClientUser(
		ISvcKv SvcKv
		,IHttpCaller HttpCaller
		,IFrontendUserCtxMgr UserCtxMgr
		,ISvcTokenStorage SvcTokenStorage
	){
		this.SvcKv = SvcKv;
		this.HttpCaller = HttpCaller;
		this.UserCtxMgr = UserCtxMgr;
		this.SvcTokenStorage = SvcTokenStorage;
	}

	[Impl]
	public async Task<nil> AddUser(
		IUserCtx User
		,ReqAddUser Req
		,CT Ct
	){
		await HttpCaller.Post<ReqAddUser, nil>(
			ConstUrl.UrlUser.AddUser
			,Req
			,Ct
		);
		return NIL;
	}

	[Impl]
	public async Task<RespLogin> Login(
		IUserCtx User
		,ReqLogin ReqLogin, CT Ct
	){
		var R = await HttpCaller.Post<ReqLogin, RespLogin>(
			ConstUrl.UrlUser.Login
			,ReqLogin
			,Ct
		);
		var UserCtx = UserCtxMgr.GetUserCtx();

		UserCtx.RefreshToken = R.RefreshToken;
		UserCtx.AccessToken = R.AccessToken;
		UserCtx.UserId = IdUser.FromLow64Base(R.UserId);
		await SvcTokenStorage.SetRefreshToken(R.RefreshToken, Ct);

		return R;
	}

	[Impl]
	public async Task<nil> Logout(IUserCtx User, ReqLogout ReqLogout, CT Ct){
		return NIL;
	}

}
