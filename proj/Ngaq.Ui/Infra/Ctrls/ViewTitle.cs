namespace Ngaq.Ui.Infra.Ctrls;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = ViewModelBase;
public partial class ViewTitle
	:UserControl
{

	public Border BdrTitle{get;set;} = new();
	public ContentControl Body{get;set;} = new ContentControl();
	public ContentControl Title{get;set;} = new ContentControl();



	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewTitle(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Star),
			]);
		});
		{{
			AutoGrid TitleBar = new(IsRow: false);
			BdrTitle.Child=TitleBar.Grid;
			{
				var o = TitleBar;
				o.Grid.ColumnDefinitions.AddRange([
					ColDef(1, GUT.Auto),
					ColDef(1, GUT.Star),
					ColDef(1, GUT.Auto),
				]);
			}
			Root.AddInit(BdrTitle, o=>{

			});
			{{
				TitleBar.AddInit(_Button(), o=>{
					o.Content = "←";
					o.Click += (s,e)=>{
						Ctx?.ViewNavi?.Back();
					};
					o.Background = Brushes.Transparent;
				});
				TitleBar.AddInit(Title, o=>{

				});
				TitleBar.Add();
			}}//TitleBar
		}}//Root
		Root.AddInit(Body);
		return NIL;
	}


}
