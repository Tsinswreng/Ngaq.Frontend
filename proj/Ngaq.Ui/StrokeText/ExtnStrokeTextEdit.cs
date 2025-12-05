namespace Ngaq.Ui.StrokeText;

using Avalonia;
using Avalonia.Media;

public static class ExtnStrokeTextEdit {
	/* 按照 PropText_() 的命名风格，把其余属性一次性补全 */
	public static StyledProperty<string> PropText_(this StrokeTextEdit z) => StrokeTextEdit.TextProperty;

	public static StyledProperty<IBrush> PropForeground_(this StrokeTextEdit z) => StrokeTextEdit.ForegroundProperty;

	public static StyledProperty<IBrush> PropFill_(this StrokeTextEdit z) => StrokeTextEdit.FillProperty;

	public static StyledProperty<IBrush> PropStroke_(this StrokeTextEdit z) => StrokeTextEdit.StrokeProperty;

	public static StyledProperty<double> PropFontSize_(this StrokeTextEdit z) => StrokeTextEdit.FontSizeProperty;

	public static StyledProperty<double> PropStrokeThickness_(this StrokeTextEdit z) => StrokeTextEdit.StrokeThicknessProperty;

	public static StyledProperty<VAlign> PropVerticalContentAlignment_(this StrokeTextEdit z) => StrokeTextEdit.VerticalContentAlignmentProperty;

	public static StyledProperty<TextWrapping> PropTextWrapping_(this StrokeTextEdit z) => StrokeTextEdit.TextWrappingProperty;
	public static StyledProperty<FontFamily> PropFontFamily_(this StrokeTextEdit z)=> StrokeTextEdit.FontFamilyProperty;
}

