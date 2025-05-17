namespace Ngaq.Ui.Views.WordManage.AddWord;

using Avalonia.Controls;
using Avalonia.Layout;
using Ctx = Vm_AddWord;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Data;
using Avalonia.Styling;
using Tsinswreng.Avalonia.Tools;

public partial class View_AddWord
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public View_AddWord(){
		//Ctx = new Ctx();
		Ctx = App.ServiceProvider.GetRequiredService<Vm_AddWord>();

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
		return null!;
	}

	protected nil Render(){
		var root = new IndexGrid(IsRow:true);
		Content = root.Grid;
		{
			var o = root;
			root.Grid.RowDefinitions.AddRange([
				new RowDefinition(8, GridUnitType.Star),
				new RowDefinition(1, GridUnitType.Auto),
				new RowDefinition(1, GridUnitType.Star),
			]);
		}
		{{
			var Tab = new TabControl();
			root.Add(Tab);
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
			root.Add(Confirm);
			{
				var o = Confirm;
				o.Content = "Confirm";
				o.HorizontalAlignment = HorizontalAlignment.Center;
				o.HorizontalContentAlignment = HorizontalAlignment.Center;
				o.Click += (s,e)=>{
					Ctx?.Confirm();
				};
			}
		}}//~IndexGrid
		return null!;
	}



	Control? ByFile(){
		var Ans = new IndexGrid(IsRow:true);
		Ans.Grid.RowDefinitions.AddRange([
			new RowDefinition(1, GridUnitType.Star),
			new RowDefinition(1, GridUnitType.Auto),
			new RowDefinition(8, GridUnitType.Star),
		]);
		{{
			Ans.Add();

			var Path = new IndexGrid(IsRow:false);
			Ans.Add(Path.Grid);
			{
				var o = Path;
				o.Grid.ColumnDefinitions.AddRange([
					new ColumnDefinition(2, GridUnitType.Star),
					new ColumnDefinition(8, GridUnitType.Star),
					//new ColumnDefinition(2, GridUnitType.Star),
				]);
			}
			{{

				var Browse = new Button();
				Path.Add(Browse);
				{
					var o = Browse;
					o.Content = "Browse";
					o.HorizontalAlignment = HorizontalAlignment.Stretch;
					o.HorizontalContentAlignment = HorizontalAlignment.Stretch;
				}


				var Input = new TextBox();
				Path.Add(Input);
				{
					var o = Input;
					o.HorizontalAlignment = HorizontalAlignment.Stretch;
					o.Bind(
						TextBox.TextProperty
						,new CBE(CBE.Pth<Ctx>(x=>x.Path)){Mode=BindingMode.TwoWay}
					);
				}

				// var Confirm = new Button();
				// Path.Add(Confirm);
				// {
				// 	var o = Confirm;
				// 	o.Content = "Confirm";
				// }
			}}
			Ans.Add();

		}}
		return Ans.Grid;
	}

	Control? ByText(){
		var Ans = new IndexGrid(IsRow:true);
		Ans.Grid.RowDefinitions.AddRange([
			new RowDefinition(1, GridUnitType.Star),
			new RowDefinition(8, GridUnitType.Star),
			new RowDefinition(1, GridUnitType.Star),
			new RowDefinition(1, GridUnitType.Star),
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
		Ans.Add();

		return Ans.Grid;
	}
}
