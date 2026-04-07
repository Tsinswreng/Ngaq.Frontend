
namespace Ngaq.Ui.Components.TempusBox;

using System.Collections.ObjectModel;
using System.Globalization;
using Ngaq.Core.Infra;
using Ngaq.Ui.Infra;
using Tsinswreng.CsCore;
using Ctx = VmTempusBox;

/// Tempus 輸入框的文本格式枚舉。
public enum ETempusTextFormat{
	Iso,
	UnixMs,
	LocalDateTime,
	DateOnly,
}

/// `ViewTempusBox` 的 ViewModel。
/// 職責：
/// 1) 維護主值 `Tempus`；
/// 2) 維護文本表示與格式切換；
/// 3) 維護日曆日期與主值同步；
/// 4) 提供只讀模式控制。
public partial class VmTempusBox: ViewModelBase, IMk<Ctx>{
	protected VmTempusBox(){
		SyncTextFromTempus();
		SyncCalendarDateFromTempus();
	}

	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmTempusBox(){
		#if DEBUG
		{
			var o = new Ctx{
				TextFormat = ETempusTextFormat.Iso,
			};
			Samples.Add(o);
		}
		#endif
	}

	/// 是否只讀。`true` 時禁用格式切換與日曆交互，文本框進入只讀模式。
	public bool IsReadOnly{
		get{return _IsReadOnly;}
		set{
			if(SetProperty(ref _IsReadOnly, value)){
				OnPropertyChanged(nameof(CanEdit));
			}
		}
	}
	bool _IsReadOnly = false;

	/// 便於綁定控件的可編輯狀態。
	public bool CanEdit => !IsReadOnly;

	/// 當前主值。所有 UI 輸入最終都匯總到此字段。
	public Tempus Tempus{
		get{return _Tempus;}
		set{
			if(SetProperty(ref _Tempus, value)){
				SyncTextFromTempus();
				SyncCalendarDateFromTempus();
			}
		}
	}
	Tempus _Tempus = Tempus.Now();

	/// 當前文本格式。
	public ETempusTextFormat TextFormat{
		get{return _TextFormat;}
		set{
			if(SetProperty(ref _TextFormat, value)){
				OnPropertyChanged(nameof(CurrentFormatDisplay));
				SyncTextFromTempus();
			}
		}
	}
	ETempusTextFormat _TextFormat = ETempusTextFormat.Iso;

	/// 給左側按鈕展示當前格式名。
	public str CurrentFormatDisplay => FormatToDisplay(TextFormat);

	/// 可選文本格式列表（給下拉選單使用）。
	public IReadOnlyList<ETempusTextFormat> FormatOptions{get;} = [
		ETempusTextFormat.Iso,
		ETempusTextFormat.UnixMs,
		ETempusTextFormat.LocalDateTime,
		ETempusTextFormat.DateOnly,
	];

	/// 中間文本框的字符串值。編輯後會嘗試反序列化回 `Tempus`。
	public str InputText{
		get{return _InputText;}
		set{
			if(SetProperty(ref _InputText, value)){
				TryApplyInputText(value);
			}
		}
	}
	str _InputText = "";

	/// 右側日曆選中值（只取日期部分）。
	public DateTime? CalendarDate{
		get{return _CalendarDate;}
		set{
			if(SetProperty(ref _CalendarDate, value)){
				ApplyCalendarDate(value);
			}
		}
	}
	DateTime? _CalendarDate = null;

	/// 最近一次文本解析是否成功，可供外部做提示樣式。
	public bool LastParseOk{
		get{return _LastParseOk;}
		set{SetProperty(ref _LastParseOk, value);}
	}
	bool _LastParseOk = true;

	/// 由視圖下拉選單調用，切換格式。
	public void SelectFormat(ETempusTextFormat Format){
		TextFormat = Format;
	}

	/// 按當前格式把 `Tempus` 同步到文本框。
	void SyncTextFromTempus(){
		var next = FormatTempus(Tempus, TextFormat);
		_IsSyncingText = true;
		ForceSetProp(ref _InputText, next, nameof(InputText));
		_IsSyncingText = false;
		LastParseOk = true;
	}

	/// 把 `Tempus` 同步到日曆日期（本地時區日期）。
	void SyncCalendarDateFromTempus(){
		var local = DateTimeOffset.FromUnixTimeMilliseconds(Tempus.Value).ToLocalTime().DateTime.Date;
		_IsSyncingCalendar = true;
		ForceSetProp(ref _CalendarDate, local, nameof(CalendarDate));
		_IsSyncingCalendar = false;
	}

	/// 文本框回寫：按當前格式解析字符串，成功則更新主值。
	void TryApplyInputText(str RawText){
		if(_IsSyncingText){
			return;
		}
		if(TryParseTempus(RawText, TextFormat, out var parsed)){
			LastParseOk = true;
			Tempus = parsed;
		}else{
			LastParseOk = false;
		}
	}

	/// 日曆回寫：只改日期，保留當前時分秒與毫秒。
	void ApplyCalendarDate(DateTime? Date){
		if(_IsSyncingCalendar){
			return;
		}
		if(Date is null){
			return;
		}
		if(IsReadOnly){
			SyncCalendarDateFromTempus();
			return;
		}

		var currentLocal = DateTimeOffset.FromUnixTimeMilliseconds(Tempus.Value).ToLocalTime().DateTime;
		var merged = new DateTime(
			Date.Value.Year,
			Date.Value.Month,
			Date.Value.Day,
			currentLocal.Hour,
			currentLocal.Minute,
			currentLocal.Second,
			currentLocal.Millisecond,
			DateTimeKind.Local
		);
		Tempus = Tempus.FromDateTime(merged);
	}

	static str FormatToDisplay(ETempusTextFormat Format){
		return Format switch{
			ETempusTextFormat.Iso => "ISO 8601",
			ETempusTextFormat.UnixMs => "Unix ms",
			ETempusTextFormat.LocalDateTime => "Local datetime",
			ETempusTextFormat.DateOnly => "Date only",
			_ => "Unknown",
		};
	}

	static str FormatTempus(Tempus Value, ETempusTextFormat Format){
		return Format switch{
			ETempusTextFormat.Iso => Value.ToIso(),
			ETempusTextFormat.UnixMs => Value.Value.ToString(CultureInfo.InvariantCulture),
			ETempusTextFormat.LocalDateTime
				=> DateTimeOffset.FromUnixTimeMilliseconds(Value.Value)
				.ToLocalTime()
				.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
			ETempusTextFormat.DateOnly
				=> DateTimeOffset.FromUnixTimeMilliseconds(Value.Value)
				.ToLocalTime()
				.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
			_ => Value.ToIso(),
		};
	}

	static bool TryParseTempus(str Text, ETempusTextFormat Format, out Tempus Result){
		Result = default;
		if(string.IsNullOrWhiteSpace(Text)){
			return false;
		}

		switch(Format){
			case ETempusTextFormat.Iso:
				return Tempus.TryFromIso(Text, out Result);

			case ETempusTextFormat.UnixMs:
				if(i64.TryParse(Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ms)){
					Result = Tempus.FromUnixMs(ms);
					return true;
				}
				return false;

			case ETempusTextFormat.LocalDateTime:
				if(DateTime.TryParseExact(
					Text,
					"yyyy-MM-dd HH:mm:ss",
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out var dt
				)){
					var local = new DateTime(
						dt.Year, dt.Month, dt.Day,
						dt.Hour, dt.Minute, dt.Second,
						DateTimeKind.Local
					);
					Result = Tempus.FromDateTime(local);
					return true;
				}
				return false;

			case ETempusTextFormat.DateOnly:
				if(DateTime.TryParseExact(
					Text,
					"yyyy-MM-dd",
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out var date
				)){
					var local = new DateTime(
						date.Year, date.Month, date.Day,
						0, 0, 0, DateTimeKind.Local
					);
					Result = Tempus.FromDateTime(local);
					return true;
				}
				return false;

			default:
				return false;
		}
	}

	bool _IsSyncingText = false;
	bool _IsSyncingCalendar = false;
}
