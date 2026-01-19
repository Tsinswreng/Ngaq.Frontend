namespace Ngaq.Ui.Views.Word.WordManage.SearchWords;

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordEdit;
using Ngaq.Ui.Views.Word.WordManage.EditWord;
using Ngaq.Ui.Views.Word.WordManage.SearchWords.SearchedWordCard;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsTools;
using Ctx = VmSearchWords;
public partial class ViewSearchWords
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSearchWords(){
		//Ctx = Ctx.Mk();
		Ctx = App.GetSvc<Ctx>();
		Style();
		Render();
	}

	public partial class Cls{

	}

	protected nil Style(){
		return NIL;
	}

	//StackPanel Root = new StackPanel();
	AutoGrid Root = new AutoGrid(IsRow: true);


	protected nil Render(){
		this.InitContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Auto),
			]);
		});
		var SearchGrid = new AutoGrid(IsRow: false);
		Root.AddInit(SearchGrid.Grid);
		{var o = SearchGrid;
			o.Grid.ColumnDefinitions.AddRange([
				ColDef(7, GUT.Star),
				ColDef(1, GUT.Star),
			]);
		}
		{{
			var searchBtn = new OpBtn();
			SearchGrid.AddInit(_TextBox(), o=>{
				o.Bind(
					o.PropText
					,CBE.Mk<Ctx>(x=>x.Input)
				);
				o.KeyBindings.Add(
					new KeyBinding{
						Gesture = new (Key.Enter),
						Command = new RelayCommand(()=>searchBtn.PerformClick())
					}
				);
			})
			.AddInit(searchBtn, o=>{
				//o.BtnContent = "🔍";
				o.BtnContent = Svgs.Search.ToIcon();
				o.SetExe((Ct)=>Ctx?.InitSearchAsy(Ct));
				o._Button.StretchCenter();
			});
		}}
		Root.AddInit(_ScrollViewer(), scrl=>{
			scrl.InitContent(_ListWordCard(), o=>{
				o.Bind(
					ItemsControl.ItemsSourceProperty
					,CBE.Mk<Ctx>(
						x=>x.GotWords
						,Mode: BindingMode.OneWay
					)
				);
			});
		});
		Root.AddInit(_PageBar(), o=>{
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
			//R.HorizontalContentAlignment = HAlign.Left;
			if(!AnyNull(View.Ctx.WordForLearn?.JnWord)){
				R.ContextMenu = ViewWordListCard.MkWordCardCtxMenu(Ctx, View.Ctx.WordForLearn.JnWord);
			}else{
				Todo.I18n();
				Ctx?.ShowMsg("Word not found.");//TODO
			}
			R.Click += (s,e)=>{
				// var Target = new ViewEditJsonWord();
				// Target.Ctx?.FromTypedObj(typedObj);

				var Target = new ViewWordEdit();
				var jnWord = VmSearchedWordCard.GetJnWordFromTypedObj(typedObj);
				Target.Ctx?.FromBo(jnWord);
				var titleStr = jnWord.Head;
				var titled = ToolView.WithTitle(titleStr, Target);
				Ctx?.ViewNavi?.GoTo(titled);
			};
			R.Styles.Add(new Style().Set(
				BackgroundProperty
				,Brushes.Transparent//背景設潙空則影響點擊判定範圍、點到空處則視潙未點、故用透明㕥代空背景
			));
			R.Styles.Add(new Style().NoMargin().NoPadding());
			return R;
		});
		return R;
	}

	Control _PageBar(){
		var R = new AutoGrid(IsRow: false);
		R.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
		]);
		R.AddInit(_Button(), o=>{
			o.Content = "<";
			o.Click += (s,e)=>{
				Ctx?.PrevPage();
			};
		});
		R.AddInit(_TextBox(), o=>{
			o.Bind(
				o.PropText
				,CBE.Mk<Ctx>(
					x=>x.PageIdx
					,Converter: new ParamFnConvtr<u64, str>(
						(idx, arg)=>(idx+1).ToString()
						,(Str, arg)=>{
							if(u64.TryParse(Str, out var R)){
								return R-1;
							}
							return 1;
						}
					)
					,Mode: BindingMode.TwoWay
				)
			);
		});
		R.AddInit(_Button(), o=>{
			o.Content = ">";
			o.Click += (s,e)=>{
				Ctx?.NextPage();
			};
		});
		return R.Grid;

	}


}
