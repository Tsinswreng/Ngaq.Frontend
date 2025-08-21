using Ngaq.Core.Model.Word.Req;
using Ngaq.Core.Models.UserCtx;
using Ngaq.Core.Tools.Io;
using Ngaq.Core.Word.Models;
using Ngaq.Core.Word.Svc;
using Tsinswreng.CsPage;

namespace Ngaq.Client.Word.Svc;

public  partial class ClientWord : ISvcWord {
	public Task<nil> AddWordId_LearnRecordss(IUserCtx UserCtx, IEnumerable<WordId_LearnRecords> WordId_LearnRecordss, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> AddWordId_PoLearnss(IUserCtx UserCtx, IEnumerable<WordId_PoLearns> WordId_PoLearnss, CT Ct) {
		throw new NotImplementedException();
	}

	public Task<nil> AddWordsFromFilePath(IUserCtx UserCtx, Path_Encode Path_Encode, CT ct) {
		throw new NotImplementedException();
	}

	public Task<nil> AddWordsFromText(IUserCtx UserCtx, string Text, CT ct) {
		throw new NotImplementedException();
	}

	public Task<IPageAsy<JnWord>> PageJnWord(IUserCtx UserCtx, IPageQry PageQry, CT Ct) {
		throw new NotImplementedException();
	}
}
