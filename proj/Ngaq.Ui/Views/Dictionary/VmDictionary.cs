namespace Ngaq.Ui.Views.Dictionary;

using System.Collections.ObjectModel;
using System.Collections.Generic;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Infra.Errors;
using Ngaq.Core.Model.Po.Kv;
using Ngaq.Core.Shared.Dictionary.Models;
using Ngaq.Core.Shared.Dictionary.Models.Po.NormLang;
using Ngaq.Core.Shared.Dictionary.Svc;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Dictionary.SimpleWord;
using Ngaq.Ui.Views.Word.WordEditV2;
using Ngaq.Ui.Views.Word.WordManage.NormLangToUserLang.NormLangToUserLangPage;
using Tsinswreng.CsErr;

using Ctx = VmDictionary;

public partial class VmDictionary: ViewModelBase, IMk<Ctx>{
	protected VmDictionary(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmDictionary(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	IFrontendUserCtxMgr? FrontendUserCtxMgr;
	ISvcDictionary? SvcDictionary;
	ISvcWordV2? SvcWordV2;
	public VmDictionary(
		ISvcDictionary? SvcDictionary
		,IFrontendUserCtxMgr? FrontendUserCtxMgr
		,ISvcWordV2? SvcWordV2
	){
		this.SvcDictionary = SvcDictionary;
		this.FrontendUserCtxMgr = FrontendUserCtxMgr;
		this.SvcWordV2 = SvcWordV2;
	}

	/// 最近一次字典查詢請求。轉換到詞庫單詞時必須一起傳入。
	public IReqLlmDict? LastReqLlmDict{get;set;}
	/// 最近一次字典查詢完整響應。供「保存到詞庫」按鈕使用。
	public IRespLlmDict? LastRespLlmDict{get;set;}

	public str Input{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	public VmSimpleWord? Result{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = App.DiOrMk<VmSimpleWord>();

	public str SrcLang{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "en";

	public str TgtLang{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "zh";

	public nil SwapLang(){
		var tmp = SrcLang;
		SrcLang = TgtLang;
		TgtLang = tmp;
		_ = PersistCurLangs(default);
		return NIL;
	}

	public nil ApplySrcNormLang(PoNormLang Po){
		SrcLang = Po.Code ?? "";
		_ = PersistSrcLang(Po, default);
		return NIL;
	}

	public nil ApplyTgtNormLang(PoNormLang Po){
		TgtLang = Po.Code ?? "";
		_ = PersistTgtLang(Po, default);
		return NIL;
	}

	public async Task<nil> InitLang(CT Ct){
		if(AnyNull(SvcDictionary, FrontendUserCtxMgr)){
			return NIL;
		}
		try{
			var dbCtx = FrontendUserCtxMgr.GetDbUserCtx();
			var src = await SvcDictionary.GetCurSrcNormLang(dbCtx, Ct);
			var tgt = await SvcDictionary.GetCurTgtNormLang(dbCtx, Ct);
			if(src is not null && !str.IsNullOrWhiteSpace(src.Code)){
				SrcLang = src.Code;
			}
			if(tgt is not null && !str.IsNullOrWhiteSpace(tgt.Code)){
				TgtLang = tgt.Code;
			}
		}catch(Exception ex){
			HandleErr(ex);
		}
		return NIL;
	}

	public async Task<nil> Lookup(CT Ct){
		if(string.IsNullOrWhiteSpace(Input)){
			return NIL;
		}
		if(AnyNull(SvcDictionary, FrontendUserCtxMgr)){
			return NIL;
		}
		var User = FrontendUserCtxMgr.GetUserCtx();

		Result ??= App.DiOrMk<VmSimpleWord>();
		Result.StartStreaming(Input.Trim());

		IList<NormLangWithName> TgtLangs = [new NormLangWithName{
			Type = ELangIdentType.Bcp47,
			Code = TgtLang,
		}];

		var Req = new ReqLlmDictEvt{
			Query = new Query{
				Term = Input.Trim(),
			},
			OptLang = new OptLang{
				SrcLang = new NormLangWithName{
					Type = ELangIdentType.Bcp47,
					Code = SrcLang,
				},
				TgtLangs = TgtLangs,
			},
			OnNewSeg = (dto, ct) => {
				Result.GotNewSeg(dto);
				return 0;
			},
			OnDone = (dto, ct) => {
				return 0;
			},
		};
		// 緩存最近一次查詢上下文，便於後續轉換為 JnWord。
		LastReqLlmDict = Req;
		LastRespLlmDict = null;

		try{
			var Resp = await SvcDictionary.Lookup(User, Req, Ct);
			Result.FromRespLlmDict(Resp);
			LastRespLlmDict = Resp;
		}catch(Exception ex){
			HandleErr(ex);
		}

		return NIL;
	}

	/// 將最近一次詞典查詢結果轉為詞庫詞條，然後跳到單詞編輯頁。
	/// 注意: 此函數本身不落庫；最終保存由 ViewWordEditV2 的 Save 按鈕執行。
	public async Task<nil> ToWordEdit(CT Ct){
		if(AnyNull(SvcWordV2, FrontendUserCtxMgr, LastReqLlmDict, LastRespLlmDict)){
			ShowMsg(Todo.I18n("請先完成一次詞典查詢，再嘗試保存到詞庫"));
			return NIL;
		}
		var Req = LastReqLlmDict!;
		var Resp = LastRespLlmDict!;
		try{
			// step 1: 先調用 ISvcWordV2 的轉換函數（按需求固定順序）。
			var JnWord = await SvcWordV2.LlmDictWordToJnWord(
				FrontendUserCtxMgr.GetDbUserCtx(),
				Req, Resp, Ct
			);
			// step 2: 轉換成功後再跳轉到編輯頁。
			GoToWordEditPage(JnWord);
		}catch(Exception Ex){
			// step 3: 捕獲特定語言映射異常，提供兩個選項。
			if(IsNormLangMappingErr(Ex)){
				ShowMsg(
					Todo.I18n(
						"未設定轉換語言映射。\n"+
						"Option 1: 轉到語言配置頁\n"+
						"Option 2: 暫不配置，直接進入編輯頁"
					),
					[
						()=>OpenNormLangMappingPage(),
						()=>GoToWordEditPage(BuildFallbackJnWord(Req, Resp)),
					]
				);
				return NIL;
			}
			HandleErr(Ex);
		}
		return NIL;
	}

	/// 判斷是否爲「NormLangToUserLang 未映射」的指定業務異常。
	bool IsNormLangMappingErr(Exception Ex){
		if(Ex is IAppErr AppErr){
			return ReferenceEquals(AppErr.Type, ItemsErr.Word.NormLangToUserLangIsNotMapped);
		}
		return false;
	}

	/// 打開語言映射配置頁。
	obj? OpenNormLangMappingPage(){
		var View = new ViewNormLangToUserLangPage();
		ViewNavi?.GoTo(ToolView.WithTitle(Todo.I18n("配置語言映射"), View));
		return NIL;
	}

	/// 跳到 WordEditV2，由用戶在編輯頁手動點 Save 才真正保存。
	obj? GoToWordEditPage(JnWord JnWord){
		var View = new ViewWordEditV2();
		if(View.Ctx is null){
			ShowMsg(Todo.I18n("Word editor context is null"));
			return NIL;
		}
		View.Ctx.FromJnWord(JnWord);
		var Title = str.IsNullOrWhiteSpace(JnWord.Word.Head)
			? Todo.I18n("Word Edit")
			: JnWord.Word.Head
		;
		ViewNavi?.GoTo(ToolView.WithTitle(Title, View));
		return NIL;
	}

	/// 語言未映射時的兜底詞條構造：保留查詢頭詞、源語和描述/讀音，供用戶在編輯頁手動確認。
	JnWord BuildFallbackJnWord(IReqLlmDict Req, IRespLlmDict Resp){
		var Head = !str.IsNullOrWhiteSpace(Resp.Head)
			? Resp.Head
			: Req.Query.Term
		;
		var Lang = Req.OptLang.SrcLang.Code ?? "";
		var Props = new List<PoWordProp>();

		// 把詞典描述映射成 description 屬性，後續可在編輯頁繼續調整。
		foreach(var Descr in Resp.Descrs){
			if(str.IsNullOrWhiteSpace(Descr)){
				continue;
			}
			Props.Add(new PoWordProp{
				KType = EKvType.Str,
				KStr = KeysProp.Inst.description,
				VType = EKvType.Str,
				VStr = Descr,
			});
		}

		// 把讀音也保留爲 pronunciation 屬性，避免查詢信息丟失。
		foreach(var Pron in Resp.Pronunciations){
			var Value = str.IsNullOrWhiteSpace(Pron.TextType)
				? Pron.Text
				: $"{Pron.TextType}: {Pron.Text}"
			;
			if(str.IsNullOrWhiteSpace(Value)){
				continue;
			}
			Props.Add(new PoWordProp{
				KType = EKvType.Str,
				KStr = KeysProp.Inst.pronunciation,
				VType = EKvType.Str,
				VStr = Value,
			});
		}

		var R = new JnWord{
			Word = new PoWord{
				Head = Head ?? "",
				Lang = Lang,
				StoredAt = Tempus.Now(),
			},
			Props = Props,
			Learns = [],
		};
		R.EnsureForeignId();
		return R;
	}

	async Task<nil> PersistCurLangs(CT Ct){
		if(AnyNull(SvcDictionary, FrontendUserCtxMgr)){
			return NIL;
		}
		try{
			var dbCtx = FrontendUserCtxMgr.GetDbUserCtx();
			var src = new PoNormLang{
				Type = ELangIdentType.Bcp47,
				Code = SrcLang,
			};
			var tgt = new PoNormLang{
				Type = ELangIdentType.Bcp47,
				Code = TgtLang,
			};
			await SvcDictionary.SetCurSrcNormLang(dbCtx, src, Ct);
			await SvcDictionary.SetCurTgtNormLang(dbCtx, tgt, Ct);
		}catch(Exception ex){
			HandleErr(ex);
		}
		return NIL;
	}

	async Task<nil> PersistSrcLang(PoNormLang Po, CT Ct){
		if(AnyNull(SvcDictionary, FrontendUserCtxMgr)){
			return NIL;
		}
		try{
			await SvcDictionary.SetCurSrcNormLang(FrontendUserCtxMgr.GetDbUserCtx(), Po, Ct);
		}catch(Exception ex){
			HandleErr(ex);
		}
		return NIL;
	}

	async Task<nil> PersistTgtLang(PoNormLang Po, CT Ct){
		if(AnyNull(SvcDictionary, FrontendUserCtxMgr)){
			return NIL;
		}
		try{
			await SvcDictionary.SetCurTgtNormLang(FrontendUserCtxMgr.GetDbUserCtx(), Po, Ct);
		}catch(Exception ex){
			HandleErr(ex);
		}
		return NIL;
	}
}
