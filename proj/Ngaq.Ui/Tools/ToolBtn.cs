namespace Ngaq.Ui.Tools;
using Avalonia.Controls;

public static class ToolBtn{
	extension<T>(T z)
		where T: Button
	{
		/// <summary>
		/// 裝飾潙語義ʸʹ按鈕(即伸展, 內容居中)
		/// </summary>
		/// <param name="z"></param>
		/// <returns></returns>
		public T StretchCenter(){
			var o = z;
			o.HorizontalAlignment = HAlign.Stretch;
			o.HorizontalContentAlignment = HAlign.Center;
			return o;
		}
	}

}
