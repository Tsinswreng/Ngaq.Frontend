namespace Ngaq.Ui.Views.Dictionary;

using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Dictionary.SimpleWord;
using Ngaq.Ui.Views.Word.WordManage.NormLang.NormLangPage;
using Ngaq.Ui.Views.Word.WordManage.NormLangToUserLang.NormLangToUserLangPage;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmDictionary;

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
		Style();
		Render();
		this.Loaded += (s, e) => {
			SearchTextBox?.Focus();
			_ = Ctx?.InitLang(default);
		};
	}
	public II18n I = I18n.Inst;
	public partial class Cls{}

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow:true);
	public TextBox SearchTextBox = new();
	public OpBtn SearchBtn = new();
	public OpBtn SaveToWordBtn = new();

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
			LangGrid.A(new Button(), o=>{
				o.CBind<Ctx>(o.PropContent, x=>x.SrcLang);
				o.HorizontalContentAlignment = HAlign.Center;
				o.VerticalContentAlignment = VAlign.Center;
				o.MinHeight = 24;
				o.Padding = new Avalonia.Thickness(0);
				o.Click += (s,e)=>OpenNormLangSelector(true);
			})
			.A(new Button(), o=>{
				o.Content = "⇄";
				o.VerticalAlignment = VAlign.Center;
				o.HorizontalAlignment = HAlign.Center;
				o.MinHeight = 24;
				o.Padding = new Avalonia.Thickness(0);
				o.Click += (s, e) => {
					Ctx?.SwapLang();
				};
			})
			.A(new Button(), o=>{
				o.CBind<Ctx>(o.PropContent, x=>x.TgtLang);
				o.HorizontalContentAlignment = HAlign.Center;
				o.VerticalContentAlignment = VAlign.Center;
				o.MinHeight = 24;
				o.Padding = new Avalonia.Thickness(0);
				o.Click += (s,e)=>OpenNormLangSelector(false);
			});
		}}

		var SearchGrid = new AutoGrid(IsRow: false);
		Root.A(SearchGrid.Grid, o=>{
			SearchGrid.ColDefs.AddRange([
				ColDef(8, GUT.Star),
				ColDef(1, GUT.Star),
				ColDef(1, GUT.Star),
			]);
		});
		{{
			SearchGrid
			.A(SearchTextBox, o=>{
				o.CBind<Ctx>(o.PropText,x=>x.Input);
				o.KeyBindings.Add(
					new KeyBinding{
						Gesture = new(Key.Enter),
						Command = new RelayCommand(()=>SearchBtn.PerformClick())
					}
				);
			})
			.A(SearchBtn, o=>{
				o._Button.StretchCenter();
				o._Button.Content = Svgs.Search().ToIcon();
				o.Background = UiCfg.Inst.MainColor;
				o.SetExe(Ct=>Ctx?.Lookup(Ct));
			})
			.A(SaveToWordBtn, o=>{
				// 保存到詞庫按鈕只負責「轉換 + 跳編輯頁」；最終保存仍在編輯頁完成。
				o._Button.StretchCenter();
				o.BtnContent = Svgs.FloppyDiskBackFill().ToIcon();
				o.SetExe(Ct=>Ctx?.ToWordEdit(Ct));
			});
		}}

		Root.A(new ScrollViewer(), sv=>{
			sv.SetContent(new ViewSimpleWord(), o=>{
				o.CBind<Ctx>(o.PropDataContext,x=>x.Result);
			});
		});

		return NIL;
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
				view.Ctx?.ViewNavi?.Back();
			});
		}else{
			view.Ctx?.SetSelectMode(po=>{
				Ctx?.ApplyTgtNormLang(po);
				view.Ctx?.ViewNavi?.Back();
			});
		}
		Ctx?.ViewNavi?.GoTo(ToolView.WithTitle(Todo.I18n("Select NormLang"), view));
	}

	public Control MkTitleMenu(){
		var menu = new ContextMenu();
		menu.Items.A(new MenuItem(), o=>{
			o.Header = Todo.I18n("配置語言映射");
			o.Click += (s,e)=>{
				var view = new ViewNormLangToUserLangPage();
				Ctx?.ViewNavi?.GoTo(ToolView.WithTitle(Todo.I18n("配置語言映射"), view));
			};
		});
		return menu;
	}
	public void ClickLookupBtn(str? SearchText = null){
		if(SearchText != null){
			SearchTextBox.Text = SearchText;
		}
		SearchBtn.PerformClick();
	}
}

