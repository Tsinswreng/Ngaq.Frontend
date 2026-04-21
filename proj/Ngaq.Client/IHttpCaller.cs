namespace Ngaq.Client;
public interface IHttpCaller {
	public Task<TResp?> Post<TReq, TResp>(
		str RelaUrl,TReq Req,CT Ct
	);
	public Task<TResp?> PostByteStream<TReq, TResp>(
		str RelaUrl,u8[] Req,CT Ct
	);

	public Task<HttpResponseMessage> SendWithRetry<TContent>(
		string RelaUrl,
		TContent RawContent,
		Func<TContent, HttpContent> ContentFactory,
		CT Ct
	);

	public Task<HttpResponseMessage> SendWithRetry<TContent>(
		string RelaUrl,
		TContent RawContent,
		Func<TContent, CT, Task<HttpContent>> ContentFactory,
		CT Ct
	);
}
