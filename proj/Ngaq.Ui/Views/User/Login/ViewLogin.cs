namespace Ngaq.Ui.Views.User.Login;

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using K = Ngaq.Ui.Infra.I18n.ItemsUiI18n.LoginRegister;


using Ctx = VmLoginRegister;
using Ngaq.Ui.Infra.I18n;

public partial class ViewLogin
	:UserControl
	//,IHasViewNavigator
{

	//public IViewNavigator? ViewNavigator{get;set;}
	public II18n I = I18n.Inst;

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}


	public ViewLogin(){
		Ctx = Ctx.Mk();
		//Ctx = App.ServiceProvider.GetRequiredService<Ctx>();
		_Style();
		_Render();
	}

public partial class Cls_{
		public str InputBox = nameof(InputBox);
		public str Logo = nameof(Logo);
	}
	public Cls_ Cls{get;set;} = new Cls_();


	protected nil _Style(){
		var cls_inputBox = new Style(x=>
			x.Is<Control>()
			.Class(Cls.InputBox)
		);
		Styles.Add(cls_inputBox);
		{
			var o = cls_inputBox;
			o.Set(
				BackgroundProperty
				,new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20))
			);
		}

		return NIL;
	}

	protected nil _Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Star),
			]);
		});
		Root.AddInit(_StackPanel(), Stk=>{
			Stk.Spacing = 4.0;

			var formItem = FnAddLabelBox(Stk);
			formItem(I[K.Email], CBE.Pth<Ctx>(x=>x.Email));
			formItem(I[K.Password], CBE.Pth<Ctx>(x=>x.Password));
			//formItem("Confirm Password", CBE.pth<Ctx>(x=>x.ConfirmPassword));
			//formItem("Captcha", CBE.pth<Ctx>(x=>x.Captcha));
			//TODO 用popup彈出框
			var errMsgSclv = new ScrollViewer{};
			Stk.Children.Add(errMsgSclv);
			{

			}
			errMsgSclv.Content = _ErrorList();
		})
		.AddInit(new Button{}, (b)=>{
			b.ContentInit(_TextBlock(), t=>{
				t.Text = I[K.Login];
				t.FontSize = UiCfg.Inst.BaseFontSize*1.2;
			});
			b.HorizontalAlignment = HAlign.Stretch;
			b.Click += (s,e)=>{
				Ctx?.Login();
			};
		});
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);

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


	protected Func<str, CompiledBindingPath, nil?> FnAddLabelBox(Panel container){
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
				o.Classes.Add(Cls.InputBox);
				label.Target = o;
				o.Bind(
					o.PropText_()
					,new CBE(pth){
						Mode = BindingMode.TwoWay
					}
				);
			}

			return null;
		};
	}


}
