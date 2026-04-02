namespace Ngaq.Ui.Views.Word.WordManage;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
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
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(9999, GUT.Star),
				RowDef(1, GUT.Star),
			]);
		});

		var menuGrid = new Grid{
			ColumnDefinitions = new ColumnDefinitions("*,*"),
			RowDefinitions = new RowDefinitions("Auto,Auto,Auto"),
			ColumnSpacing = UiCfg.Inst.BaseFontSize * 0.4,
			RowSpacing = UiCfg.Inst.BaseFontSize * 0.6,
			Margin = new Thickness(UiCfg.Inst.BaseFontSize * 0.4),
		};

		void addItem(Control item, int row, int col){
			Grid.SetRow(item, row);
			Grid.SetColumn(item, col);
			menuGrid.Children.Add(item);
		}

		addItem(_Item(Todo.I18n("Dictionary"), ()=>new ViewDictionary(), Svgs.BookA().ToIcon()), 0, 0);
		addItem(_Item(I[K.SearchMyWords], ()=>new ViewSearchWords(), Svgs.Search().ToIcon()), 0, 1);
		addItem(_Item(I[K.AddWords], ()=>new ViewAddWord(), Svgs.Add().ToIcon()), 1, 0);
		addItem(_Item(Todo.I18n("Study Plan"), ()=>new ViewStudyPlan(), Svgs.Schema().ToIcon()), 1, 1);
		addItem(_Item(I[K.BackupEtSync], ()=>new ViewWordSync(), Svgs.SyncCircle().ToIcon()), 2, 0);
		addItem(_Item(Todo.I18n("Statistics"), ()=>new ViewStatistics(), Svgs.ChartLineUpFill().ToIcon()), 2, 1);

		Root.A(menuGrid);


		return NIL;
	}

	protected Control _Item(
		str Title
		,Func<ContentControl> MkTarget
		,Control? Icon = null
	){
		var R = new SwipeLongPressBtn();
		Control? titled = null;
		R.Click += (s,e)=>{
			titled ??= ToolView.WithTitle(Title, MkTarget());
			Ctx?.ViewNavi?.GoTo(titled);
		};
		R.HorizontalContentAlignment = HAlign.Stretch;
		R.MinHeight = UiCfg.Inst.BaseFontSize * 4.8;

		var content = new StackPanel{
			Orientation = Orientation.Vertical,
			Spacing = UiCfg.Inst.BaseFontSize * 0.35,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
		};

		if(Icon is not null){
			Icon.Width = UiCfg.Inst.BaseFontSize * 2.0;
			Icon.Height = UiCfg.Inst.BaseFontSize * 2.0;
			content.Children.Add(Icon);
		}

		var titleText = _TextBlock();
		titleText.Text = Title;
		titleText.FontSize = UiCfg.Inst.BaseFontSize * 1.1;
		titleText.TextAlignment = TextAlignment.Center;
		//titleText.FontWeight = FontWeight.SemiBold;
		content.Children.Add(titleText);

		R.Content = new Border{
			Padding = new Thickness(
				UiCfg.Inst.BaseFontSize * 0.7,
				UiCfg.Inst.BaseFontSize * 0.55
			),
			BorderThickness = new Thickness(1.5),
			BorderBrush = Brushes.Gray,
			Background = new SolidColorBrush(Color.FromArgb(28, 255, 255, 255)),
			Child = content,
		};

		return R;
	}


}
