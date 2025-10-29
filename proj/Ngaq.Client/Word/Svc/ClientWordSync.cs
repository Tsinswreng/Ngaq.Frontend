namespace Ngaq.Client.Word.Svc;

using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra.Url;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Word.Svc;

public class ClientWordSync{
	IHttpCaller HttpCaller;
	IFrontendUserCtxMgr UserCtxMgr;
	ISvcWord SvcWord;
	public ClientWordSync(
		IHttpCaller HttpCaller
		,IFrontendUserCtxMgr UserCtxMgr
		,ISvcWord SvcUser
	){
		this.HttpCaller = HttpCaller;
		this.UserCtxMgr = UserCtxMgr;
		this.SvcWord = SvcUser;
	}
	public async Task<nil> AllWordsToServerNonStream(CT Ct){
		var User = UserCtxMgr.GetUserCtx();
		var DtoCompressedAllWord = await SvcWord.ZipAllWordsJson(User, Ct);
		await HttpCaller.Post<DtoCompressedWords, nil>(
			ConstUrl.UrlWord.Push
			,DtoCompressedAllWord, Ct
		);
		return NIL;
	}
}
