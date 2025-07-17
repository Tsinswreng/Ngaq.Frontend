using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Media;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

namespace Ngaq.Ui.Views.User.Login;

using Ctx = VmLoginRegister;
public partial class ViewLogin
	:UserControl
	//,IHasViewNavigator
{

	//public IViewNavigator? ViewNavigator{get;set;}

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}


	public ViewLogin(){
		Ctx = new Ctx();
		//Ctx = App.ServiceProvider.GetRequiredService<Ctx>();
		_Style();
		_Render();
	}

public class Cls_{
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
		Content = _Root();
		return NIL;
	}

	protected Control _Root(){
		var Root = new StackPanel{
			Spacing = 4.0
		};
		{{
			var backBtn = new Button();
			Root.Children.Add(backBtn);
			{
				backBtn.Content = "←";
				backBtn.Click += (s,e)=>{
					Ctx?.ViewNavigator?.Back();
				};
			}


			Root.AddInit(new AppTextLogo(){
				FontSize = 30.0
			});

			// var logo = new AppTextLogo(){
			// 	FontSize = 30.0
			// };
			// Root.Children.Add(logo);


			var formItem = FnAddLabelBox(Root);

			formItem("Email", CBE.Pth<Ctx>(x=>x.Email));
			formItem("Password", CBE.Pth<Ctx>(x=>x.Password));
			//formItem("Confirm Password", CBE.pth<Ctx>(x=>x.ConfirmPassword));
			//formItem("Captcha", CBE.pth<Ctx>(x=>x.Captcha));

			Root.AddInit(new Button{}, (o)=>{
				o.Content = "Login";
				o.HorizontalAlignment = HoriAlign.Center;
				o.Click += (s,e)=>{
					Ctx?.Login();
				};
			});

			// var Submit = new Button{};
			// Root.Children.Add(Submit);
			// {
			// 	var o = Submit;
			// 	o.Content = "Login";
			// 	o.HorizontalAlignment = HoriAlign.Center;
			// 	o.Click += (s,e)=>{
			// 		Ctx?.Login();
			// 	};
			// }

			//TODO 用popup彈出框
			var errMsgSclv = new ScrollViewer{};
			Root.Children.Add(errMsgSclv);
			{

			}
			errMsgSclv.Content = _ErrorList();
		}}
		return Root;
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
					TextBox.TextProperty
					,new CBE(pth){
						Mode = BindingMode.TwoWay
					}
				);
			}

			return null;
		};
	}


}
