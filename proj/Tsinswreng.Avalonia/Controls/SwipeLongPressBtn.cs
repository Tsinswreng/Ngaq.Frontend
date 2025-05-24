
using System;
using Avalime.controls;
using Avalonia.Controls;
using Avalonia.Input;

namespace Tsinswreng.Avalonia.Controls;

using IF;
using SwipeEventArgs = IF.ISwipeBtn.SwipeEventArgs;
using SwipeDirection = IF.ISwipeBtn.SwipeDirection;

public class SwipeLongPressBtn
	:OpenButton
	,ISwipeBtn
	,ILongPressBtn
{

	public f64 SwipeThreshold{
		get{return _SwipeBtnFn.SwipeThreshold;}
		set{_SwipeBtnFn.SwipeThreshold=value;}
	}

	public event EventHandler<SwipeEventArgs>? OnSwipe;
	public SwipeBtn.SwipeBtnFn _SwipeBtnFn{get;set;}=new ();


	public long LongPressDurationMs{
		get{return _LongPressBtnFn.LongPressDurationMs;}
		set{_LongPressBtnFn.LongPressDurationMs = value;}
	}
	public event EventHandler? OnLongPressed;

	public LongPressBtn.LongPressBtnFn _LongPressBtnFn{get;set;} = new();

	protected override Type StyleKeyOverride => typeof(Button);
	public SwipeLongPressBtn():base(){
		_SwipeBtnFn.OnSwipe = (e)=>{
			OnSwipe?.Invoke(this,e);
			return Nil;
		};
		_LongPressBtnFn.Init();
		//_longPressBtnFn.onClick = ()=>{OnClick();return Nil;};
		_LongPressBtnFn.OnLongPress = ()=>{
			OnLongPressed?.Invoke(this, EventArgs.Empty);
			return Nil;
		};
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e) {
		_SwipeBtnFn._OnPointerPressed(this,e);
		_LongPressBtnFn._OnPointerPressed(e);
		base.OnPointerPressed(e);
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e) {
		_SwipeBtnFn._OnPointerReleased(this,e);
		_LongPressBtnFn._OnPointerReleased(e);
		base.OnPointerReleased(e);
	}


	protected override void OnClick(){
		if(!_LongPressBtnFn._OnClick()){
			return;
		}
		base.OnClick();
	}

}
