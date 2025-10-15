namespace Ngaq.Ui.Infra.I18n;
using Tsinswreng.CsCfg;

public interface II18nKey:ICfgItem{

}

public class I18nKey:CfgItem<str>, II18nKey{
	public static II18nKey Mk(
		II18nKey? Parent
		,IList<str> Path
		,ICfgValue? DfltValue = null
	){
		var R = new I18nKey{
			RelaPathSegs=Path,
			DfltValue=DfltValue,
			Parent=Parent
		};
		return R;
	}
}

public class I18n:II18n{
	protected static I18n? _Inst = null;
	public static I18n Inst => _Inst??= new I18n();
	#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	protected I18n(){

	}
	public I18n(ICfgAccessor CfgAccessor){
		this.CfgAccessor = CfgAccessor;
	}
	public ICfgAccessor CfgAccessor{get;set;}
	public str Get(II18nKey Key, params obj[] Args){
		if(CfgAccessor.TryGetByPath(Key.GetFullPathSegs(), out var Value)){
			if(Value.Data is not str Template){
				throw new InvalidCastException("CfgValue.Data is not str.");
			}
			return str.Format(Template, Args);
		}
		return str.Join(".", Key.GetFullPathSegs());
	}
	public str this[II18nKey Key]{get{
		return Get(Key);
	}}
}

public interface II18n{
	public str Get(II18nKey Key, params obj[] Args);
	public str this[II18nKey Key]{get;}
}
