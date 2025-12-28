namespace Ngaq.Ui.Views.User.Register;

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using K = Ngaq.Ui.Infra.I18n.ItemsUiI18n.LoginRegister;

using Ctx = VmLoginRegister;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Infra.Ctrls;

public partial class ViewRegister
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public II18n I = I18n.Inst;


	public ViewRegister(){
		Ctx = Ctx.Mk();
		//Ctx = App.ServiceProvider.GetRequiredService<Ctx>();
		_Style();
		_Render();

	}

	public  partial class Cls_{
		public str inputBox = nameof(inputBox);
		public str logo = nameof(logo);
	}
	public Cls_ Cls{get;set;} = new Cls_();


	protected nil _Style(){
		new Style(x=>
			x.Is<Control>()
			.Class(Cls.inputBox)
		).Set(
			BackgroundProperty
			,new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20))
		).Attach(Styles);
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);
	protected nil _Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Star),
			]);
		});


		Root.AddInit(_StackPanel(), Stk=>{
			Stk.Spacing = 4.0;
			var formItem = _fn_addLabelBox(Stk);

			formItem(I[K.Email], CBE.Pth<Ctx>(x=>x.Email));
			formItem(I[K.Password], CBE.Pth<Ctx>(x=>x.Password));
			formItem(I[K.ConfirmPassword], CBE.Pth<Ctx>(x=>x.ConfirmPassword));
			//formItem("Captcha", CBE.pth<Ctx>(x=>x.Captcha));
			var errMsgSclv = new ScrollViewer{};
			Stk.Children.Add(errMsgSclv);

			errMsgSclv.Content = _ErrorList();
		})
		.AddInit(new OpBtn(), o=>{
			var b = o._Button;
			b.ContentInit(_TextBlock(), t=>{
				t.Text = I[K.Register];
				t.FontSize = UiCfg.Inst.BaseFontSize*1.2;
			});
			b.HorizontalAlignment = HAlign.Stretch;
			o.SetExe((Ct)=>Ctx?.RegisterAsy(Ct));
		});

		return NIL;
	}


	protected Control _ErrorList(){
		var ans = new ItemsControl{};
		ans.Bind(
			ItemsControl.ItemsSourceProperty
			,new CBE(CBE.Pth<Ctx>(x=>x.Msgs))
		);
		ans.ItemTemplate = new FuncDataTemplate<str>((err,b)=>{
			var ua = new SelectableTextBlock{};
			ua.Text = err;
			ua.Foreground = Brushes.Red;
			ua.TextWrapping = TextWrapping.Wrap;
			return ua;
		});
		return ans;
	}


	protected Func<str, CompiledBindingPath, nil> _fn_addLabelBox(Panel container){
		return (labelText, pth)=>{
			var label = new Label{};
			container.Children.Add(label);
			{
				var o = label;
				o.Content = labelText;
			}

			var box = new TextBox{};
			container.Children.Add(box);
			{
				var o = box;
				o.Classes.Add(Cls.inputBox);
				label.Target = o;
				o.Bind(
					o.PropText
					,new CBE(pth){
						Mode = BindingMode.TwoWay
					}
				);
			}
			return NIL;
		};
	}

}

