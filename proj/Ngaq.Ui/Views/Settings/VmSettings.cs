namespace Ngaq.Ui.Views.Settings;
using System.Collections.ObjectModel;
using Ngaq.Core.Infra.Cfg;
using Ngaq.Ui.Infra;
using Tsinswreng.CsCfg;
using Tsinswreng.CsCore;

using Ctx = VmSettings;
public partial class VmSettings: ViewModelBase, IMk<Ctx>{
	/// 供無參構造場景（如設計時或DI不可用）使用。
	protected VmSettings(){}

	public static Ctx Mk(){
		return new Ctx();
	}

	ICfgAccessor? Cfg;
	public VmSettings(ICfgAccessor? Cfg){
		this.Cfg = Cfg;
		Init();
	}

	/// 初始化設置頁可編輯配置項。
	void Init(){
		if(AnyNull(Cfg)){
			return;
		}
		Lang = Cfg.Get(KeysClientCfg.Lang)??"en";
		ServerBaseUrl = Cfg.Get(KeysClientCfg.ServerBaseUrl)??"";
		SqlitePath = Cfg.Get(KeysClientCfg.SqlitePath)??"";
		MaxDisplayedWordCount = (Cfg.Get(KeysClientCfg.Word.MaxDisplayedWordCount)).ToString();

		LlmDictionaryApiUrl = Cfg.Get(KeysClientCfg.LlmDictionary.ApiUrl)??"";
		LlmDictionaryApiKey = Cfg.Get(KeysClientCfg.LlmDictionary.ApiKey)??"";
		LlmDictionaryModel = Cfg.Get(KeysClientCfg.LlmDictionary.Model)??"";
		LlmDictionaryPrompt = Cfg.Get(KeysClientCfg.LlmDictionary.Prompt)??"";
	}

	/// 語言代碼。
	public str Lang{
		get;
		set{SetProperty(ref field, value);}
	} = "en";

	/// 後端服務基地址。
	public str ServerBaseUrl{
		get;
		set{SetProperty(ref field, value);}
	} = "";

	/// SQLite 路徑。
	public str SqlitePath{
		get;
		set{SetProperty(ref field, value);}
	} = "";

	/// 首頁最多顯示單詞數（字串形式，保存時解析）。
	public str MaxDisplayedWordCount{
		get;
		set{SetProperty(ref field, value);}
	} = "500";

	/// LLM 詞典 API 地址。
	public str LlmDictionaryApiUrl{
		get;
		set{SetProperty(ref field, value);}
	} = "";

	/// LLM 詞典 API Key。
	public str LlmDictionaryApiKey{
		get;
		set{SetProperty(ref field, value);}
	} = "";

	/// LLM 詞典模型名。
	public str LlmDictionaryModel{
		get;
		set{SetProperty(ref field, value);}
	} = "";

	/// LLM 詞典提示詞。
	public str LlmDictionaryPrompt{
		get;
		set{SetProperty(ref field, value);}
	} = "";

	/// 保存設置頁配置。
	public async Task<nil> Save(CT Ct){
		if(AnyNull(Cfg)){
			return NIL;
		}
		if(!u64.TryParse(MaxDisplayedWordCount, out var maxDisplayedWordCount)){
			ShowDialog(Todo.I18n("MaxDisplayedWordCount must be an unsigned integer."));
			return NIL;
		}
		await Task.Run(async ()=>{
			Cfg.Set(KeysClientCfg.Lang, Lang.Trim());
			Cfg.Set(KeysClientCfg.ServerBaseUrl, ServerBaseUrl.Trim());
			Cfg.Set(KeysClientCfg.SqlitePath, SqlitePath.Trim());
			Cfg.Set(KeysClientCfg.Word.MaxDisplayedWordCount, maxDisplayedWordCount);

			Cfg.Set(KeysClientCfg.LlmDictionary.ApiUrl, LlmDictionaryApiUrl.Trim());
			Cfg.Set(KeysClientCfg.LlmDictionary.ApiKey, LlmDictionaryApiKey.Trim());
			Cfg.Set(KeysClientCfg.LlmDictionary.Model, LlmDictionaryModel.Trim());
			Cfg.Set(KeysClientCfg.LlmDictionary.Prompt, LlmDictionaryPrompt);
			await Cfg.Save(Ct);
		});
		return NIL;
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmSettings(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

}
