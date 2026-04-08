namespace Ngaq.Ui.Tools;
using Avalonia.Controls;

public static class ToolBtn{
	extension<T>(T z)
		where T: Button
	{

		/// 裝飾潙語義ʸʹ按鈕(即伸展, 內容居中)
		public T StretchCenter(){
			var o = z;
			o.HorizontalAlignment = HAlign.Stretch;
			o.HorizontalContentAlignment = HAlign.Center;
			o.VerticalAlignment = VAlign.Stretch;
			o.VerticalContentAlignment = VAlign.Center;
			return o;
		}
	}

}
