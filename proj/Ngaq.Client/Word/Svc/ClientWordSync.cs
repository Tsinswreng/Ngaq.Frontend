namespace Ngaq.Client.Word.Svc;

using System.Text;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra.Url;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Tools;
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
		var Req = new ReqPackWords{
			Type = EWordsPack.LineSepJnWordJsonGZip
		};
		var textWithBlob = await SvcWord.PackAllWordsToTextWithBlobNoStream(User, Req, Ct);

		await HttpCaller.PostByteStream<DtoCompressedWords, nil>(
			ConstUrl.UrlWord.Push
			,textWithBlob.ToByteArr(), Ct
		);

		//await HttpCaller.PostBlob<nil>();
		return NIL;
	}

	public async Task<nil> SaveAllWordsFromServerNonStream(CT Ct){
		var User = UserCtxMgr.GetUserCtx();
		var Req = new ReqPackWords{
			Type = EWordsPack.LineSepJnWordJsonGZip
		};
		using var resp = await HttpCaller.SendWithRetry(
			ConstUrl.UrlWord.Pull
			,JSON.stringify(Req)
			,(json)=>new StringContent(
				json
				,Encoding.UTF8
				,"application/json"
			)
			,Ct
		);
		var bytes = await resp.Content.ReadAsByteArrayAsync(Ct);//t
		var textWithBlob = ToolTextWithBlob.Parse(bytes);
		await SvcWord.AddFromTextWithBlob(User, textWithBlob, Ct);
		return NIL;
	}
}
