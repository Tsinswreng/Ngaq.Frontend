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

public partial class VmWordEditV2: ViewModelBase, IMk<Ctx> {
	protected VmWordEditV2() {
		InitRowEvents();
	}
	public static Ctx Mk() {
		return new Ctx();
	}

	ISvcWord? SvcWord;
	IJsonSerializer? JsonSerializer;
	IFrontendUserCtxMgr? UserCtxMgr;

	public VmWordEditV2(
		ISvcWord? SvcWord
		,IJsonSerializer? JsonSerializer
		,IFrontendUserCtxMgr? UserCtxMgr
	) {
		this.SvcWord = SvcWord;
		this.JsonSerializer = JsonSerializer;
		this.UserCtxMgr = UserCtxMgr;
		InitRowEvents();
	}


	bool _isHydrating = false;

	public enum ETabIdx {
		Basic = 0,
		Props = 1,
		Learns = 2,
		Json = 3
	}

	void InitRowEvents() {
		PropRows.CollectionChanged += OnPropRowsChanged;
		LearnRows.CollectionChanged += OnLearnRowsChanged;
	}

	void OnPropRowsChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		if (e.NewItems != null) {
			foreach (var item in e.NewItems.OfType<VmWordPropRow>()) {
				item.PropertyChanged += OnChildRowChanged;
			}
		}
		if (e.OldItems != null) {
			foreach (var item in e.OldItems.OfType<VmWordPropRow>()) {
				item.PropertyChanged -= OnChildRowChanged;
			}
		}
		Touch();
	}

	void OnLearnRowsChanged(object? sender, NotifyCollectionChangedEventArgs e) {
		if (e.NewItems != null) {
			foreach (var item in e.NewItems.OfType<VmWordLearnRow>()) {
				item.PropertyChanged += OnChildRowChanged;
			}
		}
		if (e.OldItems != null) {
			foreach (var item in e.OldItems.OfType<VmWordLearnRow>()) {
				item.PropertyChanged -= OnChildRowChanged;
			}
		}
		Touch();
	}

	void OnChildRowChanged(object? sender, PropertyChangedEventArgs e) {
		Touch();
	}

	void Touch() {
		if (_isHydrating) {
			return;
		}
		IsDirty = true;
	}

	public IJnWord? Src {
		get { return field; }
		set { SetProperty(ref field, value); }
	}

	public JnWord? Draft {
		get { return field; }
		set { SetProperty(ref field, value); }
	}

	public i32 TabIndex {
		get { return field; }
		set {
			if (SetProperty(ref field, value)) {
				Touch();
			}
		}
	} = (i32)ETabIdx.Basic;

	public str Head {
		get { return field; }
		set {
			if (SetProperty(ref field, value)) {
				Touch();
			}
		}
	} = "";

	public str Lang {
		get { return field; }
		set {
			if (SetProperty(ref field, value)) {
				Touch();
			}
		}
	} = "";

	public str StoredAtIso {
		get { return field; }
		set {
			if (SetProperty(ref field, value)) {
				Touch();
			}
		}
	} = "";

	public str DelAtUnixMs {
		get { return field; }
		set {
			if (SetProperty(ref field, value)) {
				Touch();
			}
		}
	} = "";

	public str OwnerText {
		get { return field; }
		set { SetProperty(ref field, value); }
	} = "";

	public str WordIdText {
		get { return field; }
		set { SetProperty(ref field, value); }
	} = "";

	public ObservableCollection<VmWordPropRow> PropRows {
		get { return field; }
		set { SetProperty(ref field, value); }
	} = [];

	public ObservableCollection<VmWordLearnRow> LearnRows {
		get { return field; }
		set { SetProperty(ref field, value); }
	} = [];

	public str JsonText {
		get { return field; }
		set {
			if (SetProperty(ref field, value)) {
				Touch();
			}
		}
	} = "";

	public str LastError {
		get { return field; }
		set { SetProperty(ref field, value); }
	} = "";

	public bool IsDirty {
		get { return field; }
		set { SetProperty(ref field, value); }
	}

	public bool HasError => !str.IsNullOrWhiteSpace(LastError);

	public nil FromJnWord(IJnWord JnWord) {
		Src = JnWord;
		Draft = JnWord.DeepClone().AsOrToJnWord();
		LoadFromDraft();
		return NIL;
	}

	void LoadFromDraft() {
		if (Draft is null) {
			return;
		}
		_isHydrating = true;
		try {
			Head = Draft.Word.Head;
			Lang = Draft.Word.Lang;
			StoredAtIso = Draft.Word.StoredAt.ToIso();
			DelAtUnixMs = Draft.Word.DelAt.IsNullOrDefault() ? "" : (Draft.Word.DelAt.Value + "");
			OwnerText = Draft.Word.Owner.ToString();
			WordIdText = Draft.Word.Id.ToString();
			PropRows = new ObservableCollection<VmWordPropRow>(
				Draft.Props.Select(x => VmWordPropRow.FromPo(x))
			);
			LearnRows = new ObservableCollection<VmWordLearnRow>(
				Draft.Learns.Select(x => VmWordLearnRow.FromPo(x))
			);

			InitRowEvents();
			SyncJsonFromDraft();
			LastError = "";
			IsDirty = false;
			OnPropertyChanged(nameof(HasError));
		} finally {
			_isHydrating = false;
		}
	}

	public nil AddPropRow() {
		PropRows.Add(VmWordPropRow.NewRow());
		return NIL;
	}

	public nil RemovePropRow(VmWordPropRow Row) {
		PropRows.Remove(Row);
		return NIL;
	}

	public nil AddLearnRow() {
		LearnRows.Add(VmWordLearnRow.NewRow());
		return NIL;
	}

	public nil RemoveLearnRow(VmWordLearnRow Row) {
		LearnRows.Remove(Row);
		return NIL;
	}

	public nil ResetFromSource() {
		if (Src is null) {
			return NIL;
		}
		Draft = Src.DeepClone().AsOrToJnWord();
		LoadFromDraft();
		return NIL;
	}

	public nil SyncJsonFromDraft() {
		if (!TryApplyFormToDraft(out var err)) {
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			return NIL;
		}
		if (Draft is null) {
			return NIL;
		}
		var json = JsonSerializer?.Stringify(Draft) ?? "";
		JsonText = FormatJson(json);
		LastError = "";
		OnPropertyChanged(nameof(HasError));
		IsDirty = false;
		return NIL;
	}

	public nil ApplyJsonToForm() {
		if (JsonSerializer is null) {
			LastError = "No JsonSerializer";
			OnPropertyChanged(nameof(HasError));
			return NIL;
		}
		try {
			var neo = JsonSerializer.Parse<JnWord>(JsonText);
			if (neo is null) {
				LastError = "Json parse failed.";
				OnPropertyChanged(nameof(HasError));
				return NIL;
			}
			Draft = neo;
			LoadFromDraft();
			IsDirty = true;
		} catch (Exception ex) {
			LastError = ex.Message;
			OnPropertyChanged(nameof(HasError));
		}
		return NIL;
	}

	public async Task<nil> Save(CT Ct) {
		if (SvcWord is null || UserCtxMgr is null) {
			ShowMsg("Service not ready");
			return NIL;
		}
		if (!TryApplyFormToDraft(out var err)) {
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowMsg(err);
			return NIL;
		}
		if (Draft is null) {
			ShowMsg("No draft");
			return NIL;
		}
		try {
			Draft.EnsureForeignId();
			await SvcWord.UpdJnWord(UserCtxMgr.GetUserCtx(), Draft, Ct);
			Src = Draft.DeepClone().AsOrToJnWord();
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			IsDirty = false;
			ShowMsg("Saved");
		} catch (Exception ex) {
			LastError = ex.Message;
			OnPropertyChanged(nameof(HasError));
			HandleErr(ex);
		}
		return NIL;
	}

	bool TryApplyFormToDraft(out str Err) {
		Err = "";
		if (Draft is null) {
			Err = "No draft";
			return false;
		}
		if (str.IsNullOrWhiteSpace(Head)) {
			Err = "Head is required.";
			return false;
		}
		if (str.IsNullOrWhiteSpace(Lang)) {
			Err = "Lang is required.";
			return false;
		}

		Draft.Word.Head = Head.Trim();
		Draft.Word.Lang = Lang.Trim();

		try {
			Draft.Word.StoredAt = Tempus.FromIso(StoredAtIso.Trim());
		} catch {
			Err = "StoredAt must be ISO time.";
			return false;
		}

		if (str.IsNullOrWhiteSpace(DelAtUnixMs)) {
			Draft.Word.DelAt = new IdDel();
		} else {
			if (!long.TryParse(DelAtUnixMs.Trim(), out var delMs)) {
				Err = "DelAt must be Unix milliseconds.";
				return false;
			}
			Draft.Word.DelAt = IdDel.FromUnixMs(delMs);
		}

		var propErrs = new List<str>();
		var nextProps = new List<PoWordProp>();
		for (var i = 0; i < PropRows.Count; i++) {
			var row = PropRows[i];
			if (!row.TryToPo(Draft.Word.Id, out var po, out var rowErr)) {
				propErrs.Add($"Prop#{i + 1}: {rowErr}");
				continue;
			}
			nextProps.Add(po);
		}
		if (propErrs.Count > 0) {
			Err = string.Join("\n", propErrs);
			return false;
		}

		var learnErrs = new List<str>();
		var nextLearns = new List<PoWordLearn>();
		for (var i = 0; i < LearnRows.Count; i++) {
			var row = LearnRows[i];
			if (!row.TryToPo(Draft.Word.Id, out var po, out var rowErr)) {
				learnErrs.Add($"Learn#{i + 1}: {rowErr}");
				continue;
			}
			nextLearns.Add(po);
		}
		if (learnErrs.Count > 0) {
			Err = string.Join("\n", learnErrs);
			return false;
		}

		Draft.Props = nextProps;
		Draft.Learns = nextLearns;
		return true;
	}

	static str FormatJson(str UglyJson) {
		if (str.IsNullOrWhiteSpace(UglyJson)) {
			return "";
		}
		try {
			JsonNode? node = JsonNode.Parse(UglyJson);
			if (node is null) {
				return UglyJson;
			}
			return node.ToJsonString(new JsonSerializerOptions {
				WriteIndented = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			});
		} catch {
			return UglyJson;
		}
	}
}


