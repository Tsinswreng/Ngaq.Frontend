namespace Ngaq.Ui.Views.Word.WordEditV2.WordPropEdit;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Ngaq.Core.Model.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordEditV2.WordPropPage;
using Tsinswreng.Avln.Grid;
using Tsinswreng.Avln.Dsl;
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

	public partial ViewWordPropEdit();

	/// 依既定版面與綁定狀態建立此頁的控制項樹。
	partial void Render();

	/// 與 LearnEdit 同理，表單直接綁行 Vm，避免嵌套綁定丟失初值。
	partial void OnCtxChanged();

	/// 處理 EditVmPropertyChanged 事件，維持頁面狀態與資料來源同步。
	partial void OnEditVmPropertyChanged(object? Sender, System.ComponentModel.PropertyChangedEventArgs E);

	/// 執行 ApplyRowCtx 所代表的內部協調操作。
	partial void ApplyRowCtx();

	/// 建立 IdSelectableRow 所需的 UI 組件，供頁面組裝時重用。
	private partial Control MkIdSelectableRow(str Label, IBinding Binding);

	/// 建立 InputRow 所需的 UI 組件，供頁面組裝時重用。
	private partial Control MkInputRow(str Label, IBinding Binding);

	/// 建立 ComboRow 所需的 UI 組件，供頁面組裝時重用。
	private partial Control MkComboRow(str Label, IEnumerable<str> Items, IBinding Binding);

	/// 建立 EditableComboRow 所需的 UI 組件，供頁面組裝時重用。
	private partial Control MkEditableComboRow(str Label, IEnumerable<str> Items, IBinding Binding);

	/// 建立 FieldRow 所需的 UI 組件，供頁面組裝時重用。
	private partial Control MkFieldRow(str Label, Control Input);


	/// 將編輯值轉換為儲存或顯示所需的標準格式。
	private partial str ToDisplayPropKey(str RawKey);

	/// 將編輯值轉換為儲存或顯示所需的標準格式。
	private partial str ToStoredPropKey(str DisplayKey);
}
