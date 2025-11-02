// Ngaq.Frontend\proj\Ngaq.Ui\Views\User\Login\ViewLogin.cs
namespace Ngaq.Ui.Views.User.Login;

using System;
using System.Reactive.Linq;              // + 仅新增命名空间
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
using ReactiveUI;                        // + 仅新增命名空间
using Avalonia;
using System.Reactive.Disposables.Fluent;

public partial class ViewLogin : UserControl, IViewFor<Ctx>
{
	public II18n I = I18n.Inst;

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewLogin(){
		Ctx = Ctx.Mk();
		_Style();
		_Render();

		// + 下面 3 行 = 防抖唯一改动
		// this.WhenActivated(d => {
		// 	Ctx?.LoginCommand
		// 		.Throttle(TimeSpan.FromMilliseconds(400))
		// 		.InvokeCommand(Ctx.LoginCommand)
		// 		.DisposeWith(d);
		// });
	}

	public partial class Cls_{
		public str InputBox = nameof(InputBox);
		public str Logo = nameof(Logo);
	}
	public Cls_ Cls{get;set;} = new Cls_();
	public Ctx? ViewModel {get=>Ctx;set=>Ctx=value;}
	object? IViewFor.ViewModel {get=>Ctx;set=>Ctx=value as Ctx;}

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

			var errMsgSclv = new ScrollViewer{};
			Stk.Children.Add(errMsgSclv);
			errMsgSclv.Content = _ErrorList();
		})
		.AddInit(new Button{}, (b)=>{
			b.ContentInit(_TextBlock(), t=>{
				t.Text = I[K.Login];
				t.FontSize = UiCfg.Inst.BaseFontSize*1.2;
			});
			b.HorizontalAlignment = HAlign.Stretch;
			b.Bind(
				Button.CommandProperty
				,CBE.Mk<Ctx>(x=>x.LoginCommand)
			);
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
