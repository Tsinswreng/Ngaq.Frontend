namespace Ngaq.Ui.Views.Word.WordManage.WordSyncV2;

using System.IO;
using Ngaq.Client.Word.Svc;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Infra.Cfg;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Core.Tools;
using Ngaq.Ui.Infra;
using Tsinswreng.CsCfg;
using Tsinswreng.CsTools;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// <summary>
/// 單詞同步 V2 頁面的 ViewModel。
/// </summary>
public partial class VmWordSyncV2: ViewModelBase{
	ClientWordSyncV2? ClientWordSyncV2;
	ISvcWordV2? SvcWordV2;
	IFrontendUserCtxMgr? UserCtxMgr;
	ICfgAccessor? Cfg;

	/// <summary>
	/// 構造函數（依賴注入）。
	/// </summary>
	/// <param name="ClientWordSyncV2">雲端同步客戶端。</param>
	/// <param name="SvcWordV2">本地單詞 V2 服務。</param>
	/// <param name="UserCtxMgr">前端用戶上下文管理器。</param>
	/// <param name="Cfg">配置訪問器。</param>
	public VmWordSyncV2(
		ClientWordSyncV2? ClientWordSyncV2
		,ISvcWordV2? SvcWordV2
		,IFrontendUserCtxMgr? UserCtxMgr
		,ICfgAccessor? Cfg
	){
		this.ClientWordSyncV2 = ClientWordSyncV2;
		this.SvcWordV2 = SvcWordV2;
		this.UserCtxMgr = UserCtxMgr;
		this.Cfg = Cfg;

		if(Cfg is not null){
			PathExport = Cfg.Get(KeysClientCfg.Word.WordsPackExportPath)??"";
			PathImport = Cfg.Get(KeysClientCfg.Word.WordsPackImportPath)??"";
		}
	}

	/// <summary>
	/// 供設計期使用的保底構造。
	/// </summary>
	protected VmWordSyncV2(){}

	/// <summary>
	/// 導出文件路徑。
	/// </summary>
	public str PathExport{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	/// <summary>
	/// 導入文件路徑。
	/// </summary>
	public str PathImport{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	/// <summary>
	/// 上傳本地詞庫到雲端。
	/// </summary>
	/// <param name="Ct">取消令牌。</param>
	/// <returns>成功返回 NIL。</returns>
	public async Task<nil> PushAsy(CT Ct){
		if(ClientWordSyncV2 is null){
			return NIL;
		}
		try{
			await ClientWordSyncV2.Push(Ct);
		}catch(Exception Ex){
			HandleErr(Ex);
		}
		return NIL;
	}

	/// <summary>
	/// 從雲端拉取詞庫到本地。
	/// </summary>
	/// <param name="Ct">取消令牌。</param>
	/// <returns>成功返回 NIL。</returns>
	public async Task<nil> PullAsy(CT Ct){
		if(ClientWordSyncV2 is null){
			return NIL;
		}
		try{
			await ClientWordSyncV2.Pull(Ct);
		}catch(Exception Ex){
			HandleErr(Ex);
		}
		return NIL;
	}

	/// <summary>
	/// 導出本地詞庫到文件（含軟刪）。
	/// </summary>
	/// <param name="Ct">取消令牌。</param>
	/// <returns>成功返回 NIL。</returns>
	public async Task<nil> ExportAsy(CT Ct){
		if(str.IsNullOrWhiteSpace(PathExport)){
			ShowDialog(I18n[K.InvalidPath]);
			return NIL;
		}
		if(File.Exists(PathExport)){
			ShowDialog(I18n[K.FileAlreadyExistsNoOverwriteChangePath]);
			return NIL;
		}
		if(AnyNull(SvcWordV2, UserCtxMgr)){
			return NIL;
		}
		try{
			// 先確保目標文件可建立，避免導出後才發現路徑不合法。
			ToolFile.EnsureFile(PathExport);
			if(!File.Exists(PathExport)){
				ShowDialog(I18n[K.InvalidPath]);
				return NIL;
			}

			using var packed = await SvcWordV2.PackAllWordsWithDel(UserCtxMgr.GetDbUserCtx(), Ct);
			using var file = File.Open(PathExport, FileMode.Create, FileAccess.Write, FileShare.None);
			await packed.CopyToAsync(file, Ct);

			Cfg?.Set(KeysClientCfg.Word.WordsPackExportPath, PathExport);
			Cfg?.Save(default);
		}catch(Exception Ex){
			HandleErr(Ex);
		}
		return NIL;
	}

	/// <summary>
	/// 從文件導入詞庫並按 BizId 規則同步到本地。
	/// </summary>
	/// <param name="Ct">取消令牌。</param>
	/// <returns>成功返回 NIL。</returns>
	public async Task<nil> ImportAsy(CT Ct){
		if(str.IsNullOrWhiteSpace(PathImport) || !File.Exists(PathImport)){
			ShowDialog(I18n[K.InvalidPath]);
			return NIL;
		}
		if(AnyNull(SvcWordV2, UserCtxMgr)){
			return NIL;
		}
		try{
			using var file = File.OpenRead(PathImport);
			await foreach(var _ in SvcWordV2.BatSyncJnWordByBizIdFromStream(
				UserCtxMgr.GetDbUserCtx(),
				file,
				Ct
			)){
				// 消費枚舉以觸發實際同步。
			}

			Cfg?.Set(KeysClientCfg.Word.WordsPackImportPath, PathImport);
			Cfg?.Save(default);
		}catch(Exception Ex){
			HandleErr(Ex);
		}
		return NIL;
	}
}
