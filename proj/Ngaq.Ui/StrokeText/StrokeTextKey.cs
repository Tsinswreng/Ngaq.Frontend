namespace Ngaq.Ui.StrokeText;
using Avalonia.Input;
public partial class StrokeTextEdit{
	/* -------------- 交互 -------------- */
	protected override void OnKeyDown(KeyEventArgs e) {
		switch (e.Key) {
			case Key.Left when _caretIndex > 0:
				_caretIndex--;
				break;
			case Key.Right when _caretIndex < Text.Length:
				_caretIndex++;
				break;
			case Key.Back when _caretIndex > 0:
				Text = Text.Remove(--_caretIndex, 1);
				break;
			case Key.Delete when _caretIndex < Text.Length:
				Text = Text.Remove(_caretIndex, 1);
				break;
			case Key.Enter:
				Text = Text.Insert(_caretIndex++, Environment.NewLine);
				break;
			default:
				return;
		}
		InvalidateVisual();
		e.Handled = true;
	}

	protected override void OnTextInput(TextInputEventArgs e) {
		Text = Text.Insert(_caretIndex, e.Text);
		_caretIndex += e.Text.Length;
		InvalidateVisual();
		e.Handled = true;
	}

	// 简单英文/中文断行，生产环境可换成 TextLayout
	private int BreakLine(ReadOnlyMemory<char> slice, double maxWidth) {
		if (slice.Length == 0) return 0;
		if (TextWrapping == Avalonia.Media.TextWrapping.NoWrap) return slice.Length;
		var fmt = CreateFormattedText(slice.Span.ToString());
		if (fmt.Width <= maxWidth) return slice.Length;

		int low = 0, high = slice.Length;
		while (low < high) {
			int mid = low + high + 1 >> 1;
			fmt = CreateFormattedText(slice.Span.Slice(0, mid).ToString());
			if (fmt.Width <= maxWidth)
				low = mid;
			else
				high = mid - 1;
		}
		return low == 0 ? 1 : low;
	}

}
