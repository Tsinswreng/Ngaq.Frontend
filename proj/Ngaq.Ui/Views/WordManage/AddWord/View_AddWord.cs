namespace Ngaq.Ui.Views.WordManage.AddWord;

using Avalonia.Controls;
using Avalonia.Layout;
using Tsinswreng.Avalonia.Util;
using Ctx = Vm_AddWord;
public partial class View_AddWord
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public View_AddWord(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
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
		}
		Ans.Add();
		Ans.Add();

		return Ans.Grid;
	}
}
