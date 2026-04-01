namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using System.Collections.ObjectModel;
using System.Collections.Generic;
using Ngaq.Ui.Infra;

using Ctx = VmFieldsFilterCardEdit;

public class VmFieldsFilterCardEdit: ViewModelBase, IMk<Ctx>{
	protected VmFieldsFilterCardEdit(){}

	public static Ctx Mk(){
		return new Ctx();
	}

	VmPreFilterEdit? Owner{get;set;}
	VmPreFilterEdit.VmFieldsFilterRow? Target{get;set;}
	bool IsCore{get;set;}
	u64 RowIdx{get;set;}

	public str FieldsText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public ObservableCollection<VmPreFilterEdit.VmFilterItemRow> Items{get;set;} = [];

	public IReadOnlyList<str> OperationOptions => Owner?.OperationOptions ?? [];
	public IReadOnlyList<str> ValueTypeOptions => Owner?.ValueTypeOptions ?? [];

	public nil Load(
		VmPreFilterEdit Owner,
		VmPreFilterEdit.VmFieldsFilterRow Target,
		bool IsCore,
		u64 RowIdx
	){
		this.Owner = Owner;
		this.Target = Target;
		this.IsCore = IsCore;
		this.RowIdx = RowIdx;
		FieldsText = Target.FieldsText;
		Items.Clear();
		foreach(var item in Target.Items){
			Items.Add(CloneItem(item));
		}
		if(Items.Count == 0){
			Items.Add(new VmPreFilterEdit.VmFilterItemRow{
				OperationIndex = 1,
				ValueTypeIndex = 1,
				ValuesText = "",
			});
		}
		OnPropertyChanged(nameof(OperationOptions));
		OnPropertyChanged(nameof(ValueTypeOptions));
		return NIL;
	}

	public nil AddItem(){
		Items.Add(new VmPreFilterEdit.VmFilterItemRow{
			OperationIndex = 1,
			ValueTypeIndex = 1,
			ValuesText = "",
		});
		return NIL;
	}

	public nil RemoveItem(VmPreFilterEdit.VmFilterItemRow Item){
		Items.Remove(Item);
		if(Items.Count == 0){
			Items.Add(new VmPreFilterEdit.VmFilterItemRow{
				OperationIndex = 1,
				ValueTypeIndex = 1,
				ValuesText = "",
			});
		}
		return NIL;
	}

	public nil Save(){
		if(Target is null || Owner is null){
			ShowMsg("Editor not ready");
			return NIL;
		}
		Target.FieldsText = FieldsText;
		Target.Items.Clear();
		foreach(var item in Items){
			Target.Items.Add(CloneItem(item));
		}
		Owner.RefreshFieldsFilterCards();
		ShowMsg($"Saved {(IsCore?"Core":"Prop")} Filter #{RowIdx}");
		ViewNavi?.Back();
		return NIL;
	}

	static VmPreFilterEdit.VmFilterItemRow CloneItem(VmPreFilterEdit.VmFilterItemRow Src){
		return new VmPreFilterEdit.VmFilterItemRow{
			OperationIndex = Src.OperationIndex,
			ValueTypeIndex = Src.ValueTypeIndex,
			ValuesText = Src.ValuesText,
		};
	}
}
