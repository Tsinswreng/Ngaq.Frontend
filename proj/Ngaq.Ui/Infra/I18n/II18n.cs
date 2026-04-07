using Tsinswreng.CsCore;
using Tsinswreng.CsErr;

namespace Ngaq.Ui.Infra.I18n;

[Doc("")]
public interface II18n{
	public str Get(II18nKey Key, params obj[] Args);
	public str this[II18nKey Key]{get;}
}

public static class AppExtnErrItem{
	public static I18nKey ToI18nKey(this IErrNode z){
		return new I18nKey{
			RelaPathSegs=["Error", ..z.RelaPathSegs],
			DfltValueObj = z.DfltValueObj
			,Parent = z.Parent
			,Children = z.Children
		};
	}
}
