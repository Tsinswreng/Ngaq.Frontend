namespace Ngaq.Ui.Components.TempusBox;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Declarative;
using Avalonia.Styling;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmTempusBox;

/// `Tempus` 展示器兼編輯器：
/// 左：格式選擇按鈕（點擊彈下拉）
/// 中：可輸入字符串
/// 右：可視化日曆選擇
public partial class ViewTempusBox: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewTempusBox(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}

	public II18n I = I18n.Inst;

	public partial class Cls{
		public const str Input = nameof(Input);
		public const str RightPanel = nameof(RightPanel);
	}

	protected nil Style(){
		var S = Styles;
		S.A(new Style(x=>x.Class(Cls.Input)).Set(MinWidthProperty, 220.0));
		S.A(new Style(x=>x.Class(Cls.RightPanel)).Set(MinWidthProperty, 280.0));
		return NIL;
	}

	AutoGrid Root = new(IsRow: false);

	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);

		Root
		.A(MkFormatBtn())
		.A(MkInputBox())
		.A(MkCalendarPanel())
		;

		return NIL;
	}

	Button MkFormatBtn(){
		var btn = new Button();
		btn.CBind<Ctx>(btn.PropContent, x=>x.CurrentFormatDisplay, Mode: BindingMode.OneWay);
		btn.CBind<Ctx>(IsEnabledProperty, x=>x.CanEdit, Mode: BindingMode.OneWay);

		var flyout = new Flyout{
			Placement = PlacementMode.Bottom,
		};

		var panel = new StackPanel();
		foreach(var fmt in Ctx?.FormatOptions ?? []){
			var one = new Button();
			one.SetContent(new TextBlock{
				Text = fmt switch{
					ETempusTextFormat.Iso => "ISO 8601",
					ETempusTextFormat.UnixMs => "Unix ms",
					ETempusTextFormat.LocalDateTime => "Local datetime",
					ETempusTextFormat.DateOnly => "Date only",
					_ => "Unknown",
				}
			});
			one.Click += (s, e)=>{
				Ctx?.SelectFormat(fmt);
				flyout.Hide();
			};
			panel.A(one);
		}
		flyout.Content = panel;
		FlyoutBase.SetAttachedFlyout(btn, flyout);

		btn.Click += (s, e)=>{
			FlyoutBase.ShowAttachedFlyout(btn);
		};
		return btn;
	}

	TextBox MkInputBox(){
		var txt = new TextBox{
			Watermark = Todo.I18n("輸入時間字符串"),
		};
		txt.Classes.A(Cls.Input);
		txt.CBind<Ctx>(txt.PropText, x=>x.InputText, Mode: BindingMode.TwoWay);
		txt.CBind<Ctx>(TextBox.IsReadOnlyProperty, x=>x.IsReadOnly, Mode: BindingMode.OneWay);
		return txt;
	}

	Control MkCalendarPanel(){
		var wrap = new Border{
			Padding = new Thickness(6),
		};
		wrap.Classes.A(Cls.RightPanel);

		var calendar = new Calendar{
			HorizontalAlignment = HAlign.Stretch,
			VerticalAlignment = VAlign.Stretch,
		};
		calendar.CBind<Ctx>(IsEnabledProperty, x=>x.CanEdit, Mode: BindingMode.OneWay);
		calendar.CBind<Ctx>(Calendar.SelectedDateProperty, x=>x.CalendarDate, Mode: BindingMode.TwoWay);
		wrap.Child = calendar;
		return wrap;
	}
}
