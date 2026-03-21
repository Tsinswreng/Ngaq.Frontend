namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmStudyPlan;

public partial class ViewStudyPlan
	:AppViewBase
{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewStudyPlan(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls{}

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(9, GUT.Star),
		]);

		Root
		.AddInit(new TextBlock(), o=>{
			o.Margin = new Thickness(8,8,8,4);
			o.Text = "StudyPlan 模塊入口（移動端）";
			o.FontWeight = FontWeight.Bold;
		})
		.AddInit(new ScrollViewer(), sv=>{
			sv.Margin = new Thickness(8,0,8,8);
			sv.Content = MkEntryList();
		})
		;

		return NIL;
	}

	Control MkEntryList(){
		var sp = new StackPanel{ Spacing = 8 };
		sp.Children.Add(MkNavBtn("PreFilter 查詢頁", ()=>new ViewPreFilterQuery()));
		sp.Children.Add(MkNavBtn("WeightCalculator 查詢頁", ()=>new ViewWeightCalculatorQuery()));
		sp.Children.Add(MkNavBtn("WeightArg 查詢頁", ()=>new ViewWeightArgQuery()));
		sp.Children.Add(MkNavBtn("StudyPlan 查詢頁", ()=>new ViewStudyPlanQuery()));
		sp.Children.Add(new Border{ Height = 8 });
		sp.Children.Add(new TextBlock{
			Text = "說明：每個實體都拆成『分頁查詢頁 + 編輯/詳情頁』，StudyPlan 編輯頁中完成裝配。",
			TextWrapping = TextWrapping.Wrap,
			Foreground = Brushes.LightGray,
		});
		return sp;
	}

	Button MkNavBtn(str text, Func<Control> mk){
		var b = new Button{
			Content = text,
			MinHeight = 40,
			HorizontalContentAlignment = HAlign.Left,
		};
		b.Click += (s,e)=>{
			var view = mk();
			Ctx?.ViewNavi?.GoTo(ToolView.WithTitle(text, view));
		};
		return b;
	}
}
