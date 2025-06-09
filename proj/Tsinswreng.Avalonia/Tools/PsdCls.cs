namespace Tsinswreng.Avalonia.Tools;
public class PsdCls{
	protected static PsdCls? _inst;
	public static PsdCls Inst => _inst??= new PsdCls();
	public str pointerover=":"+nameof(pointerover);
	public str focus=":"+nameof(focus);
	public str focus_within=":"+"focus-within";
	public str pressed=":"+nameof(pressed);
}
