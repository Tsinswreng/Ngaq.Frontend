using Ngaq.Core.Infra.Cfg;
using Tsinswreng.CsCfg;

namespace Ngaq.Ui;


public  partial class UiCfg{
	protected static UiCfg? _Inst = null;
	public static UiCfg Inst => _Inst??= new UiCfg();

	public UiCfg(){
		BaseFontSize = LocalCfgItems.BaseFontSize.GetFrom(LocalCfg.Inst);
	}

	public f64 BaseFontSize { get; set; } = 16.0;
	public f64 WindowWidth {get;set;}= 400;
	public f64 WindowHeight {get;set;}= 700;
}
