namespace Ngaq.Ui.StrokeText;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Platform;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

/// <summary>
/// TODO 設 默認字體大小 顏色等 隨主題
/// </summary>
public partial class StrokeTextEdit : Control, IScrollable{

	// 静态构造里加回调
	static StrokeTextEdit() {
		TextProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) =>{
			x.ClearFormattedTextCache();
			x.RebuildLayout();
		});
		FillProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.InvalidateVisual());
		StrokeProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.UpdatePen());
		StrokeThicknessProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.UpdatePen());
		FontSizeProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) =>{
			x.ClearFormattedTextCache();
			x.RebuildLayout();
		});
		ForegroundProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => {
			x.Fill = x.Foreground;
			//if (!x.IsSet(FillProperty))   // 用户未显式设 Fill 才同步
		});
		TextWrappingProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.RebuildLayout());
		/* 之前已有的监听保持不变，只追加下面三行 */
		FontFamilyProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.RebuildTypeface());
		FontStyleProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.RebuildTypeface());
		FontWeightProperty.Changed.AddClassHandler<StrokeTextEdit>((x, _) => x.RebuildTypeface());
		UseVirtualizedRenderProperty.Changed.AddClassHandler<StrokeTextEdit>((x,_)=>{
			x.InvalidateVisual();
			x.RebuildLayout();
		});
	}

	/* -------------- 对外 bindable 字段 -------------- */
	public static readonly StyledProperty<bool> UseVirtualizedRenderProperty =
		AvaloniaProperty.Register<StrokeTextEdit, bool>(nameof(UseVirtualizedRender), false);

	public bool UseVirtualizedRender{
		get => GetValue(UseVirtualizedRenderProperty);
		set => SetValue(UseVirtualizedRenderProperty, value);
	}


	// 原50行：/* 可視區域（邏輯座標）*/
	// 原51行：private Rect _viewport = new Rect();
	// 新增行（51行后插入）：
	/// <summary>垂直滚动偏移</summary>
	private double _scrollOffsetY = 0;
	/// <summary>缓存每行文本的高度</summary>
	private readonly List<double> _lineHeights = new();
	/// <summary>FormattedText缓存</summary>
	private readonly Dictionary<string, FormattedText> _formattedTextCache = new();

	/* 可視區域（邏輯座標）*/
	private Rect _viewport = new Rect();

	public static readonly StyledProperty<string> TextProperty =
	AvaloniaProperty.Register<StrokeTextEdit, string>(nameof(Text), defaultValue: "",
		coerce: (_, v) => v ?? "");

	public string Text {
		get => GetValue(TextProperty);
		set{
			SetValue(TextProperty, value);
		}
	}

	public static readonly StyledProperty<FontStyle> FontStyleProperty
	=AvaloniaProperty.Register<StrokeTextEdit, FontStyle>(nameof(FontStyle), FontStyle.Normal);

	public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
		AvaloniaProperty.Register<StrokeTextEdit, TextWrapping>(nameof(Avalonia.Media.TextWrapping), Avalonia.Media.TextWrapping.NoWrap);

	public TextWrapping TextWrapping{
		get => GetValue(TextWrappingProperty);
		set => SetValue(TextWrappingProperty, value);
	}

	public static readonly StyledProperty<IBrush> ForegroundProperty =
		AvaloniaProperty.Register<StrokeTextEdit, IBrush>(nameof(Foreground), Brushes.Black);

	public IBrush Foreground {
		get => GetValue(ForegroundProperty);
		set => SetValue(ForegroundProperty, value);
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

	// public static readonly StyledProperty<FontFamily> FontFamilyProperty =
	// 	AvaloniaProperty.Register<StrokeTextEdit, FontFamily>(nameof(FontFamily), FontFamily.Default);
	public static readonly StyledProperty<FontFamily> FontFamilyProperty =
		TextElement.FontFamilyProperty.AddOwner<StrokeTextEdit>();
	public FontFamily FontFamily{
		get => GetValue(FontFamilyProperty);
		set => SetValue(FontFamilyProperty, value);
	}

	public static readonly StyledProperty<FontWeight> FontWeightProperty
	=AvaloniaProperty.Register<StrokeTextEdit, FontWeight>(nameof(FontWeight), FontWeight.Normal);

	private static readonly StyledProperty<Typeface> TypefaceProperty
	=AvaloniaProperty.Register<StrokeTextEdit, Typeface>(nameof(Typeface), new Typeface(FontFamily.Default));



	public FontWeight FontWeight{
		get => GetValue(FontWeightProperty);
		set => SetValue(FontWeightProperty, value);
	}

	public FontStyle FontStyle{
		get => GetValue(FontStyleProperty);
		set => SetValue(FontStyleProperty, value);
	}

	public Typeface Typeface{
		get => GetValue(TypefaceProperty);
		private set => SetValue(TypefaceProperty, value);
	}

	/* -------------- 内部状态 -------------- */
	private readonly List<TextLine> _lines = new();
	private int _caretIndex;

	private Pen _strokePen;

	private void UpdateTypeface(){
		//Typeface = new Typeface(FontFamily, FontStyle.Normal, FontWeight.Normal);
		Typeface = new Typeface(FontFamily, FontStyle, FontWeight);
	}


	private void UpdatePen() => _strokePen = new Pen(Stroke, StrokeThickness);

	public StrokeTextEdit() {
		//_typeface = new Typeface("Microsoft YaHei");
		Typeface = new Typeface(FontFamily.Default);
		_strokePen = new Pen(Stroke, StrokeThickness);
		UpdatePen();

		Focusable = true;
		Cursor = new Cursor(StandardCursorType.Ibeam);
		this.GetPropertyChangedObservable(BoundsProperty)
		.Subscribe(_ => {
			_viewport = new Rect(Bounds.Size);
			RebuildLayout();
			Offset = new Vector(0, Math.Clamp(_scrollOffsetY, 0, Math.Max(0, TotalHeight - _viewport.Height)));
		});

		// var DfltSty = new Style()
		// .Set(
		// 	FontSizeProperty
		// 	,new DynamicResourceExtension(KeysRsrc.ControlContentThemeFontSize)
		// ).Attach(Styles);
		var a = new DynamicResourceExtension("TextControlForeground");
		var DfltSty = new Style()
		.Set(FontSizeProperty, new DynamicResourceExtension("ControlContentThemeFontSize"))
		.Set(ForegroundProperty, new DynamicResourceExtension("TextControlForeground"))
		.Set(FillProperty, new DynamicResourceExtension("TextControlForeground"))
		//.Set(StrokeProperty, new DynamicResourceExtension("TextControlForeground"))
		//.Set(StrokeProperty, new InvertForegroundBrushExtension())
		.Set(StrokeThicknessProperty, 1.0)
		//此會致安卓中 漢字變成方塊
		//.Set(FontFamilyProperty, new DynamicResourceExtension("ContentControlThemeFontFamily"))
		.Set(VerticalContentAlignmentProperty, VAlign.Center)
		.Attach(Styles);
		if(Application.Current?.Resources.TryGetValue("SystemControlForegroundBaseHighBrush", out var brush)??false){
			if(brush is IBrush b){
				DfltSty.Set(ForegroundProperty, b);
			}
		}
	}


	private void RebuildTypeface(){
		var z = this;
		Typeface = new Typeface(FontFamily, z.FontStyle, FontWeight);
		ClearFormattedTextCache(); // 新增
		RebuildLayout();   // 布局依赖字形度量，必须刷新
	}

	/* -------------- 布局+折行 -------------- */
	// 原212-237行：private void RebuildLayout() { ... }
	// 修改后（仅改核心逻辑）：
	private void RebuildLayout() {
		_lines.Clear();
		_lineHeights.Clear(); // 新增
		ClearFormattedTextCache(); // 新增
		if (string.IsNullOrEmpty(Text)) {
			InvalidateVisual();
			return;
		}

		var maxWidth = Bounds.Width - Padding.Left - Padding.Right;
		if (maxWidth <= 0) {
			InvalidateVisual();
			return;
		}

		var text = Text.AsMemory();
		int start = 0;
		while (start < text.Length) {
			int len = BreakLine(text.Slice(start), maxWidth);
			var lineText = text.Slice(start, len).ToString(); // 新增
			_lines.Add(new TextLine {
				Text = lineText, // 修改
				Start = start,
				Length = len
			});
			// 新增：缓存行高
			_lineHeights.Add(CreateFormattedText(lineText).Height);
			start += len;
		}
		// 新增：滚动偏移适配
		if (_scrollOffsetY > TotalHeight - _viewport.Height)
		{
			Offset = new Vector(0, Math.Max(0, TotalHeight - _viewport.Height));
		}
		InvalidateVisual();
	}

	// 简单英文/中文断行，生产环境可换成 TextLayout
	private int BreakLine(ReadOnlyMemory<char> slice, double maxWidth) {
		if (slice.Length == 0) return 0;
		if (TextWrapping == TextWrapping.NoWrap) return slice.Length;
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

	private FormattedText CreateFormattedText(string txt) {
		var cacheKey = $"{txt}|{FontSize}|{Typeface}";
		if (_formattedTextCache.TryGetValue(cacheKey, out var fmt))
			return fmt;

		fmt = new(txt, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
			Typeface, FontSize, Fill);
		_formattedTextCache[cacheKey] = fmt;
		return fmt;
	}


	private (int startIndex, int endIndex) GetVisibleLineRange()
	{
		if (!UseVirtualizedRender || _lines.Count == 0 || _lineHeights.Count == 0)
			return (0, _lines.Count - 1);

		double viewportTop = _scrollOffsetY;
		double viewportBottom = _scrollOffsetY + _viewport.Height;

		double currentY = 0;
		int startIndex = 0;
		for (int i = 0; i < _lines.Count; i++)
		{
			double lineBottom = currentY + _lineHeights[i];
			if (lineBottom > viewportTop)
			{
				startIndex = i;
				break;
			}
			currentY = lineBottom;
		}

		currentY = 0;
		int endIndex = _lines.Count - 1;
		for (int i = 0; i < _lines.Count; i++)
		{
			if (currentY > viewportBottom)
			{
				endIndex = i - 1;
				break;
			}
			currentY += _lineHeights[i];
		}

		startIndex = Math.Max(0, startIndex);
		endIndex = Math.Min(_lines.Count - 1, endIndex);
		return (startIndex, endIndex);
	}


	// 在类里补一行字段
	private double _topOffset = 0;

	/* -------------- 渲染 -------------- */
	// 原267-284行：public override void Render(DrawingContext dc) { ... }
	// 修改后：
	public override void Render(DrawingContext dc) {
		if (_lines.Count == 0 || _lineHeights.Count == 0){
			return;
		}

		var (startIdx, endIdx) = GetVisibleLineRange();
		if (startIdx > endIdx) return;

		// 计算可视第一行Y坐标
		double currentY = 0;
		for (int i = 0; i < startIdx; i++)
		{
			currentY += _lineHeights[i];
		}
		currentY = currentY - _scrollOffsetY + Padding.Top;

		// 仅渲染可视行
		for (int i = startIdx; i <= endIdx; i++)
		{
			var line = _lines[i];
			double lineHeight = _lineHeights[i];
			var fmt = CreateFormattedText(line.Text);
			var origin = new Point(Padding.Left, currentY);

			var geo = fmt.BuildGeometry(origin);
			dc.DrawGeometry(null, _strokePen, geo);
			dc.DrawText(fmt, origin);

			currentY += lineHeight;
		}
		if (IsFocused) DrawCaret(dc);
	}

	private Vector[] OutlineOffsets =>
		new[]{ new Vector(-StrokeThickness, -StrokeThickness),
		   new Vector( StrokeThickness, -StrokeThickness),
		   new Vector(-StrokeThickness,  StrokeThickness),
		   new Vector( StrokeThickness,  StrokeThickness) };

	// 原292-304行：private void DrawCaret(DrawingContext dc) { ... }
	// 修改后：
	private void DrawCaret(DrawingContext dc) {
		var (line, off) = FindCaretLine();
		if (line < 0 || line >= _lines.Count || line >= _lineHeights.Count){
			return;
		}

		// 计算光标Y坐标（适配滚动）
		double y = Padding.Top;
		for (int i = 0; i < line; i++){
			y += _lineHeights[i];
		}
		y -= _scrollOffsetY;

		// 光标超出可视区不绘制
		if (y < 0 || y > _viewport.Height) return;

		var fmt = CreateFormattedText(_lines[line].Text[..off]);
		double x = Padding.Left + fmt.Width;
		dc.DrawLine(new Pen(Fill, 1), new Point(x, y), new Point(x, y + _lineHeights[line]));
	}

	private bool _needsReLayout = true;

	/*
	告訴布局系統「我需要多大」
	重寫 MeasureOverride(Size availableSize)，返回控件希望佔用的尺寸。
	如果裡面還有子元素，記得遞歸調用 child.Measure(...)。
	 */

	protected override Size MeasureOverride(Size availableSize) {
		double width = availableSize.Width;
		if (double.IsInfinity(width)) {
			width = 200; // ✅ 给一个默认宽度，或者根据文本估算
		}

		if (_needsReLayout) {
			_needsReLayout = false;
			RebuildLayout(width);
		}

		var height = _lines.Count == 0
			? CreateFormattedText("A").Height
			: _lines.Sum(l => CreateFormattedText(l.Text).Height);

		return new Size(width, height + Padding.Top + Padding.Bottom);
	}



	/*
	告訴布局系統「我怎麼擺」
	重寫 ArrangeOverride(Size finalSize)，把子元素或自己的繪圖區域安排到最終矩形。
	最後必須返回實際使用的 finalSize。
	 */
	protected override Size ArrangeOverride(Size finalSize) {
		if (finalSize.Width > 0 && _needsReLayout) {
			_needsReLayout = false;
			RebuildLayout(finalSize.Width);
			//InvalidateMeasure(); //恐觸死循環
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
	private Thickness _padding = new(0);
	public Thickness Padding {
		get => _padding;
		set { _padding = value; ClearFormattedTextCache(); RebuildLayout(); }
	}

	// 原154行：}
	// 新增行（155行开始）：
	#region IScrollable实现
	public Vector Offset
	{
		get => new Vector(0, _scrollOffsetY);
		set
		{
			double newY = Math.Clamp(value.Y, 0, Math.Max(0, TotalHeight - _viewport.Height));
			if (Math.Abs(_scrollOffsetY - newY) < double.Epsilon) return;
			_scrollOffsetY = newY;
			InvalidateVisual();
		}
	}

	public Size Extent => new Size(Bounds.Width, TotalHeight);
	public Size Viewport
	{
		get => _viewport.Size;
		set
		{
			if (_viewport.Size == value) return;
			_viewport = new Rect(value);
			InvalidateVisual();
		}
	}

	/// <summary>总行高（含Padding）</summary>
	public double TotalHeight
	{
		get
		{
			if (_lines.Count == 0)
				return CreateFormattedText("A").Height + Padding.Top + Padding.Bottom;
			return _lineHeights.Sum() + Padding.Top + Padding.Bottom;
		}
	}
	#endregion

// 原161行：private void UpdateTypeface(){
// 新增行（160行后）：
private void ClearFormattedTextCache() => _formattedTextCache.Clear();

	private record TextLine {
		public string Text { get; init; }
		public int Start { get; init; }
		public int Length { get; init; }
	}
}


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
