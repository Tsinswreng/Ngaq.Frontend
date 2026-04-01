namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

using Ctx = VmPreFilterJsonEdit;

/// <summary>
/// PoPreFilter JSON 專用編輯視圖。
/// </summary>
public class ViewPreFilterJsonEdit: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewPreFilterJsonEdit(){
		Ctx = App.DiOrMk<Ctx>();
		Render();
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Auto),
		]);
		Root.A(MkErrorBar());
		Root.A(JsonText(), o=>{
			o.CBind<Ctx>(o.PropText, x=>x.PoPreFilterJson, Mode: BindingMode.TwoWay);
		});
		Root.A(MkBottomBar());
		return NIL;
	}

	Control MkErrorBar(){
		var b = new Border{
			Background = new SolidColorBrush(Color.FromArgb(80, 180, 30, 30)),
			Padding = new Thickness(10, 6),
			IsVisible = false,
			Margin = new Thickness(10, 10, 10, 4),
		};
		b.CBind<Ctx>(IsVisibleProperty, x=>x.HasError, Mode: BindingMode.OneWay);
		var txt = new TextBlock{
			Foreground = Brushes.White,
		};
		txt.CBind<Ctx>(TextBlock.TextProperty, x=>x.LastError, Mode: BindingMode.OneWay);
		b.Child = txt;
		return b;
	}

	Control MkBottomBar(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		g.A(new Button(), o=>{
			o.Content = "Back";
			o.Click += (s,e)=>Ctx?.ViewNavi?.Back();
		});
		g.A(new Button(), o=>{
			o.Content = Svgs.FloppyDiskBackFill().ToIcon().WithText(" Save");
			o.Background = UiCfg.Inst.MainColor;
			o.Click += async (s,e)=>{
				if(Ctx is null){
					return;
				}
				await Ctx.Save();
			};
		});
		g.A(new Button(), o=>{
			o.Content = Svgs.DeleteForeverSharp().ToIcon().WithText(" Delete");
			o.Background = new SolidColorBrush(Color.FromRgb(210, 56, 56));
			o.Click += async (s,e)=>{
				if(Ctx is null){
					return;
				}
				await Ctx.Delete();
			};
		});
		return g.Grid;
	}

	TextBox JsonText(){
		var box = new TextBox{
			AcceptsReturn = true,
			AcceptsTab = true,
			TextWrapping = TextWrapping.Wrap,
			HorizontalAlignment = HAlign.Stretch,
			VerticalAlignment = VAlign.Stretch,
			Margin = new Thickness(10, 4, 10, 10),
		};
		box.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
		return box;
	}
}
