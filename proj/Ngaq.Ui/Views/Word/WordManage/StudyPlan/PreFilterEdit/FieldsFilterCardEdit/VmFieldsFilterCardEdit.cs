namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.FieldsFilterCardEdit;

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Ngaq.Core.Shared.StudyPlan.Models.PreFilter;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.FieldsFilterCardEdit.FilterItemEdit;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.PreFilterVisualEdit;

using Ctx = VmFieldsFilterCardEdit;using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// <summary>
/// Editor VM for one FieldsFilter group.
/// It separates field editing and filter-item editing.
/// </summary>
public class VmFieldsFilterCardEdit: ViewModelBase, IMk<Ctx>{
	protected VmFieldsFilterCardEdit(){}

	public static Ctx Mk(){
		return new Ctx();
	}

	VmPreFilterVisualEdit? Owner{get;set;}
	VmPreFilterVisualEdit.VmFieldsFilterRow? Target{get;set;}
	bool IsCore{get;set;}
	u64 RowIdx{get;set;}

	public class RowFilterItemCard{
		public u64 UiIdx{get;set;}
		public str UiIdxText{get;set;} = "";
		public str Operation{get;set;} = "";
		public str ValueType{get;set;} = "";
		public str ValuesPreview{get;set;} = "";
		public VmPreFilterVisualEdit.VmFilterItemRow? Raw{get;set;}
	}

	public ObservableCollection<VmPreFilterVisualEdit.VmFieldValueRow> Fields{get;set;} = [];
	public ObservableCollection<VmPreFilterVisualEdit.VmFilterItemRow> Items{get;set;} = [];
	public ObservableCollection<RowFilterItemCard> ItemCards{get;set;} = [];

	public IReadOnlyList<str> FieldOptions => IsCore
		? VmPreFilterVisualEdit.CoreWordFieldOptions
		: Owner?.PropFieldOptions ?? [];

	public IReadOnlyList<i32> OperationRawIndices{get;} = Enum
		.GetValues<EFilterOperationMode>()
		.Select((x, idx)=>(x, idx))
		.Where(x=>x.x != EFilterOperationMode.Null)
		.Select(x=>(i32)x.idx)
		.ToList();
	public IReadOnlyList<str> OperationOptionsDisplay => OperationRawIndices
		.Select(ToOperationDisplayByRawIndex)
		.ToList();

	public IReadOnlyList<i32> ValueTypeRawIndices{get;} = Enum
		.GetValues<EValueType>()
		.Select((x, idx)=>(x, idx))
		.Where(x=>x.x != EValueType.Null)
		.Select(x=>(i32)x.idx)
		.ToList();
	public IReadOnlyList<str> ValueTypeOptionsDisplay => ValueTypeRawIndices
		.Select(ToValueTypeDisplayByRawIndex)
		.ToList();

	public nil Load(
		VmPreFilterVisualEdit Owner,
		VmPreFilterVisualEdit.VmFieldsFilterRow Target,
		bool IsCore,
		u64 RowIdx
	){
		this.Owner = Owner;
		this.Target = Target;
		this.IsCore = IsCore;
		this.RowIdx = RowIdx;

		Fields.Clear();
		foreach(var field in Target.Fields){
			Fields.Add(new VmPreFilterVisualEdit.VmFieldValueRow{
				Value = field.Value,
			});
		}

		Items.Clear();
		foreach(var item in Target.Items){
			Items.Add(CloneItem(item));
		}

		RefreshItemCards();
		OnPropertyChanged(nameof(FieldOptions));
		OnPropertyChanged(nameof(OperationOptionsDisplay));
		OnPropertyChanged(nameof(ValueTypeOptionsDisplay));
		return NIL;
	}

	public nil AddField(){
		Fields.Add(new VmPreFilterVisualEdit.VmFieldValueRow());
		return NIL;
	}

	public nil RemoveField(VmPreFilterVisualEdit.VmFieldValueRow Field){
		Fields.Remove(Field);
		return NIL;
	}

	public nil AddItem(){
		Items.Add(VmPreFilterVisualEdit.MkFilterItemRow());
		CommitItemsDraft();
		return NIL;
	}

	public nil RefreshItemCards(){
		ItemCards.Clear();
		for(u64 i = 0; i < (u64)Items.Count; i++){
			var item = Items[(i32)i];
			ItemCards.Add(new RowFilterItemCard{
				UiIdx = i + 1,
				UiIdxText = (i + 1).ToString(),
				Operation = ToOperationDisplayByRawIndex(item.OperationIndex),
				ValueType = ToValueTypeDisplayByRawIndex(item.ValueTypeIndex),
				ValuesPreview = ValuesPreview(item.ValuesText),
				Raw = item,
			});
		}
		return NIL;
	}

	public nil OpenFilterItem(RowFilterItemCard? Card){
		if(Card?.Raw is null){
			return NIL;
		}
		var view = new ViewFilterItemEdit();
		view.Ctx?.Load(this, Card.Raw, Card.UiIdx);
		ViewNavi?.GoTo(ToolView.WithTitle($"Filter Item #{Card.UiIdx}", view));
		return NIL;
	}

	public nil RemoveItem(VmPreFilterVisualEdit.VmFilterItemRow? Item){
		if(Item is null){
			return NIL;
		}
		Items.Remove(Item);
		CommitItemsDraft();
		return NIL;
	}

	public nil CommitItemsDraft(){
		RefreshItemCards();
		if(Target is null || Owner is null){
			return NIL;
		}
		Target.Items.Clear();
		foreach(var item in Items){
			Target.Items.Add(CloneItem(item));
		}
		Owner.RefreshFieldsFilterCards();
		return NIL;
	}

	public nil Save(){
		if(Target is null || Owner is null){
			ShowDialog(I18n[K.EditorNotReady]);
			return NIL;
		}

		Target.Fields.Clear();
		foreach(var field in Fields){
			Target.Fields.Add(new VmPreFilterVisualEdit.VmFieldValueRow{
				Value = field.Value,
			});
		}

		Target.Items.Clear();
		foreach(var item in Items){
			Target.Items.Add(CloneItem(item));
		}

		Owner.RefreshFieldsFilterCards();
		var kind = IsCore ? I18n[K.Core] : I18n[K.Prop];
		ShowDialog(I18n.Get(K.Saved__Filter__No__, kind, RowIdx));
		ViewNavi?.Back();
		return NIL;
	}

	public nil Delete(){
		if(Target is null || Owner is null){
			ShowDialog(I18n[K.EditorNotReady]);
			return NIL;
		}
		if(IsCore){
			Owner.CoreFilterRows.Remove(Target);
		}else{
			Owner.PropFilterRows.Remove(Target);
		}
		Owner.RefreshFieldsFilterCards();
		ShowToast(I18n[K.Deleted]);
		ViewNavi?.Back();
		return NIL;
	}

	public i32 ToOperationOptionIndex(i32 rawIndex){
		for(i32 i = 0; i < OperationRawIndices.Count; i++){
			if(OperationRawIndices[i] == rawIndex){
				return i;
			}
		}
		return 0;
	}

	public i32 ToOperationRawIndex(i32 optionIndex){
		if(OperationRawIndices.Count == 0){
			return 1;
		}
		var i = ClampIndex(optionIndex, OperationRawIndices.Count);
		return OperationRawIndices[i];
	}

	public i32 ToValueTypeOptionIndex(i32 rawIndex){
		for(i32 i = 0; i < ValueTypeRawIndices.Count; i++){
			if(ValueTypeRawIndices[i] == rawIndex){
				return i;
			}
		}
		return 0;
	}

	public i32 ToValueTypeRawIndex(i32 optionIndex){
		if(ValueTypeRawIndices.Count == 0){
			return 1;
		}
		var i = ClampIndex(optionIndex, ValueTypeRawIndices.Count);
		return ValueTypeRawIndices[i];
	}

	str ToOperationDisplayByRawIndex(i32 rawIndex){
		var mode = EnumByRawIndex<EFilterOperationMode>(rawIndex);
		return mode switch{
			EFilterOperationMode.Eq => "＝",
			EFilterOperationMode.Ne => "≠",
			EFilterOperationMode.Gt => "＞",
			EFilterOperationMode.Ge => "≥",
			EFilterOperationMode.Lt => "＜",
			EFilterOperationMode.Le => "≤",
			EFilterOperationMode.IncludeAny => I18nOrSelf("Include Any"),
			EFilterOperationMode.IncludeAll => I18nOrSelf("Include All"),
			EFilterOperationMode.ExcludeAll => I18nOrSelf("Exclude All"),
			_ => I18nOrSelf(mode.ToString()),
		};
	}

	str ToValueTypeDisplayByRawIndex(i32 rawIndex){
		var type = EnumByRawIndex<EValueType>(rawIndex);
		return type switch{
			EValueType.String => I18nOrSelf("String"),
			EValueType.Number => I18nOrSelf("Number"),
			EValueType.Null => I18nOrSelf("Null"),
			_ => I18nOrSelf(type.ToString()),
		};
	}

	static str I18nOrSelf(str text){
		var localized = text;
		return str.IsNullOrWhiteSpace(localized) ? text : localized;
	}

	static TEnum EnumByRawIndex<TEnum>(i32 rawIndex)
		where TEnum : struct, Enum
	{
		var values = Enum.GetValues<TEnum>();
		if(rawIndex < 0 || rawIndex >= values.Length){
			return values[0];
		}
		return values[rawIndex];
	}

	static i32 ClampIndex(i32 value, i32 count){
		if(count <= 0){
			return 0;
		}
		if(value < 0){
			return 0;
		}
		if(value >= count){
			return count - 1;
		}
		return value;
	}

	static str ValuesPreview(str text){
		if(str.IsNullOrWhiteSpace(text)){
			return "";
		}
		const i32 maxLen = 80;
		var oneLine = text.Replace("\r", " ").Replace("\n", " | ");
		return oneLine.Length <= maxLen ? oneLine : oneLine[..maxLen] + "...";
	}

	static VmPreFilterVisualEdit.VmFilterItemRow CloneItem(VmPreFilterVisualEdit.VmFilterItemRow Src){
		return new VmPreFilterVisualEdit.VmFilterItemRow{
			OperationIndex = Src.OperationIndex,
			ValueTypeIndex = Src.ValueTypeIndex,
			ValuesText = Src.ValuesText,
		};
	}
}



