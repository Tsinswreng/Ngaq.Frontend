namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.FieldsFilterCardEdit;

using System.Collections.Generic;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.PreFilterVisualEdit;

using Ctx = VmFilterItemEdit;

/// <summary>
/// Editor VM for a single filter item row.
/// </summary>
public class VmFilterItemEdit: ViewModelBase, IMk<Ctx>{
	protected VmFilterItemEdit(){}

	public static Ctx Mk(){
		return new Ctx();
	}

	VmFieldsFilterCardEdit? Owner{get;set;}
	VmPreFilterVisualEdit.VmFilterItemRow? Target{get;set;}
	u64 ItemIdx{get;set;}

	public IReadOnlyList<str> OperationOptions => Owner?.OperationOptionsDisplay ?? [];
	public IReadOnlyList<str> ValueTypeOptions => Owner?.ValueTypeOptionsDisplay ?? [];

	public i32 SelectedOperationIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = 0;

	public i32 SelectedValueTypeIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = 0;

	public str ValuesText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public nil Load(
		VmFieldsFilterCardEdit Owner,
		VmPreFilterVisualEdit.VmFilterItemRow Target,
		u64 ItemIdx
	){
		this.Owner = Owner;
		this.Target = Target;
		this.ItemIdx = ItemIdx;
		SelectedOperationIndex = Owner.ToOperationOptionIndex(Target.OperationIndex);
		SelectedValueTypeIndex = Owner.ToValueTypeOptionIndex(Target.ValueTypeIndex);
		ValuesText = Target.ValuesText;
		OnPropertyChanged(nameof(OperationOptions));
		OnPropertyChanged(nameof(ValueTypeOptions));
		return NIL;
	}

	public nil Save(){
		if(Target is null || Owner is null){
			ShowMsg(Todo.I18n("Editor not ready"));
			return NIL;
		}
		Target.OperationIndex = Owner.ToOperationRawIndex(SelectedOperationIndex);
		Target.ValueTypeIndex = Owner.ToValueTypeRawIndex(SelectedValueTypeIndex);
		Target.ValuesText = ValuesText;
		Owner.CommitItemsDraft();
		ShowMsg(Todo.I18n($"Saved Filter Item #{ItemIdx}"));
		ViewNavi?.Back();
		return NIL;
	}

	public nil Delete(){
		if(Target is null || Owner is null){
			ShowMsg(Todo.I18n("Editor not ready"));
			return NIL;
		}
		Owner.RemoveItem(Target);
		ShowMsg(Todo.I18n($"Deleted Filter Item #{ItemIdx}"));
		ViewNavi?.Back();
		return NIL;
	}
}
