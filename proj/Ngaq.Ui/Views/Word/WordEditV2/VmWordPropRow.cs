using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.User.Models.Po;
using Ngaq.Core.Shared.User.UserCtx;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Learn;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Core.Tools;
using Ngaq.Core.Tools.Json;
using Ngaq.Ui.Infra;
using Tsinswreng.CsTools;
using JsonNode = System.Text.Json.Nodes.JsonNode;

using Ctx = VmWordPropRow;
using Ngaq.Core.Shared.Word.Models.Po.Word;
public partial class VmWordPropRow: ViewModelBase {
	public PoWordProp Raw { get; set; } = new();

	public str KTypeText {
		get { return field; }
		set { SetProperty(ref field, value); }
	} = nameof(EKvType.Str);

	public str KeyText {
		get { return field; }
		set { SetProperty(ref field, value); }
	} = "";

	public str VTypeText {
		get { return field; }
		set { SetProperty(ref field, value); }
	} = nameof(EKvType.Str);

	public str ValueText {
		get { return field; }
		set { SetProperty(ref field, value); }
	} = "";

	public static VmWordPropRow NewRow() {
		return new VmWordPropRow();
	}

	public static VmWordPropRow FromPo(PoWordProp Po) {
		var vm = new VmWordPropRow {
			Raw = (PoWordProp)Po.ShallowCloneSelf(),
			KTypeText = Po.KType.ToString(),
			VTypeText = Po.VType.ToString(),
			KeyText = ReadKv(Po.KType, Po.KStr, Po.KI64, 0, null),
			ValueText = ReadKv(Po.VType, Po.VStr, Po.VI64, Po.VF64, Po.VBinary),
		};
		return vm;
	}

	public bool TryToPo(IdWord WordId, out PoWordProp Po, out str Err) {
		Err = "";
		Po = (PoWordProp)Raw.ShallowCloneSelf();
		Po.WordId = WordId;

		if (!Enum.TryParse<EKvType>(KTypeText, true, out var kt)) {
			Err = $"Invalid KType: {KTypeText}";
			return false;
		}
		if (!Enum.TryParse<EKvType>(VTypeText, true, out var vt)) {
			Err = $"Invalid VType: {VTypeText}";
			return false;
		}
		Po.KType = kt;
		Po.VType = vt;

		if (!TryAssignKv(kt, KeyText, out var kStr, out var kI64, out var _, out var _)) {
			Err = "Invalid key value.";
			return false;
		}
		if (!TryAssignKv(vt, ValueText, out var vStr, out var vI64, out var vF64, out var vBinary)) {
			Err = "Invalid prop value.";
			return false;
		}

		Po.KStr = kStr;
		Po.KI64 = kI64;
		Po.VStr = vStr;
		Po.VI64 = vI64;
		Po.VF64 = vF64;
		Po.VBinary = vBinary;
		return true;
	}

	static str ReadKv(EKvType Type, str? S, i64 I, f64 F, u8[]? B) {
		return Type switch {
			EKvType.Str => S ?? "",
			EKvType.I64 => I + "",
			EKvType.F64 => F + "",
			EKvType.Binary => B is null ? "" : Convert.ToBase64String(B),
			_ => ""
		};
	}

	static bool TryAssignKv(
		EKvType Type,
		str RawText,
		out str? S,
		out i64 I,
		out f64 F,
		out u8[]? B
	) {
		S = null;
		I = 0;
		F = 0;
		B = null;
		switch (Type) {
		case EKvType.Str:
			S = RawText;
			return true;
		case EKvType.I64:
			return long.TryParse(RawText, out I);
		case EKvType.F64:
			return double.TryParse(RawText, out F);
		case EKvType.Binary:
			try {
				B = str.IsNullOrWhiteSpace(RawText) ? [] : Convert.FromBase64String(RawText);
				return true;
			} catch {
				return false;
			}
		default:
			return false;
		}
	}
}
