namespace Ngaq.Ui.Views.Word.WordEditV2;

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

using Ctx = VmWordEditV2;
using Ngaq.Core.Shared.Word.Models.Po.Word;
public partial class VmWordLearnRow: ViewModelBase {
	public PoWordLearn Raw { get; set; } = new();

	public str LearnResultText {
		get { return field; }
		set { SetProperty(ref field, value); }
	} = nameof(ELearn.Add);

	public str BizCreatedAtIso {
		get { return field; }
		set { SetProperty(ref field, value); }
	} = Tempus.Now().ToIso();

	public static VmWordLearnRow NewRow() {
		return new VmWordLearnRow();
	}

	public static VmWordLearnRow FromPo(PoWordLearn Po) {
		var vm = new VmWordLearnRow {
			Raw = (PoWordLearn)Po.ShallowCloneSelf(),
			LearnResultText = Po.LearnResult.ToString(),
			BizCreatedAtIso = Po.BizCreatedAt.ToIso()
		};
		return vm;
	}

	public bool TryToPo(IdWord WordId, out PoWordLearn Po, out str Err) {
		Err = "";
		Po = (PoWordLearn)Raw.ShallowCloneSelf();
		Po.WordId = WordId;

		if (!Enum.TryParse<ELearn>(LearnResultText, true, out var learn)) {
			Err = $"Invalid learn result: {LearnResultText}";
			return false;
		}
		Po.LearnResult = learn;

		try {
			Po.BizCreatedAt = Tempus.FromIso(BizCreatedAtIso);
		} catch {
			Err = "BizCreatedAt must be ISO time.";
			return false;
		}
		return true;
	}
}
