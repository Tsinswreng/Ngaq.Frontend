namespace Ngaq.Ui;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;

public class StrokeTextEdit : Control {
	/* -------------- 对外 bindable 字段 -------------- */
	public static readonly DirectProperty<StrokeTextEdit, string> TextProperty =
		AvaloniaProperty.RegisterDirect<StrokeTextEdit, string>(
			nameof(Text), o => o.Text, (o, v) => o.Text = v);

	private string _text = "";
	public string Text {
		get => _text;
		set {
			if (SetAndRaise(TextProperty, ref _text, value ?? ""))
				RebuildLayout();
		}
	}

	/* -------------- 内部状态 -------------- */
	private readonly List<TextLine> _lines = new();
	private int _caretIndex;
	private Typeface _typeface;
	private double _fontSize = 16;
	private IBrush _fill = Brushes.White;
	private IBrush _stroke = Brushes.Black;
	private Pen _strokePen;

	public StrokeTextEdit() {
		_typeface = new Typeface("Microsoft YaHei");
		_strokePen = new Pen(_stroke, 2.5);

		Focusable = true;
		Cursor = new Cursor(StandardCursorType.Ibeam);
		this.GetPropertyChangedObservable(BoundsProperty)
		.Subscribe(_ => RebuildLayout());
	}

	/* -------------- 布局+折行 -------------- */
	private void RebuildLayout() {
		_lines.Clear();
		if (string.IsNullOrEmpty(Text)) {
			InvalidateVisual();
			return;
		}

		var maxWidth = Bounds.Width - Padding.Left - Padding.Right;
		if (maxWidth <= 0) return;

		var text = Text.AsMemory();
		int start = 0;
		while (start < text.Length) {
			int len = BreakLine(text.Slice(start), maxWidth);
			_lines.Add(new TextLine {
				Text = text.Slice(start, len).ToString(),
				Start = start,
				Length = len
			});
			start += len;
		}
		InvalidateVisual();
	}

	// 简单英文/中文断行，生产环境可换成 TextLayout
	private int BreakLine(ReadOnlyMemory<char> slice, double maxWidth) {
		if (slice.Length == 0) return 0;
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

	private FormattedText CreateFormattedText(string txt) =>
		new(txt, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
			_typeface, _fontSize, _fill);

	/* -------------- 渲染 -------------- */
	public override void Render(DrawingContext dc) {
		if (_lines.Count == 0) return;

		double y = Padding.Top;
		foreach (var line in _lines) {
			var fmt = CreateFormattedText(line.Text);
			var origin = new Point(Padding.Left, y);

			// 1. 描边：上下左右偏移
			// 1. 描边：上下左右偏移
			foreach (var offset in OutlineOffsets)
				dc.DrawText(fmt, origin + offset);   // 先fmt后origin，不传Brush

			// 2. 正文
			dc.DrawText(fmt, origin);

			y += fmt.Height;
		}

		// 3. 光标
		if (IsFocused)
			DrawCaret(dc);
	}

	private static readonly Vector[] OutlineOffsets =
	{
		new(-1, -1), new(1, -1), new(-1, 1), new(1, 1), new(0, 0)
	};

	private void DrawCaret(DrawingContext dc) {
		var (line, off) = FindCaretLine();
		if (line < 0) return;
		var fmt = CreateFormattedText(_lines[line].Text[..off]);
		double x = Padding.Left + fmt.Width;
		double y = Padding.Top;
		for (int i = 0; i < line; i++) y += CreateFormattedText(_lines[i].Text).Height;
		dc.DrawLine(new Pen(_fill, 1), new Point(x, y), new Point(x, y + fmt.Height));
	}

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

	/* -------------- 光标定位 -------------- */
	private (int line, int off) FindCaretLine() {
		int acc = 0;
		for (int i = 0; i < _lines.Count; i++) {
			int next = acc + _lines[i].Length;
			if (_caretIndex <= next)
				return (i, _caretIndex - acc);
			acc = next;
		}
		return (_lines.Count - 1, _lines[^1].Text.Length);
	}

	/* -------------- 简单 Padding -------------- */
	private Thickness _padding = new(4);
	public Thickness Padding {
		get => _padding;
		set { _padding = value; RebuildLayout(); }
	}

	private record TextLine {
		public string Text { get; init; }
		public int Start { get; init; }
		public int Length { get; init; }
	}
}
