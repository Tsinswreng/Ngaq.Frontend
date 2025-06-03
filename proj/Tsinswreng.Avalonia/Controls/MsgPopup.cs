using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Tsinswreng.Avalonia.Sugar;

namespace Tsinswreng.Avalonia.Controls;


public class MsgPopup : UserControl{

	public MsgBox _MsgBox = new();

	public Border _Border{get{
		return _MsgBox._Border;
	}}
	public Border _BdrTitle{get{
		return _MsgBox._BdrTitle;
	}}
	public ContentControl _Title{get{
		return _MsgBox._Title;
	}}
	public Border _BdrBody{get{
		return _MsgBox._BdrBody;
	}}
	public ContentControl _Body{get{
		return _MsgBox._Body;
	}}
	public SwipeLongPressBtn _CloseBtn{get{
		return _MsgBox._CloseBtn;
	}}
	public Border _BdrBottomView{get{
		return _MsgBox._BdrBottomView;
	}}
	public ContentControl _BottomView{get{
		return _MsgBox._BottomView;
	}}

	public Popup _Popup;

	public MsgPopup(){
		_Style();
		Render();
		_MsgBox._CloseBtn.Click += (s,e)=>{
			if(_Popup != null){
				_Popup.IsOpen= false;
			}
		};
	}

	protected nil _Style(){
		//Styles.Add(SugarStyle.GridShowLines());
		return Nil;
	}

	protected nil Render(){
		var Top = TopLevel.GetTopLevel(this);

		_Popup = new Popup{};
		Content = _Popup;
		{var o= _Popup;
			o.Child = _MsgBox;
			//o.HorizontalOffset = TopLevel.GetTopLevel
			o.PlacementTarget = Top;
			o.Placement = PlacementMode.Center;
			//o.Placement = PlacementMode.Top;
			o.IsOpen = true;
			o.IsHitTestVisible = true;
			//o.StaysOpen = false // 点击其它地方自动关闭
		}
		return Nil;
	}
}
