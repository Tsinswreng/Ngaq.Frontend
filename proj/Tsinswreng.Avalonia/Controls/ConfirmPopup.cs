using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Tsinswreng.Avalonia.Sugar;

namespace Tsinswreng.Avalonia.Controls;


public class ConfirmPopup : UserControl{


	public MsgBox _ConfirmBox = new();
	public Popup _Popup;

	public ConfirmPopup(){
		_Style();
		Render();
		_ConfirmBox._CloseBtn.Click += (s,e)=>{
			_Popup.IsOpen = false;
		};
	}

	protected nil _Style(){
		//Styles.Add(SugarStyle.GridShowLines());
		Styles.Add(SugarStyle.NoCornerRadius());
		return Nil;
	}

	protected nil Render(){
		var Top = TopLevel.GetTopLevel(this);

		_Popup = new Popup{};
		Content = _Popup;
		{var o= _Popup;
			o.Child = _ConfirmBox;
			//o.HorizontalOffset = TopLevel.GetTopLevel
			o.PlacementTarget = Top;
			o.Placement = PlacementMode.Center;
			o.IsOpen = true;
			o.IsHitTestVisible = true;
		}
		return Nil;
	}
}
