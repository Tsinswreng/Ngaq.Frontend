namespace Ngaq.Ui.Components.TempusBox;

using Avalonia;
using Avalonia.Data;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Globalization;
using System.Linq;
using Ngaq.Core.Infra;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.CsCore;
using Tsinswreng.AvlnTools.Tools;
using Ngaq.Ui.Icons;
using Avalonia.Data.Converters;

public class TempusFormatItem{
	//默認內置 Iso8601 和 UnixMs
	[Doc(@$"默認內置。顯示本地時區。
	#Example([2026-04-07T23:16:07.344+08:00])
	")]
	public static TempusFormatItem Iso8601Full{get;set;}
	[Doc(@$"默認內置。
	#Examples([1775575237143])
	")]
	public static TempusFormatItem UnixMs{get;set;}
	[Doc(@$"
	#Examples([26-04-07])
	")]
	public static TempusFormatItem yy_MM_DD{get;set;}
	[Doc(@$"
	#Examples([26-04-07 12:34])
	")]
	public static TempusFormatItem yy_MM_DD__HH_mm{get;set;}

	[Doc(@$"該種格式的名稱 在下拉框中顯示")]
	public str FmtDisplayName{get;set;} = "";
	public IValueConverter Converter{get;set;} = null!;

	static TempusFormatItem(){
		Iso8601Full = new TempusFormatItem{
			FmtDisplayName = "ISO 8601",
			Converter = MkIsoLocalConverter(),
		};
		UnixMs = new TempusFormatItem{
			FmtDisplayName = "Unix ms",
			Converter = MkUnixMsConverter(),
		};
		yy_MM_DD = new TempusFormatItem{
			FmtDisplayName = "yy-MM-dd",
			Converter = MkDateTimePatternConverter("yy-MM-dd"),
		};
		yy_MM_DD__HH_mm = new TempusFormatItem{
			FmtDisplayName = "yy-MM-dd HH:mm",
			Converter = MkDateTimePatternConverter("yy-MM-dd HH:mm"),
		};
	}

	static IValueConverter MkIsoLocalConverter(){
		return new ParamFnConvtr<obj?, obj?>(
			(v, p)=>{
				if(v is Tempus t){
					return DateTimeOffset.FromUnixTimeMilliseconds(t.Value)
					.ToLocalTime()
					.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
				}
				return BindingNotification.UnsetValue;
			},
			(v, p)=>{
				if(v is str s && DateTimeOffset.TryParseExact(
					s,
					"yyyy-MM-ddTHH:mm:ss.fffzzz",
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out var dto
				)){
					return Tempus.FromUnixMs(dto.ToUnixTimeMilliseconds());
				}
				return BindingNotification.UnsetValue;
			}
		);
	}

	static IValueConverter MkUnixMsConverter(){
		return new ParamFnConvtr<obj?, obj?>(
			(v, p)=>{
				if(v is Tempus t){
					return t.Value.ToString(CultureInfo.InvariantCulture);
				}
				return BindingNotification.UnsetValue;
			},
			(v, p)=>{
				if(v is str s && i64.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ms)){
					return Tempus.FromUnixMs(ms);
				}
				return BindingNotification.UnsetValue;
			}
		);
	}

	static IValueConverter MkDateTimePatternConverter(str Pattern){
		var fmt = NormalizePattern(Pattern);
		return new ParamFnConvtr<obj?, obj?>(
			(v, p)=>{
				if(v is Tempus t){
					return DateTimeOffset.FromUnixTimeMilliseconds(t.Value)
					.ToLocalTime()
					.DateTime
					.ToString(fmt, CultureInfo.InvariantCulture);
				}
				return BindingNotification.UnsetValue;
			},
			(v, p)=>{
				if(v is str s && DateTime.TryParseExact(
					s,
					fmt,
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out var dt
				)){
					return Tempus.FromDateTime(new DateTime(
						dt.Year, dt.Month, dt.Day,
						dt.Hour, dt.Minute, dt.Second,
						dt.Millisecond,
						DateTimeKind.Local
					));
				}
				return BindingNotification.UnsetValue;
			}
		);
	}

	static str NormalizePattern(str Pattern){
		return Pattern
		.Replace("YYYY", "yyyy")
		.Replace("YY", "yy")
		.Replace("DD", "dd");
	}
}


public partial class TempusBox: ContentControl{
	public static readonly DirectProperty<TempusBox, Tempus> TempusProperty =
		AvaloniaProperty.RegisterDirect<TempusBox, Tempus>(
			nameof(Tempus),
			o => o.Tempus,
			(o, v) => o.Tempus = v,
			defaultBindingMode: BindingMode.TwoWay
		);

	/// 當前 `Tempus` 主值。文本輸入和日曆選擇都會回寫到此值。
	public Tempus Tempus{
		get{return _Tempus;}
		set{
			SetAndRaise(TempusProperty, ref _Tempus, value);
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

	/// 全部格式來源。下拉框只使用此列表。
	public IList<TempusFormatItem> FormatItems{get;} = [
		TempusFormatItem.Iso8601Full,
		TempusFormatItem.UnixMs,
	];

	/// 當前選中的格式下標。
	public i32 SelectedFormatIndex{
		get{return _SelectedFormatIndex;}
		set{
			var max = FormatItems.Count - 1;
			var next = max < 0 ? 0 : Math.Clamp(value, 0, max);
			_SelectedFormatIndex = next;
			SyncUiFromTempus();
		}
	}
	i32 _SelectedFormatIndex = 0;

	/// 控件統一高度，讓左按鈕與輸入框外框對齊。
	public f64 ControlHeight{
		get{return _ControlHeight;}
		set{
			_ControlHeight = value;
			ApplyControlSize();
		}
	}
	f64 _ControlHeight = 34;

	/// 最近一次文本解析是否成功，外層可讀取作提示。
	public bool LastParseOk{get;protected set;} = true;
	public readonly AutoGrid Root = new(IsRow: false);
	public TextBox _Input{get;set;} = null!;
	public Button _BtnMenu{get;set;}
	public ComboBox _ComboBoxFormat{get;set;}
	public Avalonia.Controls.Calendar _Calendar{get;set;}
	Flyout? _MenuFlyout;
	bool _SyncingUi = false;

	public TempusBox(){
		Render();
		ApplyControlSize();
		ApplyReadOnlyState();
		SyncUiFromTempus();
	}

	/// 外層修改 `FormatItems` 後調用，刷新下拉格式源。
	public void RefreshFormatOptions(){
		_SelectedFormatIndex = Math.Clamp(_SelectedFormatIndex, 0, Math.Max(0, FormatItems.Count-1));
		SyncUiFromTempus();
	}

	void Render(){
		this.Content = Root.Grid;
		Root.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Star),
		]);
		Root
		.A(MkMenuBtn())
		.A(MkInputBox());
	}

	TextBox MkInputBox(){
		_Input = new TextBox{};
		var o = _Input;
		o.MinWidth = 0;
		o.VerticalAlignment = VAlign.Stretch;
		o.VerticalContentAlignment = VAlign.Center;
		o.Margin = new Thickness(0);
		o.BorderThickness = new Thickness(1);
		o.Padding = new Thickness(8, 0, 8, 0);
		o.TextChanged += (s, e)=>{
			// 由用戶輸入觸發時，按當前格式解析；失敗則只更新標記，不覆蓋既有 Tempus。
			if(_SyncingUi){
				return;
			}
			if(TryParseBySelectedFormat(o.Text ?? "", out var parsed)){
				LastParseOk = true;
				Tempus = parsed;
			}else{
				LastParseOk = false;
			}
		};

		return o;
	}

	Button MkMenuBtn(){
		var btn = new Button{
			Content = Svgs.Calendar().ToIcon()
		};
		btn.HorizontalAlignment = HAlign.Stretch;
		btn.VerticalAlignment = VAlign.Stretch;
		btn.Margin = new Thickness(0);
		btn.BorderThickness = new Thickness(1);
		btn.Padding = new Thickness(8, 0, 8, 0);
		var flyout = new Flyout{
			Placement = PlacementMode.Bottom,
		};
		var panel = new StackPanel();
		var cbFormat = new ComboBox{
			ItemsSource = FormatItems.Select(x=>x.FmtDisplayName).ToArray(),
		};
		cbFormat.SelectionChanged += (s, e)=>{
			if(_SyncingUi){
				return;
			}
			if(cbFormat.SelectedIndex is >= 0 && cbFormat.SelectedIndex < FormatItems.Count){
				SelectedFormatIndex = cbFormat.SelectedIndex;
			}
		};
		panel.A(cbFormat);
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
			var currentLocal = DateTimeOffset.FromUnixTimeMilliseconds(Tempus.Value).ToLocalTime().DateTime;
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
			Tempus = Tempus.FromDateTime(merged);
		};
		wrap.Child = calendar;
		panel.A(wrap);
		flyout.Content = panel;
		FlyoutBase.SetAttachedFlyout(btn, flyout);
		btn.Click += (s, e)=>{
			if(IsReadOnly){
				return;
			}
			cbFormat.ItemsSource = FormatItems.Select(x=>x.FmtDisplayName).ToArray();
			cbFormat.SelectedIndex = Math.Clamp(SelectedFormatIndex, 0, Math.Max(0, FormatItems.Count-1));
			FlyoutBase.ShowAttachedFlyout(btn);
			cbFormat.IsDropDownOpen = true;
		};
		_BtnMenu = btn;
		_ComboBoxFormat = cbFormat;
		_Calendar = calendar;
		_MenuFlyout = flyout;
		return btn;
	}

	void SyncUiFromTempus(){
		if(_Input is null || _Calendar is null || _BtnMenu is null){
			return;
		}
		_SyncingUi = true;
		_Input.Text = FormatBySelectedFormat(Tempus);
		if(_ComboBoxFormat is not null){
			_ComboBoxFormat.ItemsSource = FormatItems.Select(x=>x.FmtDisplayName).ToArray();
			_ComboBoxFormat.SelectedIndex = Math.Clamp(SelectedFormatIndex, 0, Math.Max(0, FormatItems.Count-1));
		}
		var localDate = DateTimeOffset.FromUnixTimeMilliseconds(Tempus.Value).ToLocalTime().Date;
		_Calendar.SelectedDate = localDate;
		_SyncingUi = false;
		LastParseOk = true;
	}

	void ApplyReadOnlyState(){
		if(_Input is not null){
			_Input.IsReadOnly = IsReadOnly;
		}
		if(_BtnMenu is not null){
			_BtnMenu.IsEnabled = !IsReadOnly;
		}
		if(_ComboBoxFormat is not null){
			_ComboBoxFormat.IsEnabled = !IsReadOnly;
		}
		if(_Calendar is not null){
			_Calendar.IsEnabled = !IsReadOnly;
		}
		if(IsReadOnly){
			_MenuFlyout?.Hide();
		}
	}

	void ApplyControlSize(){
		if(_Input is not null){
			_Input.Height = ControlHeight;
		}
		if(_BtnMenu is not null){
			_BtnMenu.Height = ControlHeight;
		}
	}

	str FormatBySelectedFormat(Tempus Value){
		if(FormatItems.Count == 0){
			return Value.Value.ToString(CultureInfo.InvariantCulture);
		}
		var item = FormatItems[Math.Clamp(SelectedFormatIndex, 0, FormatItems.Count-1)];
		var converted = item.Converter.Convert(Value, typeof(str), null, CultureInfo.InvariantCulture);
		if(converted is str s){
			return s;
		}
		return Value.Value.ToString(CultureInfo.InvariantCulture);
	}

	bool TryParseBySelectedFormat(str Text, out Tempus Result){
		Result = default;
		if(string.IsNullOrWhiteSpace(Text)){
			return false;
		}
		if(FormatItems.Count == 0){
			return false;
		}
		var item = FormatItems[Math.Clamp(SelectedFormatIndex, 0, FormatItems.Count-1)];
		var converted = item.Converter.ConvertBack(Text, typeof(Tempus), null, CultureInfo.InvariantCulture);
		if(converted is Tempus t){
			Result = t;
			return true;
		}
		return false;
	}
}
