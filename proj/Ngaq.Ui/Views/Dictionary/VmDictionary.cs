namespace Ngaq.Ui.Views.Dictionary;

using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
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
using Ngaq.Ui.Views.Word.WordManage.NormLangToUserLang.NormLangToUserLangEdit;
using Tsinswreng.CsErr;

using Ctx = VmDictionary;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.CsTools;

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
	ISvcNormLang? SvcNormLang;
	public VmDictionary(
		ISvcDictionary? SvcDictionary
		,IFrontendUserCtxMgr? FrontendUserCtxMgr
		,ISvcWordV2? SvcWordV2
		,ISvcNormLang? SvcNormLang
	){
		this.SvcDictionary = SvcDictionary;
		this.FrontendUserCtxMgr = FrontendUserCtxMgr;
		this.SvcWordV2 = SvcWordV2;
		this.SvcNormLang = SvcNormLang;
	}

	/// 最近一次字典查詢請求。轉換到詞庫單詞時必須一起傳入。
	public IReqLlmDict? LastReqLlmDict{get;set;}
	/// 最近一次字典查詢完整響應。供「保存到詞庫」按鈕使用。
	public IRespLlmDict? LastRespLlmDict{get;set;}
	/// 最近一次 LLM 流式原始輸出文本（用于排查和展示）。
	public str LastLlmRawOutput{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

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
		if(AnyNull(SvcDictionary, FrontendUserCtxMgr, SvcNormLang)){
			return NIL;
		}
		var User = FrontendUserCtxMgr.GetUserCtx();
		// 先快照當前界面數據：若生成失敗則回滾，避免「內容被清空」。
		var PrevResult = CaptureResultSnapshot(Result);
		var PrevReq = LastReqLlmDict;
		var PrevResp = LastRespLlmDict;
		var PrevRawOutput = LastLlmRawOutput;
		var StreamedResp = new StringBuilder();

		Result ??= App.DiOrMk<VmSimpleWord>();
		Result.StartStreaming(Input.Trim());

		var normLang = await SvcNormLang.BatGetNormLangByTypeCode(
			User.ToDbUserCtx()
			,ToolAsyE.ToAsyE([(ELangIdentType.Bcp47, TgtLang)])
			,Ct
		).FirstAsync(Ct);
		IList<NormLangDetail> TgtLangs = [new NormLangDetail{
			Type = ELangIdentType.Bcp47,
			Code = TgtLang,
			NativeName = normLang?.NativeName?? "",
		}];
		var Req = new ReqLlmDictEvt{
			Query = new Query{
				Term = Input.Trim(),
			},
			OptLang = new OptLang{
				SrcLang = new NormLangDetail{
					Type = ELangIdentType.Bcp47,
					Code = SrcLang,
				},
				TgtLangs = TgtLangs,
			},
			OnNewSeg = (dto, ct) => {
				if(dto.NewSeg is not null){
					StreamedResp.Append(dto.NewSeg);
				}
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
			// 先記住流式階段已展示的文本；若最終結構化解析結果缺失描述，則回退到它。
			var StreamedDescription = Result.Description;
			var Resp = await SvcDictionary.Lookup(User, Req, Ct);
			LastLlmRawOutput = StreamedResp.ToString();
			Result.FromRespLlmDict(Resp);
			if(
				str.IsNullOrWhiteSpace(Result.Description)
				&& !str.IsNullOrWhiteSpace(StreamedDescription)
			){
				Result.Description = StreamedDescription;
				LogWarn(
					$"Dictionary lookup finalized with empty description, fallback to streamed text. " +
					$"Input={Input.Trim()}, SrcLang={SrcLang}, TgtLang={TgtLang}"
				);
			}
			LastRespLlmDict = Resp;
		}catch(Exception ex){
			RestoreResultSnapshot(PrevResult);
			LastReqLlmDict = PrevReq;
			LastRespLlmDict = PrevResp;
			LastLlmRawOutput = PrevRawOutput;
			LogError(
				$"Dictionary lookup failed. " +
				$"Input={Input.Trim()}, SrcLang={SrcLang}, TgtLang={TgtLang}, " +
				$"LlmResponse={StreamedResp}"
			);
			HandleErr(ex);
		}

		return NIL;
	}

	/// 用編輯後的原始文本重新解析詞典結果，不重調 LLM。
	public Task<nil> ReparseFromRawOutput(str RawOutput, CT Ct){
		if(str.IsNullOrWhiteSpace(RawOutput)){
			ShowDialog(Todo.I18n("原始輸出為空，無法解析"));
			return Task.FromResult<nil>(NIL);
		}
		if(AnyNull(SvcDictionary, FrontendUserCtxMgr)){
			return Task.FromResult<nil>(NIL);
		}
		var PrevResult = CaptureResultSnapshot(Result);
		var PrevReq = LastReqLlmDict;
		var PrevResp = LastRespLlmDict;
		var PrevRawOutput = LastLlmRawOutput;
		try{
			var Req = new ReqLlmDictEvt{
				Query = new Query{
					Term = Input.Trim(),
				},
				OptLang = new OptLang{
					SrcLang = new NormLangDetail{
						Type = ELangIdentType.Bcp47,
						Code = SrcLang,
					},
					TgtLangs = [new NormLangDetail{
						Type = ELangIdentType.Bcp47,
						Code = TgtLang,
					}],
				},
			};
			var Resp = SvcDictionary.ParseRawOutput(RawOutput);
			Result ??= App.DiOrMk<VmSimpleWord>();
			Result.FromRespLlmDict(Resp);
			if(str.IsNullOrWhiteSpace(Result.Description)){
				Result.Description = RawOutput;
			}
			LastReqLlmDict = Req;
			LastRespLlmDict = Resp;
			LastLlmRawOutput = RawOutput;
		}catch(Exception ex){
			RestoreResultSnapshot(PrevResult);
			LastReqLlmDict = PrevReq;
			LastRespLlmDict = PrevResp;
			LastLlmRawOutput = PrevRawOutput;
			LogError($"Dictionary reparse from raw output failed. RawOutput={RawOutput}");
			HandleErr(ex);
		}
		return Task.FromResult<nil>(NIL);
	}

	/// 查詢前快照：失敗時恢復到此狀態。
	LookupResultSnapshot CaptureResultSnapshot(VmSimpleWord? Cur){
		if(Cur is null){
			return new LookupResultSnapshot();
		}
		return new LookupResultSnapshot{
			Head = Cur.Head,
			Description = Cur.Description,
			Pronunciations = Cur.Pronunciations.Select(p=>new Pronunciation{
				TextType = p.TextType,
				Text = p.Text,
			}).ToList(),
		};
	}

	/// 查詢失敗回滾 UI 文本。
	nil RestoreResultSnapshot(LookupResultSnapshot Snapshot){
		Result ??= App.DiOrMk<VmSimpleWord>();
		Result.Head = Snapshot.Head;
		Result.Description = Snapshot.Description;
		Result.Pronunciations = Snapshot.Pronunciations;
		return NIL;
	}

	/// Lookup 失敗回滾用 DTO。
	sealed class LookupResultSnapshot{
		public str Head{get;set;} = "";
		public str Description{get;set;} = "";
		public IList<Pronunciation> Pronunciations{get;set;} = [];
	}

	/// 將最近一次詞典查詢結果轉為詞庫詞條，然後跳到單詞編輯頁。
	/// 注意: 此函數本身不落庫；最終保存由 ViewWordEditV2 的 Save 按鈕執行。
	public async Task<nil> ToWordEdit(CT Ct){
		if(AnyNull(SvcWordV2, FrontendUserCtxMgr, LastReqLlmDict, LastRespLlmDict)){
			ShowToast(Todo.I18n("請先完成一次詞典查詢，再嘗試保存到詞庫"));
			return NIL;
		}
		var Req = LastReqLlmDict!;
		var Resp = LastRespLlmDict!;
		try{
			// step 1: 先調用 ISvcWordV2 的轉換函數（按需求固定順序）。
			var JnWord = await SvcWordV2.LlmDictWordToJnWordWithLearn(
				FrontendUserCtxMgr.GetDbUserCtx(),
				Req, Resp, Ct
			);
			// step 2: 轉換成功後再跳轉到編輯頁。
			GoToWordEditPage(JnWord);
		}catch(Exception Ex){
			// step 3: 捕獲特定語言映射異常，提供兩個選項。
			if(!IsNormLangMappingErr(Ex)){
				HandleErr(Ex);
			}
			var BtnGoCfg = new Button();
			BtnGoCfg.SetContent(Todo.I18n("轉到語言配置頁"));
			BtnGoCfg.Click += (s,e)=>{
				_ = OpenNormLangMappingPage();
			};
			var BtnSkipCfg = new Button();
			BtnSkipCfg.SetContent(Todo.I18n("暫不配置，直接轉到編輯頁"));
			BtnSkipCfg.Click += (s,e)=>{
				_ = GoToWordEditPage(BuildFallbackJnWord(Req, Resp));
			};
			ShowDialog(
				Todo.I18n(
					"未設定轉換語言映射。\n"+
					"請選擇後續操作："
				),
				[
					BtnGoCfg,
					BtnSkipCfg,
				]
			);
			return NIL;
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
		// 直接進入新增映射頁，並預填當前源語言，減少用戶操作步驟。
		var View = new ViewNormLangToUserLangEdit();
		if(View.Ctx is not null){
			View.Ctx.SetCreateMode(true);
			View.Ctx.FromPoNormLangToUserLang(null);
			View.Ctx.PoNormLang = SrcLang;
		}
		ViewNavi?.GoTo(ToolView.WithTitle(Todo.I18n("Add NormLangToUserLang"), View));
		return NIL;
	}

	/// 跳到 WordEditV2，由用戶在編輯頁手動點 Save 才真正保存。
	/// TODO 違反MVVM
	obj? GoToWordEditPage(JnWord JnWord){
		var View = new ViewWordEditV2();
		if(View.Ctx is null){
			ShowDialog(Todo.I18n("Word editor context is null"));
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
