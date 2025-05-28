namespace Ngaq.Ui;


public class UiCfg{
	protected static UiCfg? _Inst = null;
	public static UiCfg Inst => _Inst??= new UiCfg();

	public f64 BaseFontSize { get; set; } = 16.0;
	public f64 WindowWidth {get;set;}= 400;
	public f64 WindowHeight {get;set;}= 700;
}
