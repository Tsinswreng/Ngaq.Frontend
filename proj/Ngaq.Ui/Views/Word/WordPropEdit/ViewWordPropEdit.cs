namespace Ngaq.Ui.Views.Word.WordPropEdit;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
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
				sp.A(MkEditableComboRow(I[K.KeyStr], GetPropKeyOptions(), CBE.Mk<VmWordPropRow>(x=>x.KStrText, Mode: BindingMode.TwoWay)));
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

	IReadOnlyList<str> GetPropKeyOptions(){
		return [
			VmWordPropRow.DescriptionAlias,
			KeysProp.Inst.summary,
			KeysProp.Inst.note,
			KeysProp.Inst.tag,
			KeysProp.Inst.source,
			KeysProp.Inst.alias,
			KeysProp.Inst.pronunciation,
			KeysProp.Inst.weight,
			KeysProp.Inst.learn,
			KeysProp.Inst.usage,
			KeysProp.Inst.example,
			KeysProp.Inst.relation,
			KeysProp.Inst.Ref,
		];
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
}
