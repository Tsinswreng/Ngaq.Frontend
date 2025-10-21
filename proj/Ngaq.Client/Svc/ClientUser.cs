namespace Ngaq.Client.Svc;

using System.Text;
using Ngaq.Core.Sys.Svc;
using Ngaq.Core.Tools;
using Tsinswreng.CsTools;
using Tsinswreng.CsCore;
using Ngaq.Core.Models.Sys.Req;
using Ngaq.Core.Infra.Url;
using Ngaq.Core.Domains.User.Models.Req;
using Ngaq.Core.Domains.User.Models.Resp;
using Ngaq.Core.Domains.User.Svc;
using Ngaq.Core.Domains.Kv.Models;
using Ngaq.Core.Domains.Word.Models.Po.Kv;

public partial class ClientUser
	:ISvcUser
{
	ISvcKv SvcKv;
	IHttpCaller HttpCaller;
	public ClientUser(
		ISvcKv SvcKv
		,IHttpCaller HttpCaller
	){
		this.SvcKv = SvcKv;
		this.HttpCaller = HttpCaller;
	}

	[Impl]
	public async Task<nil> AddUser(
		ReqAddUser Req
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
	public async Task<RespLogin> Login(ReqLogin ReqLogin, CT Ct){
		var R = await HttpCaller.Post<ReqLogin, RespLogin>(
			ConstUrl.UrlUser.Login
			,ReqLogin
			,Ct
		);
		//TODO 原子寫入 或失敗回滾; 加密ᵈ存; 內存ʸ緩存; 
		await SvcKv.SetAsy(
			new PoKv().SetStr(KeysClientKv.CurLocalUserId, R.UserId.ToString()) ,Ct
		);
		await SvcKv.SetAsy(
			new PoKv().SetStr(KeysClientKv.AccessToken, R.AccessToken) ,Ct
		);
		await SvcKv.SetAsy(
			new PoKv().SetStr(KeysClientKv.RefreshToken, R.RefreshToken) ,Ct
		);
		return R;
	}

	[Impl]
	public async Task<nil> Logout(ReqLogout ReqLogout, CT Ct){
		return NIL;
	}

}
