namespace Ngaq.Ui.Views.Word.WordEditV2;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

using Ctx = VmWordEditV2;

public partial class ViewWordEditV2: AppViewBase {
	public Ctx? Ctx {
		get { return DataContext as Ctx; }
		set { DataContext = value; }
	}

	public ViewWordEditV2() {
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}

	public partial class Cls{
		public static str MainBtn = nameof(MainBtn);
	}
	protected nil Style() {
		var S = Styles;
		new Style(
			x=>x.Is<Control>()
			.Class(Cls.MainBtn)
		).Set(
			BackgroundProperty
			,UiCfg.Inst.MainColor
		).AddTo(S)
		;
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);

	protected nil Render() {
		Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			//RowDef(1, GUT.Auto),
			RowDef(8, GUT.Star),
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
		]);

		Root
		//.AddInit(MkHeader(), o => { })
		.A(MkTabs(), o => { })
		.A(MkErrBar(), o => { })
		.A(MkBottomBar(), o => { });

		return NIL;
	}

	Button MkRmBtn(){
		var o = new Button();
		o.Content = Svgs.DeleteForeverSharp().ToIcon().WithText("Remove");
		o.HorizontalAlignment = HorizontalAlignment.Right;
		o.Background = Brushes.Red;
		return o;
	}
	Button MkBtnAddItem(){
		var o = new Button();
		o.Margin = new Thickness(10, 10, 10, 4);
		o.Content = Svgs.Add.ToIcon().WithText(" Add Item");
		return o;
	}
	Control MkHeader() {
		var bdr = new Border {
			Padding = new Thickness(12, 10),
			BorderBrush = Brushes.Gray,
			BorderThickness = new Thickness(0, 0, 0, 1),
		};

		bdr.InitChild(new StackPanel(), o=>{
			o.Orientation = Orientation.Vertical;
			o.Spacing = 4;
			var box = o;
			box.A(new TextBlock(), o=>{
				o.FontSize = UiCfg.Inst.BaseFontSize * 1.15;
				o.FontWeight = FontWeight.SemiBold;
				o.CBind<Ctx>(TextBlock.TextProperty,x => x.Head, Mode: BindingMode.OneWay);
			})
			.A(new TextBlock(), o=>{
				o.FontSize = UiCfg.Inst.BaseFontSize * 0.9;
				o.Foreground = Brushes.LightGray;
				o.CBind<Ctx>(TextBlock.TextProperty,x => x.Lang, Mode: BindingMode.OneWay);
			})
			;
		});
		return bdr;
	}

	Control MkTabs() {
		var tab = new TabControl();
		tab.Bind(
			tab.PropSelectedIndex
			,CBE.Mk<Ctx>(x => x.TabIndex, Mode: BindingMode.TwoWay)
		);

		tab.Items.A(new TabItem(), o => {
			o.Header = "Basic";
			o.Content = MkBasicTab();
		}).A(new TabItem(), o => {
			o.Header = "Props";
			o.Content = MkPropsTab();
		}).A(new TabItem(), o => {
			o.Header = "Learns";
			o.Content = MkLearnsTab();
		}).A(new TabItem(), o => {
			o.Header = "JSON";
			o.Content = MkJsonTab();
		});

		return tab;
	}

	Control MkBasicTab() {
		var sv = new ScrollViewer();
		var sp = new StackPanel {
			Spacing = 8,
			Margin = new Thickness(10)
		};
		sv.Content = sp;

		sp.A(MkReadOnlyRow("WordId", CBE.Mk<Ctx>(x => x.WordIdText, Mode: BindingMode.OneWay)));
		sp.A(MkReadOnlyRow("Owner", CBE.Mk<Ctx>(x => x.OwnerText, Mode: BindingMode.OneWay)));
		sp.A(MkInputRow("Head", CBE.Mk<Ctx>(x => x.Head, Mode: BindingMode.TwoWay)));
		sp.A(MkInputRow("Lang", CBE.Mk<Ctx>(x => x.Lang, Mode: BindingMode.TwoWay)));
		sp.A(MkInputRow("StoredAt(ISO)", CBE.Mk<Ctx>(x => x.StoredAtIso, Mode: BindingMode.TwoWay)));
		sp.A(MkInputRow("DelAt(unix ms)", CBE.Mk<Ctx>(x => x.DelAtUnixMs, Mode: BindingMode.TwoWay)));

		return sv;
	}

	Control MkPropsTab() {
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(9, GUT.Star),
		]);

		root.A(MkBtnAddItem(), o => {
			o.Click += (s,e) => {
				Ctx?.AddPropRow();
			};
		}).A(new ScrollViewer(), sv => {
			sv.Margin = new Thickness(10, 4, 10, 10);
			var list = new ItemsControl();
			list.Bind(ItemsControl.ItemsSourceProperty, CBE.Mk<Ctx>(x => x.PropRows, Mode: BindingMode.OneWay));
			list.SetItemsPanel(() => new StackPanel { Spacing = 8 });
			list.SetItemTemplate<VmWordPropRow>((row, ns) => {
				var bdr = new Border {
					Padding = new Thickness(8),
					BorderBrush = Brushes.DimGray,
					BorderThickness = new Thickness(1),
				};
				var sp = new StackPanel { Spacing = 6 };
				bdr.Child = sp;

				sp.A(MkBoundInput("KType", CBE.Mk<VmWordPropRow>(x => x.KTypeText, Mode: BindingMode.TwoWay)));
				sp.A(MkBoundInput("Key", CBE.Mk<VmWordPropRow>(x => x.KeyText, Mode: BindingMode.TwoWay)));
				sp.A(MkBoundInput("VType", CBE.Mk<VmWordPropRow>(x => x.VTypeText, Mode: BindingMode.TwoWay)));
				sp.A(MkBoundInput("Value", CBE.Mk<VmWordPropRow>(x => x.ValueText, Mode: BindingMode.TwoWay)));
				sp.A(MkRmBtn(), o=>{
					o.Click += (s, e) => Ctx?.RemovePropRow(row);
				});



				return bdr;
			});
			sv.Content = list;
		});

		return root.Grid;
	}

	Control MkLearnsTab() {
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(9, GUT.Star),
		]);

		root.A(MkBtnAddItem(), o => {
			o.Click += (s,e)=> {
				Ctx?.AddLearnRow();
			};
		});

		root.A(new ScrollViewer(), sv => {
			sv.Margin = new Thickness(10, 4, 10, 10);
			var list = new ItemsControl();
			list.Bind(
				ItemsControl.ItemsSourceProperty
				,CBE.Mk<Ctx>(x => x.LearnRows, Mode: BindingMode.OneWay)
			);
			list.SetItemsPanel(() => new StackPanel { Spacing = 8 });
			list.SetItemTemplate<VmWordLearnRow>((row, ns) => {
				var bdr = new Border {
					Padding = new Thickness(8),
					BorderBrush = Brushes.DimGray,
					BorderThickness = new Thickness(1),
				};
				var sp = new StackPanel { Spacing = 6 };
				bdr.Child = sp;

				sp.A(MkBoundInput("LearnResult(Add/Rmb/Fgt)", CBE.Mk<VmWordLearnRow>(x => x.LearnResultText, Mode: BindingMode.TwoWay)));
				sp.A(MkBoundInput("BizCreatedAt(ISO)", CBE.Mk<VmWordLearnRow>(x => x.BizCreatedAtIso, Mode: BindingMode.TwoWay)));
				sp.A(MkRmBtn(), o=>{
					o.Click += (s, e) => Ctx?.RemoveLearnRow(row);
				});
				return bdr;
			});
			sv.Content = list;
		});

		return root.Grid;
	}

	Control MkJsonTab() {
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(8, GUT.Star),
		]);

		root.A(MkJsonOps(), o => { });
		root.A(new TextBox(), tb => {
			tb.Margin = new Thickness(10, 4, 10, 10);
			tb.AcceptsReturn = true;
			tb.TextWrapping = TextWrapping.Wrap;
			tb.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
			tb.Bind(TextBox.TextProperty, CBE.Mk<Ctx>(x => x.JsonText, Mode: BindingMode.TwoWay));
		});
		return root.Grid;
	}

	Control MkJsonOps() {
		var g = new AutoGrid(IsRow: false);
		g.Grid.Margin = new Thickness(10, 10, 10, 2);
		g.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);

		g.A(new Button(), o => {
			o.Content = "Sync Form -> Json";
			o.Click += (s, e) => Ctx?.SyncJsonFromDraft();
		});
		g.A(new Button(), o => {
			o.Content = "Apply Json -> Form";
			o.Click += (s, e) => Ctx?.ApplyJsonToForm();
		});
		return g.Grid;
	}

	Control MkErrBar() {
		var b = new Border {
			Background = new SolidColorBrush(Color.FromArgb(70, 180, 20, 20)),
			Padding = new Thickness(10, 6),
			IsVisible = false
		};
		b.Bind(IsVisibleProperty, CBE.Mk<Ctx>(x => x.HasError, Mode: BindingMode.OneWay));
		var txt = new TextBlock {
			Foreground = Brushes.White
		};
		txt.Bind(TextBlock.TextProperty, CBE.Mk<Ctx>(x => x.LastError, Mode: BindingMode.OneWay));
		b.Child = txt;
		return b;
	}

	Control MkBottomBar() {
		var g = new AutoGrid(IsRow: false);
		g.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		g.Grid.Margin = new Thickness(10, 8, 10, 10);

		g.A(new Button(), o => {
			o.Content = "Reset";
			o.Click += (s, e) => Ctx?.ResetFromSource();
		}).A(new OpBtn(), o => {
			//o.Classes.Add(Cls.MainBtn);
			o.Background = UiCfg.Inst.MainColor;
			o.BtnContent = Svgs.FloppyDiskBackFill.ToIcon().WithText(" Save");
			o.CBind<Ctx>(IsEnabledProperty,x => x.IsDirty, Mode: BindingMode.OneWay);
			o.SetExe(ct => Ctx?.Save(ct));
		});
		return g.Grid;
	}

	Control MkInputRow(str Label, IBinding Binding) {
		var sp = new StackPanel {
			Orientation = Orientation.Vertical,
			Spacing = 3
		};
		sp.Children.Add(new TextBlock { Text = Label });
		var tb = new TextBox();
		tb.Bind(TextBox.TextProperty, Binding);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkReadOnlyRow(str Label, IBinding Binding) {
		var sp = new StackPanel {
			Orientation = Orientation.Vertical,
			Spacing = 3
		};
		sp.Children.Add(new TextBlock { Text = Label });
		var tb = new TextBox { IsReadOnly = true };
		tb.Bind(TextBox.TextProperty, Binding);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkBoundInput(str Label, IBinding Binding) {
		var sp = new StackPanel {
			Orientation = Orientation.Vertical,
			Spacing = 3
		};
		sp.Children.Add(new TextBlock { Text = Label, FontSize = UiCfg.Inst.BaseFontSize * 0.9 });
		var tb = new TextBox();
		tb.Bind(TextBox.TextProperty, Binding);
		sp.Children.Add(tb);
		return sp;
	}
}
