using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

namespace Ngaq.Ui;

public partial class App{
	public class Cls{
		public const str SpacedStackPanel = nameof(App)+"_"+nameof(SpacedStackPanel);
		public const str ViewPadding = nameof(App)+"_"+nameof(ViewPadding);
		public const str CenterBtn = nameof(App)+"_"+nameof(CenterBtn);
	}

	Styles CenterBtn(Styles S){
		S.A(new Style(
			x=>x.Is<Button>().Class(Cls.CenterBtn)
		).Set(TemplatedControl.HorizontalAlignmentProperty
		,HAlign.Center)
		.Set(TemplatedControl.VerticalAlignmentProperty
		,VAlign.Center)
		.Set(ContentControl.VerticalContentAlignmentProperty
		,VAlign.Center)
		.Set(ContentControl.HorizontalContentAlignmentProperty
		,HAlign.Center)
		);
		return S;
	}

	Styles SpacedStackPanel(Styles S){
		S.A(new Style(
			x=>x.Is<StackPanel>().Class(Cls.SpacedStackPanel)
		).Set(
			StackPanel.SpacingProperty
			,UiCfg.Inst.BaseFontSize*0.5
		))
		;
		return S;
	}

	Styles ViewPadding(Styles S){
		S.A(new Style(
			x=>x.Is<Control>().Class(Cls.ViewPadding)
		).Set(
			ContentControl.PaddingProperty
			,new Thickness(UiCfg.Inst.BaseFontSize)
		));
		return S;
	}

	protected nil _Style(){
		var S = Styles;
		SpacedStackPanel(S);
		ViewPadding(S);
		CenterBtn(S);

		// 如下設置後 在局部覆蓋全局字體旹 TextBlock生效洏TextBox不效、不效者 字體大小恆不變
		// var StyBaseFontSize = new Style(x=>
		// 	x.Is<Control>()
		// ).Set(
		// 	TextElement.FontSizeProperty
		// 	,UiCfg.Inst.BaseFontSize
		// );
		// Styles.Add(StyBaseFontSize);

		var StyBaseFontSize = new Style(x=>
			x.Is<Control>()
		).Set(
			TextElement.FontFamilyProperty
			//,new FontFamily("Times New Roman, STSong")
			//,new FontFamily("Times New Roman")
			,FontFamily.Default//不顯式指定Default則珩于android恐缺漢字字體
		).AddTo(Styles);
		//按鈕舒展

		Styles.A(
			new Style(x=>
				x.Is<Button>()
			).Set(
				TemplatedControl.HorizontalAlignmentProperty
				, HAlign.Stretch
			).Set(
				Button.BackgroundProperty
				,new SolidColorBrush(Color.FromArgb(255, 32,32,32))
			)
		);

		// TreeDataGrid 統一撐滿父容器；各頁列寬約定為「前列 Auto，最後一列 1 Star」以消除表頭右側空白。
		Styles.A(
			new Style(x=>
				x.Is<TreeDataGrid>()
			).Set(
				TemplatedControl.HorizontalAlignmentProperty
				,HAlign.Stretch
			).Set(
				TemplatedControl.VerticalAlignmentProperty
				,VAlign.Stretch
			)
		);

		/*
我想把按鈕的邊框顏色綁定到和他自己的背景顏色一樣、並把這當成一種優先級最低的默認行爲
如果 在 局部 顯示指定了按鈕的邊框 再不再使用默認行爲。
		 */
		TemplatedControl.BackgroundProperty.Changed.AddClassHandler<Button>((btn, e) => {
			btn.SetValue(
				TemplatedControl.BorderBrushProperty
				,e.NewValue as IBrush
				,BindingPriority.Style
			);
		});

		return NIL;
	}
}
