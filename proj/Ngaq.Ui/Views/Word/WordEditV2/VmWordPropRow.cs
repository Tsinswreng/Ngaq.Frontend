namespace Ngaq.Ui.Views.Word.WordEditV2;

using Ngaq.Core.Infra;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Ui.Infra;`r`nusing K = Ngaq.Ui.Infra.I18n.KeysUiI18n.WordEditV2;

/// 單詞屬性行編輯 ViewModel。
/// 約束：當前 UI 只允許 Str / I64 兩種鍵值類型。
public partial class VmWordPropRow: ViewModelBase{
	public static IReadOnlyList<EKvType> KvTypes{get;} = [
		EKvType.Str,
		EKvType.I64,
	];

	public PoWordProp Raw{get;set;} = new();

	public i32 KTypeIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = 0;

	/// Key 下拉/可編輯輸入（字符串鍵）。
	public str KStrText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	/// 整數鍵文本（KType=I64 時使用）。
	public str KI64Text{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "0";

	public i32 VTypeIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = 0;

	/// 字符串值（VType=Str 時使用）。
	public str VStrText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	/// 整數值文本（VType=I64 時使用）。
	public str VI64Text{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "0";

	public str KeyText => GetKvTypeByIndex(KTypeIndex) == EKvType.I64 ? KI64Text : KStrText;
	public str ValueText => GetKvTypeByIndex(VTypeIndex) == EKvType.I64 ? VI64Text : VStrText;
	public str KTypeText => GetKvTypeByIndex(KTypeIndex).ToString();
	public str VTypeText => GetKvTypeByIndex(VTypeIndex).ToString();

	public static VmWordPropRow NewRow(){
		return new VmWordPropRow{
			KTypeIndex = 0,
			VTypeIndex = 0,
			KStrText = "",
			VStrText = "",
			KI64Text = "0",
			VI64Text = "0",
		};
	}

	public static VmWordPropRow FromPo(PoWordProp Po){
		return new VmWordPropRow{
			Raw = (PoWordProp)Po.ShallowCloneSelf(),
			KTypeIndex = GetKvTypeIndex(Po.KType),
			VTypeIndex = GetKvTypeIndex(Po.VType),
			KStrText = Po.KStr ?? "",
			KI64Text = Po.KI64 + "",
			VStrText = Po.VStr ?? "",
			VI64Text = Po.VI64 + "",
		};
	}

	public bool TryToPo(IdWord WordId, out PoWordProp Po, out str Err){
		Err = "";
		Po = (PoWordProp)Raw.ShallowCloneSelf();
		Po.WordId = WordId;

		var kt = GetKvTypeByIndex(KTypeIndex);
		var vt = GetKvTypeByIndex(VTypeIndex);
		Po.KType = kt;
		Po.VType = vt;

		if(kt == EKvType.Str){
			Po.KStr = KStrText;
			Po.KI64 = 0;
		}else if(kt == EKvType.I64){
			if(!i64.TryParse(KI64Text, out var kI64)){
				Err = I18n[K.InvalidKI64];
				return false;
			}
			Po.KI64 = kI64;
			Po.KStr = null;
		}else{
			Err = I18n[K.InvalidKType];
			return false;
		}

		if(vt == EKvType.Str){
			Po.VStr = VStrText;
			Po.VI64 = 0;
		}else if(vt == EKvType.I64){
			if(!i64.TryParse(VI64Text, out var vI64)){
				Err = I18n[K.InvalidVI64];
				return false;
			}
			Po.VI64 = vI64;
			Po.VStr = null;
		}else{
			Err = I18n[K.InvalidVType];
			return false;
		}

		Po.VF64 = 0;
		Po.VBinary = null;
		return true;
	}

	static i32 GetKvTypeIndex(EKvType Type){
		for(i32 i = 0; i < KvTypes.Count; i++){
			if(KvTypes[i] == Type){
				return i;
			}
		}
		return 0;
	}

	static EKvType GetKvTypeByIndex(i32 Index){
		if(Index < 0 || Index >= KvTypes.Count){
			return EKvType.Str;
		}
		return KvTypes[Index];
	}
}

