namespace Ngaq.Ui.Views.Settings.LearnWord;
using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;

using Ctx = VmCfgLearnWord;
public partial class VmCfgLearnWord: ViewModelBase{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmCfgLearnWord(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmCfgLearnWord(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	public CancellationTokenSource Cts = new();

	protected str _LuaFilterExpr = "";
	public str LuaFilterExpr{
		get{return _LuaFilterExpr;}
		set{SetProperty(ref _LuaFilterExpr, value);}
	}



}
