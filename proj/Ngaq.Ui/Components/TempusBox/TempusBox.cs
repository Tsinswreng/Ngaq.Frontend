namespace Ngaq.Ui.Components.TempusBox;

using Avalonia;
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

/// 單類型 `TempusBox` 基礎組件（不拆 View/Vm）。
/// 左：可輸入字符串
/// 右：單一按鈕，點開同一菜單中的「格式選擇 + 日曆」
public partial class TempusBox: ContentControl{
	public enum EBuiltinFormat{
		Iso,
		UnixMs,
	}

	struct FormatOption{
		public bool IsBuiltin;
		public EBuiltinFormat Builtin;
		public str? Pattern;
		public str Display;
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

	/// 內置格式列表（可配置）。
	public IList<EBuiltinFormat> BuiltinFormats{get;} = [EBuiltinFormat.Iso, EBuiltinFormat.UnixMs];

	/// 自定義格式列表（可配置）。如 `"yy-MM-dd"`。
	public IList<str> CustomFormatPatterns{get;} = [];

	/// 當前選中的格式下標。
	public i32 SelectedFormatIndex{
		get{return _SelectedFormatIndex;}
		set{
			var max = _FormatOptions.Count - 1;
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
	readonly List<FormatOption> _FormatOptions = [];

	public TempusBox(){
		RebuildFormatOptions();
		Render();
		ApplyControlSize();
		ApplyReadOnlyState();
		SyncUiFromTempus();
	}

	/// 外層修改 `BuiltinFormats/CustomFormatPatterns` 後調用，刷新下拉格式源。
	public void RefreshFormatOptions(){
		RebuildFormatOptions();
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
		var txt = new TextBox{
			MinWidth = 0,
			VerticalAlignment = VAlign.Stretch,
		};
		txt.Margin = new Thickness(0);
		txt.BorderThickness = new Thickness(1);
		txt.Padding = new Thickness(8, 0, 8, 0);
		txt.TextChanged += (s, e)=>{
			// 由用戶輸入觸發時，按當前格式解析；失敗則只更新標記，不覆蓋既有 Tempus。
			if(_SyncingUi){
				return;
			}
			if(TryParseBySelectedFormat(txt.Text ?? "", out var parsed)){
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
			ItemsSource = _FormatOptions.Select(x=>x.Display).ToArray(),
		};
		cbFormat.SelectionChanged += (s, e)=>{
			if(_SyncingUi){
				return;
			}
			if(cbFormat.SelectedIndex is >= 0 && cbFormat.SelectedIndex < _FormatOptions.Count){
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
		};
		wrap.Child = calendar;
		panel.A(wrap);
		flyout.Content = panel;
		FlyoutBase.SetAttachedFlyout(btn, flyout);
		btn.Click += (s, e)=>{
			if(IsReadOnly){
				return;
			}
			RebuildFormatOptions();
			cbFormat.ItemsSource = _FormatOptions.Select(x=>x.Display).ToArray();
			cbFormat.SelectedIndex = Math.Clamp(SelectedFormatIndex, 0, Math.Max(0, _FormatOptions.Count-1));
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
		if(_FormatOptions.Count == 0){
			RebuildFormatOptions();
		}
		_SyncingUi = true;
		_Input.Text = FormatBySelectedFormat(_Tempus);
		if(_ComboBoxFormat is not null){
			_ComboBoxFormat.ItemsSource = _FormatOptions.Select(x=>x.Display).ToArray();
			_ComboBoxFormat.SelectedIndex = Math.Clamp(SelectedFormatIndex, 0, Math.Max(0, _FormatOptions.Count-1));
		}
		var localDate = DateTimeOffset.FromUnixTimeMilliseconds(_Tempus.Value).ToLocalTime().Date;
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

	void RebuildFormatOptions(){
		_FormatOptions.Clear();
		foreach(var b in BuiltinFormats.Distinct()){
			_FormatOptions.Add(new FormatOption{
				IsBuiltin = true,
				Builtin = b,
				Display = BuiltinToDisplay(b),
			});
		}
		foreach(var pattern in CustomFormatPatterns.Where(x=>!string.IsNullOrWhiteSpace(x)).Distinct()){
			_FormatOptions.Add(new FormatOption{
				IsBuiltin = false,
				Pattern = pattern.Trim(),
				Display = pattern.Trim(),
			});
		}
		if(_FormatOptions.Count == 0){
			_FormatOptions.Add(new FormatOption{
				IsBuiltin = true,
				Builtin = EBuiltinFormat.Iso,
				Display = BuiltinToDisplay(EBuiltinFormat.Iso),
			});
		}
		_SelectedFormatIndex = Math.Clamp(_SelectedFormatIndex, 0, _FormatOptions.Count-1);
	}

	static str BuiltinToDisplay(EBuiltinFormat Format){
		return Format switch{
			EBuiltinFormat.Iso => "ISO 8601",
			EBuiltinFormat.UnixMs => "Unix ms",
			_ => "Unknown",
		};
	}

	str FormatBySelectedFormat(Tempus Value){
		if(_FormatOptions.Count == 0){
			return Value.Value.ToString(CultureInfo.InvariantCulture);
		}
		var opt = _FormatOptions[Math.Clamp(SelectedFormatIndex, 0, _FormatOptions.Count-1)];
		if(opt.IsBuiltin){
			return opt.Builtin switch{
				// ISO 8601 顯示按本地時區輸出。
				EBuiltinFormat.Iso => DateTimeOffset.FromUnixTimeMilliseconds(Value.Value)
					.ToLocalTime()
					.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture),
				EBuiltinFormat.UnixMs => Value.Value.ToString(CultureInfo.InvariantCulture),
				_ => Value.Value.ToString(CultureInfo.InvariantCulture),
			};
		}
		try{
			var local = DateTimeOffset.FromUnixTimeMilliseconds(Value.Value).ToLocalTime().DateTime;
			return local.ToString(NormalizePattern(opt.Pattern ?? ""), CultureInfo.InvariantCulture);
		}catch{
			return DateTimeOffset.FromUnixTimeMilliseconds(Value.Value)
				.ToLocalTime()
				.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
		}
	}

	bool TryParseBySelectedFormat(str Text, out Tempus Result){
		Result = default;
		if(string.IsNullOrWhiteSpace(Text)){
			return false;
		}
		if(_FormatOptions.Count == 0){
			RebuildFormatOptions();
		}
		var opt = _FormatOptions[Math.Clamp(SelectedFormatIndex, 0, _FormatOptions.Count-1)];
		if(opt.IsBuiltin){
			switch(opt.Builtin){
				case EBuiltinFormat.Iso:
					if(DateTimeOffset.TryParseExact(
						Text,
						"yyyy-MM-ddTHH:mm:ss.fffzzz",
						CultureInfo.InvariantCulture,
						DateTimeStyles.None,
						out var dtoIso
					)){
						Result = Tempus.FromUnixMs(dtoIso.ToUnixTimeMilliseconds());
						return true;
					}
					return false;

				case EBuiltinFormat.UnixMs:
				if(i64.TryParse(Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ms)){
					Result = Tempus.FromUnixMs(ms);
					return true;
				}
				return false;
			}
			return false;
		}
		var pattern = NormalizePattern(opt.Pattern ?? "");
		if(DateTime.TryParseExact(
			Text,
			pattern,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out var dtCustom
		)){
			Result = Tempus.FromDateTime(new DateTime(
				dtCustom.Year, dtCustom.Month, dtCustom.Day,
				dtCustom.Hour, dtCustom.Minute, dtCustom.Second,
				dtCustom.Millisecond,
				DateTimeKind.Local
			));
			return true;
		}
		return false;
	}

	static str NormalizePattern(str Pattern){
		// 兼容常見習慣寫法：YY/DD（非 .NET 標準）→ yy/dd。
		return Pattern
		.Replace("YYYY", "yyyy")
		.Replace("YY", "yy")
		.Replace("DD", "dd");
	}
}
