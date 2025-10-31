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
using Ngaq.Core.Shared.Kv.Models;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Frontend.Kv;

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
			ConstUrl.UrlOpenUser.AddUser
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
			ConstUrl.UrlOpenUser.Login
			,ReqLogin
			,Ct
		);
		var UserCtx = UserCtxMgr.GetUserCtx();

		UserCtx.RefreshToken = R.RefreshToken;
		UserCtx.RefreshTokenExpireAt = R.RefreshTokenExpireAt;
		UserCtx.AccessToken = R.AccessToken;
		UserCtx.AccessTokenExpireAt = R.AccessTokenExpireAt;

		UserCtx.LoginUserId = IdUser.FromLow64Base(R.UserId);
		UserCtx.Kv??=new Dictionary<str,obj?>();
		UserCtx.Kv["PoUser"] = R.PoUser;
		UserCtxMgr.SetUserCtx(UserCtx);
		await SvcTokenStorage.SetRefreshToken(new ReqSetRefreshToken{
			RefreshToken = R.RefreshToken
			,RefreshTokenExpireAt = R.RefreshTokenExpireAt
		}, Ct);
		await SvcKv.SetAsy(
			new PoKv{Owner = IdUser.Zero}
			.SetStrStr(KeysClientKv.CurLoginUserId, UserCtx.LoginUserId.ToString())
			, Ct
		);

		return R;
	}

	[Impl]//TODO
	public async Task<nil> Logout(IUserCtx User, ReqLogout ReqLogout, CT Ct){
		var OldCtx = UserCtxMgr.GetUserCtx();

		// UserCtxMgr.SetUserCtx(

		// )
		return NIL;
	}

}
