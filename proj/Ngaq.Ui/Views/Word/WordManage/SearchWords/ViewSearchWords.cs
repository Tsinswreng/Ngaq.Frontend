namespace Ngaq.Ui.Views.Word.WordManage.SearchWords;

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Ui.Components.PageBar;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordEditV2;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Ngaq.Ui.Views.Word.WordManage.UserLang.UserLangPage;
using Ngaq.Ui.Views.Word.WordManage.SearchWords.SearchedWordCard;
using Ngaq.Ui.Views.Word.WordManage.WordSyncV2;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsTools;
using Ctx = VmSearchWords;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

//TODO 改名爲 UserWordPage / UserWordManage
public partial class ViewSearchWords
	:AppViewBase
	,I_MkTitleMenu
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSearchWords(){
		Ctx = App.GetRSvc<Ctx>();
		Style();
		Render();
	}

	public partial class Cls{}

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new AutoGrid(IsRow: true);

	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Auto),
			]);
		});
		var SearchGrid = new AutoGrid(IsRow: false);
		Root.A(SearchGrid.Grid);
		{var o = SearchGrid;
			o.Grid.ColumnDefinitions.AddRange([
				ColDef(7, GUT.Star),
				ColDef(1, GUT.Star),
			]);
		}
		{{
			var searchBtn = new OpBtn();
			SearchGrid.A(new TextBox(), o=>{
				o.CBind<Ctx>(
					o.PropText
					,x=>x.Input);
				o.KeyBindings.Add(
					new KeyBinding{
						Gesture = new (Key.Enter),
						Command = new RelayCommand(()=>searchBtn.PerformClick())
					}
				);
			})
			.A(searchBtn, o=>{
				o.BtnContent = Icons.Search().ToIcon();
				o.SetExe((Ct)=>Ctx?.InitSearchAsy(Ct));
				o._Button.StretchCenter();
				o._Button.Background = UiCfg.Inst.MainColor;
			});
		}}
		Root.A(new ScrollViewer(), scrl=>{
			scrl.SetContent(_ListWordCard(), o=>{
				o.CBind<Ctx>(
					ItemsControl.ItemsSourceProperty
					,
						x=>x.GotWords
						,Mode: BindingMode.OneWay
					);
			});
		});
		Root.A(MkPageBar(), o=>{
			o.HorizontalAlignment = HAlign.Center;
		});

		return NIL;
	}

	Control _ListWordCard(){
		var R = new ItemsControl();
		R.ItemsPanel = new FuncTemplate<Panel?>(()=>{
			return new VirtualizingStackPanel();
		});
		R.ItemTemplate = new FuncDataTemplate<ITypedObj>((typedObj, b)=>{
			var R = new Button();
			var View = new ViewSearchedWordCard(){Ctx = new VmSearchedWordCard()};
			if(typedObj == null){return View;};

			View.Ctx.FromTypedObj(typedObj);
			R.Content = View;
			if(!AnyNull(View.Ctx.WordForLearn?.JnWord)){
				R.ContextMenu = ViewWordListCard.MkWordCardCtxMenu(Ctx, View.Ctx.WordForLearn.JnWord);
			}else{
				Ctx?.ShowDialog(I[K.WordNotFound]);
			}
			R.Click += (s,e)=>{
				var Target = new ViewWordEditV2();
				var jnWord = VmSearchedWordCard.GetJnWordFromTypedObj(typedObj);
				if(AnyNull(Target.Ctx, jnWord)){
					return;
				}
				Target.Ctx.FromJnWord(jnWord);
				var titleStr = jnWord.Head;
				var titled = ToolView.WithTitle(titleStr, Target);
				ViewNavi?.GoTo(titled);
			};
			R.Styles.Add(new Style().Set(
				BackgroundProperty
				,Brushes.Transparent
			));
			R.Styles.Add(new Style().NoMargin().NoPadding());
			return R;
		});
		return R;
	}

	Control MkPageBar(){
		var view = new ViewPageBar();
		if(Ctx is not null){
			view.Ctx = Ctx.PageBar;
		}
		return view;
	}

	/// <summary>
	/// 頂欄菜單：收納單詞同步頁入口（V1 + V2）。
	/// </summary>
	/// <returns>標題菜單控件。</returns>
	public Control MkTitleMenu(){
		var menu = new ContextMenu();
		menu.Items.A(new MenuItem(), o=>{
			var title = I[K.AddWordsFromText];
			o.Header = title;
			o.Click += (s,e)=>{
				ViewNavi?.GoTo(
					ToolView.WithTitle(
						title,
						new ViewAddWord()
					)
				);
			};
		});
		menu.Items.A(new MenuItem(), o=>{
			var title = I[K.UserLang];
			o.Header = title;
			o.Click += (s,e)=>{
				ViewNavi?.GoTo(
					ToolView.WithTitle(
						title,
						new ViewUserLangPage()
					)
				);
			};
		});
		menu.Items.A(new MenuItem(), o=>{
			var title = I[K.BackupEtSync];
			o.Header = title;
			o.Click += (s,e)=>{
				ViewNavi?.GoTo(
					ToolView.WithTitle(
						title,
						new ViewWordSyncV2()
					)
				);
			};
		});
		return menu;
	}
}
