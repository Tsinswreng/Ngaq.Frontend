namespace Ngaq.Ui.Views.User.Login;

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;


using Ctx = VmLoginRegister;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra;

public partial class ViewLogin
	:AppViewBase
	//,IHasViewNavigator
{

	//public IViewNavigator? ViewNavigator{get;set;}


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
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Star),
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Star),
			]);
		});

		// 上下空白等比，內容集中在中間區域。
		Root.A(new Border(), _=>{});
		Root.A(new StackPanel(), Stk=>{
			Stk.Spacing = 8.0;

			var formItem = FnAddLabelBox(Stk);
			formItem(I[K.Email], CBE.Pth<Ctx>(x=>x.Email), false);
			formItem(I[K.Password], CBE.Pth<Ctx>(x=>x.Password), true);
			var errMsgSclv = new ScrollViewer{};
			Stk.Children.Add(errMsgSclv);
			{
			}
			errMsgSclv.Content = _ErrorList();
			Stk.A(new OpBtn(), (o)=>{
				var b = o._Button;
				b.SetContent(new TextBlock(), t=>{
					t.Text = I[K.Login];
					t.FontSize = UiCfg.Inst.BaseFontSize*1.2;
					t.TextAlignment = TextAlignment.Center;
					t.HorizontalAlignment = HAlign.Center;
				});
				b.HorizontalAlignment = HAlign.Stretch;
				b.Background = UiCfg.Inst.MainColor;
				o.SetExe((Ct)=>Ctx?.LoginAsy(Ct));
			});
		});
		Root.A(new Border(), _=>{});
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


	protected Func<str, CompiledBindingPath, bool, nil?> FnAddLabelBox(Panel Container){
		return (LabelText, Pth, IsPassword)=>{
			var label = new Label{};
			Container.Children.Add(label);
			{
				var o = label;
				o.Content = LabelText;
			}

			if(IsPassword){
				var box = new TextBox{};
				Container.Children.Add(box);
				{
					var o = box;
					o.Classes.Add(Cls.InputBox);
					o.PasswordChar = '*';
					label.Target = o;
					o.Bind(
						o.PropText
						,new CBE(Pth){
							Mode = BindingMode.TwoWay
						}
					);
				}
			}else{
				var box = new TextBox{};
				Container.Children.Add(box);
				{
					var o = box;
					o.Classes.Add(Cls.InputBox);
					label.Target = o;
					o.Bind(
						o.PropText
						,new CBE(Pth){
							Mode = BindingMode.TwoWay
						}
					);
				}
			}

			return null;
		};
	}


}
