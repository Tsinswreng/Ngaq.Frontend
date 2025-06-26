using System.Text;
using Ngaq.Core.Infra;
using Ngaq.Core.Model.Sys.Req;
using Ngaq.Core.Tools;
using Tsinswreng.CsTools.Tools;

namespace Ngaq.Client.Svc;

public class SvcUser

{
	protected I_GetBaseUrl GetBaseUrl;
	public SvcUser(
		I_GetBaseUrl GetBaseUrl
	){
		this.GetBaseUrl = GetBaseUrl;
	}

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
				,ApiUrl_User.Inst.Register
			]);
			var Resp = await client.PostAsync(url, Data, Ct);
			return NIL;
		}
		catch (System.Exception){
			//TODO
			throw;
		}
	}

}
