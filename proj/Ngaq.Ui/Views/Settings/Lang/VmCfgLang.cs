namespace Ngaq.Ui.Views.Settings.Lang;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ngaq.Core.Infra.Cfg;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Ui.Infra;
using Tsinswreng.CsCfg;

using Ctx = VmCfgLang;

public partial class VmCfgLang: ViewModelBase, IMk<Ctx>{
	protected VmCfgLang(){
		Cfg = AppCfg.Inst;
		Init();
	}

	public static Ctx Mk(){
		return new Ctx(AppCfg.Inst, null);
	}

	ICfgAccessor? Cfg;
	ISvcNormLang? SvcNormLang;
	public VmCfgLang(
		ICfgAccessor? Cfg,
		ISvcNormLang? SvcNormLang
	){
		this.Cfg = Cfg??AppCfg.Inst;
		this.SvcNormLang = SvcNormLang;
		Init();
	}

	void Init(){
		if(AnyNull(Cfg)){
			return;
		}
		Lang = Cfg.Get(KeysClientCfg.Lang)??"en";
	}

	public ObservableCollection<str> UiLangCodes{get;set;} = [];

	HashSet<str> UiLangCodeSet{get;set;} = new(StringComparer.OrdinalIgnoreCase);

	public str Lang{
		get;
		set{SetProperty(ref field, value);}
	} = "en";

	/// 加載 UI 語言候選列表。列表中的 code 用于下拉與保存校驗。
	public async Task<nil> InitUiLangOptions(CT Ct){
		if(AnyNull(SvcNormLang)){
			return NIL;
		}
		try{
			UiLangCodes.Clear();
			UiLangCodeSet.Clear();
			await foreach(var LangInfo in SvcNormLang.BatGetUiLangs(Ct).WithCancellation(Ct)){
				var Code = (LangInfo.Code ?? "").Trim();
				if(Code == ""){
					continue;
				}
				if(!UiLangCodeSet.Add(Code)){
					continue;
				}
				UiLangCodes.Add(Code);
			}
		}catch(Exception E){
			HandleErr(E);
		}
		return NIL;
	}

	public async Task<nil> Save(CT Ct){
		if(AnyNull(Cfg)){
			return NIL;
		}
		var LangCode = Lang.Trim();
		// 設置頁只允許保存接口給出的候選語言。
		if(UiLangCodeSet.Count > 0 && !UiLangCodeSet.Contains(LangCode)){
			ShowDialog(Todo.I18n("請選擇候選語言列表中的值"));
			return NIL;
		}
		await Task.Run(async ()=>{
			Cfg.Set(KeysClientCfg.Lang, LangCode);
			await Cfg.Save(Ct);
		});
		return NIL;
	}
}

