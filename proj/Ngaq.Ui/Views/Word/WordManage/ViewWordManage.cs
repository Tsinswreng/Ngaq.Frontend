namespace Ngaq.Ui.Views.Word.WordManage;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Ngaq.Ui.Views.Word.WordManage.SearchWords;
using Ngaq.Ui.Views.Word.WordManage.Statistics;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan;
using Tsinswreng.Avln.StrokeText;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ctx = Ngaq.Ui.Infra.ViewModelBase;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;
using LK = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;


public partial class ViewWordManage
	:AppViewBase
{

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

		Root.A(new ScrollViewer(), Sv=>{
			Sv.SetContent(new Grid(), g=>{
				g.ColumnDefinitions = new ColumnDefinitions("*,*");
				g.RowDefinitions = new RowDefinitions("Auto,Auto");
				g.ColumnSpacing = UiCfg.Inst.BaseFontSize * 0.4;
				g.RowSpacing = UiCfg.Inst.BaseFontSize * 0.6;
				g.Margin = new Thickness(UiCfg.Inst.BaseFontSize * 0.4);

				void addItem(Control item, int row, int col){
					Grid.SetRow(item, row);
					Grid.SetColumn(item, col);
					g.A(item);
				}

				addItem(_Item(I[K.UserWordManage], ()=>new ViewSearchWords(), Svgs.UserWordLib().ToIcon()), 0, 0);
				addItem(_Item(I[K.AddWords], ()=>new ViewAddWord(), Svgs.Add().ToIcon()), 0, 1);
				addItem(_Item(I[LK.StudyPlan], ()=>new ViewStudyPlan(), Svgs.StudyPlan().ToIcon()), 1, 0);
				addItem(_Item(I[LK.Statistics], ()=>new ViewStatistics(), Svgs.Statistics().ToIcon()), 1, 1);
			});
		});
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
			ViewNavi?.GoTo(titled);
		};
		R.HorizontalContentAlignment = HAlign.Stretch;
		R.MinHeight = UiCfg.Inst.BaseFontSize * 4.8;

		R.SetContent(new Border(), b=>{
			b.Padding = new Thickness(
				UiCfg.Inst.BaseFontSize * 0.7,
				UiCfg.Inst.BaseFontSize * 0.55
			);
			//b.BorderThickness = new Thickness(1.5);
			//b.BorderBrush = Brushes.Gray;
			//b.Background = new SolidColorBrush(Color.FromArgb(28, 255, 255, 255));
			b.SetChild(new StackPanel(), content=>{
				{var o = content;
					o.Orientation = Orientation.Vertical;
					o.Spacing = UiCfg.Inst.BaseFontSize * 0.35;
					o.HorizontalAlignment = HorizontalAlignment.Center;
					o.VerticalAlignment = VerticalAlignment.Center;
				}
				if(Icon is not null){
					content.A(Icon, o=>{
						o.Width = UiCfg.Inst.BaseFontSize * 2.0;
						o.Height = UiCfg.Inst.BaseFontSize * 2.0;
					});
				}
				content.A(new TextBlock(), o=>{
					o.Text = Title;
					o.FontSize = UiCfg.Inst.BaseFontSize * 1.1;
					o.TextAlignment = TextAlignment.Center;
				});
			});
		});

		return R;
	}


}
