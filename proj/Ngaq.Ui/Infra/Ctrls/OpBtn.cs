namespace Ngaq.Ui.Infra.Ctrls;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
//using Ctx = VmXxx;
public partial class OpBtn{
	public enum EState{
		Ready,
		Working,
		Disabled
	}
	public Button _Button{get;set;} = new();
	public CancellationTokenSource Cts{get;set;} = new();

	public Func<CT, nil> FnAsy{get;set;} = (Ct)=>NIL;
	public OpBtn(){
		//_Button.Is
	}

}
