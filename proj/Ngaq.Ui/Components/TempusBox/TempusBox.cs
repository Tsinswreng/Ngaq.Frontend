namespace Ngaq.Ui.Components.TempusBox;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Globalization;
using Ngaq.Core.Infra;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.CsCore;
using Tsinswreng.AvlnTools.Tools;
using Ngaq.Ui.Icons;

/// 單類型 `TempusBox` 基礎組件（不拆 View/Vm）。
/// 左：格式選擇按鈕（點擊後出下拉）
/// 中：可輸入字符串
/// 右：可視化日曆
public partial class TempusBox: ContentControl{
	public enum ETextFormat{
		Iso,
		UnixMs,
		LocalDateTime,
		DateOnly,
	}

	/// 當前 `Tempus` 主值。文本輸入和日曆選擇都會回寫到此值。
	public Tempus Tempus{
		get{return _Tempus;}
		set{
			_Tempus = value;
			SyncUiFromTempus();
		}
	}
	Tempus _Tempus = Tempus.Now();

	/// 是否只讀。`true` 時禁用格式切換和日曆，文本框進入只讀。
	public bool IsReadOnly{
		get{return _IsReadOnly;}
		set{
			_IsReadOnly = value;
			ApplyReadOnlyState();
		}
	}
	bool _IsReadOnly = false;

	/// 當前文本格式。
	public ETextFormat TextFormat{
		get{return _TextFormat;}
		set{
			_TextFormat = value;
			SyncUiFromTempus();
		}
	}
	ETextFormat _TextFormat = ETextFormat.Iso;

	/// 最近一次文本解析是否成功，外層可讀取作提示。
	public bool LastParseOk{get;protected set;} = true;

	public II18n I{get;set;} = I18n.Inst;

	readonly AutoGrid Root = new(IsRow: false);
	Button? _BtnFormat;
	TextBox? _Input;
	Button? _BtnCalendar;
	Avalonia.Controls.Calendar? _Calendar;
	Flyout? _FormatFlyout;
	Flyout? _CalendarFlyout;
	bool _SyncingUi = false;

	public TempusBox(){
		Render();
		ApplyReadOnlyState();
		SyncUiFromTempus();
	}

	void Render(){
		this.Content = Root.Grid;
		Root.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Auto),
		]);
		Root
		.A(MkFormatBtn())
		.A(MkInputBox())
		.A(MkCalendarBtn());
	}

	Button MkFormatBtn(){
		var btn = new Button{
			Content = Svgs.FormatLineStyle().ToIcon()
		};
		var flyout = new Flyout{
			Placement = PlacementMode.Bottom,
		};
		var panel = new StackPanel();
		foreach(var fmt in new[]{ETextFormat.Iso, ETextFormat.UnixMs, ETextFormat.LocalDateTime, ETextFormat.DateOnly}){
			var one = new Button();
			one.SetContent(FormatToDisplay(fmt));
			one.Click += (s, e)=>{
				TextFormat = fmt;
				flyout.Hide();
			};
			panel.A(one);
		}
		flyout.Content = panel;
		FlyoutBase.SetAttachedFlyout(btn, flyout);
		btn.Click += (s, e)=>{
			FlyoutBase.ShowAttachedFlyout(btn);
		};
		_BtnFormat = btn;
		_FormatFlyout = flyout;
		return btn;
	}

	TextBox MkInputBox(){
		var txt = new TextBox{
		};
		txt.TextChanged += (s, e)=>{
			// 由用戶輸入觸發時，按當前格式解析；失敗則只更新標記，不覆蓋既有 Tempus。
			if(_SyncingUi){
				return;
			}
			if(TryParseTempus(txt.Text ?? "", TextFormat, out var parsed)){
				LastParseOk = true;
				_Tempus = parsed;
				SyncUiFromTempus();
			}else{
				LastParseOk = false;
			}
		};
		_Input = txt;
		return txt;
	}

	Button MkCalendarBtn(){
		var btn = new Button{
			Content = Svgs.Calendar().ToIcon()
		};
		var flyout = new Flyout{
			Placement = PlacementMode.Bottom,
		};
		var wrap = new Border{
			Padding = new Thickness(6),
		};
		var calendar = new Avalonia.Controls.Calendar{
			HorizontalAlignment = HAlign.Stretch,
			VerticalAlignment = VAlign.Stretch,
		};
		calendar.SelectedDatesChanged += (s, e)=>{
			// 日曆僅改日期部分，保留原時間的時分秒毫秒。
			if(_SyncingUi || IsReadOnly || calendar.SelectedDate is null){
				return;
			}
			var selectedDate = calendar.SelectedDate.Value;
			var currentLocal = DateTimeOffset.FromUnixTimeMilliseconds(_Tempus.Value).ToLocalTime().DateTime;
			var merged = new DateTime(
				selectedDate.Year,
				selectedDate.Month,
				selectedDate.Day,
				currentLocal.Hour,
				currentLocal.Minute,
				currentLocal.Second,
				currentLocal.Millisecond,
				DateTimeKind.Local
			);
			_Tempus = Tempus.FromDateTime(merged);
			SyncUiFromTempus();
			// 選完日期即收起，避免右側長時間佔用空間。
			flyout.Hide();
		};
		wrap.Child = calendar;
		flyout.Content = wrap;
		FlyoutBase.SetAttachedFlyout(btn, flyout);
		btn.Click += (s, e)=>{
			if(IsReadOnly){
				return;
			}
			FlyoutBase.ShowAttachedFlyout(btn);
		};
		_BtnCalendar = btn;
		_Calendar = calendar;
		_CalendarFlyout = flyout;
		return btn;
	}

	void SyncUiFromTempus(){
		if(_BtnFormat is null || _Input is null || _Calendar is null || _BtnCalendar is null){
			return;
		}
		_SyncingUi = true;
		_BtnFormat.SetContent(FormatToDisplay(TextFormat));
		_Input.Text = FormatTempus(_Tempus, TextFormat);
		var localDate = DateTimeOffset.FromUnixTimeMilliseconds(_Tempus.Value).ToLocalTime().Date;
		_Calendar.SelectedDate = localDate;
		_BtnCalendar.SetContent(localDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
		_SyncingUi = false;
		LastParseOk = true;
	}

	void ApplyReadOnlyState(){
		if(_BtnFormat is not null){
			_BtnFormat.IsEnabled = !IsReadOnly;
		}
		if(_Input is not null){
			_Input.IsReadOnly = IsReadOnly;
		}
		if(_BtnCalendar is not null){
			_BtnCalendar.IsEnabled = !IsReadOnly;
		}
		if(_Calendar is not null){
			_Calendar.IsEnabled = !IsReadOnly;
		}
		if(IsReadOnly){
			_CalendarFlyout?.Hide();
		}
	}

	static str FormatToDisplay(ETextFormat Format){
		return Format switch{
			ETextFormat.Iso => "ISO 8601",
			ETextFormat.UnixMs => "Unix ms",
			ETextFormat.LocalDateTime => "Local datetime",
			ETextFormat.DateOnly => "Date only",
			_ => "Unknown",
		};
	}

	static str FormatTempus(Tempus Value, ETextFormat Format){
		return Format switch{
			ETextFormat.Iso => Value.ToIso(),
			ETextFormat.UnixMs => Value.Value.ToString(CultureInfo.InvariantCulture),
			ETextFormat.LocalDateTime
				=> DateTimeOffset.FromUnixTimeMilliseconds(Value.Value)
				.ToLocalTime()
				.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
			ETextFormat.DateOnly
				=> DateTimeOffset.FromUnixTimeMilliseconds(Value.Value)
				.ToLocalTime()
				.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
			_ => Value.ToIso(),
		};
	}

	static bool TryParseTempus(str Text, ETextFormat Format, out Tempus Result){
		Result = default;
		if(string.IsNullOrWhiteSpace(Text)){
			return false;
		}
		switch(Format){
			case ETextFormat.Iso:
				return Tempus.TryFromIso(Text, out Result);

			case ETextFormat.UnixMs:
				if(i64.TryParse(Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ms)){
					Result = Tempus.FromUnixMs(ms);
					return true;
				}
				return false;

			case ETextFormat.LocalDateTime:
				if(DateTime.TryParseExact(
					Text,
					"yyyy-MM-dd HH:mm:ss",
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out var dt
				)){
					Result = Tempus.FromDateTime(new DateTime(
						dt.Year, dt.Month, dt.Day,
						dt.Hour, dt.Minute, dt.Second,
						DateTimeKind.Local
					));
					return true;
				}
				return false;

			case ETextFormat.DateOnly:
				if(DateTime.TryParseExact(
					Text,
					"yyyy-MM-dd",
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out var date
				)){
					Result = Tempus.FromDateTime(new DateTime(
						date.Year, date.Month, date.Day,
						0, 0, 0,
						DateTimeKind.Local
					));
					return true;
				}
				return false;

			default:
				return false;
		}
	}
}
