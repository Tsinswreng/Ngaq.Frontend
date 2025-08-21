using System.Text;
using Ngaq.Core.Infra;
using Ngaq.Core.Model.Sys.Req;
using Ngaq.Core.Sys.Svc;
using Ngaq.Core.Tools;
using Tsinswreng.CsTools;
using Tsinswreng.CsCore;
using Ngaq.Core.Models.Sys.Req;
using Ngaq.Core.Models.Sys.Resp;

namespace Ngaq.Client.Svc;

public  partial class ClientUser
	:ISvcUser
{
	protected I_GetBaseUrl GetBaseUrl;
	public ClientUser(
		I_GetBaseUrl GetBaseUrl
	){
		this.GetBaseUrl = GetBaseUrl;
	}

	[Impl]
	public async Task<nil> AddUser(
		ReqAddUser Req
		,CT Ct
	){
		try{
			using HttpClient client = new();
			var ReqJson = JSON.stringify(Req);
			var Data = new StringContent(ReqJson, Encoding.UTF8, "application/json");
			var url = ToolPath.SlashTrimEtJoin([
				GetBaseUrl.GetBaseUrl()
				,ConstApiUrl.Inst.ApiV1SysUser
				,ApiUrl_User.Inst.AddUser
			]);
			var Resp = await client.PostAsync(url, Data, Ct);
			return NIL;
		}catch (System.Exception){
			//TODO
			throw;
		}
	}

	[Impl] //TODO 封裝請求器
	public async Task<RespLogin> Login(ReqLogin ReqLogin, CT Ct){
		try{
			using HttpClient client = new();
			var ReqJson = JSON.stringify(ReqLogin);
			var Data = new StringContent(ReqJson, Encoding.UTF8, "application/json");
			var url = ToolPath.SlashTrimEtJoin([
				GetBaseUrl.GetBaseUrl()
				,ConstApiUrl.Inst.ApiV1SysUser
				,ApiUrl_User.Inst.AddUser
			]);
			var Resp = await client.PostAsync(url, Data, Ct);
			string responseBody = await Resp.Content.ReadAsStringAsync();
			var R = JSON.parse<RespLogin>(responseBody);
			if(R == null){
				//TODO
				throw new Exception("Login failed, response is null.");
			}
			return R;
		}catch (System.Exception){
			//TODO
			throw;
		}
	}

	[Impl]
	public async Task<nil> Logout(ReqLogout ReqLogout, CT Ct){
		return NIL;
	}

}
