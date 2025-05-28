using Avalonia.Controls;
using Control = Avalonia.Controls.ContentControl;
namespace Tsinswreng.Avalonia.Navigation;


public interface IViewNavigator{

	public bool Back();

	public void GoTo(Control view);
}
