using System.Text;
using Ngaq.Core.Infra.Url;
using Ngaq.Core.Tools.Json;
using Tsinswreng.CsTools;

public interface IHttpCaller {
	public Task<TResp> Post<TReq, TResp>(
		str RelaUrl
		,TReq Req,
		CT Ct
	);
}


public class HttpCaller:IHttpCaller{
	//IHttpClientFactory

	IJsonSerializer JsonS;
	HttpClient HttpClient;
	I_GetBaseUrl BaseUrlGetter;
	public HttpCaller(
		IJsonSerializer JsonS
		,HttpClient HttpClient
		,I_GetBaseUrl BaseUrlGetter
	){
		this.HttpClient = HttpClient;
		this.JsonS = JsonS;
		this.BaseUrlGetter = BaseUrlGetter;
	}


	/// <summary>
	/// 發送 POST 請求並把回應反序列化成 TResp。
	/// 若 TResp 是 nil 可直接當作「不需回應」處理。
	/// </summary>
	public async Task<TResp> Post<TReq, TResp>(
		str RelaUrl
		,TReq Req,
		CT Ct
	) {
		var url = ToolPath.SlashTrimEtJoin([BaseUrlGetter.GetBaseUrl(), RelaUrl]);
		var Json = JsonS.Stringify(Req);
		using var content = new StringContent(
			Json
			,Encoding.UTF8
			,"application/json"
		);

		using var resp = await HttpClient.PostAsync(url, content, Ct);

		// 可視需求打開以下兩行，保證非 2xx 直接拋
		resp.EnsureSuccessStatusCode();

		var body = await resp.Content.ReadAsStringAsync(Ct);
		var R = JsonS.Parse<TResp>(body); // 若回應是空字串則會拋例外
		return R;
	}


}
