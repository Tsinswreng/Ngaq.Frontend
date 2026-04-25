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

namespace Ngaq.Client.Domains.Word;

public class HttpWordV2 : ISvcWordV2 {
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

	public IAsyncEnumerable<DtoJnWordSyncResult> BatSyncJnWordByBizId(IDbUserCtx Ctx, IAsyncEnumerable<JnWord> JnWords, CT Ct) {
		throw new NotImplementedException();
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
}
