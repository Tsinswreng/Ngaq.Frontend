namespace Ngaq.Ui.Views.Word.WordInfo;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Ui.Infra;
using Ngaq.Ui.StrokeText;
using Tsinswreng.Avln.StrokeText;
using Tsinswreng.AvlnTools;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordInfo;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

public partial class ViewWordInfo
	:AppViewBase<Ctx>
{
	public ViewWordInfo(){
		Ctx = new Ctx();
		Style();
		Render();
		DataContextChanged += (s,e)=>{
			OnCtxChanged();
		};
		OnCtxChanged();
	}

	public partial class Cls_{
		public str LightGray = nameof(LightGray);
	}
	public Cls_ Cls{get;set;} = new();

	public Color Gray = Colors.LightGray;

	public AutoGrid Root{get;set;} = new(IsRow: true);
	Grid? MainGrid;
	Control? Splitter;
	Control? SidePropsPane;
	Ctx? SubscribedCtx;

	protected nil Style(){
		new Style(
			x=>x.Is<Control>()
			.Class(Cls.LightGray)
		).Set(
			ForegroundProperty
			,new SolidColorBrush(Gray)
		).AddTo(Styles);
		return NIL;
	}

	/// 統一使用描邊文字，與學習頁和卡片頁的視覺語言保持一致。
	protected StrokeTextBlock TxtBox(){
		var R = new StrokeTextBlock{
			Foreground = Brushes.White,
			Stroke = Brushes.Black,
			StrokeThickness = 3,
			UseVirtualizedRender = true,
			TextWrapping = TextWrapping.Wrap,
		};
		return R;
	}

	/// prop key 專用弱化字樣，避免與內容文字搶視線。
	protected StrokeTextBlock SubTxt(){
		var R = TxtBox();
		R.Foreground = new SolidColorBrush(Gray);
		R.StrokeThickness = 1.5;
		R.FontSize = UiCfg.Inst.BaseFontSize*0.85;
		return R;
	}

	/// 側欄 prop 值專用字樣：保留描邊，但略收細，避免與標題擠在一起。
	protected StrokeTextBlock SideValueTxt(){
		var R = TxtBox();
		R.StrokeThickness = 1.5;
		R.FontSize = UiCfg.Inst.BaseFontSize*0.95;
		return R;
	}

	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(3, GUT.Auto),
				RowDef(100, GUT.Star),
			]);
		});

		Root
		.A(new Border(), o=>{
			o.BorderThickness = new Thickness(0, 1, 0, 1);
			o.BorderBrush = new SolidColorBrush(Gray);
			o.SetChild(TxtBox(), x=>{
				Ctx.Bind(x, x.PropText, Vm=>Vm.Head);
				x.FontSize = UiCfg.Inst.BaseFontSize*1.4;
				x.StrokeThickness = 4;
				x.Styles.Add(new Style().NoMargin().NoPadding());
			});
		})
		.A(_MainContent());

		Root.Add();
		return NIL;
	}

	/// DataContext 變更時重新訂閱當前 Vm，避免仍盯着構造時的臨時 Ctx。
	protected nil OnCtxChanged(){
		if(SubscribedCtx is not null){
			SubscribedCtx.PropertyChanged -= OnCtxPropertyChanged;
			SubscribedCtx = null;
		}
		if(Ctx is not null){
			SubscribedCtx = Ctx;
			SubscribedCtx.PropertyChanged += OnCtxPropertyChanged;
		}
		SyncSidePaneVisibility();
		return NIL;
	}

	/// 其他 prop 集合變更後同步刷新右欄顯示狀態。
	protected void OnCtxPropertyChanged(object? Sender, PropertyChangedEventArgs E){
		if(E.PropertyName == nameof(Ctx.StrProps)){
			SyncSidePaneVisibility();
		}
	}

	/// 主內容區爲左右分欄：左側 description，右側其他 prop，中間細分隔條可拖拽。
	Control _MainContent(){
		MainGrid = new Grid();
		Splitter = _ColumnSplitter();
		SidePropsPane = _SidePropsPane();
		MainGrid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(2, GUT.Pixel),
			ColDef(1, GUT.Auto),
		]);
		MainGrid.Children.Add(_MainDescriptionPane());
		MainGrid.Children.Add(Splitter);
		MainGrid.Children.Add(SidePropsPane);
		Grid.SetColumn(Splitter, 1);
		Grid.SetColumn(SidePropsPane, 2);
		MainGrid.GetObservable(BoundsProperty).Subscribe(_=>{
			SyncSidePaneVisibility();
		});
		return MainGrid;
	}

	/// 沒有其他 prop 時，收起右欄與分隔條，避免留白。
	protected nil SyncSidePaneVisibility(){
		if(MainGrid is null || Splitter is null || SidePropsPane is null){
			return NIL;
		}
		var HasAny = HasSideProps();
		Splitter.IsVisible = HasAny;
		SidePropsPane.IsVisible = HasAny;
		MainGrid.ColumnDefinitions[1].Width = HasAny
			? new GridLength(2, GUT.Pixel)
			: new GridLength(0, GUT.Pixel);
		MainGrid.ColumnDefinitions[2].Width = HasAny
			? new GridLength(1, GUT.Auto)
			: new GridLength(0, GUT.Pixel);
		if(SidePropsPane is Border SideBorder){
			SideBorder.MinWidth = HasAny ? UiCfg.Inst.BaseFontSize*2.2 : 0;
			SideBorder.MaxWidth = HasAny ? CalcSidePaneMaxWidth() : 0;
		}
		return NIL;
	}

	/// 側欄最大寬度不得超過主內容區的一半，避免比 description 區更寬。
	protected double CalcSidePaneMaxWidth(){
		var TotalWidth = MainGrid?.Bounds.Width ?? 0;
		if(TotalWidth <= 0){
			return 220;
		}
		var MaxWidth = (TotalWidth - 2) / 2;
		return Math.Max(120, MaxWidth);
	}

	/// 右欄只負責展示 description 之外的屬性。
	protected bool HasSideProps(){
		return Ctx?.StrProps?.Count > 0;
	}

	/// 主欄只展示 description 列表，並提供滾動容器。
	Control _MainDescriptionPane(){
		var R = new Border();
		R.SetChild(new ScrollViewer(), o=>{
			o.SetContent(_DescriptionList());
		});
		return R;
	}

	/// 側欄豎向展示其餘 prop，內容過長時可獨立滾動。
	Control _SidePropsPane(){
		var R = new Border();
		R.BorderThickness = new Thickness(0);
		R.MinWidth = UiCfg.Inst.BaseFontSize*2.2;
		R.HorizontalAlignment = HAlign.Left;
		R.SetChild(new ScrollViewer(), o=>{
			o.HorizontalAlignment = HAlign.Left;
			o.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
			o.SetContent(_PropList());
		});
		return R;
	}

	/// 分隔條保持細線寬度，僅用於拖拽分欄，不搶畫面。
	Control _ColumnSplitter(){
		var R = new GridSplitter();
		R.ResizeDirection = GridResizeDirection.Columns;
		R.HorizontalAlignment = HAlign.Stretch;
		R.VerticalAlignment = VAlign.Stretch;
		R.Width = 1;
		R.Background = new SolidColorBrush(Color.FromArgb(180, Gray.R, Gray.G, Gray.B));
		return R;
	}

	/// 右欄屬性列表：prop key 與內容上下分行，避免描邊文字相互遮擋。
	Control _PropList(){
		var R = new ItemsControl();
		R.HorizontalAlignment = HAlign.Left;
		R.SetItemsPanel(()=>{
			return new StackPanel{
				Orientation = Orientation.Vertical,
				HorizontalAlignment = HAlign.Left,
			};
		});
		this.Bind(
			R,
			R.PropItemsSource, x=>x.StrProps
		);
		R.ItemTemplate = new FuncDataTemplate<KeyValuePair<str, IList<str>>>((Prop,b)=>{
			var Ans = new Border();
			{var o = Ans;
				o.BorderThickness = new Thickness(0, 0, 0, 1);
				o.BorderBrush = new SolidColorBrush(Gray);
				o.Padding = new Thickness(8, 8);
				o.HorizontalAlignment = HAlign.Left;
			}
			var Stack = new StackPanel{
				Orientation = Orientation.Vertical,
				Spacing = 8,
				HorizontalAlignment = HAlign.Left,
			};
			Ans.Child = Stack;
			Stack
			.A(SubTxt(), o=>{
				o.Text = TranslatePropKey(Prop.Key);
				o.HorizontalAlignment = HAlign.Left;
			})
			.A(SideValueTxt(), o=>{
				o.Text = str.Join("\n", Prop.Value ?? []);
				o.HorizontalAlignment = HAlign.Left;
			});
			return Ans;
		});
		return R;
	}

	/// description 列表作爲主欄正文，每段單獨分隔。
	Control _DescriptionList(){
		var R = new ItemsControl();
		this.Bind(
			R,
			R.PropItemsSource, x=>x.Descrs
		);
		R.ItemTemplate = new FuncDataTemplate<str>((Descr,b)=>{
			var Ans = new Border();
			{var o = Ans;
				o.BorderThickness = new Thickness(0, 1, 0, 0);
				o.BorderBrush = new SolidColorBrush(Gray);
				o.Padding = new Thickness(8, 8);
			}
			var Grid = new AutoGrid(IsRow: true);
			Ans.Child = Grid.Grid;
			Grid.A(TxtBox(), o=>{
				o.CBind<str>(o.PropText, x=>x);
				o.FontSize = UiCfg.Inst.BaseFontSize;
			});
			return Ans;
		});
		return R;
	}

	/// 內置 prop 鍵顯示 i18n，未知鍵保留原始文本以兼容自定義 prop。
	protected str TranslatePropKey(str? Key){
		return Key switch{
			var x when x == KeysProp.Inst.summary => I[K.Summary],
			var x when x == KeysProp.Inst.description => I[K.Descr],
			var x when x == KeysProp.Inst.note => I[K.Note],
			var x when x == KeysProp.Inst.tag => I[K.Tag],
			var x when x == KeysProp.Inst.source => I[K.Source],
			var x when x == KeysProp.Inst.alias => I[K.Alias],
			var x when x == KeysProp.Inst.pronunciation => I[K.Pronunciation],
			var x when x == KeysProp.Inst.weight => I[K.Weight],
			var x when x == KeysProp.Inst.learn => I[K.Learn],
			var x when x == KeysProp.Inst.usage => I[K.Usage],
			var x when x == KeysProp.Inst.example => I[K.Example],
			var x when x == KeysProp.Inst.relation => I[K.Relation],
			var x when x == KeysProp.Inst.Ref => I[K.Ref],
			_ => Key ?? "",
		};
	}
}
