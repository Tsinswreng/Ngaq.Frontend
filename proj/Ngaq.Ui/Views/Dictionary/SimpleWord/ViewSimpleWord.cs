namespace Ngaq.Ui.Views.Dictionary.SimpleWord;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Ngaq.Core.Shared.Dictionary.Models;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ctx = VmSimpleWord;
public partial class ViewSimpleWord
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSimpleWord(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}

	public partial class Cls{

	}


	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new (IsRow: true);
	SelectableTextBlock Txt(){
		var R = new SelectableTextBlock();
		R.TextWrapping = TextWrapping.Wrap;
		return R;
	}
	protected nil Render(){
		Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
		]);
		Root
		.A(Txt(), o=>{
			o.FontSize = UiCfg.Inst.BaseFontSize*1.5;
			o.CBind<Ctx>(o.PropText,x=>x.Head);
		})
		.A(PronunciationList())
		.A(Txt(), o=>{
			o.CBind<Ctx>(o.PropText,x=>x.Description);
		})
		;
		return NIL;
	}

	/// 讀音列表：僅在解析後有 Pronunciations 時顯示。每行左側方形播放鍵+右側文本。
	Control PronunciationList(){
		var R = new ItemsControl();
		R.Bind(
			R.PropItemsSource, CBE.Mk<Ctx>(x=>x.Pronunciations)
		);

		R.SetItemTemplate<Pronunciation>((p,b)=>{
			var Row = new AutoGrid(IsRow: false);
			Row.ColDefs.AddRange([
				ColDef(1, GUT.Auto),
				ColDef(1, GUT.Auto),
			]);
			Row.A(new OpBtn(), o=>{
				var Icon = Icons.VolHigh().ToIcon();
				Icon.Height = UiCfg.Inst.BaseFontSize*0.8;
				Icon.Width = UiCfg.Inst.BaseFontSize*0.8;
				o.BtnContent = Icon;
				// 按鈕保持方形尺寸，且靠左不拉伸。
				o._Button.Width = UiCfg.Inst.BaseFontSize*1.6;
				o._Button.Height = UiCfg.Inst.BaseFontSize*1.6;
				o._Button.HorizontalAlignment = HAlign.Left;
				o.HorizontalAlignment = HAlign.Left;
				o.SetExe(Ct=>Ctx?.PlayHead(Ct));
			})
			.A(Txt(), o=>{
				o.Text = p.Text;
				o.VerticalAlignment = VAlign.Center;
				o.Margin = new Avalonia.Thickness(UiCfg.Inst.BaseFontSize*0.2, 0, 0, 0);
			});
			return Row.Grid;
		});
		R.SetItemsPanel(()=>{
			return new StackPanel{
				Orientation = Orientation.Vertical,
				HorizontalAlignment = HAlign.Left,
			};
		});

		return R;
	}


}
