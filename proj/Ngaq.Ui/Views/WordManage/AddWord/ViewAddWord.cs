namespace Ngaq.Ui.Views.WordManage.AddWord;

using Avalonia.Controls;
using Avalonia.Layout;
using Ctx = VmAddWord;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Data;
using Avalonia.Styling;
using Tsinswreng.Avalonia.Tools;
using Avalonia.Media;
using Avalonia;
using Semi.Avalonia;
using Ursa.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Tsinswreng.Avalonia.Sugar;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using Tsinswreng.Avalonia.Controls;

public partial class ViewAddWord
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewAddWord(){
		//Ctx = new Ctx();
		Ctx = App.ServiceProvider.GetRequiredService<VmAddWord>();

		Style();
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		var AcceptReturn = new Style(x=>
			x.Is<TextBox>()
		);
		Styles.Add(AcceptReturn);
		{
			var o = AcceptReturn;
			o.Set(TextBox.AcceptsReturnProperty, true);
		}

		//Styles.Add(SugarStyle.GridShowLines());
		return Nil;
	}

	protected nil Render(){
		var Root = new IndexGrid(IsRow:true);
		Content = Root.Grid;
		{
			var o = Root;
			Root.Grid.RowDefinitions.AddRange([
				new RowDef(1, GUT.Auto),//Popup
				new RowDef(1, GUT.Auto),//empty
				new RowDef(8, GUT.Star),//tab
				new RowDef(1, GUT.Auto),//Confirm
				new RowDef(1, GUT.Star),//empty
			]);
		}
		{{
			var Popup = new ConfirmPopup();
			Root.Add(Popup);
			{var o = Popup;
				//o.Width = 400;//t 不效
				//o._ConfirmBox.Root.Grid.Width = Root.Grid.Width*0.8; //不效
				//o._ConfirmBox.Root.Grid.Width = 600;
				o._Popup.Width = 320; //有效
				//o._Popup.MaxHeight = 200;
				//o._Popup.Width = Root.Grid.Width*0.8; //不效 蓋Root之寬 此時未定
				o._Popup.PlacementTarget = Root.Grid;
				o._Popup.IsOpen = true;
				o._ConfirmBox._LeftBtn.Content = "Close";
				o._ConfirmBox._LeftBtn.Click += (s,e)=>{
					o._Popup.IsOpen = false;
				};
				o._ConfirmBox._RightBtn.Content = "Ok";

				// o._ConfirmBox._Title = new SelectableTextBlock{
				// 	Text = "Error"
				// 	,FontSize = 26.0
				// };//不示
				o._ConfirmBox._Title.Content = new TextBlock{Text = "Error", FontSize = 26.0};//t

				o._ConfirmBox._Body.Content = new SelectableTextBlock{
					Text =
"Error\nmessage\nError\nmessage\nError\nmessage\nError\nmessage\nError\nmessage\nError\nmessage\nError"
				};
				o._ConfirmBox.Root.Grid.Background = Brushes.DarkSlateGray;
			}

			// var popup = new Popup();
			// Root.Add(popup);
			// {var o = popup;
			// 	o.PlacementTarget = Root.Grid;
			// 	o.Placement = PlacementMode.Top;
			// 	//var top = TopLevel.GetTopLevel(this);
			// 	// o.VerticalOffset = top?.Width/4??0.0;
			// 	// o.HorizontalOffset = top?.Height/4??0.0;
			// 	o.HorizontalAlignment = HoriAlign.Stretch;
			// 	o.VerticalAlignment = VertAlign.Stretch;
			// 	Ctx!.Errors.CollectionChanged += (s,e)=>{
			// 		Dispatcher.UIThread.Post(()=>{
			// 			if(Ctx.HasErr){o.IsOpen = true;}
			// 		});
			// 	};

			// 	// o.Bind(
			// 	// 	Popup.IsOpenProperty
			// 	// 	,new CBE(CBE.Pth<Ctx>(x=>x.HasErr))//TODO 不效
			// 	// 	// ,new CBE(CBE.Pth<Ctx>(x=>x.Errors)){
			// 	// 	// 	Converter = new FnConvtr<ObservableCollection<str>, bool>(x=>x.Count>0)
			// 	// 	// }

			// 	// );
			// }
			// {{
			// 	var ErrBlock = new TextBlock{Text="123"};
			// 	popup.Child = ErrBlock;
			// 	{var o = ErrBlock;
			// 		// o.Bind(
			// 		// 	TextBlock.TextProperty
			// 		// 	,new CBE(CBE.Pth<Ctx>(x=>x.Errors)){
			// 		// 		Mode=BindingMode.OneWay
			// 		// 		,Converter = new FnConvtr<ObservableCollection<str>, str>(
			// 		// 			x=>string.Join("\n", x)
			// 		// 		)
			// 		// 	}
			// 		// 	//,new CBE(CBE.Pth<Ctx>(x=>x.ErrStr))
			// 		// );
			// 		Ctx!.Errors.CollectionChanged += (s,e)=>{
			// 			Dispatcher.UIThread.Post(()=>{
			// 				var ErrStr = string.Join("\n", Ctx.Errors);
			// 				//System.Console.WriteLine(ErrStr+"\nErrStr");
			// 				ErrBlock.Text = ErrStr;
			// 			});
			// 		};
			// 	}
			// }}//~Popup
			Root.Add();

			var Tab = new TabControl();
			Root.Add(Tab);
			{
				var o = Tab;

			}
			{{
				var byUrl = new TabItem();
				Tab.Items.Add(byUrl);
				{
					var o = byUrl;
					//o.Content = "By URL";
					o.Header = "By File";
					o.Content = ByFile();
				}

				var byText = new TabItem();
				Tab.Items.Add(byText);
				{
					var o = byText;
					o.Header = "By Text";
					o.Content = ByText();
				}
			}}//~TabControl
			var Confirm = new Button();
			Root.Add(Confirm);
			{
				var o = Confirm;
				o.Content = "Confirm";
				o.HorizontalAlignment = HoriAlign.Center;
				o.HorizontalContentAlignment = HoriAlign.Center;
				o.Click += (s,e)=>{
					Ctx?.Confirm();
				};
			}

		}}//~IndexGrid
		return Nil;
	}

	Control? ByFile(){
		var Ans = new IndexGrid(IsRow:true);
		Ans.Grid.RowDefinitions.AddRange([
			new RowDef(1, GUT.Star),
			new RowDef(1, GUT.Auto),
			new RowDef(8, GUT.Star),
		]);
		{{
			Ans.Add();

			var Path = new IndexGrid(IsRow:false);
			Ans.Add(Path.Grid);
			{
				var o = Path;
				o.Grid.ColumnDefinitions.AddRange([
					new ColDef(2, GUT.Star),
					new ColDef(8, GUT.Star),
					//new ColumnDefinition(2, GridUnitType.Star),
				]);
			}
			{{
				var Browse = new Button();
				Path.Add(Browse);
				{
					var o = Browse;
					o.Content = "Browse";
					o.HorizontalAlignment = HoriAlign.Stretch;
					o.HorizontalContentAlignment = HoriAlign.Stretch;
				}


				var Input = new TextBox();
				Path.Add(Input);
				{
					var o = Input;
					o.HorizontalAlignment = HoriAlign.Stretch;
					o.Bind(
						TextBox.TextProperty
						,new CBE(CBE.Pth<Ctx>(x=>x.Path)){Mode=BindingMode.TwoWay}
					);
				}
			}}
			Ans.Add();

		}}
		return Ans.Grid;
	}

	Control? ByText(){
		var Ans = new IndexGrid(IsRow:true);
		Ans.Grid.RowDefinitions.AddRange([
			new RowDef(1, GUT.Star),
			new RowDef(8, GUT.Star),
			new RowDef(1, GUT.Star),
		]);

		Ans.Add();

		var Input = new TextBox();
		Ans.Add(Input);
		{
			var o = Input;
			o.Bind(
				TextBox.TextProperty
				,new CBE(CBE.Pth<Ctx>(x=>x.Text)){Mode=BindingMode.TwoWay}
			);
		}
		Ans.Add();

		return Ans.Grid;
	}
}
