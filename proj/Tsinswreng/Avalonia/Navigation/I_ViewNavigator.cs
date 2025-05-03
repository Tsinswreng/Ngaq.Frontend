using Avalonia.Controls;
using Control = Avalonia.Controls.ContentControl;
namespace Tsinswreng.Avalonia.Navigation;


public interface I_ViewNavigator{

	public bool Back();

	public void GoTo(Control view);
}
