namespace Ngaq.Ui.Views.Settings;
using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;
using Microsoft.Extensions.Logging;
using Ctx = VmCfgFont;
using Ngaq.Core.Infra.Cfg;
using Tsinswreng.CsCfg;

public partial class VmCfgFont: ViewModelBase{
	ILogger<VmCfgFont>? Log{get;set;}
	public VmCfgFont(
		ILogger<VmCfgFont>? Log
	){
		this.Log = Log;
	}

	protected VmCfgFont(){}
	public static VmCfgFont Mk(){
		return new VmCfgFont();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmCfgFont(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}


	protected str _InputFontSize = UiCfg.Inst.BaseFontSize+"";
	public str InputFontSize{
		get{return _InputFontSize;}
		set{SetProperty(ref _InputFontSize, value);}
	}


	protected f64 _FontSize = UiCfg.Inst.BaseFontSize;
	public f64 FontSize{
		get{return _FontSize;}
		set{SetProperty(ref _FontSize, value);}
	}


	public nil TryNeoFontSize(){
		try{
			//輸入0旹會崩潰 //TODO: avalonia 全局異常處理
			if(f64.TryParse(InputFontSize, out var numSize)){
				if(numSize <=0 || numSize > 64){
					this.AddMsg("Font size must be betwen in (0, 64]");
					this.ShowMsg();
					return NIL;
				}
				FontSize = numSize;
			}
		}
		catch (System.Exception e){
			Log?.LogError(e, "TryNeoFontSize");
		}
		return NIL;
	}

	public async Task<nil> ApplyNeoFontSize(){
		AppCfg.Inst.SetByPath(
			AppCfgItems.BaseFontSize.GetFullPathSegs()
			,CfgValue.Mk(FontSize)
		);
		await AppCfg.Inst.SaveAsy(default);
		return NIL;
	}


}

