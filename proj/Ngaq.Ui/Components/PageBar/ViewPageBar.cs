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
using Tsinswreng.Avln.Grid;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using CommonK = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;
using Ctx = VmPageBar;
public partial class ViewPageBar
	:AppViewBase<Ctx>
{
	public ViewPageBar(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}

	public partial class Cls{
		public static str CenterInput = nameof(CenterInput);
		public static str CenterText = nameof(CenterText);
	}


	protected nil Style(){
		var S = Styles;

		S.A(
			Sty.Is<TextBox>(
				x=>x.Class(Cls.CenterInput)
			).Set(x=>x.VerticalContentAlignment, VAlign.Center)
			.Set(x=>x.HorizontalContentAlignment, HAlign.Center)
		);

		S.A(Sty.Is<TextBlock>(
				x=>x.Class(Cls.CenterText)
			).Set(x=>x.VerticalAlignment, VAlign.Center)
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
		var r = new FnConvtr<u64, str>(
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
	GridStack Root = new(IsRow: false);
	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.SetColDefs([
			new(1, GUT.Auto),
			new(1, GUT.Auto),
			new(1, GUT.Auto),
			new(1, GUT.Auto),
			new(1, GUT.Auto),
			new(1, GUT.Auto),
		]);
		Root
		.A(MkPageBtn(), o=>{
			o.BtnContent = Icons.ArrowLeft().ToIcon();
			o.SetExe((Ct)=>Ctx?.FnPrevPage?.Invoke(Ctx, Ct));
		})
		.A(_TextBox(), o=>{
			Ctx.Bind(
				o,o=>o.Text,x=>x.PageNum
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
			// 	Converter: new FnConvtr<u64?, bool>((x,p)=>x is not null)
			// );
			o.IsReadOnly = true;
			o.Focusable = false;
			o.BorderBrush = null;
			Ctx.Bind(
				o, o=>o.Text,x=>x.TotPageCnt
				,Converter: new FnConvtr<u64?, str>((x, p)=>x?.ToString()??"")
				,Mode: BindingMode.OneWay
			);
		})
		.A(MkPageBtn(), o=>{
			o.BtnContent = Icons.ArrowRight().ToIcon();
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
				o.Text = I[CommonK.PageSize];
			})
			.A(new ComboBox(), o=>{
				o.IsEditable = true;
				o.MinWidth = 96;
				o.ItemsSource = new str[]{"10", "20", "50"};
				Ctx.Bind(
					o,o=>o.Text,x=>x.PageSize
					,Converter: ConvU64Str(10)
					,Mode: BindingMode.TwoWay
				);
			});
		}

		var btn = new Button(){
			Init =o=>{
				o.Content = Icons.Menu().ToIcon();
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

