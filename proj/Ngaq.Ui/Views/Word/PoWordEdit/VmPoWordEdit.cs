namespace Ngaq.Ui.Views.Word.PoWordEdit;

using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Ui.Infra;

/// 編輯 PoWord 核心字段（含 Biz 時間與軟刪時間）。
public partial class VmPoWordEdit: ViewModelBase{
	public str WordIdText{get{return field;} set{SetProperty(ref field, value);}} = "";
	public str Head{get{return field;} set{SetProperty(ref field, value);}} = "";
	public str Lang{get{return field;} set{SetProperty(ref field, value);}} = "";
	public str StoredAtIso{get{return field;} set{SetProperty(ref field, value);}} = "";
	public str DelAtUnixMs{get{return field;} set{SetProperty(ref field, value);}} = "";
	public str BizCreatedAtIso{get{return field;} set{SetProperty(ref field, value);}} = "";
	public str BizUpdatedAtIso{get{return field;} set{SetProperty(ref field, value);}} = "";
	public partial nil LoadFromPo(PoWord Po);
	public partial bool TryApplyToPo(PoWord Po, out str Err);
}
