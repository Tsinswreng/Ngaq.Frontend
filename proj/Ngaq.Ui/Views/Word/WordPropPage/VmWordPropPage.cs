namespace Ngaq.Ui.Views.Word.WordPropPage;

using System;
using System.Collections.ObjectModel;
using Ngaq.Core.Infra;
using Ngaq.Core.Model.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// 屬性分頁 ViewModel：管理列表、新增、轉換。
public partial class VmWordPropPage: ViewModelBase{
	public event Action<VmWordPropRow>? OnEditRequested;

	public ObservableCollection<VmWordPropRow> Rows{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = [];

	/// 已標記刪除的行，保存時走 Del，成功後清空。
	public List<VmWordPropRow> RemovedRows{get;} = [];

	/// 新增的行（DmlState == Added），保存時走 BatAdd。
	public IEnumerable<VmWordPropRow> AddedRows => Rows.Where(r => r.DmlState == EDmlState.Added);

	/// 已修改的行（DmlState == Modified），保存時走 BatUpd。
	public IEnumerable<VmWordPropRow> ModifiedRows => Rows.Where(r => r.DmlState == EDmlState.Modified);

	public nil LoadFromPoProps(IList<PoWordProp> Props){
		Rows = new ObservableCollection<VmWordPropRow>(Props.Select(VmWordPropRow.FromPo));
		return NIL;
	}

	public nil AddRow(){
		Rows.Add(VmWordPropRow.NewRow());
		return NIL;
	}

	/// 新增行直接移除；已存在的行標記爲 Removed，保存時再刪。
	public nil RemoveRow(VmWordPropRow Row){
		if(Row.DmlState == EDmlState.Added){
			Rows.Remove(Row);
		}else{
			Row.DmlState = EDmlState.Removed;
			Rows.Remove(Row);
			RemovedRows.Add(Row);
		}
		return NIL;
	}

	/// 已直接調後端刪除成功的行，只從當前列表摘除，不再進 `RemovedRows` 等待總保存。
	public nil RemovePersistedRow(VmWordPropRow Row){
		Rows.Remove(Row);
		RemovedRows.Remove(Row);
		return NIL;
	}

	public nil RequestEdit(VmWordPropRow Row){
		OnEditRequested?.Invoke(Row);
		return NIL;
	}

	/// 保存成功後重置所有行狀態。
	public nil OnSaved(){
		foreach(var row in Rows){
			row.DmlState = EDmlState.Unchanged;
		}
		RemovedRows.Clear();
		return NIL;
	}

	public bool TryBuildPoProps(IdWord WordId, out List<PoWordProp> Props, out str Err){
		Props = [];
		Err = "";
		for(i32 i = 0; i < Rows.Count; i++){
			var row = Rows[i];
			if(!row.TryToPo(WordId, out var po, out var rowErr)){
				Err = I18n.Get(K.Prop__Err__, i+1, rowErr);
				return false;
			}
			Props.Add(po);
		}
		return true;
	}
}

/// 單詞屬性行編輯 ViewModel。
public partial class VmWordPropRow: ViewModelBase{
	/// `Descr` 只是編輯頁裏的默認/顯示別名，落庫仍統一回 `description`。
	public const str DescriptionAlias = "Descr";

	public static IReadOnlyList<EKvType> KvTypes{get;} = [
		EKvType.Str,
		EKvType.I64,
	];

	public PoWordProp Raw{get;set;} = new();

	/// 行記錄 DML 狀態：決定保存時走 BatAdd/BatUpd/Del。
	public EDmlState DmlState{get;set;}

	public i32 KTypeIndex{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				MarkModified();
			}
		}
	} = 0;

	public str KStrText{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				MarkModified();
			}
		}
	} = "";

	public str KI64Text{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				MarkModified();
			}
		}
	} = "0";

	public i32 VTypeIndex{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				MarkModified();
			}
		}
	} = 0;

	public str VStrText{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				MarkModified();
			}
		}
	} = "";

	public str VI64Text{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				MarkModified();
			}
		}
	} = "0";

	/// 行記錄 Id（僅供顯示，只讀）。
	public str IdText => Raw.Id.ToString();

	public str KeyText => GetKvTypeByIndex(KTypeIndex) == EKvType.I64 ? KI64Text : KStrText;
	public str KeyDisplayText => TranslatePropKey(KeyText);
	public str ValueDisplayText => BuildValueDisplayText();

	/// 只有 Unchanged → Modified 才切換，保持 Added / Removed 不被意外覆蓋。
	void MarkModified(){
		if(DmlState == EDmlState.Unchanged){
			DmlState = EDmlState.Modified;
		}
	}

	public static VmWordPropRow NewRow(){
		return new VmWordPropRow{
			Raw = new PoWordProp{
				Id = new IdWordProp(),
			},
			KTypeIndex = 0,
			VTypeIndex = 0,
			KStrText = DescriptionAlias,
			VStrText = "",
			KI64Text = "0",
			VI64Text = "0",
			// 放最後，覆蓋屬性初始化時觸發的 MarkModified
			DmlState = EDmlState.Added,
		};
	}

	public static VmWordPropRow FromPo(PoWordProp Po){
		return new VmWordPropRow{
			Raw = (PoWordProp)Po.ShallowCloneSelf(),
			KTypeIndex = GetKvTypeIndex(Po.KType),
			VTypeIndex = GetKvTypeIndex(Po.VType),
			KStrText = ToEditorKeyText(Po.KStr),
			KI64Text = Po.KI64 + "",
			VStrText = Po.VStr ?? "",
			VI64Text = Po.VI64 + "",
			// 放最後，覆蓋屬性初始化時觸發的 MarkModified
			DmlState = EDmlState.Unchanged,
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
			Po.KStr = ToStoredKeyText(KStrText);
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

	/// 將數據庫中的內置鍵轉成更貼近編輯語境的展示文本。
	static str ToEditorKeyText(str? StoredKey){
		if(StoredKey == KeysProp.Inst.description){
			return DescriptionAlias;
		}
		return StoredKey ?? "";
	}

	/// 將編輯頁別名規整回內置鍵，避免破壞既有數據語義。
	static str ToStoredKeyText(str? EditorKey){
		if(str.Equals(EditorKey?.Trim(), DescriptionAlias, StringComparison.OrdinalIgnoreCase)){
			return KeysProp.Inst.description;
		}
		return EditorKey?.Trim() ?? "";
	}

	/// 內置 prop 鍵在 UI 中顯示譯文，非內置鍵保持原樣。
	public str TranslatePropKey(str? Key){
		return Key switch{
			DescriptionAlias => I18n[K.Descr],
			var x when x == KeysProp.Inst.summary => I18n[K.Summary],
			var x when x == KeysProp.Inst.description => I18n[K.Descr],
			var x when x == KeysProp.Inst.note => I18n[K.Note],
			var x when x == KeysProp.Inst.tag => I18n[K.Tag],
			var x when x == KeysProp.Inst.source => I18n[K.Source],
			var x when x == KeysProp.Inst.alias => I18n[K.Alias],
			var x when x == KeysProp.Inst.pronunciation => I18n[K.Pronunciation],
			var x when x == KeysProp.Inst.weight => I18n[K.Weight],
			var x when x == KeysProp.Inst.learn => I18n[K.Learn],
			var x when x == KeysProp.Inst.usage => I18n[K.Usage],
			var x when x == KeysProp.Inst.example => I18n[K.Example],
			var x when x == KeysProp.Inst.relation => I18n[K.Relation],
			var x when x == KeysProp.Inst.Ref => I18n[K.Ref],
			_ => Key ?? "",
		};
	}

	public str TranslateKvType(EKvType Type){
		return Type switch{
			EKvType.Str => I18n[K.KvTypeStr],
			EKvType.I64 => I18n[K.KvTypeI64],
			_ => Type.ToString(),
		};
	}

	/// 属性表只显示实际值；长文本统一走公用略缩格式。
	str BuildValueDisplayText(){
		var raw = GetValueRawText();
		return ToolText.FormatCompactText(raw, HeadLen: 12, TailLen: 8, EmptyText: "");
	}

	str GetValueRawText(){
		var vt = GetKvTypeByIndex(VTypeIndex);
		if(vt == EKvType.Str){
			return VStrText ?? "";
		}
		if(vt == EKvType.I64){
			return VI64Text ?? "";
		}
		if(!str.IsNullOrWhiteSpace(VStrText)){
			return VStrText;
		}
		if(!str.IsNullOrWhiteSpace(VI64Text)){
			return VI64Text;
		}
		if(Raw.VBinary is not null && Raw.VBinary.Length > 0){
			return $"<binary:{Raw.VBinary.Length}>";
		}
		if(Raw.VF64 != 0){
			return Raw.VF64.ToString();
		}
		return "";
	}
}
