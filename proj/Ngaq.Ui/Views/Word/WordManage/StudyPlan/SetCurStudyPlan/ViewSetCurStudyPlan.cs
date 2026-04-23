namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.SetCurStudyPlan;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanEdit;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.StudyPlanPage;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ctx = VmSetCurStudyPlan;using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// 設置當前學習方案頁。
/// 頁面打開後即讀取後端當前學習方案，僅展示關鍵字段並支持重新選擇。
public partial class ViewSetCurStudyPlan
	:AppViewBase
{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSetCurStudyPlan(){
		Ctx = App.DiOrMk<Ctx>();
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
			o.Text = I[K.SetCurrentStudyPlan];
			o.VerticalAlignment = VAlign.Center;
			o.Margin = new Thickness(8, 0);
		})
		.A(new OpBtn(), o=>{
			o.Classes.Add(Cls.FullStretch);
			o.BtnContent = I[K.Reload];
			o.SetExe((Ct)=>Ctx?.LoadCurStudyPlan(Ct)!);
		})
		.A(new OpBtn(), o=>{
			o.Classes.Add(Cls.FullStretch);
			o.BtnContent = I[K.RestoreBuiltin];
			o.SetExe((Ct)=>Ctx?.RestoreBuiltin(Ct)!);
		});
		return top.Grid;
	}

	Control MkBody(){
		var host = new AutoGrid(IsRow:true);
		host.Grid.RowDefinitions.AddRange([RowDef(1, GUT.Star)]);
		host.A(MkFieldsPanel());
		return host.Grid;
	}


	/// 當前方案字段展示區。

	Control MkFieldsPanel(){
		var sv = new ScrollViewer();
		var root = new StackPanel{
			Spacing = 10,
			Margin = new Thickness(10),
		};
		sv.Content = root;

		var bdr = new Border{
			BorderBrush = Brushes.DimGray,
			BorderThickness = new Thickness(1),
			Padding = new Thickness(10),
		};
		root.Children.Add(bdr);

		var sp = new StackPanel{
			Spacing = 8,
		};
		bdr.Child = sp;

		sp.A(MkInputRow(I[K.Id], CBE.Mk<Ctx>(x=>x.CurId, Mode: BindingMode.OneWay), ReadOnly: true))
		.A(MkInputRow(I[K.UniqName], CBE.Mk<Ctx>(x=>x.CurUniqName, Mode: BindingMode.OneWay), ReadOnly: true))
		.A(MkInputRow(I[K.Descr], CBE.Mk<Ctx>(x=>x.CurDescr, Mode: BindingMode.OneWay), ReadOnly: true, AcceptsReturn: true))
		.A(new OpBtn(), o=>{
			o.BtnContent = I[K.Select];
			o.SetExe((Ct)=>ChooseAndApplyStudyPlan(Ct));
		})
		.A(new Button(), o=>{
			o.Content = I[K.Edit];
			o.Click += (s,e)=>{
				var po = Ctx?.CurPoStudyPlan;
				if(po is null){
					Ctx?.ShowDialog(I[K.NoEditableStudyPlan]);
					return;
				}
				var view = new ViewStudyPlanEdit();
				view.Ctx?.SetCreateMode(false);
				view.Ctx?.FromPoStudyPlan(po);
				ViewNavi?.GoTo(ToolView.WithTitle(po.UniqName ?? I[K.EditStudyPlan], view));
			};
		});

		return sv;
	}

	Control MkInputRow(str Label, IBinding Binding, bool ReadOnly = false, bool AcceptsReturn = false){
		var sp = new StackPanel{
			Spacing = 3,
		};
		sp.Children.Add(new TextBlock{
			Text = Label,
		});
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

	async System.Threading.Tasks.Task<nil> ChooseAndApplyStudyPlan(CT Ct){
		if(Ctx is null){
			return NIL;
		}
		var tcs = new System.Threading.Tasks.TaskCompletionSource<
			Ngaq.Core.Shared.StudyPlan.Models.Po.StudyPlan.PoStudyPlan?
		>();
		var view = new ViewStudyPlanPage();
		view.DetachedFromVisualTree += (s,e)=>{
			tcs.TrySetResult(null);
		};
		view.Ctx?.SetSelectMode(po=>{
			Ctx.SelectCandidateStudyPlan(po);
			tcs.TrySetResult(po);
			view.ViewNavi?.Back();
		});
		ViewNavi?.GoTo(ToolView.WithTitle(I[K.SelectStudyPlan], view));
		Ngaq.Core.Shared.StudyPlan.Models.Po.StudyPlan.PoStudyPlan? selected = null;
		try{
			using var reg = Ct.Register(()=>tcs.TrySetCanceled(Ct));
			selected = await tcs.Task;
			if(selected is null){
				return NIL;
			}
			return await Ctx.CommitSelectedStudyPlan(Ct);
		}catch(System.OperationCanceledException){
			return NIL;
		}catch(System.Exception e){
			Ctx.HandleErr(e);
			return NIL;
		}
	}
}

