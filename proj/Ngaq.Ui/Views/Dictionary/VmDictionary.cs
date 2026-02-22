namespace Ngaq.Ui.Views.Dictionary;
using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Shared.Dictionary.Models;
using Ngaq.Core.Shared.Dictionary.Svc;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Dictionary.SimpleWord;

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
	}


	public async Task<nil> Lookup(CT Ct){
		if(string.IsNullOrWhiteSpace(Input)){
			return NIL;
		}
		if(AnyNull(SvcDictionary, FrontendUserCtxMgr)){
			return NIL;
		}
		var User = FrontendUserCtxMgr.GetUserCtx();

		var Req = new ReqLlmDict{
			Query = new Query{
				Term = Input.Trim(),
			},
			OptLang = new OptLang{
				SrcLang = new LangInfo{ Iso639_1 = "en" },
				TgtLangs = [new LangInfo{ Iso639_1 = "zh", Variety = "tw", Script = "hant" }],
			},
		};

		try{
			var Resp = await SvcDictionary.Lookup(User, Req, Ct);
			Result = App.DiOrMk<VmSimpleWord>();
			Result.FromRespLlmDict(Resp);
		}catch(Exception ex){
			HandleErr(ex);
		}

		return NIL;
	}


}
