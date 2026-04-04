namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.FieldsFilterCardEdit;

using System.Collections.ObjectModel;
using System.Collections.Generic;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.PreFilterVisualEdit;

using Ctx = VmFieldsFilterCardEdit;

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

	public IReadOnlyList<str> OperationOptions => Owner?.OperationOptions ?? [];
	public IReadOnlyList<str> ValueTypeOptions => Owner?.ValueTypeOptions ?? [];
	public IReadOnlyList<str> FieldOptions => IsCore
		? VmPreFilterVisualEdit.CoreWordFieldOptions
		: Owner?.PropFieldOptions ?? [];

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
		OnPropertyChanged(nameof(OperationOptions));
		OnPropertyChanged(nameof(ValueTypeOptions));
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
		RefreshItemCards();
		return NIL;
	}

	public nil RefreshItemCards(){
		ItemCards.Clear();
		for(u64 i = 0; i < (u64)Items.Count; i++){
			var item = Items[(i32)i];
			ItemCards.Add(new RowFilterItemCard{
				UiIdx = i + 1,
				UiIdxText = (i + 1).ToString(),
				Operation = SafeOpt(OperationOptions, item.OperationIndex),
				ValueType = SafeOpt(ValueTypeOptions, item.ValueTypeIndex),
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
		RefreshItemCards();
		return NIL;
	}

	public nil Save(){
		if(Target is null || Owner is null){
			ShowMsg(Todo.I18n("Editor not ready"));
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
		var kind = IsCore ? Todo.I18n("Core") : Todo.I18n("Prop");
		ShowMsg(Todo.I18n($"Saved {kind} Filter #{RowIdx}"));
		ViewNavi?.Back();
		return NIL;
	}

	static str SafeOpt(IReadOnlyList<str> opts, i32 idx){
		if(idx < 0 || idx >= opts.Count){
			return "";
		}
		return opts[idx];
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
