namespace Ngaq.Ui.Views.Settings.Lang;

using Ngaq.Core.Infra.Cfg;
using Ngaq.Ui.Infra;
using Tsinswreng.CsCfg;

using Ctx = VmCfgLang;

public partial class VmCfgLang: ViewModelBase, IMk<Ctx>{
	protected VmCfgLang(){
		Cfg = AppCfg.Inst;
		Init();
	}

	public static Ctx Mk(){
		return new Ctx(AppCfg.Inst);
	}

	ICfgAccessor? Cfg;
	public VmCfgLang(ICfgAccessor? Cfg){
		this.Cfg = Cfg??AppCfg.Inst;
		Init();
	}

	void Init(){
		if(AnyNull(Cfg)){
			return;
		}
		Lang = Cfg.Get(KeysClientCfg.Lang)??"en";
	}

	public str Lang{
		get;
		set{SetProperty(ref field, value);}
	} = "en";

	public async Task<nil> Save(CT Ct){
		if(AnyNull(Cfg)){
			return NIL;
		}
		await Task.Run(async ()=>{
			Cfg.Set(KeysClientCfg.Lang, Lang.Trim());
			await Cfg.Save(Ct);
		});
		return NIL;
	}
}

