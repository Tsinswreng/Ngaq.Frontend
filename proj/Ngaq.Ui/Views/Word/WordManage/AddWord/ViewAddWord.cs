namespace Ngaq.Ui.Views.Word.WordManage.AddWord;

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
		Ctx = App.ServiceProvider.GetRequiredService<Ctx>();

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
		return NIL;
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
			var Popup_ = new MsgPopup();
			Root.Add(Popup_);
			{var a = Popup_;
				var Cfg = UiCfg.Inst;
				a._Popup.Width = Cfg.WindowWidth*0.9;
				a._Popup.MaxHeight = Cfg.WindowHeight*0.6;
				a._BdrBody.MaxHeight = a._Popup.MaxHeight*0.8;
				a._Popup.PlacementTarget = Root.Grid;
				a._Title.Content = new TextBlock{Text = "Error", FontSize = 26.0};
				a._CloseBtn.Click += (s,e)=>{
					Ctx!.IsShowMsg = false;
				};
				a._Popup.Bind(
					Popup.IsOpenProperty
					,CBE.Mk<Ctx>(x=>x.IsShowMsg
						,Mode: BindingMode.OneWay
					)
				);

				var Body = new SelectableTextBlock{};
				a._Body.Content = Body;
				{var b = Body;
					b.TextWrapping = TextWrapping.Wrap;
					b.Bind(
						TextBlock.TextProperty
						,CBE.Mk<Ctx>(x=>x.Msgs
							,Converter: new SimpleFnConvtr<object?, str>(y=>{
									var ans = string.Join("\n", y);
									return ans;
								}
							)
						)
					);
				}
				a._Border.Background = new SolidColorBrush(Color.FromRgb(30,30,30));
			}

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
		return NIL;
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
