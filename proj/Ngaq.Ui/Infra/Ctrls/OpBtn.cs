namespace Ngaq.Ui.Infra.Ctrls;

using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Threading;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
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
		SetupOverlay();
	}

	void End(){
		Dispatcher.UIThread.Post(()=>{
			State = EState.Ready;
			SetdownOverlay();
		});
	}

	void Cancel(){
		Cts.Cancel();
		SetdownOverlay();
	}

	void SetdownOverlay(){
		// if(Grid.Children.Count > 1){
		// 	Grid.Children.RemoveAt(Grid.Children.Count-1);
		// }
		if(Grid.Children.Count > 1){
			var overlay = Grid.Children[1];
			overlay.IsVisible = false;
		}
	}

	void SetupOverlay(){
		//
		if(Grid.Children.Count > 1){
			var overlay = Grid.Children[1];
			overlay.IsVisible = true;
		}else{
			Overlay.IsVisible = true;
			Grid.Children.Add(Overlay);
			//Grid.SetRow(Overlay, 1);
		}
	}

	public obj? BtnContent{get=>_Button?.Content; set=>_Button.Content = value;}

	// 在 OpBtn 類內部加一行
	public void PerformClick(){
		// 把按鈕的 Click 事件手動拋出去
		_Button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
	}

	public OpBtn(){
		base.Content = Grid;
		Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
			//RowDef(1, GUT.Auto),
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
		var mkBar = ()=>{
			var p = new ProgressBar();
			p.IsIndeterminate = true;
			p.Margin = new Thickness(0, 0, 0, 0);
			p.Padding = new Thickness(0, 0, 0, 0);
			p.VerticalAlignment = VAlign.Bottom;
			//p[!WidthProperty] = _Button[!WidthProperty];
			//p.Classes.Add("Spinner");   // 内置转圈样式;
			p.Bind(
				WidthProperty
				,_Button.GetObservable(BoundsProperty).Select(b=>b.Width)
			);
			p.IsHitTestVisible = false;
			p.Height = 0;
			p.Width = 10;
			return p;
		};
		//半透明黑遮罩
		var R = new Grid {};{
			R.Background = Brushes.Black;
			R.Opacity = 0.3;
			R.HorizontalAlignment = HAlign.Stretch;
			R.VerticalAlignment = VAlign.Stretch;
			R.IsHitTestVisible = false;
			//R[WidthProperty] = _Button[!WidthProperty];
			//R.Width = _Button.Width;
		}
		R.AddInit(mkBar());
		// R.AddInit(new Grid(), g=>{
		// 	g.Background = new SolidColorBrush(Colors.Black, 0.5);
		// 	g.IsVisible = true;
		// 	g.VerticalAlignment = VAlign.Bottom;
		// 	g.HorizontalAlignment = HAlign.Stretch;
		// 	g.Margin = new Thickness(0, 0, 0, 0);
		// 	g.AddInit(new ProgressBar(), p=>{
				// p.IsIndeterminate = true;
				// p.Margin = new Thickness(0, 0, 0, 0);
				// p.Padding = new Thickness(0, 0, 0, 0);
				// p.Classes.Add("Spinner");   // 内置转圈样式;
				// p.Width = 10;
		// 	});
		// });
		return R;
	}
}

class ZeroDecorator : Decorator{
    protected override Size MeasureOverride(Size availableSize)
        => new Size(0, 0);   // 永遠報 0×0，父級就不會被撐開
}
