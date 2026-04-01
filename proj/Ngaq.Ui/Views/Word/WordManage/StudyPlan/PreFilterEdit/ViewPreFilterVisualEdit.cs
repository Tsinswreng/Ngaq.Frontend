namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

using Ctx = VmPreFilterVisualEdit;

/// <summary>
/// PreFilter GUI 主頁。
/// 僅顯示 Po 主信息 + Text 預覽，並提供跳轉到子編輯頁和 JSON 編輯頁。
/// </summary>
public class ViewPreFilterVisualEdit: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewPreFilterVisualEdit(){
		Ctx = App.DiOrMk<Ctx>();
		Render();
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		Content = Root.Grid;
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
		root.Children.Add(MkTextSection());
		return sv;
	}

	Control MkErrorBar(){
		var b = new Border{
			Background = new SolidColorBrush(Color.FromArgb(80, 180, 30, 30)),
			Padding = new Thickness(10, 6),
			IsVisible = false,
		};
		b.CBind<Ctx>(IsVisibleProperty, x=>x.HasError, Mode: BindingMode.OneWay);
		var txt = new TextBlock{
			Foreground = Brushes.White,
		};
		txt.CBind<Ctx>(TextBlock.TextProperty, x=>x.LastError, Mode: BindingMode.OneWay);
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

		sp.Children.Add(new TextBlock{
			Text = "PoPreFilter",
			FontSize = UiCfg.Inst.BaseFontSize * 1.1,
			FontWeight = FontWeight.SemiBold,
		});
		sp.Children.Add(MkInputRow("Id", CBE.Mk<Ctx>(x=>x.PoIdText, Mode: BindingMode.OneWay), ReadOnly: true));
		sp.Children.Add(MkInputRow("Name", CBE.Mk<Ctx>(x=>x.PoUniqName, Mode: BindingMode.TwoWay)));
		sp.Children.Add(MkInputRow("Description", CBE.Mk<Ctx>(x=>x.PoDescr, Mode: BindingMode.TwoWay), AcceptsReturn: true));
		sp.Children.Add(MkComboRow("Type", Ctx?.PoTypeOptions ?? [], CBE.Mk<Ctx>(x=>x.PoTypeIndex, Mode: BindingMode.TwoWay)));
		return bdr;
	}

	Control MkTextSection(){
		var bdr = new Border{
			BorderBrush = Brushes.DimGray,
			BorderThickness = new Thickness(1),
			Padding = new Thickness(10),
		};
		var sp = new StackPanel{Spacing = 8};
		bdr.Child = sp;

		sp.Children.Add(new TextBlock{
			Text = "Text (Preview)",
			FontSize = UiCfg.Inst.BaseFontSize * 1.1,
			FontWeight = FontWeight.SemiBold,
		});
		sp.Children.Add(MkInputRow("Text", CBE.Mk<Ctx>(x=>x.PoTextPreview, Mode: BindingMode.OneWay), ReadOnly: true, AcceptsReturn: true));
		var btn = new Button{
			Content = "Edit PreFilter(Text) In GUI",
			HorizontalAlignment = HAlign.Left,
		};
		btn.Click += (s,e)=>Ctx?.OpenPreFilterDataEditor();
		sp.Children.Add(btn);
		return bdr;
	}

	Control MkBottomBar(){
		var bar = new AutoGrid(IsRow:false);
		bar.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		bar.A(_Button(), o=>{
			o.HorizontalContentAlignment = HAlign.Center;
			o.Content = "Open JSON";
			o.Click += (s,e)=>Ctx?.OpenJsonEditor();
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
		foreach(var item in Items){
			cb.Items.Add(item);
		}
		cb.Bind(ComboBox.SelectedIndexProperty, Binding);
		sp.Children.Add(cb);
		return sp;
	}
}
