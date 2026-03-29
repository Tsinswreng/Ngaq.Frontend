namespace Ngaq.Ui.Infra.Ctrls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using DynamicData.Binding;
using Ngaq.Ui.Icons;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = ViewModelBase;
public partial class ViewTitle
	:UserControl
{

	public Border BdrTitle{get;set;} = new();
	public ContentControl Body{get;set;} = new ContentControl();
	public ContentControl Title{get;set;} = new ContentControl();



	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewTitle(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public  partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	Button _Button(){
		var R = new Button();
		R.VerticalContentAlignment = VAlign.Center;
		R.HorizontalContentAlignment = HAlign.Center;
		R.VerticalAlignment = VAlign.Stretch;
		R.HorizontalAlignment = HAlign.Stretch;
		return R;
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.InitContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),//Title
				RowDef(1, GUT.Star),//Content
			]);
		});
		AutoGrid TitleBar = new(IsRow: false);
		BdrTitle.Child=TitleBar.Grid;
		{
			var o = TitleBar;
			o.Grid.ColumnDefinitions.AddRange([
				ColDef(1, GUT.Auto),//Back Btn
				ColDef(1, GUT.Star),//Title Str
				ColDef(1, GUT.Auto),//Menu Btn
			]);
		}
		Root.A(BdrTitle, o=>{});
		{{
			TitleBar.A(_Button(), o=>{
				o.Content = Svgs.ArrowCircleLeftFill().ToIcon();
				o.Click += (s,e)=>{
					Ctx?.ViewNavi?.Back();
				};
				o.Background = Brushes.Transparent;
			})
			.A(Title)
			.A(_Button(), o=>{

				o.IsVisible = false;
				o.Content = Svgs.DotsHorizontalCircleOutline().ToIcon();
				o.Background = Brushes.Transparent;
				var CtxMenu = new ContextMenu(){
					Init=o=>{

					}
				};
				new Style()
					.NoMargin().NoPadding()
					.Set(BackgroundProperty, Brushes.Transparent)
				.AddTo(CtxMenu.Styles);

				o.ContextMenu = CtxMenu;
				o.Click += (s,e)=>{
					o.ContextMenu.Open();
				};
				Body.PropertyChanged += (sender, e) =>{
					if (e.Property == ContentControl.ContentProperty){
						// 调用自定义业务函数，传入 旧值/新值
						if(Body.Content is I_MkTitleMenu mk){
							CtxMenu.Items.Clear();
							var TitleMenu = mk.MkTitleMenu();
							CtxMenu.Items.Add(TitleMenu);
							o.IsVisible = true;
						}
					}
				};
				//TODO 顯示popup旹 把按鈕的背景設置成 和他被點擊時的背景 一樣
			});


		}}//TitleBar
		Root.A(Body);
		return NIL;
	}


}
