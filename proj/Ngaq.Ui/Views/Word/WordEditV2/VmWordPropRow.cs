namespace Ngaq.Ui.Views.Word.WordEditV2;

using Ngaq.Core.Infra;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Ui.Infra;

/// 單詞屬性行編輯 ViewModel。
/// 約束：當前 UI 只允許 Str / I64 兩種鍵值類型。
public partial class VmWordPropRow: ViewModelBase{
	public static IReadOnlyList<EKvType> KvTypes{get;} = [
		EKvType.Str,
		EKvType.I64,
	];

	public PoWordProp Raw{get;set;} = new();

	/// Key 類型下拉索引（僅 Str/I64）。
	public i32 KTypeIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = 0;

	/// Key 文本（可手輸，也可從預置鍵下拉選）。
	public str KeyText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	/// Value 類型下拉索引（僅 Str/I64）。
	public i32 VTypeIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = 0;

	/// Value 文本。
	public str ValueText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	/// 概要列表顯示字段。
	public str KTypeText => GetKvTypeByIndex(KTypeIndex).ToString();
	/// 概要列表顯示字段。
	public str VTypeText => GetKvTypeByIndex(VTypeIndex).ToString();

	public static VmWordPropRow NewRow(){
		return new VmWordPropRow{
			KTypeIndex = 0,
			VTypeIndex = 0,
		};
	}

	public static VmWordPropRow FromPo(PoWordProp Po){
		var vm = new VmWordPropRow{
			Raw = (PoWordProp)Po.ShallowCloneSelf(),
			KTypeIndex = GetKvTypeIndex(Po.KType),
			VTypeIndex = GetKvTypeIndex(Po.VType),
			KeyText = ReadKv(Po.KType, Po.KStr, Po.KI64),
			ValueText = ReadKv(Po.VType, Po.VStr, Po.VI64),
		};
		return vm;
	}

	public bool TryToPo(IdWord WordId, out PoWordProp Po, out str Err){
		Err = "";
		Po = (PoWordProp)Raw.ShallowCloneSelf();
		Po.WordId = WordId;

		var kt = GetKvTypeByIndex(KTypeIndex);
		var vt = GetKvTypeByIndex(VTypeIndex);
		Po.KType = kt;
		Po.VType = vt;

		if(!TryAssignKv(kt, KeyText, out var kStr, out var kI64)){
			Err = Todo.I18n("Invalid key value.");
			return false;
		}
		if(!TryAssignKv(vt, ValueText, out var vStr, out var vI64)){
			Err = Todo.I18n("Invalid prop value.");
			return false;
		}

		Po.KStr = kStr;
		Po.KI64 = kI64;
		Po.VStr = vStr;
		Po.VI64 = vI64;
		Po.VF64 = 0;
		Po.VBinary = null;
		return true;
	}

	static str ReadKv(EKvType Type, str? S, i64 I){
		return Type switch{
			EKvType.Str => S ?? "",
			EKvType.I64 => I + "",
			_ => "",
		};
	}

	static bool TryAssignKv(EKvType Type, str RawText, out str? S, out i64 I){
		S = null;
		I = 0;
		switch(Type){
			case EKvType.Str:
				S = RawText;
				return true;
			case EKvType.I64:
				return i64.TryParse(RawText, out I);
			default:
				return false;
		}
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
