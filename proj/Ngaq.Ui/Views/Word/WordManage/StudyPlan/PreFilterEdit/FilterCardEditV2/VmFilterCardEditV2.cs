namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.FilterCardEditV2;

using System.Collections.Generic;
using System.Linq;
using Ngaq.Core.Shared.StudyPlan.Models.PreFilter;
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
	protected VmFilterCardEditV2(){}
	public static Ctx Mk(){ return new Ctx(); }

	VmPreFilterVisualEditV2? Owner{get;set;}
	VmPreFilterVisualEditV2.VmFieldsFilterRow? Target{get;set;}
	u64 RowIdx{get;set;}

	public i32 OperationIndex{get=>field;set{SetProperty(ref field, value);}} = (i32)EFilterOperationMode.Eq;
	public i32 ValueTypeIndex{get=>field;set{SetProperty(ref field, value);}} = (i32)EValueType.String;
	public str Field{get=>field;set{SetProperty(ref field, value);}} = "";
	public str ValuesText{get=>field;set{SetProperty(ref field, value);}} = "";

	public IReadOnlyList<str> OperationOptions{get;} = Enum.GetNames<EFilterOperationMode>();
	public IReadOnlyList<str> ValueTypeOptions{get;} = Enum.GetNames<EValueType>();

	public nil Load(VmPreFilterVisualEditV2 Owner, VmPreFilterVisualEditV2.VmFieldsFilterRow Target, u64 RowIdx){
		this.Owner = Owner;
		this.Target = Target;
		this.RowIdx = RowIdx;

		Field = Target.Fields.FirstOrDefault()?.Value ?? "";
		var item = Target.Items.FirstOrDefault();
		OperationIndex = item?.OperationIndex ?? (i32)EFilterOperationMode.Eq;
		ValueTypeIndex = item?.ValueTypeIndex ?? (i32)EValueType.String;
		ValuesText = item?.ValuesText ?? "";
		return NIL;
	}

	public nil Save(){
		if(Owner is null || Target is null){
			ShowDialog(I18n[K.EditorNotReady]);
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
				OperationIndex = OperationIndex,
				ValueTypeIndex = ValueTypeIndex,
				ValuesText = ValuesText,
			});
		}

		Owner.RefreshFieldsFilterCards();
		ShowDialog(I18n.Get(K.Saved__Filter__No__, I18n[K.PreFilter], RowIdx));
		ViewNavi?.Back();
		return NIL;
	}

	public nil Delete(){
		if(Owner is null || Target is null){
			ShowDialog(I18n[K.EditorNotReady]);
			return NIL;
		}
		Owner.FilterRows.Remove(Target);
		Owner.RefreshFieldsFilterCards();
		ShowToast(I18n[K.Deleted]);
		ViewNavi?.Back();
		return NIL;
	}
}

