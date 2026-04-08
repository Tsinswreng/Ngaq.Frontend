namespace Ngaq.Ui.Components.TempusBox;

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Ngaq.Core.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsCore;

public partial class TempusBox: ContentControl{
	/// `Tempus` 的 Avalonia 可绑定属性。默认双向绑定。
	public static readonly DirectProperty<TempusBox, Tempus> TempusProperty =
		AvaloniaProperty.RegisterDirect<TempusBox, Tempus>(
			nameof(Tempus),
			o => o.Tempus,
			(o, v) => o.Tempus = v,
			defaultBindingMode: Avalonia.Data.BindingMode.TwoWay
		);

	/// 当前 `Tempus` 主值。文本输入和日历选择都会回写到此值。
	public partial Tempus Tempus{get;set;}

	/// 是否只读。`true` 时禁用格式切换和日历，文本框进入只读。
	public partial bool IsReadOnly{get;set;}

	/// 全部格式来源。下拉框只使用此列表。
	public partial IList<ITempusFormatItem> FormatItems{get;}

	/// 当前选中的格式下标。
	public partial i32 SelectedFormatIndex{get;set;}

	/// 控件统一高度，让左按钮与输入框外框对齐。
	public partial f64 ControlHeight{get;set;}

	/// 最近一次文本解析是否成功，外层可读取作提示。
	public partial bool LastParseOk{get;protected set;}

	public readonly AutoGrid Root = new(IsRow: false);
	public TextBox _Input{get;set;} = null!;
	public Button _BtnMenu{get;set;} = null!;
	public ComboBox _ComboBoxFormat{get;set;} = null!;
	public Avalonia.Controls.Calendar _Calendar{get;set;} = null!;

	partial void Init();

	public TempusBox(){
		Init();
	}

	/// 外层修改 `FormatItems` 后调用，刷新下拉格式源。
	public partial void RefreshFormatOptions();
}
