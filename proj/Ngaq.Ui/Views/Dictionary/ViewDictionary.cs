namespace Ngaq.Ui.Views.Dictionary;

using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using Ngaq.Core.Shared.Word.Models.Dto;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Dictionary.LlmRawOutputEdit;
using Ngaq.Ui.Views.Dictionary.SimpleWord;
using Ngaq.Ui.Views.Word.WordEditV2;
using Ngaq.Ui.Views.Word.WordManage.NormLang.NormLangPage;
using Ngaq.Ui.Views.Word.WordManage.NormLangToUserLang.NormLangToUserLangPage;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using DictK = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;
using Ctx = VmDictionary;
using Ngaq.Core.Shared.Word.Models;
using Avalonia.Media;
using Avalonia.Layout;

public partial class ViewDictionary
	:AppViewBase
	,I_MkTitleMenu
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewDictionary(){
		Ctx = App.DiOrMk<Ctx>();
		if(Ctx is not null){
			// 把導航職責留在 View 層，VM 只產生要編輯的業務對象。
			Ctx.OnOpenWordEdit = OpenWordEditPage;
		}
		Style();
		Render();
		this.Loaded += (s, e) => {
			//SearchTextBox?.Focus(); //在手機端會直接彈出輸入法、效果不佳
			_ = Ctx?.InitLang(default);
		};
	}

	public partial class Cls{}

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow:true);
	public TextBox SearchTextBox = new();
	public OpBtn SearchBtn = new();
	public OpBtn SaveToWordBtn = new();
	public OpBtn MenuBtn = new();
	TextBlock UsageGuideText = new();

	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			Root.RowDefs.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Star),
			]);
		});

		var LangGrid = new AutoGrid(IsRow: false);
		Root.A(LangGrid.Grid, o=>{
			LangGrid.ColDefs.AddRange([
				ColDef(5, GUT.Star),
				ColDef(1, GUT.Auto),
				ColDef(5, GUT.Star),
			]);
		});
		{{
			LangGrid.A(MkLangButton(x=>x.SrcLangDisplay, (s,e)=>OpenNormLangSelector(true)))
			.A(MkLangSwapButton())
			.A(MkLangButton(x=>x.TgtLangDisplay, (s,e)=>OpenNormLangSelector(false)));
		}}

		var SearchGrid = new AutoGrid(IsRow: false);
		Root.A(SearchGrid.Grid, o=>{
			SearchGrid.ColDefs.AddRange([
				ColDef(1, GUT.Star),
				ColDef(8, GUT.Star),
				ColDef(1, GUT.Star),
				ColDef(1, GUT.Star),
			]);
		});
		{{
			SearchGrid
			.A(SearchBtn, o=>{
				InitToolbarBtn(
					o,
					Icons.Search().ToIcon(),
					Ct=>Ctx?.Lookup(Ct),
					UiCfg.Inst.MainColor
				);
			})
			.A(SearchTextBox, o=>{
				o.CBind<Ctx>(o.PropText,x=>x.Input);
				o.Watermark = Todo.I18n("Search a word");
				o.KeyBindings.Add(
					new KeyBinding{
						Gesture = new(Key.Enter),
						Command = new RelayCommand(()=>SearchBtn.PerformClick())
					}
				);
			})
			.A(SaveToWordBtn, o=>{
				// 保存到詞庫按鈕只負責「轉換 + 跳編輯頁」；最終保存仍在編輯頁完成。
				ToolTip.SetTip(o, Todo.I18n("Save to user's word library"));
				InitToolbarBtn(
					o,
					Icons.BookmarkOutlineAdd().ToIcon(),
					Ct=>Ctx?.ToWordEdit(Ct)
				);
			})
			.A(MenuBtn, o=>{
				// 與搜尋/收藏按鈕統一使用 OpBtn 風格。
				InitToolbarBtn(o, Icons.Menu().ToIcon());
				o._Button.Click += (s,e)=>OpenTitleMenuNear(o._Button);
			});
		}}

		Root.A(MkResultArea());

		return NIL;
	}

	/// 語言選擇按鈕：顯示 `code + 譯名`，並保持兩端樣式一致。
	Button MkLangButton(
		System.Linq.Expressions.Expression<Func<Ctx, obj?>> BindExpr,
		EventHandler<RoutedEventArgs> OnClick
	){
		var R = new Button();
		R.CBind<Ctx>(R.PropContent, BindExpr);
		R.HorizontalContentAlignment = HAlign.Center;
		R.VerticalContentAlignment = VAlign.Center;
		R.MinHeight = 24;
		R.Padding = new Avalonia.Thickness(0);
		R.Click += OnClick;
		return R;
	}

	/// 語言互換按鈕單獨抽出，避免和左右語言按鈕重複堆屬性。
	Button MkLangSwapButton(){
		var R = new Button();
		R.Content = "⇄";
		R.VerticalAlignment = VAlign.Center;
		R.HorizontalAlignment = HAlign.Center;
		R.MinHeight = 24;
		R.Padding = new Avalonia.Thickness(0);
		R.Click += (s, e) => {
			Ctx?.SwapLang();
		};
		return R;
	}

	/// 搜索/保存/菜單三類工具按鈕共用樣式，避免同文件反覆手抄。
	nil InitToolbarBtn(OpBtn Btn, Control Content, Func<CT, Task<nil>>? Exe = null, IBrush? Background = null){
		Btn._Button.StretchCenter();
		Btn.BtnContent = Content;
		if(Background is not null){
			Btn._Button.Background = Background;
		}
		if(Exe is not null){
			Btn.SetExe(Exe);
		}
		return NIL;
	}

	/// 結果區：未查詞時顯示灰色用法提示，查詞後切到 `ViewSimpleWord`。
	Control MkResultArea(){
		var Area = new Grid();
		var ResultScroll = new ScrollViewer();
		ResultScroll.CBind<Ctx>(IsVisibleProperty, x=>x.ShowLookupResult, Mode: BindingMode.OneWay);
		ResultScroll.SetContent(new ViewSimpleWord(), o=>{
			o.CBind<Ctx>(o.PropDataContext,x=>x.Result);
		});
		Area.Children.Add(ResultScroll);

		UsageGuideText.Text = Todo.I18n("Dictionary usage guide");
		UsageGuideText.Foreground = Brushes.LightGray;
		UsageGuideText.TextWrapping = TextWrapping.Wrap;
		UsageGuideText.VerticalAlignment = VerticalAlignment.Top;
		UsageGuideText.Margin = new Avalonia.Thickness(0, UiCfg.Inst.BaseFontSize * 0.5, 0, 0);
		UsageGuideText.CBind<Ctx>(IsVisibleProperty, x=>x.ShowUsageGuide, Mode: BindingMode.OneWay);
		Area.Children.Add(UsageGuideText);

		return Area;
	}

	void OpenNormLangSelector(bool IsSrc){
		var view = new ViewNormLangPage();
		if(view.Ctx is not null){
			if(IsSrc){
				view.Ctx.Input = Ctx?.SrcLang ?? "";
			}else{
				view.Ctx.Input = Ctx?.TgtLang ?? "";
			}
		}
		if(IsSrc){
			view.Ctx?.SetSelectMode(po=>{
				Ctx?.ApplySrcNormLang(po);
				view.ViewNavi?.Back();
			});
		}else{
			view.Ctx?.SetSelectMode(po=>{
				Ctx?.ApplyTgtNormLang(po);
				view.ViewNavi?.Back();
			});
		}
		ViewNavi?.GoTo(ToolView.WithTitle(I[DictK.SelectNormLang], view));
	}

	public Control MkTitleMenu(){
		var menu = new ContextMenu();
		menu.Items.A(new MenuItem(), o=>{
			o.Header = I[DictK.NormLang];
			o.Click += (s,e)=>{
				var view = new ViewNormLangPage();
				ViewNavi?.GoTo(ToolView.WithTitle(I[DictK.NormLang], view));
			};
		});
		menu.Items.A(new MenuItem(), o=>{
			o.Header = I[DictK.ConfigureLangMapping];
			o.Click += (s,e)=>{
				var view = new ViewNormLangToUserLangPage();
				ViewNavi?.GoTo(ToolView.WithTitle(I[DictK.ConfigureLangMapping], view));
			};
		});
		menu.Items.A(new MenuItem(), o=>{
			o.Header = I[DictK.ViewLlmRawOutput];
			o.Click += (s,e)=>{
				var DictCtx = Ctx;
				if(DictCtx is null){
					return;
				}
				var view = new ViewLlmRawOutputEdit();
				if(view.Ctx is not null){
					view.Ctx.SetRawOutput(DictCtx.LastLlmRawOutput);
					view.Ctx.SetOnConfirm((raw, ct)=>{
						return DictCtx.ReparseFromRawOutput(raw, ct);
					});
				}
				ViewNavi?.GoTo(ToolView.WithTitle(I[DictK.ViewLlmRawOutput], view));
			};
		});
		return menu;
	}

	void OpenTitleMenuNear(Control? Anchor){
		if(Anchor is null){
			return;
		}
		var menu = MkTitleMenu() as ContextMenu;
		if(menu is null){
			return;
		}
		// 顯式傳入錨點控件，避免 ContextMenu 內部取到空 PlacementTarget。
		menu.Open(Anchor);
	}

	nil OpenWordEditPage(JnWord JnWord){
		var view = new ViewWordEditV2();
		if(view.Ctx is null){
			Ctx?.ShowDialog(I[DictK.WordEditorContextIsNull]);
			return NIL;
		}
		view.Ctx.FromJnWord(JnWord);
		view.Ctx.SaveMode = VmWordEditV2.ESaveMode.Merge;
		var title = str.IsNullOrWhiteSpace(JnWord.Word.Head)
			? I[DictK.WordEdit]
			: JnWord.Word.Head
		;
		ViewNavi?.GoTo(ToolView.WithTitle(title, view));
		return NIL;
	}

	public void ClickLookupBtn(str? SearchText = null){
		if(SearchText != null){
			SearchTextBox.Text = SearchText;
		}
		SearchBtn.PerformClick();
	}
}

