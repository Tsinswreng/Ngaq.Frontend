namespace Ngaq.Ui.Views.Word.WordEditV2;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.PoWordEdit;
using Ngaq.Ui.Views.Word.WordLearnEdit;
using Ngaq.Ui.Views.Word.WordLearnPage;
using Ngaq.Ui.Views.Word.WordPropEdit;
using Ngaq.Ui.Views.Word.WordPropPage;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordEditV2;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// 單詞編輯主頁：僅承擔三個分頁容器與全局保存/刪除。
public partial class ViewWordEditV2: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordEditV2(){
		Ctx = App.DiOrMk<Ctx>();
		Render();
		HookSubPageEvents();
	}

	AutoGrid Root = new(IsRow: true);

	void Render(){
		Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(9, GUT.Star),
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
		]);
		Root
		.A(MkTabs())
		.A(MkErrBar())
		.A(MkBottomBar())
		;
	}

	void HookSubPageEvents(){
		if(Ctx is null){
			return;
		}
		Ctx.WordPropPage.OnEditRequested += row=>{
			var editVm = new VmWordPropEdit{Row = row};
			editVm.OnRemove = r=>Ctx?.WordPropPage.RemoveRow(r);
			var editView = new ViewWordPropEdit{Ctx = editVm};
			ViewNavi?.GoTo(ToolView.WithTitle(I[K.EditProp], editView));
		};
		Ctx.WordLearnPage.OnEditRequested += row=>{
			var editVm = new VmWordLearnEdit{Row = row};
			editVm.OnRemove = r=>Ctx?.WordLearnPage.RemoveRow(r);
			var editView = new ViewWordLearnEdit{Ctx = editVm};
			ViewNavi?.GoTo(ToolView.WithTitle(I[K.EditLearn], editView));
		};
	}

	Control MkTabs(){
		var tab = new TabControl();
		tab.Bind(tab.PropSelectedIndex, CBE.Mk<Ctx>(x=>x.TabIndex, Mode: BindingMode.TwoWay));
		tab.Items.A(new TabItem(), o=>{
			o.Header = I[K.Basic];
			o.Content = new ViewPoWordEdit{Ctx = Ctx?.PoWordEdit};
		})
		.A(new TabItem(), o=>{
			o.Header = I[K.Props];
			o.Content = new ViewWordPropPage{Ctx = Ctx?.WordPropPage};
		})
		.A(new TabItem(), o=>{
			o.Header = I[K.Learns];
			o.Content = new ViewWordLearnPage{Ctx = Ctx?.WordLearnPage};
		});
		return tab;
	}

	Control MkErrBar(){
		var b = new Border{
			Background = new SolidColorBrush(Color.FromArgb(70, 180, 20, 20)),
			Padding = new Thickness(10, 6),
			IsVisible = false,
		};
		b.Bind(IsVisibleProperty, CBE.Mk<Ctx>(x=>x.HasError, Mode: BindingMode.OneWay));
		var txt = new TextBlock{Foreground = Brushes.White};
		txt.Bind(TextBlock.TextProperty, CBE.Mk<Ctx>(x=>x.LastError, Mode: BindingMode.OneWay));
		b.Child = txt;
		return b;
	}

	Control MkBottomBar(){
		var g = new AutoGrid(IsRow: false);
		g.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		g.Grid.Margin = new Thickness(10, 8, 10, 10);

		g.A(new OpBtn(), o=>{
			o._Button.Background = UiCfg.Inst.DelBtnBg;
			o._Button.StretchCenter();
			o.BtnContent = Icons.Delete().ToIcon().WithText(I[K.Delete]);
			o.SetExe(ct=>Ctx?.Delete(ct));
		})
		.A(new OpBtn(), o=>{
			o._Button.Background = UiCfg.Inst.MainColor;
			o._Button.StretchCenter();
			o.BtnContent = Icons.Save().ToIcon().WithText(I[K.Save]);
			o.SetExe(ct=>Ctx?.Save(ct));
		});
		return g.Grid;
	}
}
