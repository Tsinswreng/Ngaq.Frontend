namespace Ngaq.Ui.Views.Word.WordManage.NormLang.NormLangPage;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;
using Ngaq.Ui;
using Ngaq.Ui.Components.PageBar;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.NormLang.NormLangEdit;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ctx = VmNormLangPage;`r`nusing K = Ngaq.Ui.Infra.I18n.KeysUiI18n.NormLangPage;

public partial class ViewNormLangPage
	:AppViewBase
	,I_MkTitleMenu
{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewNormLangPage(){
		Ctx = App.DiOrMk<Ctx>();
		if(Ctx is not null){
			Ctx.OnOpenDetailRequested += OpenDetail;
		}
		Style();
		Render();
		InitDataGrid();
		Loaded += async(s,e)=>{
			_ = Ctx?.InitSearch(default);
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
	TreeDataGrid? Grid;
	FlatTreeDataGridSource<Ctx.RowNormLang>? GridSource;

	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Auto),
		]);
		Root.A(MkTopBar());
		Root.A(MkGridHost());
		Root.A(MkPageBarHost());
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
		top.A(new TextBox(), o=>{
			o.CBind<Ctx>(o.PropText, x=>x.Input);
			o.KeyBindings.Add(new KeyBinding{
				Gesture = new KeyGesture(Key.Enter),
				Command = new RelayCommand(()=>searchBtn.PerformClick()),
			});
		})
		.A(searchBtn, o=>{
			o.Classes.Add(Cls.FullStretch);
			o.BtnContent = Svgs.Search().ToIcon();
			o.Background = UiCfg.Inst.MainColor;
			o.SetExe((Ct)=>Ctx?.InitSearch(Ct)!);
		})
		.A(new Button(), o=>{
			o.Classes.Add(Cls.FullStretch);
			o.CBind<Ctx>(IsVisibleProperty, x=>x.CanCreate, Mode: BindingMode.OneWay);
			o.Content = Svgs.Add().ToIcon();
			o.Click += (s,e)=>Ctx?.OpenDetail();
		});
		return top.Grid;
	}

	protected Control MkGridHost(){
		Grid = new TreeDataGrid{
			MinHeight = 280,
		};
		Grid.Styles.Add(
			new Style(x=>x.OfType<TreeDataGridRow>().Class(":pointerover"))
			.Set(TemplatedControl.BackgroundProperty, new SolidColorBrush(Color.FromRgb(46, 46, 46)))
		);
		Grid.Styles.Add(
			new Style(x=>x.OfType<TreeDataGridRow>().Class(":pressed"))
			.Set(TemplatedControl.BackgroundProperty, new SolidColorBrush(Color.FromRgb(70, 70, 70)))
		);
		Grid.AddHandler(InputElement.TappedEvent, OnGridTapped, RoutingStrategies.Bubble, true);
		return Grid;
	}

	protected Control MkPageBarHost(){
		var pageBar = new ViewPageBar();
		if(Ctx is not null){
			pageBar.Ctx = Ctx.PageBar;
		}
		return pageBar;
	}

	protected nil InitDataGrid(){
		if(Ctx is null || Grid is null){
			return NIL;
		}
		GridSource = new FlatTreeDataGridSource<Ctx.RowNormLang>(Ctx.Rows){
			Columns = {
				new TextColumn<Ctx.RowNormLang, str>(I[K.Empty], x=>x.UiIdxText),
				new TextColumn<Ctx.RowNormLang, str>(I[K.Code], x=>x.Code),
				new TextColumn<Ctx.RowNormLang, str>(I[K.NativeName], x=>x.NativeName),
			},
		};
		Grid.Source = GridSource;
		return NIL;
	}

	protected void OnGridTapped(object? sender, TappedEventArgs e){
		if(Ctx is null || Grid is null){
			return;
		}
		if(e.Source is not StyledElement src){
			return;
		}
		for(StyledElement? cur = src; cur is not null; cur = cur.Parent){
			if(cur is ToggleButton){
				return;
			}
			if(cur is TreeDataGridRow row){
				if(row.DataContext is Ctx.RowNormLang vmRow){
					Ctx.OpenDetail(vmRow);
					e.Handled = true;
				}
				return;
			}
		}
	}

	void OpenDetail(Ctx.RowNormLang? row){
		var view = new ViewNormLangEdit();
		view.Ctx?.SetCreateMode(row?.Raw is null);
		view.Ctx?.FromPoNormLang(row?.Raw);
		var title = row?.Raw?.Code ?? I[K.AddNormLang];
		var titled = ToolView.WithTitle(title, view);
		ViewNavi?.GoTo(titled);
	}

	public Control MkTitleMenu(){
		var menu = new ContextMenu();
		menu.Items.A(new MenuItem(), o=>{
			o.Header = I[K.InitBuiltin];
			o.Click += async(s,e)=>{
				if(Ctx is null){
					return;
				}
				await Ctx.InitBuiltinNormLang(default);
			};
		});
		return menu;
	}
}

