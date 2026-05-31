namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.FilterCardEditV2;

using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.PreFilterVisualEditV2;
using Tsinswreng.Avln.Grid;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

using Ctx = VmFilterCardEditV2;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// <summary>
/// 單個篩選卡片 V2 編輯頁。
/// 將字段、運算、值類型和值內容收斂到一頁內編輯。
/// </summary>
public class ViewFilterCardEditV2: AppViewBase<Ctx>{

	public ViewFilterCardEditV2(){
		Ctx = App.DiOrMk<Ctx>();
		Render();
	}

	GridStack Root = new(IsRow: true);

	protected nil Render(){
		Content = Root.Grid;
		Root.SetRowDefs([
			new(1, GUT.Star),
			new(1, GUT.Auto),
		]);
		Root.A(MkBody());
		Root.A(MkBottomBar());
		return NIL;
	}

	Control MkBody(){
		var sv = new ScrollViewer();
		var root = new StackPanel{Spacing = 10, Margin = new(10)};
		sv.Content = root;
		root
		.A(MkFieldRow())
		.A(MkOperationRow())
		.A(MkValueTypeRow())
		.A(MkValuesRow());
		return sv;
	}

	Control MkFieldRow(){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = I[K.Fields]});
		var cb = new ComboBox{
			IsEditable = true,
			HorizontalAlignment = HAlign.Stretch,
		};
		cb.Bind(ComboBox.ItemsSourceProperty, CBE.Mk<Ctx>(x=>x.FieldOptions, Mode: BindingMode.OneWay));
		cb.Bind(ComboBox.TextProperty, CBE.Mk<Ctx>(x=>x.Field, Mode: BindingMode.TwoWay));
		sp.Children.Add(cb);
		return sp;
	}

	Control MkOperationRow(){
		return MkComboRow(I[K.Operation], Ctx?.OperationOptions ?? [], CBE.Mk<Ctx>(x=>x.OperationIndex, Mode: BindingMode.TwoWay));
	}

	Control MkValueTypeRow(){
		return MkComboRow(I[K.ValueType], Ctx?.ValueTypeOptions ?? [], CBE.Mk<Ctx>(x=>x.ValueTypeIndex, Mode: BindingMode.TwoWay));
	}

	Control MkValuesRow(){
		return MkInputRow(I[K.ValuesNewlineSeparated], CBE.Mk<Ctx>(x=>x.ValuesText, Mode: BindingMode.TwoWay), AcceptsReturn: true);
	}

	Control MkBottomBar(){
		var g = new GridStack(IsRow:false);
		g.Grid.SetColDefs([
			new(1, GUT.Star),
			new(1, GUT.Star),
		]);
		g.A(new Button(), o=>{
			o.HorizontalContentAlignment = HAlign.Center;
			o.Background = UiCfg.Inst.DelBtnBg;
			o.Content = Icons.Delete().ToIcon().WithText(I[K.Delete]);
			o.Click += (s,e)=>Ctx?.Delete();
		});
		g.A(new Button(), o=>{
			o.HorizontalContentAlignment = HAlign.Center;
			o.Background = UiCfg.Inst.MainColor;
			o.Content = Icons.Save().ToIcon().WithText(I[K.Save]);
			o.Click += (s,e)=>Ctx?.Save();
		});
		return g.Grid;
	}

	Control MkInputRow(str Label, IBinding Binding, bool AcceptsReturn = false){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		var tb = new TextBox{
			AcceptsReturn = AcceptsReturn,
			TextWrapping = AcceptsReturn ? TextWrapping.Wrap : TextWrapping.NoWrap,
			MaxHeight = AcceptsReturn ? 140 : double.PositiveInfinity,
		};
		tb.Bind(TextBox.TextProperty, Binding);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkComboRow(str Label, IEnumerable<str> Items, IBinding Binding){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		var cb = new ComboBox();
		foreach(var item in Items){ cb.Items.Add(item); }
		cb.Bind(ComboBox.SelectedIndexProperty, Binding);
		sp.Children.Add(cb);
		return sp;
	}
}

