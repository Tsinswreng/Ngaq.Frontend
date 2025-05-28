using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Media;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using Tsinswreng.Avalonia.Tools;
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

	public class Cls_{
		public str inputBox = nameof(inputBox);
		public str logo = nameof(logo);
	}
	public Cls_ Cls{get;set;} = new Cls_();


	protected nil _Style(){
		var cls_inputBox = new Style(x=>
			x.Is<Control>()
			.Class(Cls.inputBox)
		);
		Styles.Add(cls_inputBox);
		{
			var o = cls_inputBox;
			o.Set(
				BackgroundProperty
				,new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20))
			);
		}
		return Nil;
	}

	protected nil _Render(){
		Content = _root();
		return Nil;
	}

	protected Control _root(){
		var root = new StackPanel{
			Spacing = 4.0
		};
		{{
			var backBtn = new Button();
			root.Children.Add(backBtn);
			{
				backBtn.Content = "←";
				backBtn.Click += (s,e)=>{
					Ctx?.ViewNavigator?.Back();
				};
			}

			var logo = new AppTextLogo(){
				FontSize = 30.0
			};
			root.Children.Add(logo);


			var formItem = _fn_addLabelBox(root);

			formItem("Email", CBE.Pth<Ctx>(x=>x.Email));
			formItem("Password", CBE.Pth<Ctx>(x=>x.Password));
			formItem("Confirm Password", CBE.Pth<Ctx>(x=>x.ConfirmPassword));
			//formItem("Captcha", CBE.pth<Ctx>(x=>x.Captcha));

			var submit = new Button{};
			root.Children.Add(submit);
			{
				var o = submit;
				o.Content = "Register";
				o.HorizontalAlignment = HoriAlign.Center;
				o.Click += (s,e)=>{
					//Ctx?.SubmitAsy().ContinueWith(d=>{});
				};
			}

			var errMsgSclv = new ScrollViewer{};
			root.Children.Add(errMsgSclv);
			{

			}
			errMsgSclv.Content = _ErrorList();
		}}
		return root;
	}

	protected Control _ErrorList(){
		var ans = new ItemsControl{};
		ans.Bind(
			ItemsControl.ItemsSourceProperty
			,new CBE(CBE.Pth<Ctx>(x=>x.Errors))
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

			return Nil;
		};
	}



}
