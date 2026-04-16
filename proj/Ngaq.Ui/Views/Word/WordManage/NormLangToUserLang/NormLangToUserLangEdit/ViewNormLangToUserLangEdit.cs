namespace Ngaq.Ui.Views.Word.WordManage.NormLangToUserLang.NormLangToUserLangEdit;

using System;
using System.Collections.Generic;
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
using Ngaq.Ui.Views.Word.WordManage.NormLang.NormLangPage;
using Ngaq.Ui.Views.Word.WordManage.UserLang.UserLangPage;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ctx = VmNormLangToUserLangEdit;`r`nusing K = Ngaq.Ui.Infra.I18n.KeysUiI18n.NormLangToUserLangEdit;

/// 標準語言到用戶語言映射詳情編輯頁。
public partial class ViewNormLangToUserLangEdit
	:AppViewBase
{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewNormLangToUserLangEdit(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}

	public partial class Cls{}

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
		Root.A(MkBody());
		Root.A(MkBottomBar());
		return NIL;
	}

	Control MkBody(){
		var sv = new ScrollViewer();
		var root = new StackPanel{
			Spacing = 10,
			Margin = new Thickness(10),
		};
		sv.Content = root;
		root.Children.Add(MkErrorBar());
		root.Children.Add(MkPoSection());
		return sv;
	}

	Control MkErrorBar(){
		var b = new Border{
			Background = new SolidColorBrush(Color.FromArgb(80, 180, 30, 30)),
			Padding = new Thickness(10, 6),
			IsVisible = false,
		};
		//b.CBind<Ctx>(IsVisibleProperty, x=>x.HasError, Mode: BindingMode.OneWay);
		var txt = new TextBlock{
			Foreground = Brushes.White,
		};
		//txt.CBind<Ctx>(TextBlock.TextProperty, x=>x.LastError, Mode: BindingMode.OneWay);
		b.Child = txt;
		return b;
	}

	Control MkPoSection(){
		var bdr = new Border{
			BorderBrush = Brushes.DimGray,
			BorderThickness = new Thickness(1),
			Padding = new Thickness(10),
		};
		var sp = new StackPanel{Spacing = 8};
		bdr.Child = sp;

		sp.A(new TextBlock{
			Text = I[K.PoNormLangToUserLang],
			FontSize = UiCfg.Inst.BaseFontSize * 1.1,
			FontWeight = FontWeight.SemiBold,
		})
		.A(MkIdRow(I[K.Id], CBE.Mk<Ctx>(x=>x.PoIdText, Mode: BindingMode.OneWay)));
		var typeRow = MkComboRow(I[K.NormLangType], Ctx?.NormLangTypeOptions ?? [], CBE.Mk<Ctx>(x=>x.PoNormLangTypeIndex, Mode: BindingMode.TwoWay));
		typeRow.CBind<Ctx>(IsVisibleProperty, x=>x.ShowNormLangTypeField, Mode: BindingMode.OneWay);
		sp.A(typeRow)
			.A(MkPickerRow(I[K.NormLang], CBE.Mk<Ctx>(x=>x.PoNormLang, Mode: BindingMode.TwoWay), ()=>OpenNormLangSelector()))
			.A(MkPickerRow(I[K.UserLang], CBE.Mk<Ctx>(x=>x.PoUserLang, Mode: BindingMode.TwoWay), ()=>OpenUserLangSelector()))
			.A(MkInputRow(I[K.Descr], CBE.Mk<Ctx>(x=>x.PoDescr, Mode: BindingMode.TwoWay), AcceptsReturn: true));
		return bdr;
	}

	Control MkBottomBar(){
		var bar = new AutoGrid(IsRow:false);
		bar.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		bar.A(new OpBtn(), o=>{
			o.Background = new SolidColorBrush(Color.FromRgb(210, 56, 56));
			o._Button.HorizontalContentAlignment = HAlign.Center;
			o.BtnContent = Svgs.DeleteForeverSharp().ToIcon().WithText(I[K.Delete]);
			o.SetExe((Ct)=>Ctx?.Delete(Ct));
		})
		.A(new OpBtn(), o=>{
			o.Background = UiCfg.Inst.MainColor;
			o._Button.HorizontalContentAlignment = HAlign.Center;
			o.BtnContent = Svgs.FloppyDiskBackFill().ToIcon().WithText(I[K.Save]);
			o.SetExe((Ct)=>Ctx?.Save(Ct));
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
			MaxHeight = AcceptsReturn ? 180 : double.PositiveInfinity,
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

	Control MkPickerRow(str Label, IBinding Binding, Action OnPick){
		var root = new StackPanel{Spacing = 3};
		root.Children.Add(new TextBlock{Text = Label});
		var row = new AutoGrid(IsRow:false);
		row.Grid.ColumnDefinitions.AddRange([
			ColDef(8, GUT.Star),
			ColDef(2, GUT.Star),
		]);
		row.A(new TextBox(), tb=>{
			tb.Bind(TextBox.TextProperty, Binding);
		})
		.A(new Button(), o=>{
			o.Content = Svgs.Search().ToIcon().WithText(I[K.Pick]);
			o.Click += (s,e)=>OnPick();
		});
		root.Children.Add(row.Grid);
		return root;
	}

	Control MkIdRow(str Label, IBinding Binding){
		var row = new StackPanel{
			Spacing = 6,
			Orientation = Orientation.Horizontal,
		};
		row.Children.Add(new TextBlock{
			Text = Label + ":",
			FontSize = UiCfg.Inst.BaseFontSize * 0.8,
		});
		var value = new TextBlock{
			FontSize = UiCfg.Inst.BaseFontSize * 0.8,
		};
		value.Bind(TextBlock.TextProperty, Binding);
		row.Children.Add(value);
		return row;
	}

	void OpenNormLangSelector(){
		var view = new ViewNormLangPage();
		if(view.Ctx is not null){
			view.Ctx.Input = Ctx?.PoNormLang ?? "";
			view.Ctx.SetSelectMode(po=>{
				Ctx?.ApplyNormLangSelection(po);
				view.ViewNavi?.Back();
			});
		}
		ViewNavi?.GoTo(ToolView.WithTitle(I[K.SelectNormLang], view));
	}

	void OpenUserLangSelector(){
		var view = new ViewUserLangPage();
		if(view.Ctx is not null){
			view.Ctx.Input = Ctx?.PoUserLang ?? "";
			view.Ctx.SetSelectMode(po=>{
				Ctx?.ApplyUserLangSelection(po);
				view.ViewNavi?.Back();
			});
		}
		ViewNavi?.GoTo(ToolView.WithTitle(I[K.SelectUserLang], view));
	}
}


