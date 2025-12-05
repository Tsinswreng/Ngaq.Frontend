namespace Ngaq.Ui.StrokeText;

using Avalonia;
using Avalonia.Media;

public static class ExtnStrokeTextEdit {
	/* 按照 PropText_() 的命名风格，把其余属性一次性补全 */
	public static StyledProperty<string> PropText_(this StrokeTextBlock z) => StrokeTextBlock.TextProperty;

	public static StyledProperty<IBrush> PropForeground_(this StrokeTextBlock z) => StrokeTextBlock.ForegroundProperty;

	public static StyledProperty<IBrush> PropFill_(this StrokeTextBlock z) => StrokeTextBlock.FillProperty;

	public static StyledProperty<IBrush> PropStroke_(this StrokeTextBlock z) => StrokeTextBlock.StrokeProperty;

	public static StyledProperty<double> PropFontSize_(this StrokeTextBlock z) => StrokeTextBlock.FontSizeProperty;

	public static StyledProperty<double> PropStrokeThickness_(this StrokeTextBlock z) => StrokeTextBlock.StrokeThicknessProperty;

	public static StyledProperty<VAlign> PropVerticalContentAlignment_(this StrokeTextBlock z) => StrokeTextBlock.VerticalContentAlignmentProperty;

	public static StyledProperty<TextWrapping> PropTextWrapping_(this StrokeTextBlock z) => StrokeTextBlock.TextWrappingProperty;
	public static StyledProperty<FontFamily> PropFontFamily_(this StrokeTextBlock z)=> StrokeTextBlock.FontFamilyProperty;
}

