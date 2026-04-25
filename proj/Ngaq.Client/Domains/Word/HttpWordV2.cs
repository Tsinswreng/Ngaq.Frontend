using Ngaq.Core.Infra;
using Ngaq.Core.Model.Po.Kv;
using Ngaq.Core.Model.Po.Learn_;
using Ngaq.Core.Shared.Dictionary.Models;
using Ngaq.Core.Shared.StudyPlan.Models.PreFilter;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Learn;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Core.Tools.Json;
using Tsinswreng.CsTools;
using U = Ngaq.Core.Infra.Url.KeysUrl.WordV2;

namespace Ngaq.Client.Domains.Word;

public class HttpWordV2 : ISvcWordV2 {
	/// HTTP 請求調用器。
	protected IHttpCaller HttpCaller{get;set;}

	/// JSON 序列化器。
	protected IJsonSerializer JsonS{get;set;}

	/// 構造函數。
	/// <param name="HttpCaller">HTTP 請求調用器。</param>
	/// <param name="JsonS">JSON 序列化器。</param>
	public HttpWordV2(IHttpCaller HttpCaller, IJsonSerializer JsonS){
		this.HttpCaller = HttpCaller;
		this.JsonS = JsonS;
	}

	public Task<nil> BatAddJnWord(IDbUserCtx Ctx, IAsyncEnumerable<JnWord> Words, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> BatAddNewLearnRecord(IDbUserCtx Ctx, IAsyncEnumerable<PoWordLearn> PoWordLearnAsyE, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> BatAddWordLearn(IDbUserCtx Ctx, IAsyncEnumerable<PoWordLearn> WordLearns, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> BatAddWordProp(IDbUserCtx Ctx, IAsyncEnumerable<PoWordProp> WordProps, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> BatChangeId(IDbUserCtx Ctx, IAsyncEnumerable<(IdWord Old, IdWord New)> Ids, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> BatSyncByDto(IDbUserCtx Ctx, IAsyncEnumerable<DtoJnWordSyncResult> Dtos, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<DtoJnWordSyncResult> BatSyncJnWordByBizId(
		IDbUserCtx Ctx, IAsyncEnumerable<JnWord> JnWords, CT Ct
	){
		return CallBatSyncJnWordByBizId(JnWords, Ct);
	}

	public IAsyncEnumerable<DtoJnWordSyncResult> BatSyncJnWordByBizIdFromStream(IDbUserCtx Ctx, Stream TextWithStream, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<RespUpdBizId> BatUpdHeadLang(IDbUserCtx Ctx, IAsyncEnumerable<PoWord> PoWords, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IAsyncEnumerable<RespUpdPoWord>> BatUpdPoWord(IDbUserCtx Ctx, IAsyncEnumerable<PoWord> PoWords, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> BatUpdWordLearn(IDbUserCtx Ctx, IAsyncEnumerable<PoWordLearn> WordLearns, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> BatUpdWordProp(IDbUserCtx Ctx, IAsyncEnumerable<PoWordProp> WordProps, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> DelWordLearnInId(IDbUserCtx Ctx, IAsyncEnumerable<IdWordLearn> Ids, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> DelWordPropInId(IDbUserCtx Ctx, IAsyncEnumerable<IdWordProp> Ids, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<JnWord> GetAllWordsWithDel(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<IJnWordMergeResult> GetWordMergeResult(IDbUserCtx Ctx, IAsyncEnumerable<JnWord> Words, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<JnWord> GetWordsToLearn(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<JnWord> GetWordsToLearn(IDbUserCtx Ctx, PreFilter? Prefilter, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<JnWord> LlmDictWordToJnWord(IDbUserCtx Ctx, IReqLlmDict Req, IRespLlmDict LlmDict, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<JnWord> LlmDictWordToJnWordWithLearn(IDbUserCtx Ctx, IReqLlmDict Req, IRespLlmDict LlmDict, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> MergeWord(IDbUserCtx Ctx, IAsyncEnumerable<IJnWordMergeResult> Words, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> MergeWord(IDbUserCtx Ctx, IAsyncEnumerable<JnWord> Words, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> MergeWord_NewDescrAsAdd(IDbUserCtx Ctx, IAsyncEnumerable<JnWord> Words, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<Stream> PackAllWordsWithDel(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> SoftDelJnWordInId(IDbUserCtx Ctx, IAsyncEnumerable<IdWord> Ids, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<JnWord> UnpackJnWords(Stream TextWithStream, CT Ct) {
		throw new NotImplementedException();
	}

	/// 調用服務端 BatSyncJnWordByBizId 接口，請求與響應都用 gzip 行流。
	/// <param name="JnWords">待同步單詞流。</param>
	/// <param name="Ct">取消令牌。</param>
	/// <returns>服務端返回的同步結果流。</returns>
	protected async IAsyncEnumerable<DtoJnWordSyncResult> CallBatSyncJnWordByBizId(
		IAsyncEnumerable<JnWord> JnWords,
		[System.Runtime.CompilerServices.EnumeratorCancellation] CT Ct
	){
		// 請求體: JnWord -> Json 行 -> gzip 流。
		using var resp = await HttpCaller.SendWithRetry(
			U.BatSyncJnWordByBizId,
			JnWords,
			async (Words, ct)=>{
				var lines = Words.Select(word=>JsonS.Stringify(word));
				var body = await GZipLinesUtf8.ToStream(lines, ct);
				return new StreamContent(body){
					Headers = {
						ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream")
					}
				};
			},
			Ct
		);
		resp.EnsureSuccessStatusCode();

		// 響應體: gzip 行流 -> Json 行 -> Dto。
		using var stream = await resp.Content.ReadAsStreamAsync(Ct);
		await foreach(var line in GZipLinesUtf8.ToLines(stream, Ct)){
			yield return JsonS.Parse<DtoJnWordSyncResult>(line)!;
		}
	}
}
