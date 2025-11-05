namespace Ngaq.Ui.Views.Word.WordManage.WordSync;
using System.Collections.ObjectModel;
using Ngaq.Client.Svc;
using Ngaq.Client.Word.Svc;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra.Cfg;
using Ngaq.Core.Shared.Kv.Svc;
using Ngaq.Core.Shared.User.Svc;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Tools;
using Ngaq.Core.Word.Svc;
using Ngaq.Ui.Infra;
using Tsinswreng.CsCfg;
using Tsinswreng.CsTools;
using Ctx = VmWordSync;
public partial class VmWordSync: ViewModelBase{
	ISvcKv? SvcKv;
	ClientWordSync? ClientWordSync;
	ISvcWord? SvcWord;
	IFrontendUserCtxMgr? UserCtxMgr;
	ICfgAccessor? Cfg;
	public VmWordSync(
		ISvcKv? SvcKv
		,ClientWordSync? ClientWordSync
		,ISvcWord? SvcWord
		,IFrontendUserCtxMgr? UserCtxMgr
		,ICfgAccessor? Cfg
	){
		this.SvcKv = SvcKv;
		this.ClientWordSync = ClientWordSync;
		this.SvcWord = SvcWord;
		this.UserCtxMgr = UserCtxMgr;
		this.Cfg = Cfg;

		if(Cfg is not null){
			PathExport = Cfg.Get(ItemsClientCfg.Word.WordsPackExportPath)??"";
			PathImport = Cfg.Get(ItemsClientCfg.Word.WordsPackImportPath)??"";
		}

	}

	protected VmWordSync(){}
	public static Ctx Mk(){
		return new Ctx();
	}
	public static ObservableCollection<Ctx> Samples = [];
	static VmWordSync(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}


	public CancellationTokenSource Cts = new();
	public async Task<nil> PushAsy(CT Ct=default){
		if(ClientWordSync is null){
			return NIL;
		}
		await ClientWordSync.AllWordsToServerNonStream(Ct);
		return NIL;
	}

	public nil Push(){
		PushAsy(Cts.Token).ContinueWith(t=>{
			HandleErr(t);
		});
		return NIL;
	}

	public async Task<nil> PullAsy(CT Ct){
		if(ClientWordSync is null){
			return NIL;
		}
		await ClientWordSync.SaveAllWordsFromServerNonStream(Ct);
		return NIL;
	}

	public nil Pull(){
		PullAsy(Cts.Token).ContinueWith(t=>{
			HandleErr(t);
		});
		return NIL;
	}

	protected str _PathExport = "";
	public str PathExport{
		get{return _PathExport;}
		set{
			SetProperty(ref _PathExport, value);
		}
	}

	protected str _PathImport = "";
	public str PathImport{
		get{return _PathImport;}
		set{
			SetProperty(ref _PathImport, value);
		}
	}

	public async Task<nil> ExportAsy(CT Ct=default){
		await Task.Run(async()=>{
			if(SvcWord is null
				|| UserCtxMgr is null
			){
				return;
			}
			var User = UserCtxMgr.GetUserCtx();
			var textWithBlob = await SvcWord.PackAllWordsToTextWithBlobNoStream(
				User,
				new ReqPackWords{Type = EWordsPack.LineSepJnWordJsonGZip}
				,Ct
			);
			var bytes = textWithBlob.ToByteArr();
			ToolFile.EnsureFile(PathExport);
			await File.WriteAllBytesAsync(PathExport, bytes, Ct);
			Cfg?.Set(ItemsClientCfg.Word.WordsPackExportPath, PathExport);
			Cfg?.SaveAsy(default);
		});
		return NIL;
	}

	public async Task<nil> ImportAsy(CT Ct=default){
		await Task.Run(async()=>{
			if(SvcWord is null || UserCtxMgr is null){
				return;
			}
			var bytes = await File.ReadAllBytesAsync(PathImport, Ct);
			var textWithBlob = ToolTextWithBlob.Parse(bytes);
			await SvcWord.SyncFromTextWithBlob(UserCtxMgr.GetUserCtx(), textWithBlob, Ct);
			Cfg?.Set(ItemsClientCfg.Word.WordsPackImportPath, PathImport);
			Cfg?.SaveAsy(default);
		});
		return NIL;
	}

}
