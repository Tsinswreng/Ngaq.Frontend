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

	// 静态构造里加回调
	static StrokeTextEdit() {
		TextProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.RebuildLayout());
		FillProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.InvalidateVisual());
		StrokeProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.UpdatePen());
		StrokeThicknessProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.UpdatePen());
		FontSizeProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.RebuildLayout());
	}

	/* -------------- 对外 bindable 字段 -------------- */
	// 改成 StyledProperty 就能绑
	public static readonly StyledProperty<string> TextProperty =
	AvaloniaProperty.Register<StrokeTextEdit, string>(nameof(Text), defaultValue: "",
		coerce: (_, v) => v ?? "");

	public string Text {
		get => GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}


	// 注册三个可绑属性
	public static readonly StyledProperty<IBrush> FillProperty =
		AvaloniaProperty.Register<StrokeTextEdit, IBrush>(nameof(Fill), Brushes.Black);

	public static readonly StyledProperty<IBrush> StrokeProperty =
		AvaloniaProperty.Register<StrokeTextEdit, IBrush>(nameof(Stroke), Brushes.Black);

	public static readonly StyledProperty<double> FontSizeProperty =
		AvaloniaProperty.Register<StrokeTextEdit, double>(nameof(FontSize), 16d);

	public IBrush Fill {
		get => GetValue(FillProperty);
		set => SetValue(FillProperty, value);
	}

	public IBrush Stroke {
		get => GetValue(StrokeProperty);
		set => SetValue(StrokeProperty, value);
	}

	public double FontSize {
		get => GetValue(FontSizeProperty);
		set => SetValue(FontSizeProperty, value);
	}


	public static readonly StyledProperty<double> StrokeThicknessProperty =
	AvaloniaProperty.Register<StrokeTextEdit, double>(nameof(StrokeThickness), 2.5);

	public double StrokeThickness {
		get => GetValue(StrokeThicknessProperty);
		set => SetValue(StrokeThicknessProperty, value);
	}

	public static readonly StyledProperty<VAlign> VerticalContentAlignmentProperty =
		AvaloniaProperty.Register<StrokeTextEdit, VAlign>(nameof(VerticalContentAlignment), VAlign.Center);

	public VAlign VerticalContentAlignment {
		get => GetValue(VerticalContentAlignmentProperty);
		set => SetValue(VerticalContentAlignmentProperty, value);
	}



	/* -------------- 内部状态 -------------- */
	private readonly List<TextLine> _lines = new();
	private int _caretIndex;
	private Typeface _typeface;
	private Pen _strokePen;


	private void UpdatePen() => _strokePen = new Pen(Stroke, StrokeThickness);

	public StrokeTextEdit() {
		_typeface = new Typeface("Microsoft YaHei");
		_strokePen = new Pen(Stroke, StrokeThickness);
		UpdatePen();

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
			_typeface, FontSize, Fill);


// 在类里补一行字段
private double _topOffset = 0;

	/* -------------- 渲染 -------------- */
	public override void Render(DrawingContext dc) {
		if (_lines.Count == 0) return;

		double y = _topOffset;                 // 不再直接 Padding.Top
		foreach (var line in _lines) {
			var fmt = CreateFormattedText(line.Text);
			var origin = new Point(Padding.Left, y);

			var geo = fmt.BuildGeometry(origin);
			dc.DrawGeometry(null, new Pen(Stroke, StrokeThickness), geo);
			dc.DrawText(fmt, origin);

			y += fmt.Height;
		}
		if (IsFocused) DrawCaret(dc);
	}

	private Vector[] OutlineOffsets =>
		new[]{ new Vector(-StrokeThickness, -StrokeThickness),
		   new Vector( StrokeThickness, -StrokeThickness),
		   new Vector(-StrokeThickness,  StrokeThickness),
		   new Vector( StrokeThickness,  StrokeThickness) };
	private void DrawCaret(DrawingContext dc) {
		var (line, off) = FindCaretLine();
		if (line < 0) return;
		var fmt = CreateFormattedText(_lines[line].Text[..off]);
		double x = Padding.Left + fmt.Width;
		double y = Padding.Top;
		for (int i = 0; i < line; i++) y += CreateFormattedText(_lines[i].Text).Height;
		dc.DrawLine(new Pen(Fill, 1), new Point(x, y), new Point(x, y + fmt.Height));
	}

	private bool _needsReLayout = true;

	protected override Size MeasureOverride(Size availableSize) {
		// 宽度未确定时先不测行，只返回最小高度
		if (availableSize.Width <= 0 || double.IsInfinity(availableSize.Width))
			return new Size(0, 20);

		if (_needsReLayout) {
			_needsReLayout = false;
			RebuildLayout(availableSize.Width); // 把可用宽度传进去
			InvalidateMeasure();                // 立即重新测量
			return base.MeasureOverride(availableSize);
		}

		var h = _lines.Sum(l => CreateFormattedText(l.Text).Height) + Padding.Top + Padding.Bottom;
		return new Size(availableSize.Width, h);
	}

	protected override Size ArrangeOverride(Size finalSize) {
		if (finalSize.Width > 0 && _needsReLayout) {
			_needsReLayout = false;
			RebuildLayout(finalSize.Width);
			InvalidateMeasure();
		}
		return finalSize;
	}

	private void RebuildLayout(double maxWidth) {
		_lines.Clear();
		if (string.IsNullOrEmpty(Text)) return;
		maxWidth -= Padding.Left + Padding.Right;
		if (maxWidth <= 0) return;

		var text = Text.AsMemory();
		int start = 0;
		while (start < text.Length) {
			int len = BreakLine(text.Slice(start), maxWidth);
			_lines.Add(new TextLine { Text = text.Slice(start, len).ToString() });
			start += len;
		}
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


public static class ExtnStrokeTextEdit {
	public static StyledProperty<string> PropText_(this StrokeTextEdit z) => StrokeTextEdit.TextProperty;
}
