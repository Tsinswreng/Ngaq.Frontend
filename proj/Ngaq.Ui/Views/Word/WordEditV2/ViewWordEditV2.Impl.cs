namespace Ngaq.Ui.Views.Word.WordEditV2;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
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
using Tsinswreng.Avln.Grid;
using Tsinswreng.Avln.Dsl;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordEditV2;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

public partial class ViewWordEditV2{
	public partial ViewWordEditV2(){
		Ctx = App.DiOrMk<Ctx>();
		Render();
		HookSubPageEvents();
	}

	partial void Render(){
		this.SetContent(Root.Grid);
		Root.SetRowDefs([new(9, GUT.Star), new(1, GUT.Auto), new(1, GUT.Auto)]);
		Root.A(MkTabs()).A(MkErrBar()).A(MkBottomBar());
	}

	partial void HookSubPageEvents(){
		if(Ctx is null){
			return;
		}
		Ctx.WordPropPage.OnEditRequested += Row=>{
			var EditVm = new VmWordPropEdit{Row = Row};
			EditVm.OnDelete = Ct=>Ctx?.DeletePropRow(Row, Ct) ?? Task.FromResult(false);
			var EditView = new ViewWordPropEdit{Ctx = EditVm};
			ViewNavi?.GoTo(ToolView.WithTitle(I[K.EditProp], EditView));
		};
		Ctx.WordLearnPage.OnEditRequested += Row=>{
			var EditVm = new VmWordLearnEdit{Row = Row};
			EditVm.OnDelete = Ct=>Ctx?.DeleteLearnRow(Row, Ct) ?? Task.FromResult(false);
			var EditView = new ViewWordLearnEdit{Ctx = EditVm};
			ViewNavi?.GoTo(ToolView.WithTitle(I[K.EditLearn], EditView));
		};
	}

	private partial Control MkTabs(){
		var Tab = new TabControl();
		var PoWordEdit = new ViewPoWordEdit{Ctx = Ctx?.PoWordEdit};
		this.PoWordEdit = PoWordEdit;
		Tab.Bind(Tab.PropSelectedIndex, CBE.Mk<Ctx>(X=>X.TabIndex, Mode: BindingMode.TwoWay));
		Tab.Items.A(new TabItem(), O=>{
			O.Header = I[K.Basic];
			O.SetContent(PoWordEdit);
		}).A(new TabItem(), O=>{
			O.Header = I[K.Props];
			O.SetContent(new ViewWordPropPage{Ctx = Ctx?.WordPropPage});
		}).A(new TabItem(), O=>{
			O.Header = I[K.Learns];
			O.SetContent(new ViewWordLearnPage{Ctx = Ctx?.WordLearnPage});
		});
		return Tab;
	}

	private partial Control MkErrBar(){
		var Border = new Border{Background = new SolidColorBrush(Color.FromArgb(70, 180, 20, 20)), Padding = new(10, 6), IsVisible = false};
		Border.Bind(IsVisibleProperty, CBE.Mk<Ctx>(X=>X.HasError, Mode: BindingMode.OneWay));
		Border.SetChild(new TextBlock{Foreground = Brushes.White}, Text=>{
			Text.Bind(TextBlock.TextProperty, CBE.Mk<Ctx>(X=>X.LastError, Mode: BindingMode.OneWay));
		});
		return Border;
	}

	private partial Control MkBottomBar(){
		var Grid = new GridStack(IsRow: false);
		Grid.Grid.SetColDefs([new(1, GUT.Star), new(1, GUT.Star)]);
		Grid.Grid.Margin = new(10, 8, 10, 10);
		Grid.A(new OpBtn(), O=>{
			DeleteBtn = O;
			O._Button.Background = UiCfg.Inst.DelBtnBg;
			O._Button.StretchCenter();
			O.BtnContent = Icons.Delete().ToIcon().WithText(I[K.Delete]);
			O.SetExe(Ct=>Ctx?.Delete(Ct));
			O.HookDoneEvent(()=>DoneDelete?.Invoke(this, EventArgs.Empty));
		}).A(new OpBtn(), O=>{
			SaveBtn = O;
			O._Button.Background = UiCfg.Inst.MainColor;
			O._Button.StretchCenter();
			O.BtnContent = Icons.Save().ToIcon().WithText(I[K.Save]);
			O.SetExe(Ct=>Ctx?.Save(Ct));
			O.HookDoneEvent(()=>DoneSave?.Invoke(this, EventArgs.Empty));
		});
		return Grid.Grid;
	}
}
