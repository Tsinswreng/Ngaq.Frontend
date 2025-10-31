using System.Text;
using System.Net.Http.Headers;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra.Url;
using Ngaq.Core.Tools.Json;
using Tsinswreng.CsTools;
using Ngaq.Core.Shared.User.Models.Req;
using Ngaq.Core.Infra.Core;
using Ngaq.Core.Shared.User.Models.Resp;
using Ngaq.Core.Infra;
using Ngaq.Core.Infra.Errors;

public interface IHttpCaller {
	public Task<TResp?> Post<TReq, TResp>(
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
	IFrontendUserCtxMgr UserCtxMgr;
	public HttpCaller(
		IJsonSerializer JsonS
		,HttpClient HttpClient
		,I_GetBaseUrl BaseUrlGetter
		,IFrontendUserCtxMgr UserCtxMgr
	){
		this.HttpClient = HttpClient;
		this.JsonS = JsonS;
		this.BaseUrlGetter = BaseUrlGetter;
		this.UserCtxMgr = UserCtxMgr;
	}

	/// <summary>
	/// 發送 POST 請求並把回應反序列化成 TResp。
	/// 若 TResp 是 nil 可直接當作「不需回應」處理。
	/// </summary>
	public async Task<TResp?> Post<TReq, TResp>(
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

		HttpResponseMessage resp = null!;
		for(var i = 0;i < 1;i++){
			var userCtx = UserCtxMgr.GetUserCtx();
			var token = userCtx?.AccessToken;
			using var reqMsg = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
			if(!str.IsNullOrEmpty(token)){
				reqMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}
			resp = await HttpClient.SendAsync(reqMsg, Ct);
			if(resp.StatusCode == System.Net.HttpStatusCode.Unauthorized){
				var Refresh = await RefreshBothToken(Ct);
				//TODO 處理 Refresh 錯誤 如過期等
				continue;
			}else{
				break;
			}
		}

		var body = await resp.Content.ReadAsStringAsync(Ct);
		if(str.IsNullOrEmpty(body)){
			return default;
		}

		IWebAns<TResp>? Ans = null!;
		try{
			Ans = WebAns.Deserialize<TResp>(body);
		}catch(Exception e){
			System.Console.WriteLine("Json反序列化失敗: "+e);//t
		}

		if(Ans.Errors is not null && Ans.Errors.Count > 0){
			throw AppErr.FromViews(Ans.Errors);
		}
		resp.EnsureSuccessStatusCode();
		return Ans.Data;
	}

	/// <summary>
	/// 發送 POST 請求並把回應反序列化成 TResp。
	/// </summary>
	public async Task<TResp?> PostBlob<TReq, TResp>(
		str RelaUrl
		,u8[] Req,
		CT Ct
	) {
		var url = ToolPath.SlashTrimEtJoin([BaseUrlGetter.GetBaseUrl(), RelaUrl]);
		var Json = JsonS.Stringify(Req);
		using var content = new ByteArrayContent(Req);
		content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

		HttpResponseMessage resp = null!;
		for(var i = 0;i < 1;i++){
			var userCtx = UserCtxMgr.GetUserCtx();
			var token = userCtx?.AccessToken;
			using var reqMsg = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
			if(!str.IsNullOrEmpty(token)){
				reqMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}
			resp = await HttpClient.SendAsync(reqMsg, Ct);
			if(resp.StatusCode == System.Net.HttpStatusCode.Unauthorized){
				var Refresh = await RefreshBothToken(Ct);
				//TODO 處理 Refresh 錯誤 如過期等
				continue;
			}else{
				break;
			}
		}

		// 可視需求打開以下兩行，保證非 2xx 直接拋
		resp.EnsureSuccessStatusCode();

		var body = await resp.Content.ReadAsStringAsync(Ct);
		if(str.IsNullOrEmpty(body)){
			return default;
		}
		try{
			var R = JsonS.Parse<TResp>(body); // 若回應是空字串則會拋例外
			return R;
		}catch(Exception){
			throw;//再次引发捕获到的异常会更改堆栈信息CA2200
			//System.Console.WriteLine("Json反序列化失敗: "+e);//t
		}
	}

	public async Task<IAnswer<nil>> RefreshBothToken(CT Ct){
		var R = new Answer<nil>();
		var User = UserCtxMgr.GetUserCtx();
		if(str.IsNullOrEmpty(User.RefreshToken)){
			return R.AddErrStr("str.IsNullOrEmpty(User.RefreshToken)");
		}
		var Url = ToolPath.SlashTrimEtJoin([BaseUrlGetter.GetBaseUrl(), ConstUrl.UrlUser.TokenRefresh]);
		var Req = new ReqRefreshTheToken{
			RefreshToken = User.RefreshToken
		};
		var Json = JsonS.Stringify(Req);
		using var content = new StringContent(
			Json, Encoding.UTF8, "application/json"
		);
		using var resp = await HttpClient.PostAsync(Url, content, Ct);
		resp.EnsureSuccessStatusCode();

		var body = await resp.Content.ReadAsStringAsync(Ct);
		var R2 = JsonS.Parse<RespRefreshBothToken>(body);

		User.RefreshToken = R2.RefreshToken;
		User.AccessToken = R2.AccessToken;
		User.AccessTokenExpireAt = R2.AccessTokenExpireAt;
		User.RefreshTokenExpireAt = R2.RefreshTokenExpireAt;

		UserCtxMgr.SetUserCtx(User);


		return R.OkWith(NIL);
	}


}
