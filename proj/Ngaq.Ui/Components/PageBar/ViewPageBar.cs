namespace Ngaq.Ui.Components.PageBar;

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Declarative;
using Avalonia.Styling;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmPageBar;
public partial class ViewPageBar
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewPageBar(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls{
		public static str CenterInput = nameof(CenterInput);
		public static str CenterText = nameof(CenterText);
	}


	protected nil Style(){
		var S = Styles;

		var centerText = new Style(
			x=>x.Class(Cls.CenterInput)
		).Set(VerticalContentAlignmentProperty, VAlign.Center)
		.Set(HorizontalContentAlignmentProperty, HAlign.Center)
		.AddTo(S);

		S.A(new Style(
				x=>x.Class(Cls.CenterText)
			).Set(VerticalAlignmentProperty, VAlign.Center)
		);

		return NIL;
	}

	TextBox _TextBox(){
		var R = new TextBox();
		R.Classes.A(Cls.CenterInput);
		R.MinWidth = 0;
		return R;
	}

	TextBlock _TextBlock(){
		var R = new TextBlock();
		R.Classes.A(Cls.CenterText);
		return R;
	}

	IValueConverter ConvU64Str(u64 Dflt){
		var r = new ParamFnConvtr<u64, str>(
			(x, p)=>x.ToString()
			,(text, p)=>{
				if(u64.TryParse(text, out var value)){
					return value;
				}
				return Dflt;
			}
		);
		return r;
	}
	AutoGrid Root = new(IsRow: false);
	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
		]);
		Root
		.A(MkPageBtn(), o=>{
			o.BtnContent = Svgs.ArrowCircleLeftFill().ToIcon();
			o.SetExe((Ct)=>Ctx?.FnPrevPage?.Invoke(Ctx, Ct));
		})
		.A(_TextBox(), o=>{
			o.CBind<Ctx>(
				o.PropText
				,x=>x.PageNum
				,Converter: ConvU64Str(1)
				,Mode: BindingMode.TwoWay
			);
			o.BorderBrush = null;
		})
		.A(_TextBlock(), o=>{
			o.Text = " / ";
		})
		.A(_TextBox(), o=>{
			// o.CBind<Ctx>(o.PropIsVisible, x=>x.TotCnt,
			// 	Converter: new ParamFnConvtr<u64?, bool>((x,p)=>x is not null)
			// );
			o.IsReadOnly = true;
			o.Focusable = false;
			o.BorderBrush = null;
			o.CBind<Ctx>(
				o.PropText
				,x=>x.TotPageCnt
				,Converter: new ParamFnConvtr<u64?, str>((x, p)=>x?.ToString()??"")
				,Mode: BindingMode.OneWay
			);
		})
		.A(MkPageBtn(), o=>{
			o.BtnContent = Svgs.ArrowCircleRightFill().ToIcon();
			o.SetExe((Ct)=>Ctx?.FnNextPage?.Invoke(Ctx, Ct));
		})
		.A(MkPageSizeMenuBtn())
		;
		return NIL;
	}

	Button MkPageSizeMenuBtn(){
		var flyout = new Flyout();{
			var panel = new StackPanel(){
				Init = o=>{
					flyout.Content = o;
					o.Spacing = 6;
				}
			};

			panel.A(new TextBlock(), o=>{
				o.Text = Todo.I18n("Page Size:");
			})
			.A(new ComboBox(), o=>{
				o.IsEditable = true;
				o.MinWidth = 96;
				o.ItemsSource = new str[]{"10", "20", "50"};
				o.CBind<Ctx>(
					ComboBox.TextProperty
					,x=>x.PageSize
					,Converter: ConvU64Str(10)
					,Mode: BindingMode.TwoWay
				);
			});
		}

		var btn = new Button(){
			Init =o=>{
				o.Content = Svgs.DotsHorizontalCircleOutline().ToIcon();
				StyleBtn(o);
			}
		};
		FlyoutBase.SetAttachedFlyout(btn, flyout);
		btn.Click += (s,e)=>{
			FlyoutBase.ShowAttachedFlyout(btn);
		};
		return btn;
	}

	OpBtn MkPageBtn(){
		var r = new OpBtn();
		StyleBtn(r._Button);
		return r;
	}

	void StyleBtn(Button b){
		b.HAlign(x=>x.Stretch);
		b.VAlign(x=>x.Stretch);
		b.Background = null;
	}
}

