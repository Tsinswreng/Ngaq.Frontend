namespace Ngaq.Ui.Infra.I18n;

using Jeffijoe.MessageFormat;
using Ngaq.Core.Infra.Errors;
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
	MessageFormatter MsgFmt = new();
	public str Get(II18nKey Key, params obj[] Args){
		if(!CfgAccessor.TryGetBoxedByPath(Key.GetFullPathSegs(), out var Value)){
			return Key.GetFullPathSegs().Last();
		}
		if(Value.Data is str Template){
			if(Args.Length == 0){
				return Template;
			}else{
				var ArgDict = new Dictionary<str, obj?>();
				for(var i = 0; i < Args.Length; i++){
					ArgDict[i+""] = i;
				}
				return MsgFmt.FormatMessage(Template, ArgDict);
			}
		}
		//TODO handle Dict {type: "xxx", data: ""}
		throw new NotImplementedException();
	}

	public str this[II18nKey Key]{get{
		return Get(Key);
	}}
}

public interface II18n{
	public str Get(II18nKey Key, params obj[] Args);
	public str this[II18nKey Key]{get;}
}

public static class AppExtnErrItem{
	public static I18nKey ToI18nKey(this IErrItem z, params obj?[] Args){
		return new I18nKey{
			RelaPathSegs=["Error", ..z.RelaPathSegs],
			DfltValue = z.DfltValue
			,Parent = z.Parent
			,Children = z.Children
		};
	}
}
