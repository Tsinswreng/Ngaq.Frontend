namespace Ngaq.Ui.Infra.Ctrls;

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Ngaq.Ui.Infra;
using Ctx = VmOpBtn;


public partial class OpBtn: ContentControl{
	public enum EState{
		Ready,
		Working,
		Disabled
	}
	public Button _Button{get;set;} = new();
	public CancellationTokenSource Cts{get;set;} = new();

	public Func<CT, Task<nil>?>? FnExeAsy{get;set;} = async(Ct)=>NIL;
	public Func<obj?>? FnOk{get;set;}
	public Func<Exception?, obj?>? FnFail{get;set;}
	public Func<obj?>? FnCancel{get;set;}
	public EState State{get;set;}

	public Grid Grid = new();
	public Control Overlay;
	void Start(){
		Cts = new();
		State = EState.Working;
		Grid.Children.Add(Overlay);
	}

	void End(){
		Dispatcher.UIThread.Post(()=>{
			State = EState.Ready;
			RemoveOverlay();
		});
	}

	void Cancel(){
		Cts.Cancel();
		RemoveOverlay();
	}

	void RemoveOverlay(){
		if(Grid.Children.Count > 1){
			Grid.Children.RemoveAt(Grid.Children.Count-1);
		}
	}

	public obj? BtnContent{get=>_Button?.Content; set=>_Button.Content = value;}

	public OpBtn(){
		base.Content = Grid;
		Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
		]);
		Grid.Children.Add(_Button);

		var FnMkOverlay = ()=>{
			return MkOverlay();
		};
		Overlay = FnMkOverlay();



		_Button.Click+= (s,e)=>{
			if(State == EState.Working){
				Cancel();
				return;
			}

			Start();
			var R = FnExeAsy(Cts.Token);
			if(R is null){
				End();
				return;
			}
			R.ContinueWith(t=>{
				if(t.IsCanceled){
					FnCancel?.Invoke();
				}else if(t.IsFaulted){
					FnFail?.Invoke(t.Exception.InnerException);
				}else{
					FnOk?.Invoke();
				}
				End();
			});
		};
	}

	public Control MkOverlay(){
		var R = new Grid {
			Background = Brushes.Black,
			Opacity = 0.5,
			HorizontalAlignment = HAlign.Stretch,
			VerticalAlignment = VAlign.Stretch,
			IsHitTestVisible = false
		};
		var loading = new Grid{
			Background = new SolidColorBrush(Colors.Black, 0.5),
			IsVisible = false,
			Children ={
				new Viewbox{
					Width = 60,
					Height = 0,
					Child = new ProgressBar{
						IsIndeterminate = true,
						Classes = { "Spinner" }   // 内置转圈样式
					}
				}
			}
		};
		R.Children.Add(loading);
		return R;
	}
}
