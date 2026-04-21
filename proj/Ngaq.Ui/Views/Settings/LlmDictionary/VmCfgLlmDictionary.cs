namespace Ngaq.Ui.Views.Settings.LlmDictionary;

using Ngaq.Core.Infra.Cfg;
using Ngaq.Ui.Infra;
using Tsinswreng.CsCfg;

using Ctx = VmCfgLlmDictionary;

public partial class VmCfgLlmDictionary: ViewModelBase, IMk<Ctx>{
	protected VmCfgLlmDictionary(){
		Cfg = AppCfg.Inst;
		Init();
	}

	public static Ctx Mk(){
		return new Ctx(AppCfg.Inst);
	}

	ICfgAccessor? Cfg;
	public VmCfgLlmDictionary(ICfgAccessor? Cfg){
		this.Cfg = Cfg??AppCfg.Inst;
		Init();
	}

	void Init(){
		if(AnyNull(Cfg)){
			return;
		}
		ApiUrl = Cfg.Get(KeysClientCfg.LlmDictionary.ApiUrl)??"";
		ApiKey = Cfg.Get(KeysClientCfg.LlmDictionary.ApiKey)??"";
		Model = Cfg.Get(KeysClientCfg.LlmDictionary.Model)??"";
		Prompt = Cfg.Get(KeysClientCfg.LlmDictionary.Prompt)??"";
	}

	public str ApiUrl{
		get;
		set{SetProperty(ref field, value);}
	} = "";

	public str ApiKey{
		get;
		set{SetProperty(ref field, value);}
	} = "";

	public str Model{
		get;
		set{SetProperty(ref field, value);}
	} = "";

	public str Prompt{
		get;
		set{SetProperty(ref field, value);}
	} = "";

	public async Task<nil> Save(CT Ct){
		if(AnyNull(Cfg)){
			return NIL;
		}
		await Task.Run(async ()=>{
			Cfg.Set(KeysClientCfg.LlmDictionary.ApiUrl, ApiUrl.Trim());
			Cfg.Set(KeysClientCfg.LlmDictionary.ApiKey, ApiKey.Trim());
			Cfg.Set(KeysClientCfg.LlmDictionary.Model, Model.Trim());
			Cfg.Set(KeysClientCfg.LlmDictionary.Prompt, Prompt);
			await Cfg.Save(Ct);
		});
		return NIL;
	}
}

