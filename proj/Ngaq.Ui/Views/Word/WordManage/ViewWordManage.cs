namespace Ngaq.Ui.Views.Word.WordManage;

using Ngaq.Ui.Infra.I18n;
using Avalonia.Controls;


using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Ngaq.Ui.Views.Word.WordManage.SearchWords;
using Ngaq.Ui.Views.Word.WordManage.WordSync;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = Ngaq.Ui.Infra.ViewModelBase;
using K = Ngaq.Ui.Infra.I18n.ItemsUiI18n.Library;
using Ngaq.Ui.Views.Word.WordManage.Statistics;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Views.Dictionary;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.StrokeText;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

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
		return NIL;
	}

	public AutoGrid Root = new(IsRow:true);

	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(9999, GUT.Star),
				RowDef(1, GUT.Star),
			]);
		});
		Root.AddInit(_StackPanel(), Sp=>{
			Sp.AddInit(_Item("Dictionary", new ViewDictionary(), Svgs.BookA.ToIcon()))
			.AddInit(_Item(I[K.SearchMyWords], new ViewSearchWords(), Svgs.Search.ToIcon()))
			.AddInit(_Item(I[K.AddWords], new ViewAddWord(), Svgs.Add.ToIcon()))
			.AddInit(_Item(I[K.BackupEtSync], new ViewWordSync(), Svgs.SyncCircle.ToIcon()))
			.AddInit(_Item("Statistics", new ViewStatistics(), Svgs.ChartLineUpFill.ToIcon()))//TODO i18n

			;
			var Txt = new StrokeTextEdit{
				TextWrapping = TextWrapping.Wrap,
				UseVirtualizedRender = true
			};
			// var Txt = new TextBox{
			// 	AcceptsReturn = true,
			// 	TextWrapping = TextWrapping.Wrap,
			// };
			var Log = App.GetSvc<ILogger>();
			Sp.AddInit(new Button(), o=>{
				o.Click += ((s,e)=>{
					var l = new List<str>();
					for(var i = 0; i < 5000; i++){
						l.Add("0");
					}
					var t = str.Join("", l);
					var sw = Stopwatch.StartNew();
					Txt.Text=t;
					sw.Stop();
					Log.LogInformation("Long:"+sw.ElapsedMilliseconds);
					return ;
				});
				o.Content = "測試長";
			})
			.AddInit(new Button(), o=>{
				o.Content = "測試短";
				o.Click+= (s,e)=>{
					var sw = Stopwatch.StartNew();
					Txt.Text = "1234567890";
					sw.Stop();
					Log.LogInformation("Short:"+sw.ElapsedMilliseconds);
				};
			})
			.AddInit(new ScrollViewer(), Sv=>{
				Sv.ContentInit(new Border(), Bd=>{
					Bd.Height = 300;
					Bd.Child = Txt;
				});
			})
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
		R.HorizontalContentAlignment = HAlign.Left;
		if(Icon is null){
			R.ContentInit(_TextBlock(), o=>{
				o.Text = Title;
			});
		}else{
			var G = new AutoGrid(IsRow:false);
			R.ContentInit(G.Grid, o=>{
				o.ColumnDefinitions.AddRange([
					ColDef(1, GUT.Auto),
					ColDef(UiCfg.Inst.BaseFontSize, GUT.Pixel),
					ColDef(1, GUT.Auto),
				]);
				G.AddInit(Icon);
				G.Add();
				G.AddInit(_TextBlock(), t=>{
					t.Text = Title;
					t.FontSize = UiCfg.Inst.BaseFontSize*1.2;
				});
			});
		}

		return R;
	}


}
