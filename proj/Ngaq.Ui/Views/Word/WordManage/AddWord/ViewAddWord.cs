namespace Ngaq.Ui.Views.Word.WordManage.AddWord;

using Avalonia.Controls;
using Avalonia.Layout;
using Ctx = VmAddWord;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Data;
using Avalonia.Styling;
using Tsinswreng.AvlnTools.Tools;
using Avalonia.Media;
using Avalonia;
using Semi.Avalonia;
using Ursa.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;

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

		this.KeyDown += (s,e)=>{
			if(e.Key == Avalonia.Input.Key.Escape){

			}
		};
	}

	public  partial class Cls_{

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
		var Root = new AutoGrid(IsRow:true);
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),//Popup
				RowDef(1, GUT.Auto),//empty
				RowDef(8, GUT.Star),//tab
				RowDef(1, GUT.Auto),//Confirm
				RowDef(1, GUT.Star),//empty
			]);
		});
		{{

			Root.AddInit(new MsgPopup(), a=>{
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

				//a._Body.Content = Body;
				a._Body.ContentInit(new SelectableTextBlock{}, b=>{
					b.TextWrapping = TextWrapping.Wrap;
					b.Bind(
						TextBlock.TextProperty
						,CBE.Mk<Ctx>(x=>x.Msgs
							,Converter: new SimpleFnConvtr<ICollection<object?>, str>(y=>{
								var ans = string.Join("\n", y);
								return ans;
							})
						)
					);
				});
				a._Border.Background = new SolidColorBrush(Color.FromRgb(30,30,30));
			});

			Root.Add();
			Root.AddInit(_TabControl(), Tab=>{
				Tab.Items.AddInit(_TabItem(), o=>{
					o.Header = "By File";
					o.Content = ByFile();
				});
				Tab.Items.AddInit(_TabItem(), o=>{
					o.Header = "By Text";
					o.Content = ByText();
				});
			});
			Root.AddInit(_Button(), o=>{
				o.Content = "Confirm";
				o.HorizontalAlignment = HoriAlign.Center;
				o.HorizontalContentAlignment = HoriAlign.Center;
				o.Click += (s,e)=>{
					Ctx?.Confirm();
				};
			});
		}}//~IndexGrid
		return NIL;
	}

	Control? ByFile(){
		var Ans = new AutoGrid(IsRow:true);
		Ans.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Auto),
			RowDef(8, GUT.Star),
		]);
		{{
			Ans.Add();

			var Path = new AutoGrid(IsRow:false);
			Ans.AddInit(Path.Grid, o=>{
				o.ColumnDefinitions.AddRange([
					ColDef(2, GUT.Star),
					ColDef(8, GUT.Star),
				]);
			});
			{{

				Path.AddInit(_Button(), o=>{
					o.Content = "Browse";
					o.HorizontalAlignment = HoriAlign.Stretch;
					o.HorizontalContentAlignment = HoriAlign.Stretch;
				});
				Path.AddInit(_TextBox(), o=>{
					o.HorizontalAlignment = HoriAlign.Stretch;
					o.Bind(
						TextBox.TextProperty
						,new CBE(CBE.Pth<Ctx>(x=>x.Path)){Mode=BindingMode.TwoWay}
					);
				});

			}}
			Ans.Add();

		}}
		return Ans.Grid;
	}

	Control? ByText(){
		var Ans = new AutoGrid(IsRow:true);
		Ans.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
			RowDef(8, GUT.Star),
			RowDef(1, GUT.Star),
		]);

		Ans.Add();

		Ans.AddInit(_TextBox(), o=>{
			o.Bind(
				TextBox.TextProperty
				,new CBE(CBE.Pth<Ctx>(x=>x.Text)){Mode=BindingMode.TwoWay}
			);
		});

		Ans.Add();

		return Ans.Grid;
	}
}
