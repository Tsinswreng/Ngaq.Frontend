namespace Ngaq.Ui.Views.Word.WordLearnEdit;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Ngaq.Ui.Components.TempusBox;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordLearnPage;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsTempus;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// 單行學習記錄編輯頁。
public partial class ViewWordLearnEdit: AppViewBase{
	public VmWordLearnEdit? Ctx{
		get{return DataContext as VmWordLearnEdit;}
		set{DataContext = value;}
	}

	StackPanel? EditorForm;
	VmWordLearnEdit? SubscribedCtx;

	IReadOnlyList<str> LearnResultOptions => [
		I[K.Learn_Add],
		I[K.Learn_Rmb],
		I[K.Learn_Fgt],
	];

	public ViewWordLearnEdit(){
		Render();
		DataContextChanged += (s, e)=>OnCtxChanged();
	}

	void Render(){
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(8, GUT.Star),
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
		]);
		root.A(new ScrollViewer(), sv=>{
			sv.SetContent(new StackPanel(), sp=>{
				EditorForm = sp;
				sp.Margin = new Thickness(10);
				sp.Spacing = 8;
				sp.A(MkComboRow(I[K.LearnResult], LearnResultOptions, CBE.Mk<VmWordLearnRow>(x=>x.LearnResultIndex, Mode: BindingMode.TwoWay)));
				sp.A(MkTempusRow(I[K.Biz_CreatedAt], CBE.Mk<VmWordLearnRow>(x=>x.BizCreatedAtIso, Mode: BindingMode.TwoWay, Converter: new IsoToTempusConverter())));
			});
		});
		root.A(new Button(), o=>{
			o.Margin = new Thickness(10, 6, 10, 6);
			o.StretchCenter();
			o.Background = UiCfg.Inst.MainColor;
			o.Content = Icons.Save().ToIcon().WithText(I[K.Save]);
			o.Click += (s, e)=>ViewNavi?.Back();
		});
		root.A(new Button(), o=>{
			o.Margin = new Thickness(10, 0, 10, 10);
			o.StretchCenter();
			o.Background = UiCfg.Inst.DelBtnBg;
			o.Content = Icons.Delete().ToIcon().WithText(I[K.Remove]);
			o.Click += (s, e)=>{
				if(Ctx is not null){
					Ctx.OnRemove?.Invoke(Ctx.Row);
				}
				ViewNavi?.Back();
			};
		});
		Content = root.Grid;
	}

	/// 編輯表單直接綁到行 Vm，自避開 `Ctx.Row.Xxx` 這種嵌套綁定在後置注入時失效的問題。
	void OnCtxChanged(){
		if(SubscribedCtx is not null){
			SubscribedCtx.PropertyChanged -= OnEditVmPropertyChanged;
			SubscribedCtx = null;
		}
		if(Ctx is null){
			if(EditorForm is not null){
				EditorForm.DataContext = null;
			}
			return;
		}
		SubscribedCtx = Ctx;
		Ctx.PropertyChanged += OnEditVmPropertyChanged;
		ApplyRowCtx();
	}

	void OnEditVmPropertyChanged(object? Sender, System.ComponentModel.PropertyChangedEventArgs E){
		if(E.PropertyName == nameof(VmWordLearnEdit.Row)){
			ApplyRowCtx();
		}
	}

	void ApplyRowCtx(){
		if(EditorForm is not null){
			EditorForm.DataContext = Ctx?.Row;
		}
	}

	Control MkComboRow(str Label, IEnumerable<str> Items, IBinding Binding){
		var cb = new ComboBox{ItemsSource = Items};
		cb.Bind(ComboBox.SelectedIndexProperty, Binding);
		return MkFieldRow(Label, cb);
	}

	Control MkTempusRow(str Label, IBinding Binding){
		var tb = new TempusBox();
		tb.Bind(TempusBox.TempusProperty, Binding);
		tb.FormatItems.Add(TempusFormatItem.yy_MM_DD);
		tb.FormatItems.Add(TempusFormatItem.yy_MM_DD__HH_mm);
		tb.SelectedFormat = TempusFormatItem.yy_MM_DD__HH_mm;
		return MkFieldRow(Label, tb);
	}

	Control MkFieldRow(str Label, Control Input){
		var sp = new StackPanel{Orientation = Orientation.Vertical, Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		sp.Children.Add(Input);
		return sp;
	}

	sealed class IsoToTempusConverter: IValueConverter{
		public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture){
			if(value is str s && !str.IsNullOrWhiteSpace(s)){
				try{return UnixMs.FromIso(s.Trim());}catch{}
			}
			return UnixMs.Now();
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture){
			if(value is UnixMs t){
				return t.ToIso();
			}
			return "";
		}
	}
}
