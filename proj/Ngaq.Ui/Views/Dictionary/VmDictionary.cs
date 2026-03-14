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

	/// 查詢結果
	public VmSimpleWord? Result{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = App.DiOrMk<VmSimpleWord>();

	#region 語言選擇

	/// 源語言（ISO 639-1 代碼，如 "en", "zh", "ja"）
	public str SrcLang{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "en";

	/// 目標語言（ISO 639-1 代碼，如 "en", "zh", "ja"）
	public str TgtLang{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "zh";

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

		IList<LangInfo> TgtLangs = [new LangInfo{ Iso639_1 = TgtLang }];

		var Req = new ReqLlmDictEvt{
			Query = new Query{
				Term = Input.Trim(),
			},

			OptLang = new OptLang{
				SrcLang = new LangInfo{ Iso639_1 = SrcLang },
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
