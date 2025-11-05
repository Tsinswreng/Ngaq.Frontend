namespace Ngaq.Ui.Infra.Ctrls;
using Avalonia.Threading;
using Ngaq.Ui.Views;

public static class ExtnAppOpBtn{
	public static OpBtn SetExt(
		this OpBtn z
		,Func<CT, Task<nil>?>? FnExeAsy
	){
		var op = z;
		op.FnExeAsy = FnExeAsy;
		op.FnOk = ()=>{
			Dispatcher.UIThread.Post(()=>{
				MainView.Inst.ShowMsg("Ok");
			});
			return NIL;
		};
		op.FnFail = (err)=>{
			Dispatcher.UIThread.Post(()=>{
				MainView.Inst.HandleErr(err);
			});
			return NIL;
		};
		return z;
	}
}
