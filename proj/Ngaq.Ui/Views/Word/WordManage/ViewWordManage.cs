namespace Ngaq.Ui.Views.Word.WordManage;

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Dictionary;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Ngaq.Ui.Views.Word.WordManage.SearchWords;
using Ngaq.Ui.Views.Word.WordManage.Statistics;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan;
using Ngaq.Ui.Views.Word.WordManage.WordSync;
using Tsinswreng.Avln.StrokeText;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = Ngaq.Ui.Infra.ViewModelBase;
using K = Ngaq.Ui.Infra.I18n.ItemsUiI18n.Library;

public partial class ViewWordManage
	:UserControl
{

	public II18n I = I18n.Inst;
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordManage(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		var S = Styles;
		new Style(x=>x.Is<Button>())
		.Set(Button.BackgroundProperty, Brushes.Transparent)
		.AddTo(S);
		return NIL;
	}

	public AutoGrid Root = new(IsRow:true);

	protected nil Render(){
		this.InitContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(9999, GUT.Star),
				RowDef(1, GUT.Star),
			]);
		});
		Root.A(_StackPanel(), Sp=>{
			Sp.A(_Item(Todo.I18n("Dictionary"), new ViewDictionary(), Svgs.BookA().ToIcon()))
			.A(_Item(I[K.SearchMyWords], new ViewSearchWords(), Svgs.Search().ToIcon()))
			.A(_Item(I[K.AddWords], new ViewAddWord(), Svgs.Add().ToIcon()))
			.A(_Item(Todo.I18n("Study Plan"), new ViewStudyPlan(), Svgs.Schema().ToIcon()))
			.A(_Item(I[K.BackupEtSync], new ViewWordSync(), Svgs.SyncCircle().ToIcon()))
			.A(_Item(Todo.I18n("Statistics"), new ViewStatistics(), Svgs.ChartLineUpFill().ToIcon()))
			;
		});


		return NIL;
	}

	protected Control _Item(
		str Title
		,ContentControl Target
		,Control? Icon = null
	){
		var R = new SwipeLongPressBtn();
		var titled = ToolView.WithTitle(Title, Target);
		R.Click += (s,e)=>{
			Ctx?.ViewNavi?.GoTo(titled);
		};
		R.HCAlign(x=>x.Left);
		if(Icon is null){
			R.InitContent(_TextBlock(), o=>{
				o.Text = Title;
			});
		}else{
			var G = new AutoGrid(IsRow:false);
			R.InitContent(G.Grid, o=>{
				o.ColumnDefinitions.AddRange([
					ColDef(1, GUT.Auto),
					ColDef(UiCfg.Inst.BaseFontSize, GUT.Pixel),
					ColDef(1, GUT.Auto),
				]);
				G.A(Icon);
				G.Add();
				G.A(_TextBlock(), t=>{
					t.Text = Title;
					t.FontSize = UiCfg.Inst.BaseFontSize*1.2;
				});
			});
		}

		return R;
	}


}
