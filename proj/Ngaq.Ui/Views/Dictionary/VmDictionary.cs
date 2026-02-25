namespace Ngaq.Ui.Views.Dictionary;
using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Shared.Dictionary.Models;
using Ngaq.Core.Shared.Dictionary.Svc;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Dictionary.SimpleWord;
using System.Collections.Specialized;

using Ctx = VmDictionary;
public partial class VmDictionary: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
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
	public VmDictionary(
		ISvcDictionary? SvcDictionary
		,IFrontendUserCtxMgr? FrontendUserCtxMgr
	){
		this.SvcDictionary = SvcDictionary;
		this.FrontendUserCtxMgr = FrontendUserCtxMgr;
	}


	public str Input{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	/// <summary>
	/// 查詢結果
	/// </summary>
	public VmSimpleWord? Result{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = App.DiOrMk<VmSimpleWord>();

	#region 語言選擇

	/// <summary>
	/// 可選的源語言列表
	/// </summary>
	public IList<LanguageOption> AvailableSrcLanguages{
		get;
	} = LanguageOptions.SourceLanguages;

	/// <summary>
	/// 當前選中的源語言
	/// </summary>
	public LanguageOption? SelectedSrcLanguage{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				UpdateTargetLanguages();
			}
		}
	} = LanguageOptions.DefaultSourceLanguage;

	/// <summary>
	/// 可選的目標語言列表
	/// </summary>
	public IList<LanguageOption> AvailableTgtLanguages{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = LanguageOptions.GetTargetLanguages(LanguageOptions.DefaultSourceLanguage);

	/// <summary>
	/// 當前選中的目標語言
	/// </summary>
	public LanguageOption? SelectedTgtLanguage{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = LanguageOptions.DefaultTargetLanguage;

	/// <summary>
	/// 更新目標語言列表（當源語言改變時調用）
	/// </summary>
	private nil UpdateTargetLanguages(){
		var newTgtLangs = LanguageOptions.GetTargetLanguages(SelectedSrcLanguage);
		AvailableTgtLanguages = newTgtLangs;
		// 如果當前選中的目標語言不在新列表中，則選擇第一個
		if(SelectedTgtLanguage == null || !newTgtLangs.Any(l => l.LangInfo.Iso639_1 == SelectedTgtLanguage.LangInfo.Iso639_1)){
			SelectedTgtLanguage = newTgtLangs.FirstOrDefault();
		}
		return NIL;
	}

	#endregion


	public async Task<nil> Lookup(CT Ct){
		if(string.IsNullOrWhiteSpace(Input)){
			return NIL;
		}
		if(AnyNull(SvcDictionary, FrontendUserCtxMgr)){
			return NIL;
		}
		var User = FrontendUserCtxMgr.GetUserCtx();

		// 初始化 Result 并开始流式查询
		Result ??= App.DiOrMk<VmSimpleWord>();
		Result.StartStreaming(Input.Trim());

		IList<LangInfo> TgtLangs = [SelectedTgtLanguage?.LangInfo ?? new LangInfo{ Iso639_1 = "zh", Variety = "tw", Script = "hant" }];

		var Req = new ReqLlmDictEvt{
			Query = new Query{
				Term = Input.Trim(),
			},

			OptLang = new OptLang{
				SrcLang = SelectedSrcLanguage?.LangInfo ?? new LangInfo{ Iso639_1 = "en" },
				TgtLangs = TgtLangs,
			},
			// 流式回调：收到新片段时更新 UI
			OnNewSeg = (dto, ct) => {
				Result.GotNewSeg(dto);
				return 0;
			},
			// 流结束回调
			OnDone = (dto, ct) => {
				return 0;
			},
		};

		try{
			var Resp = await SvcDictionary.Lookup(User, Req, Ct);
			// 收到完整响应后，按原来的方式处理
			Result.FromRespLlmDict(Resp);
		}catch(Exception ex){
			HandleErr(ex);
		}

		return NIL;
	}


}
