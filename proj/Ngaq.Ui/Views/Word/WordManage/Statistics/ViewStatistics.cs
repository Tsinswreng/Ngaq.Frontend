namespace Ngaq.Ui.Views.Word.WordManage.Statistics;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Core.Infra;
using Ngaq.Ui.Components.PageBar;
using Ngaq.Ui.Components.TempusBox;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using ScottPlot.Avalonia;
using System.Linq.Expressions;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmStatistics;
public partial class ViewStatistics
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		protected set{DataContext = value;}
	}

	public ViewStatistics(){
		//Ctx = Ctx.Mk();
		Ctx = App.GetRSvc<Ctx>();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		new Style(x=>
			x.Is<TextBox>()
		).Set(
			BackgroundProperty
			,new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20))
		).AddTo(Styles);
		return NIL;
	}


	Control MkLabeledRow(str Title, Control ValueCtrl){
		var row = new AutoGrid(IsRow: false);
		row.Grid.ColumnDefinitions.AddRange([
			ColDef(3, GUT.Star),
			ColDef(7, GUT.Star),
		]);
		row.A(new TextBlock(), o=>{
			o.Text = Title;
			o.VAlign(x=>x.Center);
		})
		.A(ValueCtrl);
		return row.Grid;
	}

	Control MkTimeBoxRow(str Title, Expression<Func<Ctx, object?>> Expr){
		var box = new TempusBox();
		box.Bind(
			TempusBox.TempusProperty,
			CBE.Mk<Ctx>(Expr, Mode: BindingMode.TwoWay)
		);
		return MkLabeledRow(Title, box);
	}

	Control MkIntervalRow(){
		var row = new AutoGrid(IsRow: false);
		row.Grid.ColumnDefinitions.AddRange([
			ColDef(3, GUT.Star),
			ColDef(4, GUT.Star),
			ColDef(4, GUT.Star),
		]);
		row.A(new TextBlock(), o=>{
			o.Text = Todo.I18n("Interval");
			o.VAlign(x=>x.Center);
		})
		.A(new TextBox(), o=>{
			o.CBind<Ctx>(o.PropText, x=>x.IntervalNoUnit);
		})
		.A(new ComboBox(), o=>{
			o.ItemsSource = new str[]{
				Todo.I18n("Second"),
				Todo.I18n("Minute"),
				Todo.I18n("Hour"),
				Todo.I18n("Day"),
				Todo.I18n("Week"),
				Todo.I18n("Month"),
				Todo.I18n("Year"),
			};
			o.CBind<Ctx>(o.PropSelectedIndex, x=>x.IntervalUnit, Mode: BindingMode.TwoWay);
		});
		return row.Grid;
	}

	void InitOptPanel(Panel P){
		P.A(MkTimeBoxRow(Todo.I18n("StartTime"), x=>x.TimeStart))
		.A(MkTimeBoxRow(Todo.I18n("EndTime"), x=>x.TimeEnd))
		.A(MkIntervalRow())
		.A(MkLabeledRow(Todo.I18n("Learn Result"), new ComboBox{
			Init = o=>{
				o.ItemsSource = Ctx.LearnResultOptions;
				o.CBind<Ctx>(o.PropSelectedIndex, x=>x.LearnResultIndex, Mode: BindingMode.TwoWay);
			}
		}))
		.A(new OpBtn(), o=>{
			o._Button.StretchCenter();
			o.BtnContent = Todo.I18n("Count");
			o.SetExe((Ct)=>Ctx?.GetDataAsy(Ct));
		});
	}

	Control MkPageBar(){
		var view = new ViewPageBar();
		if(Ctx is not null){
			view.Ctx = Ctx.PageBar;
		}
		return view;
	}

	void DrawPlot(AvaPlot o){
		if (Ctx is null){return;}
		// 1. 先清画布，画散点
		o.Plot.Clear();
		o.Plot.Add.Scatter(Ctx.Points, null);

		// 2. 生成底部轴的自定义刻度（Unix ms -> MM-dd）
		var tickPositions = new List<double>();
		var tickLabels    = new List<string>();

		for (int i = 0; i < Ctx.Points.Count; ++i){
			double ms = Ctx.Points[i].X;
			var dt = DateTimeOffset.FromUnixTimeMilliseconds((long)ms);
			tickPositions.Add(ms);
			tickLabels.Add(dt.ToString("MM-dd"));
		}

		o.Plot.Axes.Bottom.TickGenerator =
			new ScottPlot.TickGenerators.NumericManual(tickPositions.ToArray(), tickLabels.ToArray());

		// 3. 刻度文字竖排
		o.Plot.Axes.Bottom.TickLabelStyle.Rotation = 90;

		// 4. 在每个散点上方标注 Y 值（只做一次）
		for (int i = 0; i < Ctx.Points.Count; i++){
			var p  = Ctx.Points[i];
			var txt = o.Plot.Add.Text(p.Y.ToString("0"), p.X, p.Y);
			txt.OffsetY        = -0.3f;                          // 往上挪一点
			txt.LabelAlignment = ScottPlot.Alignment.LowerCenter;
		}
		// 5. 一次性刷新
		o.Refresh();
		o.Plot.Axes.AutoScale();
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(7, GUT.Star),//結果
				RowDef(5, GUT.Star),//查詢條件
			]);
		});
		var resultPanel = new AutoGrid(IsRow: true);
		resultPanel.Grid.RowDefinitions.AddRange([
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Auto),
			]);
		resultPanel
			.A(new AvaPlot(), o=>{
				// 圖表只監聽一次事件，資料更新後統一重繪。
				Ctx?.GraphChanged += (s,e)=>{
					DrawPlot(o);
				};
			})
			.A(MkPageBar(), o=>{
				o.HAlign(x=>x.Center);
			});
		Root.A(resultPanel.Grid)
		.A(new ScrollViewer(), sv=>{
			var grid = new AutoGrid(IsRow: true);
			sv.SetContent(grid.Grid, o=>{
				o.RowDefinitions.AddRange([
					RowDef(1, GUT.Auto),
				]);
			});
			grid
			.A(new StackPanel(), Sp=>{
				Sp.Margin = new Thickness(8);
				Sp.Spacing = 8;
				InitOptPanel(Sp);
			});
		})
		;
		return NIL;
	}


}


#if false
double[] dataX = { 1, 2, 3, 4, 5 };
				double[] dataY = { 1, 4, 9, 16, 25 };
				o.Plot.Add.Scatter(dataX, dataY);
				// 2. 逐个加标签
				for (int i = 0; i < dataX.Length; i++){
					var txt = o.Plot.Add.Text(dataY[i].ToString("0"), dataX[i], dataY[i]);
					txt.OffsetY = (f32)(-0.3);          // 往上挪一点，避免压住点
					txt.LabelAlignment = ScottPlot.Alignment.LowerCenter; // 文字底部居中对着散点
				}
#endif
