namespace Ngaq.Ui.Views.Dictionary;

using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using Avalonia.Interactivity;
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
using Tsinswreng.AvlnTools;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using DictK = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;
using Ctx = VmDictionary;
using Ngaq.Core.Shared.Word.Models;
using Avalonia.Media;
using Avalonia.Layout;
using Tsinswreng.Avln.Grid;

public partial class ViewDictionary
	:AppViewBase<Ctx>
	,I_MkTitleMenu
{
	public ViewDictionary(){
		Ctx = App.DiOrMk<Ctx>();
		if(Ctx is not null){
			// 把導航職責留在 View 層，VM 只產生要編輯的業務對象。
			Ctx.OnOpenWordEdit = OpenWordEditPage;
			Ctx.PropertyChanged += OnCtxPropertyChanged;
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

	GridStack Root = new(IsRow:true);
	public TextBox SearchTextBox = new();
	public OpBtn SearchBtn = new();
	public OpBtn SaveToWordBtn = new();
	public OpBtn MenuBtn = new();
	GridStack? _langGrid;
	GridStack? _searchGrid;
	Grid? _resultArea;

	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			Root.RowDefs = new("Auto,Auto,*");
		});

		var LangGrid = new GridStack(IsRow: false);
		_langGrid = LangGrid;
		Root.A(LangGrid.Grid, o=>{
			LangGrid.ColDefs = new("*,Auto,*");
			o.ZIndex = 20;
		});
		{{
			LangGrid.A(MkLangButton(), o=>{
				Ctx.Bind(o, o=>o.Content, x=>x.SrcLangDisplay);
				o.Click += (s, e) => OpenNormLangSelector(true);
			})
			.A(MkLangSwapButton())
			.A(MkLangButton(), o=>{
				Ctx.Bind(o, o=>o.Content, x=>x.TgtLangDisplay);
				o.Click += (s, e) => OpenNormLangSelector(false);
			});
		}}

		var SearchGrid = new GridStack(IsRow: false);
		_searchGrid = SearchGrid;
		Root.A(SearchGrid.Grid, o=>{
			SearchGrid.ColDefs = new("1*,8*,1*,1*");
			o.ZIndex = 20;
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
				Ctx.Bind(o, o=>o.Text,x=>x.Input);
				o.Watermark = I[DictK.InputNewWordToSearch];
				o.KeyBindings.Add(
					new KeyBinding{
						Gesture = new(Key.Enter),
						Command = new RelayCommand(()=>SearchBtn.PerformClick())
					}
				);
			})
			.A(SaveToWordBtn, o=>{
				// 詞典頁主按鈕改為快速保存：
				// 1. 已查詞：直接保存到詞庫。
				// 2. 未查詞：仍直接進入自由加詞頁。
				ToolTip.SetTip(o, I[DictK.SaveToUserWordLibrary]);
				InitToolbarBtn(
					o,
					Icons.BookmarkOutlineAdd().ToIcon(),
					QuickSaveOrFreeAdd
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

	/// 語言選擇按鈕：只負責共用樣式，具體綁定與行為在初始化函數中完成。
	Button MkLangButton(){
		var R = new Button();
		R.HorizontalContentAlignment = HAlign.Center;
		R.VerticalContentAlignment = VAlign.Center;
		R.MinHeight = 24;
		R.Padding = new Avalonia.Thickness(0);
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

	str GuideText(){
		return I[DictK.DictionaryUsageGuide_];
	}

	/// 主按鈕優先走快速保存；尚未查詞時仍保留自由加詞入口。
	async Task<nil> QuickSaveOrFreeAdd(CT Ct){
		if(Ctx is null){
			return NIL;
		}
		if(Ctx.HasLookupStarted){
			if(!Ctx.CanQuickSaveCurrentLookup || Ctx.LastReqLlmDict is null || Ctx.LastRespLlmDict is null){
				Ctx.ShowToast(I[DictK.CompleteDictionaryQueryBeforeSave]);
				return NIL;
			}
			if(Ctx.HasQuickSavedCurrentLookup){
				return NIL;
			}
			var ok = await Ctx.QuickSaveToWord(Ct);
			if(ok){
				MarkQuickSaveBtnAsSaved();
			}
			return NIL;
		}
		return OpenFreeAddWordPage();
	}

	/// 菜單入口保留原先「先進編輯頁再保存」的完整流程。
	async Task<nil> OpenSaveViaWordEditOrFreeAdd(CT Ct){
		if(Ctx?.LastReqLlmDict is not null && Ctx.LastRespLlmDict is not null){
			return await Ctx.ToWordEdit(Ct);
		}
		return OpenFreeAddWordPage();
	}

	nil MarkQuickSaveBtnAsSaved(){
		SaveToWordBtn._Button.Background = Brushes.Green;
		return NIL;
	}

	/// 快速收藏按鈕在「未收藏」狀態下應恢復到主題默認按鈕樣式，
	/// 不能把 Background 直接設為 null。
	/// 在 Avalonia 下，局部設 null 容易把可命中區域退化成只剩內容附近，
	/// 表現爲「看起來整個按鈕都在，但只有中心圖標附近能點」。
	nil ResetQuickSaveBtnToDefaultStyle(){
		SaveToWordBtn._Button.ClearValue(Button.BackgroundProperty);
		SaveToWordBtn._Button.ClearValue(Button.BorderBrushProperty);
		return NIL;
	}

	/// 收藏按鈕的顏色只是顯示細節；底層保存狀態由 VM 持有。
	void OnCtxPropertyChanged(object? Sender, PropertyChangedEventArgs E){
		if(E.PropertyName != nameof(Ctx.HasQuickSavedCurrentLookup)){
			return;
		}
		if(Ctx?.HasQuickSavedCurrentLookup == true){
			MarkQuickSaveBtnAsSaved();
			return;
		}
		ResetQuickSaveBtnToDefaultStyle();
	}

	/// 結果區：未查詞時顯示灰色用法提示，查詞後切到 `ViewSimpleWord`。
	Control MkResultArea(){
		var Area = new Grid();
		_resultArea = Area;
		Area.ZIndex = 0;
		Area.ClipToBounds = true;
		// 結果區與上方工具欄留出固定縫隙，避免邊界像素落到 ScrollViewer 命中區。
		Area.Margin = new Avalonia.Thickness(0, UiCfg.Inst.BaseFontSize * 0.35, 0, 0);
		Area.A(new ScrollViewer(), o=>{
			Ctx.Bind(o, IsVisibleProperty, x=>x.ShowLookupResult, Mode: BindingMode.OneWay);
			o.SetContent(new ViewSimpleWord(), o=>{
				Ctx.Bind(o,o.PropDataContext,x=>x.Result);
			});
		})
		.A(new TextBlock(), o=>{
			o.Foreground = Brushes.LightGray;
			o.TextWrapping = TextWrapping.Wrap;
			o.VerticalAlignment = VerticalAlignment.Top;
			o.Margin = new Avalonia.Thickness(0, UiCfg.Inst.BaseFontSize * 0.5, 0, 0);
			Ctx.Bind(o, IsVisibleProperty, x=>x.ShowUsageGuide, Mode: BindingMode.OneWay);
		})
		;
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
		ViewNavi?.GoTo(ToolView.WithTitle(
			IsSrc ? I[DictK.SelectDictionarySrcLang] : I[DictK.SelectDictionaryTgtLang],
			view
		));
	}

	public Control MkTitleMenu(){
		var menu = new ContextMenu();
		var item = menu.Items;
		item
		.A(new MenuItem(), o=>{
			o.Header = Todo.I18n("Edit & Save");
			o.Click += async(s,e)=>{
				await OpenSaveViaWordEditOrFreeAdd(default);
			};
		})
		.A(new MenuItem(), o=>{
			o.Header = I[DictK.Help];
			o.Click += (s,e)=>{
				ViewNavi?.GoTo(
					ToolView.WithTitle(
						I[DictK.Help],
						new TextBox{
							Classes = {App.Cls.RoTextBox},
							Text= GuideText(),
						}
					)
				);
			};
		})
		.A(new MenuItem(), o=>{
			o.Header = I[DictK.NormLang];
			o.Click += (s,e)=>{
				var view = new ViewNormLangPage();
				ViewNavi?.GoTo(ToolView.WithTitle(I[DictK.NormLangManage], view));
			};
		}).A(new MenuItem(), o=>{
			o.Header = I[DictK.ConfigureLangMapping];
			o.Click += (s,e)=>{
				var view = new ViewNormLangToUserLangPage();
				ViewNavi?.GoTo(ToolView.WithTitle(I[DictK.ConfigureLangMapping], view));
			};
		}).A(new MenuItem(), o=>{
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


	/// 未查詞時允許直接進入空白編輯頁，自由手動新增單詞。
	nil OpenFreeAddWordPage(){
		var view = new ViewWordEditV2();
		if(view.Ctx is null){
			Ctx?.ShowDialog(I[DictK.WordEditorContextIsNull]);
			return NIL;
		}
		view.Ctx.InitFreeAddDraft(Ctx?.SrcLang ?? "");
		ViewNavi?.GoTo(ToolView.WithTitle(I[DictK.AddWords], view));
		return NIL;
	}

	public void ClickLookupBtn(str? SearchText = null){
		if(SearchText != null){
			SearchTextBox.Text = SearchText;
		}
		SearchBtn.PerformClick();
	}
}
