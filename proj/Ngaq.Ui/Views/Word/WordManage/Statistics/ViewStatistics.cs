namespace Ngaq.Ui.Views.Word.WordManage.Statistics;

using Avalonia.Controls;
using Avalonia.Media;
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
		return NIL;
	}


	protected nil Render(){
		this.ContentInit(new ScrollViewer(), Sv=>{
			var grid = new AutoGrid(IsRow:true);
			Sv.ContentInit(grid.Grid, o=>{
				o.RowDefinitions.AddRange([
					RowDef(1, GUT.Auto),
					RowDef(1, GUT.Auto),//splitter
					RowDef(1, GUT.Star),
				]);
			});
			grid
			.AddInit(new StackPanel(), Sp=>{
				Sp
				.AddInit(new TextBlock(), o=>{
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
						,CBE.Mk<Ctx>(x=>x.TimeInterval, Converter: ConvtrTempus.Inst.Int64)
					);
				})
				.AddInit(new TextBlock(), o=>{
					o.Text = "Unit";//TODO i18n
				})
				.AddInit(new ComboBox(), o=>{
					o.Items.Add("Day");//TODO i18n
					o.Items.Add("Week");//TODO i18n
					o.Items.Add("Month");//TODO i18n
					o.Items.Add("Year");//TODO i18n
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
			});

			grid
			//.AddInit(new GridSplitter(), o=>{})//效不佳、會使scrollViewer失效
			.Add();
			grid
			.AddInit(new AvaPlot(), o=>{
				o.MinHeight = 200;
				double[] dataX = { 1, 2, 3, 4, 5 };
				double[] dataY = { 1, 4, 9, 16, 25 };
				o.Plot.Add.Scatter(dataX, dataY);
				// 2. 逐个加标签
				for (int i = 0; i < dataX.Length; i++){
					var txt = o.Plot.Add.Text(dataY[i].ToString("0"), dataX[i], dataY[i]);
					txt.OffsetY = (f32)(-0.3);          // 往上挪一点，避免压住点
					txt.LabelAlignment = ScottPlot.Alignment.LowerCenter; // 文字底部居中对着散点
				}

				//不雅。未慮況芝Ctx引用變
				Ctx?.GraphChanged += (s,e)=>{
					if(Ctx is null){return;}
					o.Plot.Clear();
					o.Plot.Add.Scatter(Ctx.Points, null);
					o.Refresh();
				};
//avaPlot1.Plot.Add.Scatter(dataX, dataY);
//avaPlot1.Refresh();
			})

			;
		});
		return NIL;
	}


}
