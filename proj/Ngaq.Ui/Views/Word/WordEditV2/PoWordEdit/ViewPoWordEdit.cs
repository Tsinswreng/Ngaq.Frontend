namespace Ngaq.Ui.Views.Word.WordEditV2.PoWordEdit;

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
{
	public SelectableTextBlock? IdCtrl{get;set;}
	public TextBox? HeadCtrl{get;set;}
	public TextBox? LangCtrl{get;set;}
	public TempusBox? StoredAtCtrl{get;set;}
	public TempusBox? BizCreatedAtCtrl{get;set;}
	public TempusBox? BizUpdatedAtCtrl{get;set;}
	public TempusBox? DelAtCtrl{get;set;}

	/// 將 ISO 字串轉為可由 TempusBox 編輯的時間值。
	static readonly IValueConverter IsoConverter = new IsoToTempusConverter();
	/// 將可空的軟刪除毫秒值轉為可編輯的時間值。
	static readonly IValueConverter DelAtConverter = new DelAtUnixMsToTempusConverter();

	/// 建構後立即建立控件樹；資料內容由宿主後置注入的 Ctx 提供。
	public partial ViewPoWordEdit();

	
	/// 建立可捲動的基本資料表單，避免小尺寸視窗裁切時間欄位。
	partial void Render();

	/// 建立 TempusRow 所需的 UI 組件，供頁面組裝時重用。
	private partial Control MkTempusRow(str Label, IBinding Binding);

	/// 建立 TempusRow 所需的 UI 組件，供頁面組裝時重用。
	private partial Control MkTempusRow(str Label, IBinding Binding, Action<TempusBox> Init);

	/// 建立 InputRow 所需的 UI 組件，供頁面組裝時重用。
	private partial Control MkInputRow(str Label, IBinding Binding);

	/// 建立 InputRow 所需的 UI 組件，供頁面組裝時重用。
	private partial Control MkInputRow(str Label, IBinding Binding, Action<TextBox> Init);

	/// 建立 IdSelectableRow 所需的 UI 組件，供頁面組裝時重用。
	private partial Control MkIdSelectableRow(str Label, IBinding Binding);

	/// 建立 IdSelectableRow 所需的 UI 組件，供頁面組裝時重用。
	private partial Control MkIdSelectableRow(str Label, IBinding Binding, Action<SelectableTextBlock> Init);

	/// 以標籤在上、輸入控件在下的垂直結構統一所有字段布局。
	private partial Control MkFieldRow(str Label, Control Input);

	/// 解析輸入字串；無法解析時回傳空值以避免傳遞無效資料。
	private partial UnixMs? ParseIso(str? value);

	/// 解析輸入字串；無法解析時回傳空值以避免傳遞無效資料。
	private partial UnixMs? ParseUnixMs(str? value);


}
