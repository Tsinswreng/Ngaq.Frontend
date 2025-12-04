namespace Ngaq.Ui.Views.Word.WordManage.Statistics;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui.Converters;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using ScottPlot.Avalonia;
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
		Ctx = App.GetSvc<Ctx>();
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
		).Attach(Styles);
		return NIL;
	}


	void InitOptPanel(Panel P){
		P.AddInit(new TextBlock(), o=>{
			o.Text = "StartTime:";//TODO i18n
		})
		.AddInit(new TextBox(), o=>{
			o.Bind(
				o.PropText
				,CBE.Mk<Ctx>(x=>x.TimeStart, Converter: ConvtrTempus.Inst.Iso)
			);
		})
		.AddInit(new TextBlock(), o=>{
			o.Text = "EndTime:";//TODO i18n
		})
		.AddInit(new TextBox(), o=>{
			o.Bind(
				o.PropText
				,CBE.Mk<Ctx>(x=>x.TimeEnd, Converter: ConvtrTempus.Inst.Iso)
			);
		})
		.AddInit(new TextBlock(), o=>{
			o.Text = "Interval:";//TODO i18n
		})
		.AddInit(new TextBox(), o=>{
			o.Bind(
				o.PropText
				,CBE.Mk<Ctx>(x=>x.IntervalNoUnit)
			);
		})
		.AddInit(new TextBlock(), o=>{
			o.Text = "Unit";//TODO i18n
		})
		.AddInit(new ComboBox(), o=>{
			o.Items.Add("Second");//TODO i18n
			o.Items.Add("Minute");//TODO i18n
			o.Items.Add("Hour");//TODO i18n
			o.Items.Add("Day");//TODO i18n
			o.Items.Add("Week");//TODO i18n
			o.Items.Add("Month");//TODO i18n
			o.Items.Add("Year");//TODO i18n
			o.SelectedIndex = (i32)Ctx.ETimeUnit.Day;
			o.Bind(o.PropSelectedIndex, CBE.Mk<Ctx>(x=>x.IntervalUnit));
		})

		.AddInit(new TextBlock(), o=>{
			o.Text = "Learn Result";//TODO i18n
		})
		.AddInit(new TextBox(), o=>{
			o.Bind(
				o.PropText
				,CBE.Mk<Ctx>(x=>x.LearnResult)
			);
		})
		.AddInit(new TextBlock(), o=>{
			o.Text = "PageIndex";
		})
		.AddInit(new TextBox(), o=>{
			o.Bind(
				o.PropText
				,CBE.Mk<Ctx>(x=>x.PageIdx)
			);
		})
		.AddInit(new TextBlock(), o=>{
			o.Text = "PageSize";
		})
		.AddInit(new TextBox(), o=>{
			o.Bind(
				o.PropText
				,CBE.Mk<Ctx>(x=>x.PageSize)
			);
		})
		.AddInit(new OpBtn(), o=>{
			o._Button.StretchCenter();
			o.BtnContent = "Count";//TODO i18n
			o.SetExt((Ct)=>Ctx?.GetDataAsy(Ct));
		});
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
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Star),//cfgPanel
				RowDef(1, GUT.Auto),//splitter
				RowDef(1, GUT.Star),//graph
			]);
		});
		Root.AddInit(new ScrollViewer(), Sv=>{
			var grid = new AutoGrid(IsRow:true);
			Sv.ContentInit(grid.Grid, o=>{
				o.RowDefinitions.AddRange([
					RowDef(1, GUT.Auto),
				]);
			});
			grid
			.AddInit(new StackPanel(), Sp=>{
				InitOptPanel(Sp);
			});
		})
		.AddInit(new GridSplitter(), o=>{
			o.GrayBarWith3Dots();
		})
		.AddInit(new AvaPlot(), o=>{
			//o.MinHeight = 200;
			//不雅。未慮況芝Ctx引用變
			Ctx?.GraphChanged += (s,e)=>{
				DrawPlot(o);
			};
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
