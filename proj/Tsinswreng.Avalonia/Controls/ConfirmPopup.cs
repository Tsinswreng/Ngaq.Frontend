using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Tsinswreng.Avalonia.Controls;


public class ConfirmPopup : UserControl{


	public ConfirmBox _ConfirmBox;
	public Popup _Popup;

	public ConfirmPopup(){
		Render();
	}

	protected nil Render(){
		var Top = TopLevel.GetTopLevel(this);

		_Popup = new Popup{};
		{var o= _Popup;
			o.Child = _ConfirmBox;
			//o.HorizontalOffset = TopLevel.GetTopLevel
			o.PlacementTarget = Top;
			o.Placement = PlacementMode.Center;
			o.IsOpen = true;
		}
		return Nil;
	}
}
