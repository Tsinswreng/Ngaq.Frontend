namespace Ngaq.Client.Word.Svc;

using System.IO;
using System.Text;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Infra.Url;
using Ngaq.Core.Shared.Word.Svc;

/// <summary>
/// 單詞同步 V2 客戶端。
/// - Push: 本地打包（含軟刪）後上傳到服務端。
/// - Pull: 從服務端下載後按 BizId 規則同步到本地。
/// </summary>
public class ClientWordSyncV2{
	IHttpCaller HttpCaller;
	IFrontendUserCtxMgr UserCtxMgr;
	ISvcWordV2 SvcWordV2;

	/// <summary>
	/// 構造函數。
	/// </summary>
	/// <param name="HttpCaller">HTTP 請求調用器。</param>
	/// <param name="UserCtxMgr">前端用戶上下文管理器。</param>
	/// <param name="SvcWordV2">本地單詞 V2 服務。</param>
	public ClientWordSyncV2(
		IHttpCaller HttpCaller
		,IFrontendUserCtxMgr UserCtxMgr
		,ISvcWordV2 SvcWordV2
	){
		this.HttpCaller = HttpCaller;
		this.UserCtxMgr = UserCtxMgr;
		this.SvcWordV2 = SvcWordV2;
	}

	/// <summary>
	/// 把本地單詞（含軟刪）打包後推送到服務端。
	/// </summary>
	/// <param name="Ct">取消令牌。</param>
	/// <returns>成功返回 NIL。</returns>
	public async Task<nil> Push(CT Ct){
		using var packed = await SvcWordV2.PackAllWordsWithDel(UserCtxMgr.GetDbUserCtx(), Ct);
		using var ms = new MemoryStream();
		await packed.CopyToAsync(ms, Ct);
		await HttpCaller.PostByteStream<obj, nil>(
			KeysUrl.WordV2.Push
			,ms.ToArray()
			,Ct
		);
		return NIL;
	}

	/// <summary>
	/// 從服務端拉取單詞包並同步到本地。
	/// </summary>
	/// <param name="Ct">取消令牌。</param>
	/// <returns>成功返回 NIL。</returns>
	public async Task<nil> Pull(CT Ct){
		using var resp = await HttpCaller.SendWithRetry(
			KeysUrl.WordV2.Pull
			,str.Empty
			,(json)=>new StringContent(json, Encoding.UTF8, "application/json")
			,Ct
		);
		resp.EnsureSuccessStatusCode();

		var bytes = await resp.Content.ReadAsByteArrayAsync(Ct);
		using var stream = new MemoryStream(bytes, writable: false);
		await foreach(var _ in SvcWordV2.BatSyncJnWordByBizIdFromStream(
			UserCtxMgr.GetDbUserCtx(),
			stream,
			Ct
		)){
			// 消費枚舉以觸發實際同步寫入。
		}
		return NIL;
	}
}
