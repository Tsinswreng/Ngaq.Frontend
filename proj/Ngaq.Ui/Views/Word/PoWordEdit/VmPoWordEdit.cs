namespace Ngaq.Ui.Views.Word.PoWordEdit;

using Ngaq.Core.Infra;
using Ngaq.Core.Shared.User.Models.Po;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Core.Tools;
using Ngaq.Ui.Infra;
using Tsinswreng.CsTempus;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// 編輯 PoWord 核心字段（含 Biz 時間與軟刪時間）。
public partial class VmPoWordEdit: ViewModelBase{
	public str WordIdText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str Head{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str Lang{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str StoredAtIso{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str DelAtUnixMs{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str BizCreatedAtIso{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str BizUpdatedAtIso{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public nil LoadFromPo(PoWord Po){
		WordIdText = Po.Id.ToString();
		Head = Po.Head;
		Lang = Po.Lang;
		StoredAtIso = Po.StoredAt.ToIso();
		DelAtUnixMs = Po.DelAt.IsNullOrDefault() ? "" : (Po.DelAt.Value+"");
		BizCreatedAtIso = Po.BizCreatedAt.ToIso();
		BizUpdatedAtIso = Po.BizUpdatedAt.ToIso();
		return NIL;
	}

	public bool TryApplyToPo(PoWord Po, out str Err){
		Err = "";
		if(str.IsNullOrWhiteSpace(Head)){
			Err = I18n[K.HeadIsRequired];
			return false;
		}
		if(str.IsNullOrWhiteSpace(Lang)){
			Err = I18n[K.LangIsRequired];
			return false;
		}
		Po.Head = Head.Trim();
		Po.Lang = Lang.Trim();

		try{
			Po.StoredAt = UnixMs.FromIso(StoredAtIso.Trim());
		}catch{
			Err = I18n[K.StoredAtMustBeIsoTime];
			return false;
		}

		if(str.IsNullOrWhiteSpace(DelAtUnixMs)){
			Po.DelAt = default;
		}else{
			if(!i64.TryParse(DelAtUnixMs.Trim(), out var delMs)){
				Err = I18n[K.DelAtMustBeUnixMilliseconds];
				return false;
			}
			Po.DelAt = IdDel.FromUnixMs(delMs);
		}

		try{
			Po.BizCreatedAt = UnixMs.FromIso(BizCreatedAtIso.Trim());
		}catch{
			Err = I18n[K.BizCreatedAtMustBeIsoTime];
			return false;
		}

		try{
			Po.BizUpdatedAt = UnixMs.FromIso(BizUpdatedAtIso.Trim());
		}catch{
			Err = I18n[K.BizUpdatedAtMustBeIsoTime];
			return false;
		}
		return true;
	}
}
