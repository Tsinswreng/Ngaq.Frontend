namespace Ngaq.Ui.Views.Word.WordPropEdit;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Ngaq.Core.Model.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordPropPage;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// 單行屬性編輯頁。
public partial class ViewWordPropEdit: AppViewBase{
	public VmWordPropEdit? Ctx{
		get{return DataContext as VmWordPropEdit;}
		set{DataContext = value;}
	}

	StackPanel? EditorForm;
	VmWordPropEdit? SubscribedCtx;

	IReadOnlyList<str> KvTypeOptions => [
		I[K.KvTypeStr],
		I[K.KvTypeI64],
	];

	IReadOnlyList<str> PropKeyDisplayOptions => [
		I[K.Descr],
		I[K.Summary],
		I[K.Note],
		I[K.Tag],
		I[K.Source],
		I[K.Alias],
		I[K.Pronunciation],
		I[K.Weight],
		I[K.Learn],
		I[K.Usage],
		I[K.Example],
		I[K.Relation],
		I[K.Ref],
	];

	public ViewWordPropEdit(){
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
				sp.A(MkComboRow(I[K.KType], KvTypeOptions, CBE.Mk<VmWordPropRow>(x=>x.KTypeIndex, Mode: BindingMode.TwoWay)));
				sp.A(MkEditableComboRow(
					I[K.KeyStr],
					PropKeyDisplayOptions,
					CBE.Mk<VmWordPropRow>(
						x=>x.KStrText,
						Mode: BindingMode.TwoWay,
						Converter: new PropKeyDisplayConverter(this)
					)
				));
				sp.A(MkInputRow(I[K.KeyI64], CBE.Mk<VmWordPropRow>(x=>x.KI64Text, Mode: BindingMode.TwoWay)));
				sp.A(MkComboRow(I[K.VType], KvTypeOptions, CBE.Mk<VmWordPropRow>(x=>x.VTypeIndex, Mode: BindingMode.TwoWay)));
				sp.A(MkInputRow(I[K.VStr], CBE.Mk<VmWordPropRow>(x=>x.VStrText, Mode: BindingMode.TwoWay)));
				sp.A(MkInputRow(I[K.VI64], CBE.Mk<VmWordPropRow>(x=>x.VI64Text, Mode: BindingMode.TwoWay)));
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

	/// 與 LearnEdit 同理，表單直接綁行 Vm，避免嵌套綁定丟失初值。
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
		if(E.PropertyName == nameof(VmWordPropEdit.Row)){
			ApplyRowCtx();
		}
	}

	void ApplyRowCtx(){
		if(EditorForm is not null){
			EditorForm.DataContext = Ctx?.Row;
		}
	}

	Control MkInputRow(str Label, IBinding Binding){
		var tb = new TextBox();
		tb.Bind(TextBox.TextProperty, Binding);
		return MkFieldRow(Label, tb);
	}

	Control MkComboRow(str Label, IEnumerable<str> Items, IBinding Binding){
		var cb = new ComboBox{ItemsSource = Items};
		cb.Bind(ComboBox.SelectedIndexProperty, Binding);
		return MkFieldRow(Label, cb);
	}

	Control MkEditableComboRow(str Label, IEnumerable<str> Items, IBinding Binding){
		var cb = new ComboBox{IsEditable = true, ItemsSource = Items};
		cb.Bind(ComboBox.TextProperty, Binding);
		return MkFieldRow(Label, cb);
	}

	Control MkFieldRow(str Label, Control Input){
		var sp = new StackPanel{Orientation = Orientation.Vertical, Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		sp.Children.Add(Input);
		return sp;
	}

	sealed class PropKeyDisplayConverter: IValueConverter{
		readonly ViewWordPropEdit Host;
		public PropKeyDisplayConverter(ViewWordPropEdit Host){
			this.Host = Host;
		}

		public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture){
			if(value is not str raw){
				return "";
			}
			return Host.ToDisplayPropKey(raw);
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture){
			if(value is not str display){
				return "";
			}
			return Host.ToStoredPropKey(display);
		}
	}

	str ToDisplayPropKey(str RawKey){
		return RawKey switch{
			var x when str.Equals(x, VmWordPropRow.DescriptionAlias, StringComparison.OrdinalIgnoreCase) => I[K.Descr],
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
			_ => RawKey,
		};
	}

	str ToStoredPropKey(str DisplayKey){
		var trimmed = DisplayKey.Trim();
		if(trimmed == I[K.Descr]){
			return VmWordPropRow.DescriptionAlias;
		}
		if(trimmed == I[K.Summary]){
			return KeysProp.Inst.summary;
		}
		if(trimmed == I[K.Note]){
			return KeysProp.Inst.note;
		}
		if(trimmed == I[K.Tag]){
			return KeysProp.Inst.tag;
		}
		if(trimmed == I[K.Source]){
			return KeysProp.Inst.source;
		}
		if(trimmed == I[K.Alias]){
			return KeysProp.Inst.alias;
		}
		if(trimmed == I[K.Pronunciation]){
			return KeysProp.Inst.pronunciation;
		}
		if(trimmed == I[K.Weight]){
			return KeysProp.Inst.weight;
		}
		if(trimmed == I[K.Learn]){
			return KeysProp.Inst.learn;
		}
		if(trimmed == I[K.Usage]){
			return KeysProp.Inst.usage;
		}
		if(trimmed == I[K.Example]){
			return KeysProp.Inst.example;
		}
		if(trimmed == I[K.Relation]){
			return KeysProp.Inst.relation;
		}
		if(trimmed == I[K.Ref]){
			return KeysProp.Inst.Ref;
		}
		return DisplayKey;
	}
}
