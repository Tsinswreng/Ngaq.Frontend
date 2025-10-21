namespace Ngaq.Windows;

using Ngaq.Core.Domains.Kv.Models;
using Ngaq.Core.Domains.User.Models.Po.User;
using Ngaq.Core.Domains.User.Svc;
using Ngaq.Core.Domains.User.UserCtx;
using Ngaq.Core.Domains.Word.Models.Po.Kv;
using Ngaq.Local.Sql;
using Ngaq.Ui;




public class AppIniter{
	protected static AppIniter? _Inst = null;
	public static AppIniter Inst => _Inst??= new AppIniter();

	public async Task<nil> Init(CT Ct){
		await InitDbSchema(Ct);
		await InitUserCtx(Ct);
		return NIL;
	}

	public async Task<nil> InitUserCtx(CT Ct){
		var _userCtxMgr = App.GetSvc<IUserCtxMgr>();
		var SvcKv = App.GetSvc<ISvcKv>();
		if(_userCtxMgr is not UserCtxMgr userCtxMgr){
			throw new NotImplementedException("");
		}

		var CurLocalUserKv = await SvcKv.GetByOwnerEtKeyAsy(
			IdUser.Zero
			,KeysClientKv.CurLocalUserId
			,Ct
		);

		if(CurLocalUserKv is not null){
			userCtxMgr.UserCtx.UserId = IdUser.FromLow64Base(
				CurLocalUserKv.VStr??throw new InvalidOperationException("Invalid User Id")
			);
		}else{
			var kv = new PoKv();
			kv.SetStr(KeysClientKv.CurLocalUserId, userCtxMgr.GetUserCtx().UserId.ToString());
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
