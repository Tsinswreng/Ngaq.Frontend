namespace Ngaq.Ui.Views.Word.WordManage.Statistics;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui.Converters;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using ScottPlot.Avalonia;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmStatistics;

public partial class ViewStatisticsV2
	: AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		protected set{DataContext = value;}
	}

	AvaPlot? PlotCtrl;
	TextBlock? SummaryText;

	public ViewStatisticsV2(){
		Ctx = App.GetRSvc<Ctx>();
		Style();
		Render();
		if(Ctx is not null){
			Ctx.GraphChanged += (s, e)=>{
				if(PlotCtrl is not null){
					DrawPlot(PlotCtrl);
				}
				UpdateSummary();
			};
		}
		UpdateSummary();
	}

	public partial class Cls{
		public const str PanelCard = nameof(PanelCard);
		public const str PanelHeader = nameof(PanelHeader);
		public const str FieldTitle = nameof(FieldTitle);
		public const str InputBox = nameof(InputBox);
		public const str MainBtn = nameof(MainBtn);
	}

	protected nil Style(){
		var S = Styles;
		var baseFont = UiCfg.Inst.BaseFontSize;

		new Style(x=>x.Is<Border>().Class(Cls.PanelCard))
		.Set(Border.BorderBrushProperty, Brushes.DimGray)
		.Set(Border.BorderThicknessProperty, new Thickness(1))
		.Set(Border.BackgroundProperty, new SolidColorBrush(Color.FromArgb(20, 255, 255, 255)))
		.Set(Border.PaddingProperty, new Thickness(baseFont * 0.65))
		.AddTo(S);

		new Style(x=>x.Is<TextBlock>().Class(Cls.PanelHeader))
		.Set(TextBlock.FontSizeProperty, baseFont * 1.1)
		.AddTo(S);

		new Style(x=>x.Is<TextBlock>().Class(Cls.FieldTitle))
		.Set(TextBlock.FontSizeProperty, baseFont * 0.85)
		.Set(TextBlock.ForegroundProperty, Brushes.Gainsboro)
		.AddTo(S);

		new Style(x=>x.Is<TextBox>().Class(Cls.InputBox))
		.Set(TextBox.BackgroundProperty, new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E)))
		.Set(TextBox.ForegroundProperty, Brushes.White)
		.Set(TextBox.BorderBrushProperty, Brushes.Gray)
		.Set(TextBox.BorderThicknessProperty, new Thickness(1))
		.Set(TextBox.FontSizeProperty, baseFont * 0.95)
		.Set(TextBox.MinHeightProperty, baseFont * 2.0)
		.AddTo(S);

		new Style(x=>x.Is<Button>().Class(Cls.MainBtn))
		.Set(Button.MinHeightProperty, baseFont * 2.3)
		.Set(Button.FontSizeProperty, baseFont)
		.Set(Button.BackgroundProperty, UiCfg.Inst.MainColor ?? Brushes.DodgerBlue)
		.Set(Button.ForegroundProperty, Brushes.White)
		.Set(Button.BorderBrushProperty, Brushes.Transparent)
		.AddTo(S);
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Star),
			]);
			o.RowSpacing = UiCfg.Inst.BaseFontSize * 0.55;
			o.Margin = new Thickness(UiCfg.Inst.BaseFontSize * 0.45);
		});

		Root
		.A(MkFilterPanel())
		.A(MkChartPanel());
		return NIL;
	}

	Control MkFilterPanel(){
		var card = new Border();
		card.Classes.Add(Cls.PanelCard);
		var panel = new StackPanel();
		card.Child = panel;
		panel.Spacing = UiCfg.Inst.BaseFontSize * 0.45;
		panel.A(new TextBlock(), o=>{
			o.Classes.Add(Cls.PanelHeader);
			o.Text = Todo.I18n("Statistics Filters");
		});
		panel.A(MkPrimaryFilterRow());
		panel.A(MkSecondaryFilterRow());
		return card;
	}

	Control MkPrimaryFilterRow(){
		var row = new WrapPanel{
			Orientation = Orientation.Horizontal,
		};

		var txtStart = new TextBox();
		txtStart.Classes.Add(Cls.InputBox);
		txtStart.Width = UiCfg.Inst.BaseFontSize * 10;
		txtStart.CBind<Ctx>(txtStart.PropText, x=>x.TimeStart, Converter: ConvtrTempus.Inst.Iso);
		row.Children.Add(MkField(Todo.I18n("Start"), txtStart));

		var txtEnd = new TextBox();
		txtEnd.Classes.Add(Cls.InputBox);
		txtEnd.Width = UiCfg.Inst.BaseFontSize * 10;
		txtEnd.CBind<Ctx>(txtEnd.PropText, x=>x.TimeEnd, Converter: ConvtrTempus.Inst.Iso);
		row.Children.Add(MkField(Todo.I18n("End"), txtEnd));

		var txtVal = new TextBox();
		txtVal.Classes.Add(Cls.InputBox);
		txtVal.Width = UiCfg.Inst.BaseFontSize * 4.5;
		txtVal.CBind<Ctx>(txtVal.PropText, x=>x.IntervalNoUnit);
		row.Children.Add(MkField(Todo.I18n("Interval"), txtVal));

		var cmbUnit = new ComboBox();
		cmbUnit.Width = UiCfg.Inst.BaseFontSize * 7;
		cmbUnit.ItemsSource = new[]{
			Todo.I18n("Second"),
			Todo.I18n("Minute"),
			Todo.I18n("Hour"),
			Todo.I18n("Day"),
			Todo.I18n("Week"),
			Todo.I18n("Month"),
			Todo.I18n("Year"),
		};
		cmbUnit.CBind<Ctx>(cmbUnit.PropSelectedIndex, x=>x.IntervalUnit);
		row.Children.Add(MkField(Todo.I18n("Unit"), cmbUnit));

		var btn = new OpBtn();
		btn._Button.Classes.Add(Cls.MainBtn);
		btn.BtnContent = Todo.I18n("Count");
		btn.SetExe((Ct)=>Ctx?.GetDataAsy(Ct));
		btn.MinWidth = UiCfg.Inst.BaseFontSize * 7.5;
		btn.Margin = new Thickness(0, UiCfg.Inst.BaseFontSize * 1.5, 0, 0);
		row.Children.Add(btn);
		return row;
	}

	Control MkSecondaryFilterRow(){
		var row = new WrapPanel{
			Orientation = Orientation.Horizontal,
		};

		var txtResult = new TextBox();
		txtResult.Classes.Add(Cls.InputBox);
		txtResult.Width = UiCfg.Inst.BaseFontSize * 8;
		txtResult.CBind<Ctx>(txtResult.PropText, x=>x.LearnResult);
		row.Children.Add(MkField(Todo.I18n("Learn Result"), txtResult));

		var txtPageIdx = new TextBox();
		txtPageIdx.Classes.Add(Cls.InputBox);
		txtPageIdx.Width = UiCfg.Inst.BaseFontSize * 4.5;
		txtPageIdx.CBind<Ctx>(txtPageIdx.PropText, x=>x.PageIdx);
		row.Children.Add(MkField(Todo.I18n("Page"), txtPageIdx));

		var txtPageSize = new TextBox();
		txtPageSize.Classes.Add(Cls.InputBox);
		txtPageSize.Width = UiCfg.Inst.BaseFontSize * 4.5;
		txtPageSize.CBind<Ctx>(txtPageSize.PropText, x=>x.PageSize);
		row.Children.Add(MkField(Todo.I18n("Size"), txtPageSize));
		return row;
	}

	Control MkChartPanel(){
		var card = new Border();
		card.Classes.Add(Cls.PanelCard);
		var grid = new AutoGrid(IsRow: true);
		card.Child = grid.Grid;
		grid.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
		]);
		grid.Grid.RowSpacing = UiCfg.Inst.BaseFontSize * 0.45;

		grid
		.A(new TextBlock(), o=>{
			SummaryText = o;
			o.Classes.Add(Cls.FieldTitle);
			o.FontSize = UiCfg.Inst.BaseFontSize * 0.95;
			o.Foreground = Brushes.LightGray;
		})
		.A(new AvaPlot(), o=>{
			PlotCtrl = o;
			o.MinHeight = UiCfg.Inst.BaseFontSize * 15;
		});
		return card;
	}

	Control MkField(str title, Control editor){
		var panel = new StackPanel{
			Orientation = Orientation.Vertical,
			Spacing = UiCfg.Inst.BaseFontSize * 0.2,
			Margin = new Thickness(0, 0, UiCfg.Inst.BaseFontSize * 0.4, UiCfg.Inst.BaseFontSize * 0.35),
		};
		panel.A(new TextBlock(), o=>{
			o.Classes.Add(Cls.FieldTitle);
			o.Text = title;
		});
		panel.Children.Add(editor);
		return panel;
	}

	void DrawPlot(AvaPlot o){
		if(Ctx is null){
			return;
		}
		o.Plot.Clear();

		if(Ctx.Points.Count == 0){
			o.Refresh();
			return;
		}

		var line = o.Plot.Add.Scatter(Ctx.Points.ToArray(), null);
		line.LineWidth = 2;
		line.MarkerSize = 5;

		var tickPositions = new List<double>();
		var tickLabels = new List<string>();

		var step = Math.Max(1, Ctx.Points.Count / 8);
		for(i32 i = 0; i < Ctx.Points.Count; i += step){
			var ms = Ctx.Points[i].X;
			var dt = DateTimeOffset.FromUnixTimeMilliseconds((i64)ms);
			tickPositions.Add(ms);
			tickLabels.Add(dt.ToString("MM-dd"));
		}

		if(tickPositions.Count > 0){
			o.Plot.Axes.Bottom.TickGenerator =
				new ScottPlot.TickGenerators.NumericManual(tickPositions.ToArray(), tickLabels.ToArray());
		}
		o.Plot.Axes.Bottom.TickLabelStyle.Rotation = 35;

		if(Ctx.Points.Count <= 24){
			for(i32 i = 0; i < Ctx.Points.Count; i++){
				var p = Ctx.Points[i];
				var txt = o.Plot.Add.Text(p.Y.ToString("0"), p.X, p.Y);
				txt.OffsetY = -0.25f;
				txt.LabelAlignment = ScottPlot.Alignment.LowerCenter;
			}
		}

		o.Plot.Axes.AutoScale();
		o.Refresh();
	}

	void UpdateSummary(){
		if(SummaryText is null){
			return;
		}
		if(Ctx is null || Ctx.Points.Count == 0){
			SummaryText.Text = Todo.I18n("No data yet");
			return;
		}

		var first = Ctx.Points[0];
		var last = Ctx.Points[Ctx.Points.Count - 1];
		var start = DateTimeOffset.FromUnixTimeMilliseconds((i64)first.X).ToString("yyyy-MM-dd");
		var end = DateTimeOffset.FromUnixTimeMilliseconds((i64)last.X).ToString("yyyy-MM-dd");
		SummaryText.Text =
			$"{Todo.I18n("Points")}: {Ctx.Points.Count} | {Todo.I18n("Range")}: {start} ~ {end} | {Todo.I18n("Step")}: {Ctx.IntervalNoUnit} {Ctx.IntervalUnit}";
	}
}
