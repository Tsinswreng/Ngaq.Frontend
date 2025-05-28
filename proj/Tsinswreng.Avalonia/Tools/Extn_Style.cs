using Avalonia;
using Avalonia.Styling;

namespace Tsinswreng.Avalonia.Tools;

public static class Extn_Style{
	public static Style Set(
		this Style z, AvaloniaProperty property, object? value
	){
		z.Setters.Add(new Setter(property, value));
		return z;
	}
}
