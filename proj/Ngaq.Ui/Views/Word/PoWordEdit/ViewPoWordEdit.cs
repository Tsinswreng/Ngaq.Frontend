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
using Tsinswreng.Avln.Dsl;
using Tsinswreng.CsTempus;
using Ctx = VmPoWordEdit;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// PoWord 基本信息編輯頁。
public partial class ViewPoWordEdit
	: AppViewBase<Ctx>
	, IViewPoWordEdit
{
	public SelectableTextBlock? IdCtrl{get;set;}
	public TextBox? HeadCtrl{get;set;}
	public TextBox? LangCtrl{get;set;}
	public TempusBox? StoredAtCtrl{get;set;}
	public TempusBox? BizCreatedAtCtrl{get;set;}
	public TempusBox? BizUpdatedAtCtrl{get;set;}
	public TempusBox? DelAtCtrl{get;set;}

	static readonly IValueConverter IsoConverter = new IsoToTempusConverter();
	static readonly IValueConverter DelAtConverter = new DelAtUnixMsToTempusConverter();

	public ViewPoWordEdit(){
		Render();
	}

	[Impl]
	public str? Id{
		get{return IdCtrl?.Text;}
	}

	[Impl]
	public str? Head{
		get{return HeadCtrl?.Text;}
		set{
			if(HeadCtrl is not null){
				HeadCtrl.Text = value;
			}
		}
	}

	[Impl]
	public str? Lang{
		get{return LangCtrl?.Text;}
		set{
			if(LangCtrl is not null){
				LangCtrl.Text = value;
			}
		}
	}

	[Impl]
	public str? StoredAt{
		get{return StoredAtCtrl?.Tempus?.ToIso() ?? "";}
		set{
			if(StoredAtCtrl is not null){
				StoredAtCtrl.Tempus = ParseIso(value);
			}
		}
	}

	[Impl]
	public str? BizCreatedAt{
		get{return BizCreatedAtCtrl?.Tempus?.ToIso() ?? "";}
		set{
			if(BizCreatedAtCtrl is not null){
				BizCreatedAtCtrl.Tempus = ParseIso(value);
			}
		}
	}

	[Impl]
	public str? BizUpdatedAt{
		get{return BizUpdatedAtCtrl?.Tempus?.ToIso() ?? "";}
		set{
			if(BizUpdatedAtCtrl is not null){
				BizUpdatedAtCtrl.Tempus = ParseIso(value);
			}
		}
	}

	[Impl]
	public str? DelAt{
		get{
			var tempus = DelAtCtrl?.Tempus;
			if(tempus is null){
				return "";
			}
			return tempus.Value.Value.ToString(CultureInfo.InvariantCulture);
		}
		set{
			if(DelAtCtrl is not null){
				DelAtCtrl.Tempus = ParseUnixMs(value);
			}
		}
	}

	void Render(){
		var sv = new ScrollViewer();
		sv.SetContent(new StackPanel(), sp=>{
			sp.Margin = new(10);
			sp.Spacing = 8;
			sp.A(MkIdSelectableRow(I[K.WordId], CBE.Mk<Ctx>(x=>x.WordIdText, Mode: BindingMode.OneWay), o=>{
				IdCtrl = o;
			}));
			sp.A(MkInputRow(I[K.Head], CBE.Mk<Ctx>(x=>x.Head, Mode: BindingMode.TwoWay), o=>{
				HeadCtrl = o;
			}));
			sp.A(MkInputRow(I[K.Lang], CBE.Mk<Ctx>(x=>x.Lang, Mode: BindingMode.TwoWay), o=>{
				LangCtrl = o;
			}));
			sp.A(MkTempusRow(I[K.StoredAt], CBE.Mk<Ctx>(x=>x.StoredAtIso, Mode: BindingMode.TwoWay, Converter: IsoConverter), o=>{
				StoredAtCtrl = o;
			}));
			sp.A(MkTempusRow(I[K.Biz_CreatedAt], CBE.Mk<Ctx>(x=>x.BizCreatedAtIso, Mode: BindingMode.TwoWay, Converter: IsoConverter), o=>{
				BizCreatedAtCtrl = o;
			}));
			sp.A(MkTempusRow(I[K.Biz_UpdatedAt], CBE.Mk<Ctx>(x=>x.BizUpdatedAtIso, Mode: BindingMode.TwoWay, Converter: IsoConverter), o=>{
				BizUpdatedAtCtrl = o;
			}));
			sp.A(MkTempusRow(I[K.SoftDeleteTime], CBE.Mk<Ctx>(x=>x.DelAtUnixMs, Mode: BindingMode.TwoWay, Converter: DelAtConverter), o=>{
				DelAtCtrl = o;
			}));
		});
		this.SetContent(sv);
	}

	Control MkTempusRow(str Label, IBinding Binding){
		var tb = new TempusBox();
		tb.Bind(TempusBox.TempusProperty, Binding);
		tb.FormatItems.Add(TempusFormatItem.yy_MM_DD);
		tb.FormatItems.Add(TempusFormatItem.yy_MM_DD__HH_mm);
		tb.SelectedFormat = TempusFormatItem.yy_MM_DD__HH_mm;
		return MkFieldRow(Label, tb);
	}

	Control MkTempusRow(str Label, IBinding Binding, Action<TempusBox> Init){
		var tb = new TempusBox();
		Init(tb);
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

	Control MkInputRow(str Label, IBinding Binding, Action<TextBox> Init){
		var tb = new TextBox();
		Init(tb);
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

	Control MkIdSelectableRow(str Label, IBinding Binding, Action<SelectableTextBlock> Init){
		var tb = new SelectableTextBlock{
			FontSize = UiCfg.Inst.BaseFontSize*0.8,
			TextWrapping = TextWrapping.Wrap,
		};
		Init(tb);
		tb.Bind(TextBlock.TextProperty, Binding);
		return MkFieldRow(Label, tb);
	}

	Control MkFieldRow(str Label, Control Input){
		var sp = new StackPanel{Orientation = Orientation.Vertical, Spacing = 3};
		sp.A(new TextBlock(), o=>{
			o.Text = Label;
		}).A(Input);
		return sp;
	}

	UnixMs? ParseIso(str? value){
		if(str.IsNullOrWhiteSpace(value)){
			return null;
		}
		try{
			return UnixMs.FromIso(value.Trim());
		}catch{
			return null;
		}
	}

	UnixMs? ParseUnixMs(str? value){
		if(str.IsNullOrWhiteSpace(value)){
			return null;
		}
		if(i64.TryParse(value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var unixMs) && unixMs > 0){
			return UnixMs.FromUnixMs(unixMs);
		}
		return null;
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

