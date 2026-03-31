namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Ngaq.Core.Shared.StudyPlan.Models;
using Ngaq.Core.Shared.StudyPlan.Models.Po.PreFilter;
using Ngaq.Core.Shared.StudyPlan.Models.PreFilter;
using Ngaq.Core.Tools.Json;
using Ngaq.Ui.Infra;

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

	IJsonSerializer JsonSerializer {get;set;} = AppJsonSerializer.Inst;

	public VmPreFilterEdit(
		IJsonSerializer? JsonSerializer
	){
		this.JsonSerializer = JsonSerializer ?? AppJsonSerializer.Inst;
		InitRowEvents();
		BoPreFilter = MkEmptyBoPreFilter();
		SyncAllFromBo();
	}

	bool _isHydrating = false;

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

	public BoPreFilter BoPreFilter{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = MkEmptyBoPreFilter();

	public i32 TabIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=0;

	public str PoPreFilterJson{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str PreFilterJson{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str LastError{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public bool HasError => !str.IsNullOrWhiteSpace(LastError);

	public str PoIdText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str PoUniqName{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str PoDescr{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public i32 PoTypeIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = 1;

	public str PreFilterVersion{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "1.0.0.0";

	public ObservableCollection<VmFieldsFilterRow> CoreFilterRows{get;set;} = [];
	public ObservableCollection<VmFieldsFilterRow> PropFilterRows{get;set;} = [];

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
		this.BoPreFilter = MkEmptyBoPreFilter();
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
		return NIL;
	}

	public nil RemoveCoreGroup(VmFieldsFilterRow Row){
		CoreFilterRows.Remove(Row);
		return NIL;
	}

	public nil AddPropGroup(){
		PropFilterRows.Add(MkFieldsFilterRow());
		return NIL;
	}

	public nil RemovePropGroup(VmFieldsFilterRow Row){
		PropFilterRows.Remove(Row);
		return NIL;
	}

	public nil AddFilterItem(VmFieldsFilterRow Row){
		Row.Items.Add(MkFilterItemRow());
		return NIL;
	}

	public nil RemoveFilterItem(VmFieldsFilterRow Row, VmFilterItemRow Item){
		Row.Items.Remove(Item);
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
		SyncJsonFromBo();
	}

	void SyncVisualFromBo(){
		var po = BoPreFilter?.PoPreFilter ?? MkEmptyBoPreFilter().PoPreFilter;
		var pre = BoPreFilter?.PreFilter ?? MkEmptyBoPreFilter().PreFilter;

		_isHydrating = true;
		try{
			PoIdText = po.Id.ToString();
			PoUniqName = po.UniqName ?? "";
			PoDescr = po.Descr;
			PoTypeIndex = ClampIndex((i32)po.Type, PoTypeOptions.Count);
			PreFilterVersion = pre.Version?.ToString() ?? "1.0.0.0";

			CoreFilterRows = new ObservableCollection<VmFieldsFilterRow>(
				(pre.CoreFilter ?? [])
				.Select(MkRowFromModel)
			);
			PropFilterRows = new ObservableCollection<VmFieldsFilterRow>(
				(pre.PropFilter ?? [])
				.Select(MkRowFromModel)
			);

			if(CoreFilterRows.Count == 0){
				CoreFilterRows.Add(MkFieldsFilterRow());
			}
			if(PropFilterRows.Count == 0){
				PropFilterRows.Add(MkFieldsFilterRow());
			}

			LastError = "";
			OnPropertyChanged(nameof(HasError));
		}finally{
			_isHydrating = false;
		}
	}

	void SyncJsonFromBo(){
		var po = BoPreFilter?.PoPreFilter ?? MkEmptyBoPreFilter().PoPreFilter;
		var pre = BoPreFilter?.PreFilter ?? MkEmptyBoPreFilter().PreFilter;
		PoPreFilterJson = FormatJson(JsonSerializer.Stringify(po));
		PreFilterJson = FormatJson(JsonSerializer.Stringify(pre));
	}

	VmFieldsFilterRow MkRowFromModel(FieldsFilter Row){
		var vm = new VmFieldsFilterRow{
			FieldsText = string.Join(", ", Row.Fields ?? []),
		};
		var items = Row.Filters ?? [];
		foreach(var item in items){
			vm.Items.Add(new VmFilterItemRow{
				OperationIndex = ClampIndex((i32)item.Operation, OperationOptions.Count),
				ValueTypeIndex = ClampIndex((i32)item.ValueType, ValueTypeOptions.Count),
				ValuesText = string.Join(", ", (item.Values ?? []).Select(x => x?.ToString() ?? "")),
			});
		}
		if(vm.Items.Count == 0){
			vm.Items.Add(MkFilterItemRow());
		}
		return vm;
	}

	bool TryBuildBoFromVisual(
		out BoPreFilter Bo
		,out str Err
	){
		Bo = this.BoPreFilter ?? MkEmptyBoPreFilter();
		Err = "";
		try{
			var po = Bo.PoPreFilter?.DeepClone() ?? new PoPreFilter();
			po.UniqName = str.IsNullOrWhiteSpace(PoUniqName)? null : PoUniqName.Trim();
			po.Descr = PoDescr?.Trim() ?? "";
			po.Type = EnumOrDefault<EPreFilterType>(PoTypeIndex);
			if(po.Type == EPreFilterType.Unknown){
				po.Type = EPreFilterType.Json;
			}

			if(!Version.TryParse((PreFilterVersion??"").Trim(), out var ver)){
				Err = "Version format invalid. Example: 1.0.0.0";
				return false;
			}

			var pre = new Ngaq.Core.Shared.StudyPlan.Models.PreFilter.PreFilter{
				Version = ver,
				CoreFilter = BuildFieldsFilterList(CoreFilterRows, out var coreErr),
				PropFilter = [],
			};
			if(coreErr is not null){
				Err = coreErr;
				return false;
			}
			pre.PropFilter = BuildFieldsFilterList(PropFilterRows, out var propErr);
			if(propErr is not null){
				Err = propErr;
				return false;
			}

			po.DataSchemaVer = pre.Version;
			po.Data = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Stringify(pre));

			Bo = new BoPreFilter{
				PoPreFilter = po,
				PreFilter = pre,
			};
			return true;
		}catch(Exception e){
			Err = e.Message;
			return false;
		}
	}

	IList<FieldsFilter> BuildFieldsFilterList(
		IEnumerable<VmFieldsFilterRow> Rows
		,out str? Err
	){
		Err = null;
		var ans = new List<FieldsFilter>();
		var rowIdx = 0;
		foreach(var row in Rows){
			rowIdx++;
			var fields = SplitCsv(row.FieldsText).ToList();
			var filters = new List<FilterItem>();
			var itemIdx = 0;
			foreach(var item in row.Items){
				itemIdx++;
				var valueType = EnumOrDefault<EValueType>(item.ValueTypeIndex);
				var values = ParseValues(item.ValuesText, valueType, out var parseErr);
				if(parseErr is not null){
					Err = $"Row#{rowIdx}, Item#{itemIdx}: {parseErr}";
					return [];
				}
				filters.Add(new FilterItem{
					Operation = EnumOrDefault<EFilterOperationMode>(item.OperationIndex),
					ValueType = valueType,
					Values = values,
				});
			}
			ans.Add(new FieldsFilter{
				Fields = fields,
				Filters = filters,
			});
		}
		return ans;
	}

	IList<obj?> ParseValues(str Text, EValueType ValueType, out str? Err){
		Err = null;
		var parts = SplitCsv(Text).ToList();
		if(parts.Count == 0){
			return [];
		}
		if(ValueType == EValueType.String || ValueType == EValueType.Null){
			return parts.Cast<obj?>().ToList();
		}
		if(ValueType == EValueType.Number){
			var ans = new List<obj?>();
			foreach(var p in parts){
				if(long.TryParse(p, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i)){
					ans.Add(i);
					continue;
				}
				if(double.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)){
					ans.Add(d);
					continue;
				}
				Err = $"'{p}' is not a valid number";
				return [];
			}
			return ans;
		}
		return parts.Cast<obj?>().ToList();
	}

	static IEnumerable<str> SplitCsv(str? Text){
		if(str.IsNullOrWhiteSpace(Text)){
			return [];
		}
		return Text
			.Split([',', '\n', '\r', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Where(x=>!str.IsNullOrWhiteSpace(x));
	}

	static TEnum EnumOrDefault<TEnum>(i32 Index)
		where TEnum : struct, Enum
	{
		var values = Enum.GetValues<TEnum>();
		if(Index < 0 || Index >= values.Length){
			return values[0];
		}
		return values[Index];
	}

	static i32 ClampIndex(i32 Value, i32 Count){
		if(Count <= 0){
			return 0;
		}
		if(Value < 0){
			return 0;
		}
		if(Value >= Count){
			return Count - 1;
		}
		return Value;
	}

	protected bool TryBuildBoFromJson(
		out BoPreFilter Bo
		,out str Err
	){
		Bo = this.BoPreFilter ?? MkEmptyBoPreFilter();
		Err = "";
		try{
			var po = JsonSerializer.Parse<PoPreFilter>(PoPreFilterJson);
			if(po is null){
				Err = "PoPreFilter JSON parse failed";
				return false;
			}
			var pre = JsonSerializer.Parse<Ngaq.Core.Shared.StudyPlan.Models.PreFilter.PreFilter>(PreFilterJson);
			if(pre is null){
				Err = "PreFilter JSON parse failed";
				return false;
			}
			po.DataSchemaVer = pre.Version;
			po.Data = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Stringify(pre));
			Bo = new BoPreFilter{
				PoPreFilter = po,
				PreFilter = pre,
			};
			return true;
		}catch(Exception e){
			Err = e.Message;
			return false;
		}
	}

	protected static str FormatJson(str UglyJson){
		if(str.IsNullOrWhiteSpace(UglyJson)){
			return "";
		}
		try{
			var node = System.Text.Json.Nodes.JsonNode.Parse(UglyJson);
			return node?.ToJsonString(new System.Text.Json.JsonSerializerOptions{
				WriteIndented = true,
				Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			}) ?? UglyJson;
		}catch{
			return UglyJson;
		}
	}
}
