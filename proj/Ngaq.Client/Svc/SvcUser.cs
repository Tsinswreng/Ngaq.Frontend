using System.Text;
using Ngaq.Core.Infra;
using Ngaq.Core.Model.Sys.Req;
using Ngaq.Core.Tools;

namespace Ngaq.Client.Svc;

public class SvcUser

{
	protected I_GetBaseUrl GetBaseUrl;
	public SvcUser(
		I_GetBaseUrl GetBaseUrl
	){
		this.GetBaseUrl = GetBaseUrl;
	}



	public async Task<nil> Register(
		ReqAddUser Req
		,CT Ct
	){
		try{
			using HttpClient client = new();
			var ReqJson = JSON.stringify(Req);
			var Data = new StringContent(ReqJson, Encoding.UTF8, "application/json");
			var Resp = await client.PostAsync(
				GetBaseUrl.GetBaseUrl()
			);
		}
		catch (System.Exception)
		{

			throw;
		}
	}

}
