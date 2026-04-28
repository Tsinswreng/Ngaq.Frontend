namespace Ngaq.Ui.Views.Word.PoWordEdit;

using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Ngaq.Core.Infra;
using Ngaq.Ui.Components.TempusBox;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsTempus;
using Ctx = VmPoWordEdit;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// PoWord 基本信息編輯頁。
public partial class ViewPoWordEdit: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	static readonly IValueConverter IsoConverter = new IsoToTempusConverter();
	static readonly IValueConverter DelAtConverter = new DelAtUnixMsToTempusConverter();

	public ViewPoWordEdit(){
		Render();
	}

	void Render(){
		var sv = new ScrollViewer();
		sv.SetContent(new StackPanel(), sp=>{
			sp.Margin = new Thickness(10);
			sp.Spacing = 8;
			sp.A(MkIdSelectableRow(I[K.WordId], CBE.Mk<Ctx>(x=>x.WordIdText, Mode: BindingMode.OneWay)));
			sp.A(MkInputRow(I[K.Head], CBE.Mk<Ctx>(x=>x.Head, Mode: BindingMode.TwoWay)));
			sp.A(MkInputRow(I[K.Lang], CBE.Mk<Ctx>(x=>x.Lang, Mode: BindingMode.TwoWay)));
			sp.A(MkTempusRow(I[K.StoredAt], CBE.Mk<Ctx>(x=>x.StoredAtIso, Mode: BindingMode.TwoWay, Converter: IsoConverter)));
			sp.A(MkTempusRow(I[K.Biz_CreatedAt], CBE.Mk<Ctx>(x=>x.BizCreatedAtIso, Mode: BindingMode.TwoWay, Converter: IsoConverter)));
			sp.A(MkTempusRow(I[K.Biz_UpdatedAt], CBE.Mk<Ctx>(x=>x.BizUpdatedAtIso, Mode: BindingMode.TwoWay, Converter: IsoConverter)));
			sp.A(MkTempusRow(I[K.SoftDeleteTime], CBE.Mk<Ctx>(x=>x.DelAtUnixMs, Mode: BindingMode.TwoWay, Converter: DelAtConverter)));
		});
		Content = sv;
	}

	Control MkTempusRow(str Label, IBinding Binding){
		var tb = new TempusBox();
		tb.Bind(TempusBox.TempusProperty, Binding);
		tb.FormatItems.Add(TempusFormatItem.yy_MM_DD);
		tb.FormatItems.Add(TempusFormatItem.yy_MM_DD__HH_mm);
		tb.SelectedFormat = TempusFormatItem.yy_MM_DD__HH_mm;
		return MkFieldRow(Label, tb);
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

	Control MkFieldRow(str Label, Control Input){
		var sp = new StackPanel{Orientation = Orientation.Vertical, Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		sp.Children.Add(Input);
		return sp;
	}

	sealed class IsoToTempusConverter: IValueConverter{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture){
			if(value is str s && !str.IsNullOrWhiteSpace(s)){
				try{return UnixMs.FromIso(s.Trim());}catch{}
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
			if(value is str s
				&& i64.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var unixMs)
				&& unixMs > 0){
				return UnixMs.FromUnixMs(unixMs);
			}
			return null;
		}
		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture){
			if(value is UnixMs t){
				if(t.Value <= 0){
					return "";
				}
				return t.Value.ToString(CultureInfo.InvariantCulture);
			}
			return "";
		}
	}
}
