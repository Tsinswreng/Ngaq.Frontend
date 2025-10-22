using Ngaq.Core.Shared.User.UserCtx;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Infra;
using Ngaq.Core.Model.Po.Word;
using Ngaq.Core.Model.Word.Req;
using Ngaq.Core.Stream;
using Ngaq.Core.Tools.Io;
using Ngaq.Core.Word.Models.Dto;
using Ngaq.Core.Word.Svc;
using Tsinswreng.CsCore;
using Tsinswreng.CsPage;
using Tsinswreng.CsTools;

namespace Ngaq.Client.Word.Svc;


public partial class WebClientWord : ISvcWord {
	[Impl]
	public Task<nil> AddWordId_LearnRecordss(IUserCtx UserCtx, IEnumerable<WordId_LearnRecords> WordId_LearnRecordss, CT Ct) {
		throw new NotImplementedException();
	}

	[Impl]
	public Task<nil> AddWordId_PoLearnss(IUserCtx UserCtx, IEnumerable<WordId_PoLearns> WordId_PoLearnss, CT Ct) {
		throw new NotImplementedException();
	}

	[Impl]
	public Task<nil> AddWordsFromFilePath(IUserCtx UserCtx, Path_Encode Path_Encode, CT ct) {
		throw new NotImplementedException();
	}

	[Impl]
	public Task<nil> AddWordsFromText(IUserCtx UserCtx, string Text, CT ct) {
		throw new NotImplementedException();
	}

	[Impl]
	public Task<IPage<JnWord>> PageJnWord(IUserCtx UserCtx, IPageQry PageQry, CT Ct) {
		throw new NotImplementedException();
	}

	[Impl]
	public Task<nil> AddJnWords(
		IUserCtx UserCtx
		,IEnumerable<JnWord> JnWords
		,CT Ct
	){
		throw new NotImplementedException();
	}

	[Impl]
	public Task<nil> AddWordsByJsonLineIter(
		IUserCtx User
		,IAsyncEnumerable<str> JsonLineIter
		,CT Ct
	){
		throw new NotImplementedException();
	}

	public async Task<IPage<JnWord>> SearchWord(
		IUserCtx User
		,IPageQry PageQry
		,ReqSearchWord Req
		,CT Ct
	){
		throw new NotImplementedException();
	}

	public async Task<nil> SoftDelJnWordsByIds(
		IUserCtx User
		,IEnumerable<IdWord> Ids
		,CT Ct
	){
		throw new NotImplementedException();
	}

	public async Task<nil> UpdJnWord(IUserCtx User, JnWord JnWord, CT Ct){
		throw new NotImplementedException();
	}

	public async Task<IPage<JnWord>> PageChangedWordsWithDelWordsAfterTime(
		IUserCtx User
		,IPageQry PageQry
		,Tempus Tempus
		,CT Ct
	){
		throw new NotImplementedException();
	}

	public Task<IPage<ITypedObj>> PageSearch(
		IUserCtx User
		,IPageQry PageQry
		,ReqSearchWord Req
		,CT Ct
	){
		throw new NotImplementedException();
	}

}
