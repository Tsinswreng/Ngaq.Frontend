namespace Ngaq.Ui.Views.Dictionary;

using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Dictionary.DictionaryApi;
using Ngaq.Ui.Views.Dictionary.SimpleWord;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmDictionary;
public partial class ViewDictionary
	:AppViewBase
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
		};
	}
	public II18n I = I18n.Inst;
	public partial class Cls{}

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow:true);
	public TextBox SearchTextBox = new();//主查詢輸入框
	public OpBtn SearchBtn = new();
	protected nil Render(){
		this.InitContent(Root.Grid, o=>{
			Root.RowDefs.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),

			]);
		});

		// 語言選擇行
		var LangGrid = new AutoGrid(IsRow: false);
		Root.A(LangGrid.Grid, o=>{
			LangGrid.ColDefs.AddRange([
				ColDef(5, GUT.Star),
				ColDef(1, GUT.Auto),
				ColDef(5, GUT.Star),
			]);
		});
		{{
			// 源語言輸入
			LangGrid.A(new TextBox(), o=>{
				o.CBind<Ctx>(o.PropText,x=>x.SrcLang);
				o.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
				o.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
				o.FontSize = UiCfg.Inst.BaseFontSize*0.7;
				o.Watermark = "en";
				o.MinHeight = 24;
				o.Padding = new Avalonia.Thickness(0);
			})
			// 切換按鈕
			.A(new Button(), o=>{
				o.Content = "⇄";
				o.VerticalAlignment = VAlign.Center;
				o.HorizontalAlignment = HAlign.Center;
				o.MinHeight = 24;
				o.Padding = new Avalonia.Thickness(0);
				o.Click += (s, e) => {
					if(Ctx != null) {
						var tmp = Ctx.SrcLang;
						Ctx.SrcLang = Ctx.TgtLang;
						Ctx.TgtLang = tmp;
					}
				};
			})
			// 目標語言輸入
			.A(new TextBox(), o=>{
				o.CBind<Ctx>(o.PropText,x=>x.TgtLang);
				o.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
				o.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
				o.FontSize = UiCfg.Inst.BaseFontSize*0.7;
				o.Watermark = "zh";
				o.MinHeight = 24;
				o.Padding = new Avalonia.Thickness(0);
			});
		}}

		// 搜索?
		var SearchGrid = new AutoGrid(IsRow: false);
		Root.A(SearchGrid.Grid, o=>{
			SearchGrid.ColDefs.AddRange([
				ColDef(10, GUT.Star),
				ColDef(2, GUT.Star),
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
				Todo.I18n();
				//o._Button.Content = "Search";
				o._Button.StretchCenter();
				o._Button.Content = Svgs.Search.ToIcon();
				o.SetExe(Ct=>Ctx?.Lookup(Ct));
			})
			;

		}}
		Root.A(new ViewSimpleWord(), o=>{
			o.CBind<Ctx>(o.PropDataContext,x=>x.Result);
		})

		;

		return NIL;
	}

	public void ClickLookupBtn(str? SearchText = null){
		if(SearchText != null){
			SearchTextBox.Text = SearchText;
		}
		SearchBtn.PerformClick();
	}




}
