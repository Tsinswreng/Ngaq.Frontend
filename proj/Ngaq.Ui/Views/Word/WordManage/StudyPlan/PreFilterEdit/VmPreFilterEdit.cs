namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Ngaq.Core.Shared.StudyPlan.Models;
using Ngaq.Core.Shared.StudyPlan.Models.Po.PreFilter;
using Ngaq.Core.Shared.StudyPlan.Models.PreFilter;
using Ngaq.Core.Tools.Json;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;

using Ctx = VmPreFilterEdit;

public partial class VmPreFilterEdit: ViewModelBase, IMk<Ctx>{
	protected VmPreFilterEdit(){
		InitRowEvents();
	}

	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmPreFilterEdit(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	IJsonSerializer JsonSerializer{get;set;} = AppJsonSerializer.Inst;
	bool _isHydrating = false;

	public VmPreFilterEdit(IJsonSerializer? JsonSerializer){
		this.JsonSerializer = JsonSerializer ?? AppJsonSerializer.Inst;
		InitRowEvents();
		BoPreFilter = MkEmptyBoPreFilter();
		SyncAllFromBo();
	}

	public enum ETabIdx{
		Visual = 0,
		PoJson = 1,
		PreFilterJson = 2,
	}

	public class VmFilterItemRow: ViewModelBase{
		public i32 OperationIndex{
			get{return field;}
			set{SetProperty(ref field, value);}
		} = 0;

		public i32 ValueTypeIndex{
			get{return field;}
			set{SetProperty(ref field, value);}
		} = 0;

		public str ValuesText{
			get{return field;}
			set{SetProperty(ref field, value);}
		} = "";
	}

	public class VmFieldsFilterRow: ViewModelBase{
		public str FieldsText{
			get{return field;}
			set{SetProperty(ref field, value);}
		} = "";

		public ObservableCollection<VmFilterItemRow> Items{get;set;} = [];
	}

	public class RowFieldsFilterCard{
		public u64 UiIdx{get;set;}
		public str UiIdxText{get;set;} = "";
		public str Kind{get;set;} = "";
		public str FieldsPreview{get;set;} = "";
		public str FilterCountText{get;set;} = "";
		public VmFieldsFilterRow? Raw{get;set;}
	}

	public BoPreFilter BoPreFilter{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = MkEmptyBoPreFilter();

	public i32 TabIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = 0;

	public str LastError{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public bool HasError => !str.IsNullOrWhiteSpace(LastError);

	public ObservableCollection<VmFieldsFilterRow> CoreFilterRows{get;set;} = [];
	public ObservableCollection<VmFieldsFilterRow> PropFilterRows{get;set;} = [];

	public ObservableCollection<RowFieldsFilterCard> CoreFilterCards{get;set;} = [];
	public ObservableCollection<RowFieldsFilterCard> PropFilterCards{get;set;} = [];

	public IReadOnlyList<str> PoTypeOptions{get;} = Enum.GetNames<EPreFilterType>();
	public IReadOnlyList<str> OperationOptions{get;} = Enum.GetNames<EFilterOperationMode>();
	public IReadOnlyList<str> ValueTypeOptions{get;} = Enum.GetNames<EValueType>();

	void InitRowEvents(){
		CoreFilterRows.CollectionChanged += OnFieldsRowsChanged;
		PropFilterRows.CollectionChanged += OnFieldsRowsChanged;
	}

	void OnFieldsRowsChanged(object? sender, NotifyCollectionChangedEventArgs e){
		if(e.NewItems is not null){
			foreach(var row in e.NewItems.OfType<VmFieldsFilterRow>()){
				row.PropertyChanged += OnFieldsRowChanged;
				row.Items.CollectionChanged += OnFilterItemsChanged;
				foreach(var item in row.Items){
					item.PropertyChanged += OnFilterItemChanged;
				}
			}
		}
		if(e.OldItems is not null){
			foreach(var row in e.OldItems.OfType<VmFieldsFilterRow>()){
				row.PropertyChanged -= OnFieldsRowChanged;
				row.Items.CollectionChanged -= OnFilterItemsChanged;
				foreach(var item in row.Items){
					item.PropertyChanged -= OnFilterItemChanged;
				}
			}
		}
		Touch();
	}

	void OnFilterItemsChanged(object? sender, NotifyCollectionChangedEventArgs e){
		if(e.NewItems is not null){
			foreach(var item in e.NewItems.OfType<VmFilterItemRow>()){
				item.PropertyChanged += OnFilterItemChanged;
			}
		}
		if(e.OldItems is not null){
			foreach(var item in e.OldItems.OfType<VmFilterItemRow>()){
				item.PropertyChanged -= OnFilterItemChanged;
			}
		}
		Touch();
	}

	void OnFieldsRowChanged(object? sender, PropertyChangedEventArgs e){
		Touch();
	}

	void OnFilterItemChanged(object? sender, PropertyChangedEventArgs e){
		Touch();
	}

	void Touch(){
		if(_isHydrating){
			return;
		}
		RefreshFieldsFilterCards();
	}

	public nil RefreshFieldsFilterCards(){
		CoreFilterCards.Clear();
		for(u64 i = 0; i < (u64)CoreFilterRows.Count; i++){
			var row = CoreFilterRows[(i32)i];
			CoreFilterCards.Add(new RowFieldsFilterCard{
				UiIdx = i + 1,
				UiIdxText = (i + 1).ToString(),
				Kind = "Core",
				FieldsPreview = str.IsNullOrWhiteSpace(row.FieldsText) ? "-" : row.FieldsText,
				FilterCountText = (row.Items?.Count ?? 0).ToString(),
				Raw = row,
			});
		}
		PropFilterCards.Clear();
		for(u64 i = 0; i < (u64)PropFilterRows.Count; i++){
			var row = PropFilterRows[(i32)i];
			PropFilterCards.Add(new RowFieldsFilterCard{
				UiIdx = i + 1,
				UiIdxText = (i + 1).ToString(),
				Kind = "Prop",
				FieldsPreview = str.IsNullOrWhiteSpace(row.FieldsText) ? "-" : row.FieldsText,
				FilterCountText = (row.Items?.Count ?? 0).ToString(),
				Raw = row,
			});
		}
		return NIL;
	}

	public nil OpenCoreFilterCard(RowFieldsFilterCard? Card){
		return OpenFieldsFilterCard(Card, true);
	}

	public nil OpenPropFilterCard(RowFieldsFilterCard? Card){
		return OpenFieldsFilterCard(Card, false);
	}

	nil OpenFieldsFilterCard(RowFieldsFilterCard? Card, bool IsCore){
		if(Card?.Raw is null){
			return NIL;
		}
		var view = new ViewFieldsFilterCardEdit();
		view.Ctx?.Load(this, Card.Raw, IsCore, Card.UiIdx);
		var title = $"{(IsCore?"Core":"Prop")} Filter #{Card.UiIdx}";
		ViewNavi?.GoTo(ToolView.WithTitle(title, view));
		return NIL;
	}

	public nil FromPoPreFilter(PoPreFilter? PoPreFilter){
		var bo = MkEmptyBoPreFilter();
		if(PoPreFilter is not null){
			bo.FromPoPreFilter(PoPreFilter);
		}
		return FromBoPreFilter(bo);
	}

	public nil FromBoPreFilter(BoPreFilter? BoPreFilter){
		this.BoPreFilter = BoPreFilter ?? MkEmptyBoPreFilter();
		SyncAllFromBo();
		return NIL;
	}

	public BoPreFilter ToBoPreFilter(){
		if(TabIndex == (i32)ETabIdx.Visual){
			if(!TryBuildBoFromVisual(out var boVisual, out var errVisual)){
				LastError = errVisual;
				OnPropertyChanged(nameof(HasError));
				return this.BoPreFilter;
			}
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			this.BoPreFilter = boVisual;
			SyncJsonFromBo();
			return boVisual;
		}

		if(!TryBuildBoFromJson(out var boJson, out var errJson)){
			LastError = errJson;
			OnPropertyChanged(nameof(HasError));
			return this.BoPreFilter;
		}
		LastError = "";
		OnPropertyChanged(nameof(HasError));
		this.BoPreFilter = boJson;
		SyncVisualFromBo();
		return boJson;
	}

	public async Task<nil> Save(CT Ct = default){
		await Task.Yield();
		if(TabIndex == (i32)ETabIdx.Visual){
			if(!TryBuildBoFromVisual(out var boVisual, out var errVisual)){
				LastError = errVisual;
				OnPropertyChanged(nameof(HasError));
				ShowMsg(errVisual);
				return NIL;
			}
			this.BoPreFilter = boVisual;
			SyncJsonFromBo();
		}else{
			if(!TryBuildBoFromJson(out var boJson, out var errJson)){
				LastError = errJson;
				OnPropertyChanged(nameof(HasError));
				ShowMsg(errJson);
				return NIL;
			}
			this.BoPreFilter = boJson;
			SyncVisualFromBo();
		}

		LastError = "";
		OnPropertyChanged(nameof(HasError));
		ShowMsg("Saved");
		return NIL;
	}

	public async Task<nil> Delete(CT Ct = default){
		await Task.Yield();
		LastError = "";
		OnPropertyChanged(nameof(HasError));
		BoPreFilter = MkEmptyBoPreFilter();
		SyncAllFromBo();
		ShowMsg("Deleted");
		return NIL;
	}

	public nil GoToPoJson(){
		if(!TryBuildBoFromVisual(out var bo, out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowMsg(err);
			return NIL;
		}
		BoPreFilter = bo;
		SyncJsonFromBo();
		TabIndex = (i32)ETabIdx.PoJson;
		return NIL;
	}

	public nil GoToPreFilterJson(){
		if(!TryBuildBoFromVisual(out var bo, out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowMsg(err);
			return NIL;
		}
		BoPreFilter = bo;
		SyncJsonFromBo();
		TabIndex = (i32)ETabIdx.PreFilterJson;
		return NIL;
	}

	public nil GoToVisual(){
		TabIndex = (i32)ETabIdx.Visual;
		return NIL;
	}

	public nil ApplyJsonToVisual(){
		if(!TryBuildBoFromJson(out var bo, out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowMsg(err);
			return NIL;
		}
		BoPreFilter = bo;
		SyncVisualFromBo();
		LastError = "";
		OnPropertyChanged(nameof(HasError));
		ShowMsg("Applied JSON to visual editor");
		return NIL;
	}

	public nil SyncJsonFromVisual(){
		if(!TryBuildBoFromVisual(out var bo, out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowMsg(err);
			return NIL;
		}
		BoPreFilter = bo;
		SyncJsonFromBo();
		LastError = "";
		OnPropertyChanged(nameof(HasError));
		ShowMsg("Synced visual editor to JSON");
		return NIL;
	}

	public nil AddCoreGroup(){
		CoreFilterRows.Add(MkFieldsFilterRow());
		RefreshFieldsFilterCards();
		return NIL;
	}

	public nil RemoveCoreGroup(VmFieldsFilterRow Row){
		CoreFilterRows.Remove(Row);
		RefreshFieldsFilterCards();
		return NIL;
	}

	public nil AddPropGroup(){
		PropFilterRows.Add(MkFieldsFilterRow());
		RefreshFieldsFilterCards();
		return NIL;
	}

	public nil RemovePropGroup(VmFieldsFilterRow Row){
		PropFilterRows.Remove(Row);
		RefreshFieldsFilterCards();
		return NIL;
	}

	public nil AddFilterItem(VmFieldsFilterRow Row){
		Row.Items.Add(MkFilterItemRow());
		RefreshFieldsFilterCards();
		return NIL;
	}

	public nil RemoveFilterItem(VmFieldsFilterRow Row, VmFilterItemRow Item){
		Row.Items.Remove(Item);
		RefreshFieldsFilterCards();
		return NIL;
	}

	protected static BoPreFilter MkEmptyBoPreFilter(){
		return new BoPreFilter{
			PoPreFilter = new PoPreFilter{
				Type = EPreFilterType.Json,
			},
			PreFilter = new Ngaq.Core.Shared.StudyPlan.Models.PreFilter.PreFilter(),
		};
	}

	static VmFieldsFilterRow MkFieldsFilterRow(){
		var row = new VmFieldsFilterRow();
		row.Items.Add(MkFilterItemRow());
		return row;
	}

	static VmFilterItemRow MkFilterItemRow(){
		return new VmFilterItemRow{
			OperationIndex = 1,
			ValueTypeIndex = 1,
		};
	}

	void SyncAllFromBo(){
		SyncVisualFromBo();
		RefreshFieldsFilterCards();
		SyncJsonFromBo();
	}
}
