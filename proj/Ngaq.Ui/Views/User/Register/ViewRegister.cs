using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
namespace Ngaq.Ui.Views.User.Register;

using Ctx = VmLoginRegister;
public partial class ViewRegister
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}


	public ViewRegister(){
		Ctx = new Ctx();
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
		var cls_inputBox = new Style(x=>
			x.Is<Control>()
			.Class(Cls.inputBox)
		);
		Styles.AddInit(cls_inputBox, o=>o.Set(
			BackgroundProperty
			,new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20))
		));

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

			formItem("Email", CBE.Pth<Ctx>(x=>x.Email));
			formItem("Password", CBE.Pth<Ctx>(x=>x.Password));
			formItem("Confirm Password", CBE.Pth<Ctx>(x=>x.ConfirmPassword));
			//formItem("Captcha", CBE.pth<Ctx>(x=>x.Captcha));
			var errMsgSclv = new ScrollViewer{};
			Stk.Children.Add(errMsgSclv);

			errMsgSclv.Content = _ErrorList();
		})
		.AddInit(_Button(), b=>{
			b.ContentInit(_TextBlock(), t=>{
				t.Text = "Register";
				t.FontSize = UiCfg.Inst.BaseFontSize*1.2;
			});
			b.HorizontalAlignment = HAlign.Stretch;
			b.Click += (s,e)=>{
				Ctx?.Register();
			};
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
					TextBox.TextProperty
					,new CBE(pth){
						Mode = BindingMode.TwoWay
					}
				);
			}
			return NIL;
		};
	}

}

