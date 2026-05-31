namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.FilterCardEditV2;

using System;
using System.Collections.Generic;
using System.Linq;
using Ngaq.Core.Shared.StudyPlan.Models.PreFilter;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.PreFilterVisualEditV2;

using Ctx = VmFilterCardEditV2;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// <summary>
/// 單個篩選卡片 V2 編輯 VM。
/// 規則：字段僅 1 個、篩選項僅 1 個，且全部在同一頁編輯。
/// </summary>
public class VmFilterCardEditV2: ViewModelBase, IMk<Ctx>{
	public VmFilterCardEditV2(){}
	public static Ctx Mk(){ return new Ctx(); }

	VmPreFilterVisualEditV2? Owner{get;set;}
	VmPreFilterVisualEditV2.VmFieldsFilterRow? Target{get;set;}
	u64 RowIdx{get;set;}
	public event Action? OnBackRequested;
	public event Action<str>? OnDialogRequested;
	public event Action<str>? OnToastRequested;
	static readonly i32 DefaultOperationRawIndex = (i32)EFilterOperationMode.Eq;
	static readonly i32 DefaultValueTypeRawIndex = (i32)EValueType.String;

	public class RowTextOption{
		public str Raw{get;set;} = "";
		public str Display{get;set;} = "";
	}

	public str SelectedOperation{
		get=>field;
		set{SetProperty(ref field, value);}
	} = "";

	public str SelectedValueType{
		get=>field;
		set{SetProperty(ref field, value);}
	} = "";

	public str Field{get=>field;set{SetProperty(ref field, value);}} = "";
	public str ValuesText{get=>field;set{SetProperty(ref field, value);}} = "";

	public IReadOnlyList<str> FieldOptionsRaw{get;} = VmPreFilterVisualEditV2.DefaultFieldOptions;
	public IReadOnlyList<RowTextOption> FieldOptionRows => FieldOptionsRaw.Select(x=>new RowTextOption{
		Raw = x,
		Display = ToFieldDisplay(x),
	}).ToList();

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

	public nil Load(VmPreFilterVisualEditV2 Owner, VmPreFilterVisualEditV2.VmFieldsFilterRow Target, u64 RowIdx){
		this.Owner = Owner;
		this.Target = Target;
		this.RowIdx = RowIdx;

		Field = Target.Fields.FirstOrDefault()?.Value ?? "";
		var item = Target.Items.FirstOrDefault();
		SelectedOperation = ToOperationDisplayByRawIndex(item?.OperationIndex ?? DefaultOperationRawIndex);
		SelectedValueType = ToValueTypeDisplayByRawIndex(item?.ValueTypeIndex ?? DefaultValueTypeRawIndex);
		ValuesText = item?.ValuesText ?? "";
		OnPropertyChanged(nameof(FieldOptionRows));
		OnPropertyChanged(nameof(OperationOptionsDisplay));
		OnPropertyChanged(nameof(ValueTypeOptionsDisplay));
		OnPropertyChanged(nameof(SelectedOperation));
		OnPropertyChanged(nameof(SelectedValueType));
		return NIL;
	}

	public nil Save(){
		if(Owner is null || Target is null){
			OnDialogRequested?.Invoke(I18n[K.EditorNotReady]);
			return NIL;
		}

		// step 1: 僅保留 1 個字段。
		Target.Fields.Clear();
		var field = Field?.Trim() ?? "";
		if(!str.IsNullOrWhiteSpace(field)){
			Target.Fields.Add(new VmPreFilterVisualEditV2.VmFieldValueRow{Value = field});
		}

		// step 2: 僅保留 1 個篩選項。
		Target.Items.Clear();
		if(!str.IsNullOrWhiteSpace(ValuesText)){
			Target.Items.Add(new VmPreFilterVisualEditV2.VmFilterItemRow{
				OperationIndex = OperationDisplayToRawIndex(SelectedOperation),
				ValueTypeIndex = ValueTypeDisplayToRawIndex(SelectedValueType),
				ValuesText = ValuesText,
			});
		}

		Owner.RefreshFieldsFilterCards();
		OnDialogRequested?.Invoke(I18n.Get(K.Saved__Filter__No__, I18n[K.PreFilter], RowIdx));
		OnBackRequested?.Invoke();
		return NIL;
	}

	public nil Delete(){
		if(Owner is null || Target is null){
			OnDialogRequested?.Invoke(I18n[K.EditorNotReady]);
			return NIL;
		}
		Owner.FilterRows.Remove(Target);
		Owner.RefreshFieldsFilterCards();
		OnToastRequested?.Invoke(I18n[K.Deleted]);
		OnBackRequested?.Invoke();
		return NIL;
	}

	public str ToFieldDisplay(str raw){
		if(str.IsNullOrWhiteSpace(raw)){
			return "";
		}
		return raw switch{
			nameof(PoWord.Head) => I18n[K.Head],
			nameof(PoWord.Lang) => I18n[K.Lang],
			nameof(PoWord.BizCreatedAt) => I18n[K.Biz_CreatedAt],
			nameof(PoWord.BizUpdatedAt) => I18n[K.Biz_UpdatedAt],
			"summary" => I18n[K.Summary],
			"description" => I18n[K.Description],
			"note" => I18n[K.Note],
			"tag" => I18n[K.Tag],
			"source" => I18n[K.Source],
			"alias" => I18n[K.Alias],
			"pronunciation" => I18n[K.Pronunciation],
			"weight" => I18n[K.Weight],
			"learn" => I18n[K.Learn],
			"usage" => I18n[K.Usage],
			"example" => I18n[K.Example],
			"relation" => I18n[K.Relation],
			"ref" => I18n[K.Ref],
			_ => raw,
		};
	}

	public str ToFieldRaw(str? displayOrRaw){
		var text = displayOrRaw?.Trim() ?? "";
		if(str.IsNullOrWhiteSpace(text)){
			return "";
		}
		foreach(var option in FieldOptionRows){
			if(option.Display == text || option.Raw == text){
				return option.Raw;
			}
		}
		return text;
	}

	public i32 OperationDisplayToRawIndex(str? display){
		var text = display?.Trim() ?? "";
		for(i32 i = 0; i < OperationRawIndices.Count; i++){
			var raw = OperationRawIndices[i];
			if(ToOperationDisplayByRawIndex(raw) == text){
				return raw;
			}
		}
		return DefaultOperationRawIndex;
	}

	public i32 ValueTypeDisplayToRawIndex(str? display){
		var text = display?.Trim() ?? "";
		for(i32 i = 0; i < ValueTypeRawIndices.Count; i++){
			var raw = ValueTypeRawIndices[i];
			if(ToValueTypeDisplayByRawIndex(raw) == text){
				return raw;
			}
		}
		return DefaultValueTypeRawIndex;
	}

	str ToOperationDisplayByRawIndex(i32 rawIndex){
		var mode = EnumByRawIndex<EFilterOperationMode>(rawIndex);
		return mode switch{
			EFilterOperationMode.Eq => "=",
			EFilterOperationMode.Ne => "!=",
			EFilterOperationMode.Gt => ">",
			EFilterOperationMode.Ge => ">=",
			EFilterOperationMode.Lt => "<",
			EFilterOperationMode.Le => "<=",
			EFilterOperationMode.IncludeAny => I18n[K.IncludeAny],
			EFilterOperationMode.IncludeAll => I18n[K.IncludeAll],
			EFilterOperationMode.ExcludeAll => I18n[K.ExcludeAll],
			_ => mode.ToString(),
		};
	}

	str ToValueTypeDisplayByRawIndex(i32 rawIndex){
		var type = EnumByRawIndex<EValueType>(rawIndex);
		return type switch{
			EValueType.String => I18n[K.String],
			EValueType.Number => I18n[K.Number],
			EValueType.Null => I18n[K.Null],
			_ => type.ToString(),
		};
	}

	static TEnum EnumByRawIndex<TEnum>(i32 rawIndex) where TEnum : struct, Enum{
		var values = Enum.GetValues<TEnum>();
		if(rawIndex < 0 || rawIndex >= values.Length){
			return values[0];
		}
		return values[rawIndex];
	}
}

