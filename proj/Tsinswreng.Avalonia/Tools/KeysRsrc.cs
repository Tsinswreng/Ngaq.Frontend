namespace Tsinswreng.Avalonia.Tools;

public class KeysRsrc{
protected static KeysRsrc? _Inst = null;
public static KeysRsrc Inst => _Inst??= new KeysRsrc();

	public str ControlContentThemeFontSize = nameof(ControlContentThemeFontSize);
}
