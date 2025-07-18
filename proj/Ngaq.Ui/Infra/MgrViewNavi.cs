using Tsinswreng.AvlnTools.Navigation;

namespace Ngaq.Ui.Infra;

public interface I_GetViewNavi{
	public IViewNavi? GetViewNavi();
}


public class MgrViewNavi:I_GetViewNavi{
protected static MgrViewNavi? _Inst = null;
public static MgrViewNavi Inst => _Inst??= new MgrViewNavi();

	public static IViewNavi ViewNavi{get;set;} = new ViewNavi();
	public IViewNavi? GetViewNavi(){
		return ViewNavi;
	}
}


