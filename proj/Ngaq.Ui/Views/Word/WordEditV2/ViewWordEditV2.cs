namespace Ngaq.Ui.Views.Word.WordEditV2;

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Ui;
using Ngaq.Ui.Components.TempusBox;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsTempus;
using Ctx = VmWordEditV2;using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

public partial class ViewWordEditV2: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordEditV2(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
		HookCtx();
	}

	public partial class Cls{
		public static str MainBtn = nameof(MainBtn);
	}

	protected nil Style(){
		var S = Styles;
		new Style(x=>x.Is<Control>().Class(Cls.MainBtn))
			.Set(BackgroundProperty, UiCfg.Inst.MainColor)
			.AddTo(S)
		;
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);
	TreeDataGrid? PropGrid;
	FlatTreeDataGridSource<VmWordPropRow>? PropGridSource;
	TreeDataGrid? LearnGrid;
	FlatTreeDataGridSource<VmWordLearnRow>? LearnGridSource;

	IReadOnlyList<str> _KvTypeOptions => [
		I[K.KvTypeStr],
		I[K.KvTypeI64],
	];

	IReadOnlyList<str> _LearnResultOptions => [
		I[K.LearnAdd],
		I[K.LearnRmb],
		I[K.LearnFgt],
	];

	static readonly IValueConverter _IsoTempusConverter = new IsoToTempusConverter();
	static readonly IValueConverter _DelAtUnixMsConverter = new DelAtUnixMsToTempusConverter();

	protected nil Render(){
		Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(8, GUT.Star),
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
		]);

		Root
		.A(MkTabs())
		.A(MkErrBar())
		.A(MkBottomBar())
		;

		return NIL;
	}

	void HookCtx(){
		if(Ctx is null){
			return;
		}
		Ctx.PropertyChanged += (s, e)=>{
			if(e.PropertyName == nameof(Ctx.PropRows)){
				RebuildPropGridSource();
			}
			if(e.PropertyName == nameof(Ctx.LearnRows)){
				RebuildLearnGridSource();
			}
		};
		RebuildPropGridSource();
		RebuildLearnGridSource();
	}

	Control MkTabs(){
		var tab = new TabControl();
		tab.Bind(tab.PropSelectedIndex, CBE.Mk<Ctx>(x=>x.TabIndex, Mode: BindingMode.TwoWay));

		tab.Items.A(new TabItem(), o=>{
			o.Header = I[K.Basic];
			o.Content = MkBasicTab();
		})
		.A(new TabItem(), o=>{
			o.Header = I[K.Props];
			o.Content = MkPropsTab();
		})
		.A(new TabItem(), o=>{
			o.Header = I[K.Learns];
			o.Content = MkLearnsTab();
		});
		return tab;
	}

	Control MkBasicTab(){
		var sv = new ScrollViewer();
		var sp = new StackPanel{
			Spacing = 8,
			Margin = new Thickness(10),
		};
		sv.Content = sp;

		sp.A(MkIdSelectableRow(I[K.WordId], CBE.Mk<Ctx>(x=>x.WordIdText, Mode: BindingMode.OneWay)));
		sp.A(MkInputRow(I[K.Head], CBE.Mk<Ctx>(x=>x.Head, Mode: BindingMode.TwoWay)));
		sp.A(MkInputRow(I[K.Lang], CBE.Mk<Ctx>(x=>x.Lang, Mode: BindingMode.TwoWay)));
		sp.A(MkTempusRow(I[K.StoredAt], CBE.Mk<Ctx>(x=>x.StoredAtIso, Mode: BindingMode.TwoWay, Converter: _IsoTempusConverter)));
		sp.A(MkTempusRow(I[K.BizCreatedAt], CBE.Mk<Ctx>(x=>x.BizCreatedAtIso, Mode: BindingMode.TwoWay, Converter: _IsoTempusConverter)));
		sp.A(MkTempusRow(I[K.BizUpdatedAt], CBE.Mk<Ctx>(x=>x.BizUpdatedAtIso, Mode: BindingMode.TwoWay, Converter: _IsoTempusConverter)));
		// DelAt 允許留空表示「未刪除」；不能用 TempusBox，否則空值會被轉成當前時間。
		sp.A(MkInputRow(
			I[K.DelAtUnixMs],
			CBE.Mk<Ctx>(x=>x.DelAtUnixMs, Mode: BindingMode.TwoWay)
		));
		return sv;
	}

	Control MkPropsTab(){
		return MkRowsTab(
			I[K.AddProp],
			()=>Ctx?.AddPropRow(),
			MkPropGridHost
		);
	}

	Control MkLearnsTab(){
		return MkRowsTab(
			I[K.AddLearn],
			()=>Ctx?.AddLearnRow(),
			MkLearnGridHost
		);
	}

	Control MkRowsTab(str AddBtnLabel, Action? OnAdd, Func<Control> MkGridHost){
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(9, GUT.Star),
		]);

		root.A(MkBtnAddItem(AddBtnLabel), o=>{
			o.Click += (s, e)=>OnAdd?.Invoke();
		});
		root.A(MkGridHost());
		return root.Grid;
	}

	Control MkPropGridHost(){
		PropGrid = MkGridCore();
		PropGrid.AddHandler(InputElement.TappedEvent, OnPropGridTapped, RoutingStrategies.Bubble, true);
		RebuildPropGridSource();
		return PropGrid;
	}

	Control MkLearnGridHost(){
		LearnGrid = MkGridCore();
		LearnGrid.AddHandler(InputElement.TappedEvent, OnLearnGridTapped, RoutingStrategies.Bubble, true);
		RebuildLearnGridSource();
		return LearnGrid;
	}

	TreeDataGrid MkGridCore(){
		var grid = new TreeDataGrid{
			Margin = new Thickness(10, 4, 10, 10),
			MinHeight = 260,
		};
		grid.Styles.Add(
			new Style(x=>x.OfType<TreeDataGridRow>().Class(":pointerover"))
			.Set(TemplatedControl.BackgroundProperty, new SolidColorBrush(Color.FromRgb(46, 46, 46)))
		);
		grid.Styles.Add(
			new Style(x=>x.OfType<TreeDataGridRow>().Class(":pressed"))
			.Set(TemplatedControl.BackgroundProperty, new SolidColorBrush(Color.FromRgb(70, 70, 70)))
		);
		return grid;
	}

	void RebuildPropGridSource(){
		if(Ctx is null || PropGrid is null){
			return;
		}
		PropGridSource = new FlatTreeDataGridSource<VmWordPropRow>(Ctx.PropRows){
			Columns = {
				new TextColumn<VmWordPropRow, str>("", x=>GetPropRowIdxText(x)),
				new TextColumn<VmWordPropRow, str>(I[K.Key], x=>x.KeyText),
				new TextColumn<VmWordPropRow, str>(I[K.KType], x=>x.KTypeText),
				new TextColumn<VmWordPropRow, str>(I[K.VType], x=>x.VTypeText),
			},
		};
		PropGrid.Source = PropGridSource;
	}

	void RebuildLearnGridSource(){
		if(Ctx is null || LearnGrid is null){
			return;
		}
		LearnGridSource = new FlatTreeDataGridSource<VmWordLearnRow>(Ctx.LearnRows){
			Columns = {
				new TextColumn<VmWordLearnRow, str>(I[K.NumberSign], x=>GetLearnRowIdxText(x)),
				new TextColumn<VmWordLearnRow, str>(I[K.LearnResult], x=>x.LearnResultText),
				new TextColumn<VmWordLearnRow, str>(I[K.BizCreatedAt], x=>x.BizCreatedAtDisplay),
			},
		};
		LearnGrid.Source = LearnGridSource;
	}

	str GetPropRowIdxText(VmWordPropRow Row){
		if(Ctx is null){
			return "";
		}
		var idx = Ctx.PropRows.IndexOf(Row);
		return idx < 0 ? "" : (idx+1)+"";
	}

	str GetLearnRowIdxText(VmWordLearnRow Row){
		if(Ctx is null){
			return "";
		}
		var idx = Ctx.LearnRows.IndexOf(Row);
		return idx < 0 ? "" : (idx+1)+"";
	}

	void OnPropGridTapped(object? Sender, TappedEventArgs E){
		if(Ctx is null || PropGrid is null){
			return;
		}
		HandleGridTapped<VmWordPropRow>(E, OpenPropDetail);
	}

	void OnLearnGridTapped(object? Sender, TappedEventArgs E){
		if(Ctx is null || LearnGrid is null){
			return;
		}
		HandleGridTapped<VmWordLearnRow>(E, OpenLearnDetail);
	}

	void HandleGridTapped<TRow>(TappedEventArgs E, Action<TRow> OpenDetail) where TRow : class{
		if(E.Source is not StyledElement Src){
			return;
		}
		for(StyledElement? cur = Src; cur is not null; cur = cur.Parent){
			if(cur is ToggleButton){
				return;
			}
			if(cur is TreeDataGridRow row){
				if(row.DataContext is TRow vmRow){
					OpenDetail(vmRow);
					E.Handled = true;
				}
				return;
			}
		}
	}

	void OpenPropDetail(VmWordPropRow Row){
		var view = new UserControl();
		view.SetContent(MkPropDetailContent(Row));
		ViewNavi?.GoTo(ToolView.WithTitle(I[K.EditProp], view));
	}

	void OpenLearnDetail(VmWordLearnRow Row){
		var view = new UserControl();
		view.SetContent(MkLearnDetailContent(Row));
		ViewNavi?.GoTo(ToolView.WithTitle(I[K.EditLearn], view));
	}

	Control MkPropDetailContent(VmWordPropRow Row){
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(9, GUT.Star),
			RowDef(1, GUT.Auto),
		]);

		root.A(new ScrollViewer(), sv=>{
			sv.SetContent(new StackPanel(), sp=>{
				sp.Margin = new Thickness(10);
				sp.Spacing = 8;
				sp.A(MkComboRow(I[K.KType], _KvTypeOptions, CBE.Mk<VmWordPropRow>(x=>x.KTypeIndex, Mode: BindingMode.TwoWay)));
				sp.A(MkEditableComboRow(I[K.KeyStr], GetPropKeyOptions(), CBE.Mk<VmWordPropRow>(x=>x.KStrText, Mode: BindingMode.TwoWay)));
				sp.A(MkInputRow(I[K.KeyI64], CBE.Mk<VmWordPropRow>(x=>x.KI64Text, Mode: BindingMode.TwoWay)));
				sp.A(MkComboRow(I[K.VType], _KvTypeOptions, CBE.Mk<VmWordPropRow>(x=>x.VTypeIndex, Mode: BindingMode.TwoWay)));
				sp.A(MkInputRow(I[K.VStr], CBE.Mk<VmWordPropRow>(x=>x.VStrText, Mode: BindingMode.TwoWay)));
				sp.A(MkInputRow(I[K.VI64], CBE.Mk<VmWordPropRow>(x=>x.VI64Text, Mode: BindingMode.TwoWay)));
				sp.DataContext = Row;
			});
		});

		root.A(new Button(), o=>{
			o.Margin = new Thickness(10, 6, 10, 10);
			o.Background = Brushes.Red;
			o.Content = Svgs.DeleteForeverSharp().ToIcon().WithText(I[K.Remove]);
			o.Click += (s, e)=>{
				Ctx?.RemovePropRow(Row);
				ViewNavi?.Back();
			};
		});
		return root.Grid;
	}

	Control MkLearnDetailContent(VmWordLearnRow Row){
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(9, GUT.Star),
			RowDef(1, GUT.Auto),
		]);

		root.A(new ScrollViewer(), sv=>{
			sv.SetContent(new StackPanel(), sp=>{
				sp.Margin = new Thickness(10);
				sp.Spacing = 8;
				sp.A(MkComboRow(I[K.LearnResult], _LearnResultOptions, CBE.Mk<VmWordLearnRow>(x=>x.LearnResultIndex, Mode: BindingMode.TwoWay)));
				sp.A(MkTempusRow(I[K.BizCreatedAt], CBE.Mk<VmWordLearnRow>(x=>x.BizCreatedAtIso, Mode: BindingMode.TwoWay, Converter: _IsoTempusConverter)));
				sp.DataContext = Row;
			});
		});

		root.A(new Button(), o=>{
			o.Margin = new Thickness(10, 6, 10, 10);
			o.Background = Brushes.Red;
			o.Content = Svgs.DeleteForeverSharp().ToIcon().WithText(I[K.Remove]);
			o.Click += (s, e)=>{
				Ctx?.RemoveLearnRow(Row);
				ViewNavi?.Back();
			};
		});
		return root.Grid;
	}

	IReadOnlyList<str> GetPropKeyOptions(){
		var k = KeysProp.Inst;
		return [
			Todo.I18n(k.summary),
			Todo.I18n(k.description),
			Todo.I18n(k.note),
			Todo.I18n(k.tag),
			Todo.I18n(k.source),
			Todo.I18n(k.alias),
			Todo.I18n(k.pronunciation),
			Todo.I18n(k.weight),
			Todo.I18n(k.learn),
			Todo.I18n(k.usage),
			Todo.I18n(k.example),
			Todo.I18n(k.relation),
			Todo.I18n(k.Ref),
		];
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
			o._Button.Background = new SolidColorBrush(Color.FromRgb(210, 56, 56));
			o._Button.HorizontalContentAlignment = HAlign.Center;
			o.BtnContent = Svgs.DeleteForeverSharp().ToIcon().WithText(I[K.Delete]);
			o.SetExe(ct=>Ctx?.Delete(ct));
		})
		.A(new OpBtn(), o=>{
			o._Button.Background = UiCfg.Inst.MainColor;
			o.BtnContent = Svgs.FloppyDiskBackFill().ToIcon().WithText(I[K.Save]);
			o._Button.StretchCenter();
			o.CBind<Ctx>(IsEnabledProperty, x=>x.IsDirty, Mode: BindingMode.OneWay);
			o.SetExe(ct=>Ctx?.Save(ct));
		});
		return g.Grid;
	}

	Button MkBtnAddItem(str Label){
		var o = new Button();
		o.Margin = new Thickness(10, 10, 10, 4);
		o.Content = Svgs.Add().ToIcon().WithText(" "+Label);
		return o;
	}

	Control MkTempusRow(str Label, IBinding Binding){
		var tb = new TempusBox();
		tb.Bind(TempusBox.TempusProperty, Binding);
		InitTempusBox(tb);
		return MkFieldRow(Label, tb);
	}

	void InitTempusBox(TempusBox Box){
		Box.FormatItems.Add(TempusFormatItem.yy_MM_DD);
		Box.FormatItems.Add(TempusFormatItem.yy_MM_DD__HH_mm);
		Box.SelectedFormat = TempusFormatItem.yy_MM_DD__HH_mm;
	}

	Control MkInputRow(str Label, IBinding Binding){
		var tb = new TextBox();
		tb.Bind(TextBox.TextProperty, Binding);
		return MkFieldRow(Label, tb);
	}

	Control MkIdSelectableRow(str Label, IBinding Binding){
		var tb = new SelectableTextBlock{
			FontSize = UiCfg.Inst.BaseFontSize*0.8,
			TextWrapping = TextWrapping.Wrap,
		};
		tb.Bind(TextBlock.TextProperty, Binding);
		return MkFieldRow(Label, tb);
	}

	Control MkComboRow(str Label, IEnumerable<str> Items, IBinding Binding){
		var cb = new ComboBox{
			ItemsSource = Items,
		};
		cb.Bind(ComboBox.SelectedIndexProperty, Binding);
		return MkFieldRow(Label, cb);
	}

	Control MkEditableComboRow(str Label, IEnumerable<str> Items, IBinding Binding){
		var cb = new ComboBox{
			IsEditable = true,
			HorizontalAlignment = HAlign.Stretch,
			ItemsSource = Items,
		};
		cb.Bind(ComboBox.TextProperty, Binding);
		return MkFieldRow(Label, cb);
	}

	Control MkFieldRow(str Label, Control Input){
		var sp = new StackPanel{
			Orientation = Orientation.Vertical,
			Spacing = 3,
		};
		sp.Children.Add(new TextBlock{
			Text = Label,
		});
		sp.Children.Add(Input);
		return sp;
	}

	sealed class IsoToTempusConverter: IValueConverter{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture){
			if(value is str s && !str.IsNullOrWhiteSpace(s)){
				try{
					return UnixMs.FromIso(s.Trim());
				}catch{}
			}
			return UnixMs.Now();
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture){
			if(value is UnixMs t){
				return t.ToIso();
			}
			return "";
		}
	}
	sealed class DelAtUnixMsToTempusConverter: IValueConverter{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture){
			if(value is str s && i64.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var unixMs)){
				return UnixMs.FromUnixMs(unixMs);
			}
			return UnixMs.Now();
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture){
			if(value is UnixMs t){
				return t.Value.ToString(CultureInfo.InvariantCulture);
			}
			return "";
		}
	}
}








