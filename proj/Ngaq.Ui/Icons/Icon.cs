namespace Ngaq.Ui.Icons;

using Avalonia.Controls;
using Avalonia.Media;
using SPath = Avalonia.Controls.Shapes.Path;
public class Icon:SPath{
	public static Icon FromSvg(Svg Svg){
		var R = new Icon();
		R.Data = Geometry.Parse(Svg);
		R.Height = R.Width = UiCfg.Inst.BaseFontSize;
		R.HorizontalAlignment = HAlign.Center;
		R.VerticalAlignment = VAlign.Center;
		R.Fill = Brushes.White;
		R.Stretch = Stretch.Uniform;   // 让图形自动缩放到控件大小
		return R;
	}
}
