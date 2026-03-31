namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using System.Collections.Generic;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmPreFilterEdit;

public partial class ViewPreFilterEdit: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewPreFilterEdit(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}

	public II18n I = I18n.Inst;
	public partial class Cls{}

	protected nil Style(){
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Auto),
		]);
		Root.A(MkTabs());
		Root.A(MkBottomBar());
		return NIL;
	}

	protected Control MkTabs(){
		var tab = new TabControl();
		tab.CBind<Ctx>(tab.PropSelectedIndex, x=>x.TabIndex);
		tab.Items.Add(MkVisualTab());
		tab.Items.Add(MkPoTab());
		tab.Items.Add(MkPreFilterTab());
		return tab;
	}

	protected Control MkBottomBar(){
		var bar = new AutoGrid(IsRow:false);
		bar.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		bar.A(_Button(), o=>{
			o.HorizontalContentAlignment = HAlign.Center;
			o.Content = "Sync Visual -> JSON";
			o.Click += (s,e)=>Ctx?.SyncJsonFromVisual();
		})
		.A(_Button(), o=>{
			o.Background = UiCfg.Inst.MainColor;
			o.HorizontalContentAlignment = HAlign.Center;
			o.Content = Svgs.FloppyDiskBackFill().ToIcon().WithText(" Save");
			o.Click += async (s,e)=>{
				if(Ctx is null){
					return;
				}
				await Ctx.Save();
			};
		})
		.A(_Button(), o=>{
			o.Background = new SolidColorBrush(Color.FromRgb(210, 56, 56));
			o.HorizontalContentAlignment = HAlign.Center;
			o.Content = Svgs.DeleteForeverSharp().ToIcon().WithText(" Delete");
			o.Click += async (s,e)=>{
				if(Ctx is null){
					return;
				}
				await Ctx.Delete();
			};
		});
		return bar.Grid;
	}

	Control MkInputRow(str Label, IBinding Binding, bool ReadOnly = false, bool AcceptsReturn = false){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		var tb = new TextBox{
			IsReadOnly = ReadOnly,
			AcceptsReturn = AcceptsReturn,
			TextWrapping = AcceptsReturn ? TextWrapping.Wrap : TextWrapping.NoWrap,
		};
		tb.Bind(TextBox.TextProperty, Binding);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkComboRow(str Label, IEnumerable<str> Items, IBinding Binding){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		var cb = new ComboBox();
		foreach(var item in Items){
			cb.Items.Add(item);
		}
		cb.Bind(ComboBox.SelectedIndexProperty, Binding);
		sp.Children.Add(cb);
		return sp;
	}

	Control MkBoundInput(str Label, IBinding Binding, bool AcceptsReturn = false){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label, FontSize = UiCfg.Inst.BaseFontSize * 0.9});
		var tb = new TextBox{
			AcceptsReturn = AcceptsReturn,
			TextWrapping = AcceptsReturn ? TextWrapping.Wrap : TextWrapping.NoWrap,
		};
		tb.Bind(TextBox.TextProperty, Binding);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkBoundCombo(str Label, IEnumerable<str> Items, IBinding Binding){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label, FontSize = UiCfg.Inst.BaseFontSize * 0.9});
		var cb = new ComboBox();
		foreach(var item in Items){
			cb.Items.Add(item);
		}
		cb.Bind(ComboBox.SelectedIndexProperty, Binding);
		sp.Children.Add(cb);
		return sp;
	}
}
