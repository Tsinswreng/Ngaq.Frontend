namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmPreFilterEdit;
public partial class ViewPreFilterEdit
	:AppViewBase
{

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
	public partial class Cls{

	}


	protected nil Style(){
		return NIL;
	}


	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.Content = Root.Grid;
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
		tab.CBind<Ctx>(
			tab.PropSelectedIndex
			,x=>x.TabIndex
		);
		tab.Items.Add(MkPoTab());
		tab.Items.Add(MkPreFilterTab());
		return tab;
	}

	protected TabItem MkPoTab(){
		var tab = new TabItem{
			Header = Todo.I18n("PoPreFilter"),
		};
		tab.InitContent(JsonText(), o=>{
			o.CBind<Ctx>(o.PropText, x=>x.PoPreFilterJson);
		});
		return tab;
	}

	protected TabItem MkPreFilterTab(){
		var tab = new TabItem{
			Header = Todo.I18n("PreFilter"),
		};
		tab.InitContent(JsonText(), o=>{
			o.CBind<Ctx>(o.PropText, x=>x.PreFilterJson);
		});
		return tab;
	}

	TextBox JsonText(){
		var box = new TextBox{
			AcceptsReturn = true,
			AcceptsTab = true,
			TextWrapping = TextWrapping.Wrap,
			HorizontalAlignment = HAlign.Stretch,
			VerticalAlignment = VAlign.Stretch,
		};
		return box;
	}

	protected Control MkBottomBar(){
		var bar = new AutoGrid(IsRow:false);
		bar.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		bar.A(_Button(), o=>{
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
}

