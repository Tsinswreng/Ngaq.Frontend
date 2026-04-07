namespace Ngaq.Ui.Views.Dictionary;

using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Shared.Dictionary.Models;
using Ngaq.Core.Shared.Dictionary.Models.Po.NormLang;
using Ngaq.Core.Shared.Dictionary.Svc;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Dictionary.SimpleWord;

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
		return NIL;
	}

	public nil ApplySrcNormLang(PoNormLang Po){
		SrcLang = Po.Code ?? "";
		return NIL;
	}

	public nil ApplyTgtNormLang(PoNormLang Po){
		TgtLang = Po.Code ?? "";
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

		try{
			var Resp = await SvcDictionary.Lookup(User, Req, Ct);
			Result.FromRespLlmDict(Resp);
		}catch(Exception ex){
			HandleErr(ex);
		}

		return NIL;
	}
}
