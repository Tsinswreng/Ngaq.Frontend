namespace Ngaq.Windows;

using Ngaq.Core.Shared.Kv.Models;
using Ngaq.Core.Shared.User.Models.Po.User;
using Ngaq.Core.Shared.User.UserCtx;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Frontend.Kv;
using Ngaq.Local.Sql;
using Ngaq.Ui;
using Ngaq.Core.Shared.Kv.Svc;
using Ngaq.Core.Frontend.User;

public class AppIniter{
	protected static AppIniter? _Inst = null;
	public static AppIniter Inst => _Inst??= new AppIniter();

	public async Task<nil> Init(CT Ct){
		await InitDbSchema(Ct);
		await InitUserCtx(Ct);
		return NIL;
	}


	//TODO 初始化ClientId
	public async Task<nil> InitUserCtx(CT Ct){
		var userCtxMgr = App.GetSvc<IFrontendUserCtxMgr>();
		var SvcKv = App.GetSvc<ISvcKv>();


		var CurLocalUserKv = await SvcKv.GetByOwnerEtKeyAsy(
			IdUser.Zero
			,KeysClientKv.CurLocalUserId
			,Ct
		);

		if(CurLocalUserKv is not null){
			userCtxMgr.GetUserCtx().UserId = IdUser.FromLow64Base(
				CurLocalUserKv.VStr??throw new InvalidOperationException("Invalid User Id")
			);
		}else{
			var kv = new PoKv();
			var UserId = new IdUser();
			userCtxMgr.GetUserCtx().UserId = UserId;
			kv.SetStrStr(KeysClientKv.CurLocalUserId, userCtxMgr.GetUserCtx().UserId.ToString());
			await SvcKv.SetAsy(
				kv, Ct
			);
		}

		return NIL;
	}

	public async Task<nil> InitDbSchema(CT Ct){
		var DbIniter = App.GetSvc<DbIniter>();
		_ = DbIniter.Init(Ct).Result;
		return NIL;
	}
}
