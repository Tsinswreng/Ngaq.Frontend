namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
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
		InitDataGrid();
		_ = Ctx?.InitSearchAsy();
	}
	public II18n I = I18n.Inst;
	public partial class Cls{

	}


	protected nil Style(){
		return NIL;
	}


	AutoGrid Root = new(IsRow: true);
	TreeDataGrid? WeightArgGrid;
	FlatTreeDataGridSource<Ctx.RowWeightArg>? GridSource;

	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Auto),
		]);

		Root.A(MkTopBar());
		Root.A(MkGridHost());
		Root.A(MkPageBar());
		return NIL;
	}

	protected Control MkTopBar(){
		var top = new AutoGrid(IsRow:false);
		top.Grid.ColumnDefinitions.AddRange([
			ColDef(7, GUT.Star),
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		var searchBtn = new OpBtn();
		top.A(_TextBox(), o=>{
			o.CBind<Ctx>(
				o.PropText,
				x=>x.Input
			);
			o.KeyBindings.Add(new KeyBinding{
				Gesture = new KeyGesture(Key.Enter),
				Command = new RelayCommand(()=>searchBtn.PerformClick()),
			});
		})
		.A(searchBtn, o=>{
			o.BtnContent = "搜索";
			o.SetExe((Ct)=>Ctx?.InitSearchAsy(Ct)!);
		})
		.A(_Button(), o=>{
			o.Content = "添加";
			o.Click += (s,e)=>Ctx?.OpenDetail();
		});
		return top.Grid;
	}

	protected Control MkGridHost(){
		WeightArgGrid = new TreeDataGrid{
			MinHeight = 280,
		};
		return WeightArgGrid;
	}

	protected Control MkPageBar(){
		var page = new AutoGrid(IsRow:false);
		page.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
		]);
		page.Grid.ColumnSpacing = 8;
		page.A(_Button(), o=>{
			o.Content = "上一頁";
			o.Click += (s,e)=>Ctx?.PrevPage();
		});
		page.A(_TextBlock(), o=>{
			o.Text = "當前頁";
			o.VerticalAlignment = VAlign.Center;
		});
		var goBtn = new Button();
		page.A(_TextBox(), o=>{
			o.Width = 70;
			o.CBind<Ctx>(
				o.PropText,
				x=>x.CurPageInput,
				Mode: BindingMode.TwoWay
			);
			o.KeyBindings.Add(new KeyBinding{
				Gesture = new KeyGesture(Key.Enter),
				Command = new RelayCommand(()=>goBtn.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent))),
			});
		});
		page.A(goBtn, o=>{
			o.Content = "跳轉";
			o.Click += (s,e)=>Ctx?.GoInputPage();
		});
		page.A(_Button(), o=>{
			o.Content = "下一頁";
			o.Click += (s,e)=>Ctx?.NextPage();
		});
		page.A(_TextBlock(), o=>{
			o.CBind<Ctx>(
				TextBlock.TextProperty,
				x=>x.TotalPageText,
				Converter: new ParamFnConvtr<str, str>((x,arg)=>$"共 {x} 頁", (x,arg)=>x)
			);
			o.VerticalAlignment = VAlign.Center;
		});
		return page.Grid;
	}

	protected nil InitDataGrid(){
		if(Ctx is null || WeightArgGrid is null){
			return NIL;
		}
		GridSource = new FlatTreeDataGridSource<Ctx.RowWeightArg>(Ctx.Rows){
			Columns = {
				new CheckBoxColumn<Ctx.RowWeightArg>("", x=>x.IsChecked, (x,v)=>x.IsChecked = v),
				new TextColumn<Ctx.RowWeightArg, str>("序號", x=>x.UiIdx.ToString()),
				new TextColumn<Ctx.RowWeightArg, str>("名稱", x=>x.Name),
				new TextColumn<Ctx.RowWeightArg, str>("修改時間", x=>x.ModifiedTime),
			},
		};
		GridSource.RowSelection?.SelectionChanged += (s,e)=>{
			var row = GridSource.RowSelection?.SelectedItem;
			if(row is null){
				return;
			}
			Ctx?.OpenDetail(row);
		};
		WeightArgGrid.Source = GridSource;
		return NIL;
	}
}
