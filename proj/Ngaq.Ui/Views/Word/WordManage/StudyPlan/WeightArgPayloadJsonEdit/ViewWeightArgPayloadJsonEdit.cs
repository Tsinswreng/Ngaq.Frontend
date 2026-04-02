namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.WeightArgPayloadJsonEdit;

using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

using Ctx = VmWeightArgPayloadJsonEdit;

/// WeightArg Payload(JSON) 編輯視圖。
public class ViewWeightArgPayloadJsonEdit: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWeightArgPayloadJsonEdit(){
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
			o.CBind<Ctx>(o.PropText, x=>x.PayloadJson, Mode: BindingMode.TwoWay);
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
		]);
		g.A(new Button(), o=>{
			o.Content = Todo.I18n("Back");
			o.Click += (s,e)=>Ctx?.ViewNavi?.Back();
		});
		g.A(new OpBtn(), o=>{
			o.BtnContent = Svgs.FloppyDiskBackFill().ToIcon().WithText(Todo.I18n("Apply"));
			o.Background = UiCfg.Inst.MainColor;
			o.SetExe((Ct)=>{
				Ctx?.ApplyAndBack();
				return Task.FromResult(NIL);
			});
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
