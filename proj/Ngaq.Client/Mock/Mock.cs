using Ngaq.Core.Frontend.ImgBg;
using Ngaq.Core.Infra;
using Ngaq.Core.Model.Po.Kv;
using Ngaq.Core.Model.Po.Learn_;
using Ngaq.Core.Shared.Audio;
using Ngaq.Core.Shared.Dictionary.Models;
using Ngaq.Core.Shared.Dictionary.Models.Po.NormLang;
using Ngaq.Core.Shared.Dictionary.Svc;
using Ngaq.Core.Shared.StudyPlan.Models;
using Ngaq.Core.Shared.StudyPlan.Models.Po.PreFilter;
using Ngaq.Core.Shared.StudyPlan.Models.Po.StudyPlan;
using Ngaq.Core.Shared.StudyPlan.Models.Po.WeightArg;
using Ngaq.Core.Shared.StudyPlan.Models.Po.WeightCalculator;
using Ngaq.Core.Shared.StudyPlan.Models.PreFilter;
using Ngaq.Core.Shared.StudyPlan.Models.Req;
using Ngaq.Core.Shared.StudyPlan.Svc;
using Ngaq.Core.Shared.User.UserCtx;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Learn;
using Ngaq.Core.Shared.Word.Models.Po.UserLang;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Core.Tools;
using Ngaq.Core.Tools.Io;
using Ngaq.Core.Word.Svc;
using Tsinswreng.CsPage;
using Tsinswreng.CsTempus;
using Tsinswreng.CsTools;

namespace Ngaq.Client.Mock;

public class MockSvcDictionary : ISvcDictionary {
	public Task<PoNormLang?> GetCurSrcNormLang(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<PoNormLang?> GetCurTgtNormLang(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IRespLlmDict> Lookup(IUserCtx User, IReqLlmDict Req, CT Ct) {
		throw new NotImplementedException();
	}

	public IRespLlmDict ParseRawOutput(string RawOutput) {
		throw new NotImplementedException();
	}

	public Task<PoNormLang?> SetCurSrcNormLang(IDbUserCtx Ctx, PoNormLang Po, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<PoNormLang?> SetCurTgtNormLang(IDbUserCtx Ctx, PoNormLang Po, CT Ct) {
		throw new NotImplementedException();
	}
}


public class MockSvcWordV2 : ISvcWordV2 {
	public Task<object> BatAddJnWord(IDbUserCtx Ctx, IAsyncEnumerable<JnWord> Words, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatAddNewLearnRecord(IDbUserCtx Ctx, IAsyncEnumerable<PoWordLearn> PoWordLearnAsyE, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatAddWordLearn(IDbUserCtx Ctx, IAsyncEnumerable<PoWordLearn> WordLearns, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatAddWordProp(IDbUserCtx Ctx, IAsyncEnumerable<PoWordProp> WordProps, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatChangeId(IDbUserCtx Ctx, IAsyncEnumerable<(IdWord Old, IdWord New)> Ids, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatSyncByDto(IDbUserCtx Ctx, IAsyncEnumerable<DtoJnWordSyncResult> Dtos, CT Ct) {
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

	public Task<object> BatUpdWordLearn(IDbUserCtx Ctx, IAsyncEnumerable<PoWordLearn> WordLearns, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatUpdWordProp(IDbUserCtx Ctx, IAsyncEnumerable<PoWordProp> WordProps, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> DelWordLearnInId(IDbUserCtx Ctx, IAsyncEnumerable<IdWordLearn> Ids, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> DelWordPropInId(IDbUserCtx Ctx, IAsyncEnumerable<IdWordProp> Ids, CT Ct) {
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

	public Task<object> MergeWord(IDbUserCtx Ctx, IAsyncEnumerable<IJnWordMergeResult> Words, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> MergeWord(IDbUserCtx Ctx, IAsyncEnumerable<JnWord> Words, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> MergeWord_NewDescrAsAdd(IDbUserCtx Ctx, IAsyncEnumerable<JnWord> Words, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<Stream> PackAllWordsWithDel(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> SoftDelJnWordInId(IDbUserCtx Ctx, IAsyncEnumerable<IdWord> Ids, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<JnWord> UnpackJnWords(Stream TextWithStream, CT Ct) {
		throw new NotImplementedException();
	}
}

public class MockSvcStudyPlan : ISvcStudyPlan, IStudyPlanGetter {
	public Task<object> BatAddPreFilter(IDbUserCtx Ctx, IAsyncEnumerable<PoPreFilter> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatAddStudyPlan(IDbUserCtx Ctx, IAsyncEnumerable<PoStudyPlan> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatAddWeightArg(IDbUserCtx Ctx, IAsyncEnumerable<PoWeightArg> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatAddWeightCalculator(IDbUserCtx Ctx, IAsyncEnumerable<PoWeightCalculator> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<PoPreFilter?> BatGetPreFilterById(IDbUserCtx Ctx, IAsyncEnumerable<IdPreFilter> Ids, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<PoWeightArg?> BatGetWeightArgById(IDbUserCtx Ctx, IAsyncEnumerable<IdWeightArg> Ids, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<PoWeightCalculator?> BatGetWeightCalculatorById(IDbUserCtx Ctx, IAsyncEnumerable<IdWeightCalculator> Ids, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatSoftDelPreFilter(IDbUserCtx Ctx, IAsyncEnumerable<PoPreFilter> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatSoftDelStudyPlan(IDbUserCtx Ctx, IAsyncEnumerable<PoStudyPlan> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatSoftDelWeightArg(IDbUserCtx Ctx, IAsyncEnumerable<PoWeightArg> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatSoftDelWeightCalculator(IDbUserCtx Ctx, IAsyncEnumerable<PoWeightCalculator> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatUpdPreFilter(IDbUserCtx Ctx, IAsyncEnumerable<PoPreFilter> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatUpdStudyPlan(IDbUserCtx Ctx, IAsyncEnumerable<PoStudyPlan> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatUpdWeightArg(IDbUserCtx Ctx, IAsyncEnumerable<PoWeightArg> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatUpdWeightCalculator(IDbUserCtx Ctx, IAsyncEnumerable<PoWeightCalculator> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<bool> EnsureCurStudyPlan(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<BoStudyPlan?> GetCurBoStudyPlan(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<JnStudyPlan?> GetCurJnStudyPlan(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IdStudyPlan?> GetCurStudyPlanId(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IWeightCalctr?> GetCurWeightCalctr(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<BoStudyPlan> GetDfltStudyPlan(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<BoStudyPlan> GetStudyPlan(IUserCtx User, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IPageAsyE<PoPreFilter>> PagePreFilter(IDbUserCtx Ctx, ReqPagePreFilter Req, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IPageAsyE<PoStudyPlan>> PageStudyPlan(IDbUserCtx Ctx, ReqPageStudyPlan Req, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IPageAsyE<PoWeightArg>> PageWeightArg(IDbUserCtx Ctx, ReqPageWeightArg Req, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IPageAsyE<PoWeightCalculator>> PageWeightCalculator(IDbUserCtx Ctx, ReqPageWeightCalculator Req, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> RestoreBuiltinStudyPlan(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> SetCurStudyPlanId(IDbUserCtx Ctx, IdStudyPlan IdStudyPlan, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> SyncPreFilter(IDbUserCtx Ctx, IAsyncEnumerable<PoPreFilter> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> SyncStudyPlan(IDbUserCtx Ctx, IAsyncEnumerable<PoStudyPlan> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> SyncWeightArg(IDbUserCtx Ctx, IAsyncEnumerable<PoWeightArg> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> SyncWeightCalculator(IDbUserCtx Ctx, IAsyncEnumerable<PoWeightCalculator> Pos, CT Ct) {
		throw new NotImplementedException();
	}
}

public class MockSvcNormLang : ISvcNormLang {
	public Task<object> BatAddNormLang(IDbUserCtx Ctx, IAsyncEnumerable<PoNormLang> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<PoNormLang?> BatGetNormLangByTypeCode(IDbUserCtx Ctx, IAsyncEnumerable<(ELangIdentType, string)> Type_Code, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<string?> BatGetTranslatedName(IDbUserCtx Ctx, INormLang TargetLang, IAsyncEnumerable<INormLang> NormLangs, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<INormLangDetail> BatGetUiLangs(CT CT) {
		throw new NotImplementedException();
	}

	public Task<object> BatSoftDelNormLang(IDbUserCtx Ctx, IAsyncEnumerable<PoNormLang> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatUpdNormLang(IDbUserCtx Ctx, IAsyncEnumerable<PoNormLang> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> InitBuiltinNormLang(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IPageAsyE<PoNormLang>> PageNormLang(IDbUserCtx Ctx, ReqPageNormLang Req, CT Ct) {
		throw new NotImplementedException();
	}
}

public class MockSvcTts : ISvcTts {
	public Task<Audio> GetAudio(string Text, INormLang Lang) {
		throw new NotImplementedException();
	}
}


public class MockAudioPlayer : IAudioPlayer {
	public Task<IPlayState?> Play(Stream S, EAudioType Type, CT Ct) {
		throw new NotImplementedException();
	}
}


public class MockSvcWord : ISvcWord {
	public Task<object> AddEtMergeWords(IUserCtx UserCtx, IEnumerable<IJnWord> JnWords, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> AddWordId_LearnRecordss(IUserCtx UserCtx, IEnumerable<WordId_LearnRecords> WordId_LearnRecordss, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> AddWordsByJsonLineIter(IUserCtx User, IAsyncEnumerable<string> JsonLineIter, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> AddWordsFromFilePath(IUserCtx UserCtx, Path_Encode Path_Encode, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> AddWordsFromText(IUserCtx UserCtx, string Text, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<NgaqTextWithBlob> PackAllWordsToTextWithBlobNoStream(IUserCtx User, ReqPackWords Req, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IPage<IJnWord>> PageChangedWordsWithDelWordsAfterTime(IUserCtx User, IPageQry PageQry, UnixMs Tempus, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IPage<ITypedObj>> PageSearch(IUserCtx User, IPageQry PageQry, ReqSearchWord Req, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IPageAsyE<IJnWord>> PageWord(IUserCtx UserCtx, IPageQry PageQry, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<RespScltWordsOfLearnResultByTimeInterval> ScltAddedWordsByTimeInterval(ReqScltWordsOfLearnResultByTimeInterval Req, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IPage<IJnWord>> SearchWord(IUserCtx User, IPageQry PageQry, ReqSearchWord Req, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> SoftDelJnWordsByIds(IUserCtx User, IEnumerable<IdWord> Ids, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> SyncCompressedWord(IUserCtx User, DtoCompressedWords Dto, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> SyncFromTextWithBlob(IUserCtx User, NgaqTextWithBlob TextWithBlob, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> UpdJnWord(IUserCtx User, IJnWord JnWord, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<DtoCompressedWords> ZipAllWordsJson(IUserCtx User, ReqPackWords ReqPackWords, CT Ct) {
		throw new NotImplementedException();
	}
}


public class MockImgGetter : IImgGetter {
	public IEnumerable<ITypedObj> GetN(ulong n) {
		throw new NotImplementedException();
	}
}


public class MockSvcUserLang : ISvcUserLang {
	public Task<object> AddAllUnregisteredUserLangs(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatAddUserLang(IDbUserCtx Ctx, IAsyncEnumerable<PoUserLang> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<PoUserLang?> BatGetUserLang(IDbUserCtx Ctx, IAsyncEnumerable<string> UniqNames, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<object> BatUpdUserLang(IDbUserCtx Ctx, IAsyncEnumerable<PoUserLang> Pos, CT Ct) {
		throw new NotImplementedException();
	}

	public IAsyncEnumerable<string> GetUnregisteredUserLangs(IDbUserCtx Ctx, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<IPageAsyE<PoUserLang>> PageUserLang(IDbUserCtx Ctx, ReqPageUserLang Req, CT Ct) {
		throw new NotImplementedException();
	}
}

