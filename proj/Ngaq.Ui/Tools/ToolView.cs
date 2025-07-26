using Avalonia.Controls;
using Avalonia.Media;
using Ngaq.Ui.Infra.Ctrls;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Navigation;

namespace Ngaq.Ui.Tools;

public partial class ToolView{

	public static Control WithTitle(str Title, Control Target){
		var titled = new ViewTitle();
		titled.Body.Content = Target;
		//titled.Title.Content = new TextBlock(){Text = Title};
		titled.Title.ContentInit(_TextBlock(), o=>{
			o.Text = Title;
			o.VerticalAlignment = VertAlign.Center;
			o.HorizontalAlignment = HoriAlign.Center;
			o.FontSize = UiCfg.Inst.BaseFontSize*1.2;
		});
		titled.BdrTitle.Background = new SolidColorBrush(Color.FromRgb(32, 32, 32));
		return titled;
	}


}
