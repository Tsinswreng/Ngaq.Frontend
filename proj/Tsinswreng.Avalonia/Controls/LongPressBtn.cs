using System;
using Avalime.controls;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace Tsinswreng.Avalonia.Controls;
using IF;
public class LongPressBtn
	:OpenButton
	,ILongPressBtn
{
	public long LongPressDurationMs{
		get{return FnLongPressBtn.LongPressDurationMs;}
		set{FnLongPressBtn.LongPressDurationMs = value;}
	}
	public event EventHandler? LongPressed;

	public LongPressBtnFn FnLongPressBtn{get;set;} = new LongPressBtnFn();
	public LongPressBtn():base(){
		FnLongPressBtn.Init();
		//_longPressBtnFn.onClick = ()=>{OnClick();return Nil;};
		FnLongPressBtn.OnLongPress = ()=>{
			LongPressed?.Invoke(this, EventArgs.Empty);
			return Nil;
		};
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e){
		base.OnPointerPressed(e);
		FnLongPressBtn._OnPointerPressed(e);
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e){
		base.OnPointerReleased(e);
		FnLongPressBtn._OnPointerReleased(e);
	}

	protected override void OnClick(){
		if(!FnLongPressBtn._OnClick()){
			return;
		}
		base.OnClick();
	}

	#region Fn

public class LongPressBtnFn{
	protected DispatcherTimer _PressTimer;
	protected bool _IsLongPressTriggered;

	protected bool _HasLongPressed = false;
	public i64 LongPressDurationMs{get;set;} = 500; // in milliseconds
	//public Func<zero> onClick{get;set;} = ()=>0; // 点击事件
	public Func<nil> OnLongPress{get;set;} = ()=>0; // 长按事件

	bool _Inited = false;
	public nil Init(){
		if(_Inited){return Nil;}
		_PressTimer = new DispatcherTimer(){
			Interval = TimeSpan.FromMilliseconds(LongPressDurationMs)
		};
		_PressTimer.Tick += OnTimerElapsed;
		_Inited = true;
		return Nil;
	}

	public nil _OnPointerPressed(PointerPressedEventArgs e){
		_IsLongPressTriggered = false;
		_PressTimer.Start();
		return Nil;
	}

	public nil _OnPointerReleased(PointerReleasedEventArgs e){
		_PressTimer.Stop(); // 松开时停止计时器
		if (!_IsLongPressTriggered) {
			// 未触发长按时，执行点击逻辑
			//OnClick();
			//onClick?.Invoke();
		}
		_IsLongPressTriggered = false;
		return Nil;
	}

	private void OnTimerElapsed(object? sender, EventArgs e){
		_IsLongPressTriggered = true;
		_PressTimer.Stop();
		// 触发长按事件
		OnLongPress?.Invoke();
		_HasLongPressed = true;
	}


	public bool _OnClick(){
		if (_HasLongPressed) {
			_HasLongPressed = false;
			return false;
		}
		return true;
	}

	// public event EventHandler? LongPressed;
	// public virtual void OnLongPress(){
	// 	LongPressed?.Invoke(this, EventArgs.Empty);
	// }

}


	#endregion



}
