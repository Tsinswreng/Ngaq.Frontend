namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.SetCurStudyPlan;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Core.Shared.StudyPlan.Models;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanEdit;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmSetCurStudyPlan;

/// <summary>
/// 設置當前學習方案頁。
/// 頁面打開後即讀取後端當前學習方案，並直接顯示 StudyPlan 編輯頁。
/// </summary>
public partial class ViewSetCurStudyPlan
	:AppViewBase
{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSetCurStudyPlan(){
		Ctx = App.DiOrMk<Ctx>();
		if(Ctx is not null){
			Ctx.OnLoadedCurStudyPlan += ApplyCurStudyPlan;
		}
		Style();
		Render();
		Loaded += async(s,e)=>{
			if(HasAutoLoaded){
				return;
			}
			HasAutoLoaded = true;
			_ = Ctx?.LoadCurStudyPlan(default);
		};
	}

	public II18n I = I18n.Inst;
	public partial class Cls{
		public static str FullStretch = nameof(FullStretch);
	}

	protected nil Style(){
		var S = Styles;
		new Style(
			x=>x.Is<Control>()
			.Class(Cls.FullStretch)
		)
		.Set(HorizontalAlignmentProperty, HAlign.Stretch)
		.Set(VerticalAlignmentProperty, VAlign.Stretch)
		.AddTo(S);
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);
	ViewStudyPlanEdit EditView = new();
	bool HasAutoLoaded = false;

	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
		]);
		Root.A(MkTopBar());
		Root.A(MkBody());
		return NIL;
	}

	Control MkTopBar(){
		var top = new AutoGrid(IsRow:false);
		top.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
		]);
		top.A(new TextBlock(), o=>{
			o.Text = Todo.I18n("設置當前學習方案");
			o.VerticalAlignment = VAlign.Center;
			o.Margin = new Thickness(8, 0);
		})
		.A(new OpBtn(), o=>{
			o.Classes.Add(Cls.FullStretch);
			o.BtnContent = Todo.I18n("Reload");
			o.SetExe((Ct)=>Ctx?.LoadCurStudyPlan(Ct)!);
		})
		.A(new OpBtn(), o=>{
			o.Classes.Add(Cls.FullStretch);
			o.BtnContent = Todo.I18n("RestoreBuiltin");
			o.SetExe((Ct)=>Ctx?.RestoreBuiltin(Ct)!);
		});
		return top.Grid;
	}

	Control MkBody(){
		var host = new AutoGrid(IsRow:true);
		host.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
		]);
		host.A(MkErrorBar());
		host.A(EditView, o=>{
			o.Classes.Add(Cls.FullStretch);
		});
		return host.Grid;
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

	/// <summary>
	/// 把後端返回的當前 BoStudyPlan 灌入編輯頁。
	/// </summary>
	void ApplyCurStudyPlan(BoStudyPlan? Bo){
		EditView.Ctx?.SetCreateMode(Bo?.PoStudyPlan is null);
		EditView.Ctx?.FromBoStudyPlan(Bo);
	}
}
