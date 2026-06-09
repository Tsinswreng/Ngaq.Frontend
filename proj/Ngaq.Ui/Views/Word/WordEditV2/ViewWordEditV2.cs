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
using Tsinswreng.Avln.Grid;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordEditV2;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// 單詞編輯主頁：僅承擔三個分頁容器與全局保存/刪除。
public partial class ViewWordEditV2
	: AppViewBase<Ctx>
	, IViewWordEditV2
{
	public OpBtn? DeleteBtn{get;set;}
	public OpBtn? SaveBtn{get;set;}
	public IViewPoWordEdit? PoWordEdit{get;set;}
	public event EventHandler? DoneDelete;
	public event EventHandler? DoneSave;

	public ViewWordEditV2(){
		Ctx = App.DiOrMk<Ctx>();
		Render();
		HookSubPageEvents();
	}

	GridStack Root = new(IsRow: true);

	[Impl]
	public async Task<nil> ClickDelete(CT Ct){
		if(DeleteBtn is null){
			return NIL;
		}
		await DeleteBtn.ClickAndWaitDone(Ct);
		return NIL;
	}

	[Impl]
	public async Task<nil> ClickSave(CT Ct){
		if(SaveBtn is null){
			return NIL;
		}
		await SaveBtn.ClickAndWaitDone(Ct);
		return NIL;
	}

	void Render(){
		Content = Root.Grid;
		Root.SetRowDefs([
			new(9, GUT.Star),
			new(1, GUT.Auto),
			new(1, GUT.Auto),
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
		var poWordEdit = new ViewPoWordEdit{Ctx = Ctx?.PoWordEdit};
		PoWordEdit = poWordEdit;
		tab.Bind(tab.PropSelectedIndex, CBE.Mk<Ctx>(x=>x.TabIndex, Mode: BindingMode.TwoWay));
		tab.Items.A(new TabItem(), o=>{
			o.Header = I[K.Basic];
			o.Content = poWordEdit;
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
			Padding = new(10, 6),
			IsVisible = false,
		};
		b.Bind(IsVisibleProperty, CBE.Mk<Ctx>(x=>x.HasError, Mode: BindingMode.OneWay));
		var txt = new TextBlock{Foreground = Brushes.White};
		txt.Bind(TextBlock.TextProperty, CBE.Mk<Ctx>(x=>x.LastError, Mode: BindingMode.OneWay));
		b.Child = txt;
		return b;
	}

	Control MkBottomBar(){
		var g = new GridStack(IsRow: false);
		g.Grid.SetColDefs([
			new(1, GUT.Star),
			new(1, GUT.Star),
		]);
		g.Grid.Margin = new(10, 8, 10, 10);

		g.A(new OpBtn(), o=>{
			DeleteBtn = o;
			o._Button.Background = UiCfg.Inst.DelBtnBg;
			o._Button.StretchCenter();
			o.BtnContent = Icons.Delete().ToIcon().WithText(I[K.Delete]);
			o.SetExe(ct=>Ctx?.Delete(ct));
			o.HookDoneEvent(()=>DoneDelete?.Invoke(this, EventArgs.Empty));
		})
		.A(new OpBtn(), o=>{
			SaveBtn = o;
			o._Button.Background = UiCfg.Inst.MainColor;
			o._Button.StretchCenter();
			o.BtnContent = Icons.Save().ToIcon().WithText(I[K.Save]);
			o.SetExe(ct=>Ctx?.Save(ct));
			o.HookDoneEvent(()=>DoneSave?.Invoke(this, EventArgs.Empty));
		});
		return g.Grid;
	}
}
