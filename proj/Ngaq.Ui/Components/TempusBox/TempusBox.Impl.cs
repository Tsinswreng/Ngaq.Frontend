namespace Ngaq.Ui.Components.TempusBox;

using Avalonia;
using Avalonia.Data;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Globalization;
using System.Linq;
using Ngaq.Core.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Ngaq.Ui.Icons;

public partial class TempusBox: ContentControl{
	Tempus _Tempus = Tempus.Now();
	bool _IsReadOnly = false;
	i32 _SelectedFormatIndex = 0;
	f64 _ControlHeight = 34;
	
	readonly IList<ITempusFormatItem> _FormatItems = [
		TempusFormatItem.Iso8601Full,
		TempusFormatItem.UnixMs,
	];
	bool _LastParseOk = true;

	public partial Tempus Tempus{
		get{return _Tempus;}
		set{
			SetAndRaise(TempusProperty, ref _Tempus, value);
			SyncUiFromTempus();
		}
	}

	public partial bool IsReadOnly{
		get{return _IsReadOnly;}
		set{
			_IsReadOnly = value;
			ApplyReadOnlyState();
		}
	}

	public partial IList<ITempusFormatItem> FormatItems{
		get{return _FormatItems;}
	}

	public partial i32 SelectedFormatIndex{
		get{return _SelectedFormatIndex;}
		set{
			var max = FormatItems.Count - 1;
			var next = max < 0 ? 0 : Math.Clamp(value, 0, max);
			_SelectedFormatIndex = next;
			SyncUiFromTempus();
		}
	}

	public partial f64 ControlHeight{
		get{return _ControlHeight;}
		set{
			_ControlHeight = value;
			ApplyControlSize();
		}
	}

	public partial bool LastParseOk{
		get{return _LastParseOk;}
		protected set{_LastParseOk = value;}
	}

	Flyout? _MenuFlyout;
	bool _SyncingUi = false;

	partial void Init(){
		Render();
		ApplyControlSize();
		ApplyReadOnlyState();
		SyncUiFromTempus();
	}

	public partial void RefreshFormatOptions(){
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
		o.VerticalContentAlignment = VAlign.Center;
		o.Margin = new Thickness(0);
		o.BorderThickness = new Thickness(1);
		//o.Padding = new Thickness(8, 0, 8, 0);
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
