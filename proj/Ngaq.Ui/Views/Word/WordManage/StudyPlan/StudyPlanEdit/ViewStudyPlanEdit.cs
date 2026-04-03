namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanEdit;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterPage;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.WeightArgPage;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.WeightCalculatorPage;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmStudyPlanEdit;

public partial class ViewStudyPlanEdit
	:AppViewBase
{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewStudyPlanEdit(){
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
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Auto),
		]);
		Root.A(MkBody());
		Root.A(MkBottomBar());
		return NIL;
	}

	Control MkBody(){
		var sv = new ScrollViewer();
		var root = new StackPanel{
			Spacing = 10,
			Margin = new Thickness(10),
		};
		sv.Content = root;
		root.Children.Add(MkErrorBar());
		root.Children.Add(MkPoSection());
		return sv;
	}

	Control MkErrorBar(){
		var b = new Border{
			Background = new SolidColorBrush(Color.FromArgb(80, 180, 30, 30)),
			Padding = new Thickness(10, 6),
			IsVisible = false,
		};
		b.CBind<Ctx>(IsVisibleProperty, x=>x.HasError, Mode: BindingMode.OneWay);
		var txt = new TextBlock{
			Foreground = Brushes.White,
		};
		txt.CBind<Ctx>(TextBlock.TextProperty, x=>x.LastError, Mode: BindingMode.OneWay);
		b.Child = txt;
		return b;
	}

	Control MkPoSection(){
		var bdr = new Border{
			BorderBrush = Brushes.DimGray,
			BorderThickness = new Thickness(1),
			Padding = new Thickness(10),
		};
		var sp = new StackPanel{Spacing = 8};
		bdr.Child = sp;

		sp.A(new TextBlock{
			Text = Todo.I18n("PoStudyPlan"),
			FontSize = UiCfg.Inst.BaseFontSize * 1.1,
			FontWeight = FontWeight.SemiBold,
		})
		.A(MkInputRow(Todo.I18n("Id"), CBE.Mk<Ctx>(x=>x.PoIdText, Mode: BindingMode.OneWay), ReadOnly: true))
		.A(MkInputRow(Todo.I18n("Name"), CBE.Mk<Ctx>(x=>x.PoUniqName, Mode: BindingMode.TwoWay)))
		.A(MkInputRow(Todo.I18n("Description"), CBE.Mk<Ctx>(x=>x.PoDescr, Mode: BindingMode.TwoWay), AcceptsReturn: true))
		.A(ToolStudyPlanView.MkInputWithBtnRow(
			Todo.I18n("PreFilter"),
			CBE.Mk<Ctx>(x=>x.PreFilterUniqNameText, Mode: BindingMode.OneWay),
			Todo.I18n("Choose"),
			()=>{
				var view = new ViewPreFilterPage();
				view.Ctx?.SetSelectMode(po=>{
					Ctx?.ApplySelectedPreFilter(po);
					Ctx?.ViewNavi?.Back();
				});
				Ctx?.ViewNavi?.GoTo(ToolView.WithTitle(Todo.I18n("選擇PreFilter"), view));
			},
			ReadOnly: true
		))
		.A(ToolStudyPlanView.MkInputWithBtnRow(
			Todo.I18n("WeightCalculator"),
			CBE.Mk<Ctx>(x=>x.WeightCalculatorUniqNameText, Mode: BindingMode.OneWay),
			Todo.I18n("Choose"),
			()=>{
				var view = new ViewWeightCalculatorPage();
				view.Ctx?.SetSelectMode(po=>{
					Ctx?.ApplySelectedWeightCalculator(po);
					Ctx?.ViewNavi?.Back();
				});
				Ctx?.ViewNavi?.GoTo(ToolView.WithTitle(Todo.I18n("選擇WeightCalculator"), view));
			},
			ReadOnly: true
		))
		.A(ToolStudyPlanView.MkInputWithBtnRow(
			Todo.I18n("WeightArg"),
			CBE.Mk<Ctx>(x=>x.WeightArgUniqNameText, Mode: BindingMode.OneWay),
			Todo.I18n("Choose"),
			()=>{
				var view = new ViewWeightArgPage();
				view.Ctx?.SetSelectMode(po=>{
					Ctx?.ApplySelectedWeightArg(po);
					Ctx?.ViewNavi?.Back();
				});
				Ctx?.ViewNavi?.GoTo(ToolView.WithTitle(Todo.I18n("選擇WeightArg"), view));
			},
			ReadOnly: true
		))
		;
		return bdr;
	}

	Control MkBottomBar(){
		var bar = new AutoGrid(IsRow:false);
		bar.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		bar.A(new OpBtn(), o=>{
			o.Background = new SolidColorBrush(Color.FromRgb(210, 56, 56));
			o._Button.HorizontalContentAlignment = HAlign.Center;
			o.BtnContent = Svgs.DeleteForeverSharp().ToIcon().WithText(Todo.I18n("Delete"));
			o.SetExe((Ct)=>Ctx?.Delete(Ct));
		})
		.A(new OpBtn(), o=>{
			o.Background = UiCfg.Inst.MainColor;
			o._Button.HorizontalContentAlignment = HAlign.Center;
			o.BtnContent = Svgs.FloppyDiskBackFill().ToIcon().WithText(Todo.I18n("Save"));
			o.SetExe((Ct)=>Ctx?.Save(Ct));
		});
		return bar.Grid;
	}

	Control MkInputRow(str Label, IBinding Binding, bool ReadOnly = false, bool AcceptsReturn = false){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		var tb = new TextBox{
			IsReadOnly = ReadOnly,
			AcceptsReturn = AcceptsReturn,
			TextWrapping = AcceptsReturn ? TextWrapping.Wrap : TextWrapping.NoWrap,
			MaxHeight = AcceptsReturn ? 140 : double.PositiveInfinity,
		};
		tb.Bind(TextBox.TextProperty, Binding);
		sp.Children.Add(tb);
		return sp;
	}

}
