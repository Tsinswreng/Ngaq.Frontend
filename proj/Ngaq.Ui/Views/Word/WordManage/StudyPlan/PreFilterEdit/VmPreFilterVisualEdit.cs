namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.StudyPlan.Models;
using Ngaq.Core.Shared.StudyPlan.Models.Po.PreFilter;
using Ngaq.Core.Shared.StudyPlan.Models.PreFilter;
using Ngaq.Core.Shared.StudyPlan.Svc;
using Ngaq.Core.Tools.Json;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Tsinswreng.CsTools;

using Ctx = VmPreFilterVisualEdit;

/// <summary>
/// PreFilter GUI 主編輯 ViewModel。
/// 僅負責 GUI 視圖，不承載 JSON 視圖狀態。
/// </summary>
public class VmPreFilterVisualEdit: ViewModelBase, IMk<Ctx>{
	protected VmPreFilterVisualEdit(){
		InitRowEvents();
	}

	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmPreFilterVisualEdit(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	ISvcStudyPlan? SvcStudyPlan{get;set;}
	IFrontendUserCtxMgr? UserCtxMgr{get;set;}
	IJsonSerializer JsonSerializer{get;set;} = AppJsonSerializer.Inst;
	bool _isHydrating = false;

	/// <summary>
	/// 依賴注入構造器。
	/// </summary>
	public VmPreFilterVisualEdit(
		ISvcStudyPlan? SvcStudyPlan
		,IFrontendUserCtxMgr? UserCtxMgr
		,IJsonSerializer? JsonSerializer
	){
		this.SvcStudyPlan = SvcStudyPlan;
		this.UserCtxMgr = UserCtxMgr;
		this.JsonSerializer = JsonSerializer ?? AppJsonSerializer.Inst;
		InitRowEvents();
		BoPreFilter = MkEmptyBoPreFilter();
		SyncFromBo();
	}

	/// <summary>
	/// GUI 子頁列表中的單條過濾條件。
	/// </summary>
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

	/// <summary>
	/// GUI 子頁中的一組 Fields + Filters。
	/// </summary>
	public class VmFieldsFilterRow: ViewModelBase{
		public str FieldsText{
			get{return field;}
			set{SetProperty(ref field, value);}
		} = "";

		public ObservableCollection<VmFilterItemRow> Items{get;set;} = [];
	}

	/// <summary>
	/// TreeDataGrid 顯示行。
	/// </summary>
	public class RowFieldsFilterCard{
		public u64 UiIdx{get;set;}
		public str UiIdxText{get;set;} = "";
		public str Kind{get;set;} = "";
		public str FieldsPreview{get;set;} = "";
		public str FilterCountText{get;set;} = "";
		public VmFieldsFilterRow? Raw{get;set;}
	}

	/// <summary>
	/// 當前編輯中的業務模型。
	/// </summary>
	public BoPreFilter BoPreFilter{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = MkEmptyBoPreFilter();

	/// <summary>
	/// GUI 主頁展示的 Text 預覽，只顯示前段內容。
	/// </summary>
	public str PoTextPreview{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

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
		TouchRows();
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
		TouchRows();
	}

	void OnFieldsRowChanged(object? sender, PropertyChangedEventArgs e){
		TouchRows();
	}

	void OnFilterItemChanged(object? sender, PropertyChangedEventArgs e){
		TouchRows();
	}

	void TouchRows(){
		if(_isHydrating){
			return;
		}
		RefreshFieldsFilterCards();
	}

	/// <summary>
	/// 刷新 Core/Prop 的列表卡片預覽。
	/// </summary>
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

	/// <summary>
	/// 由列表頁傳入 Po 實體初始化 GUI 主頁。
	/// </summary>
	public nil FromPoPreFilter(PoPreFilter? PoPreFilter){
		var bo = MkEmptyBoPreFilter();
		if(PoPreFilter is not null){
			bo.FromPoPreFilter(PoPreFilter);
		}
		return FromBoPreFilter(bo);
	}

	/// <summary>
	/// 由既有業務模型初始化 GUI 主頁。
	/// </summary>
	public nil FromBoPreFilter(BoPreFilter? BoPreFilter){
		this.BoPreFilter = BoPreFilter ?? MkEmptyBoPreFilter();
		SyncFromBo();
		return NIL;
	}

	/// <summary>
	/// 導航到 JSON 專用編輯視圖。
	/// </summary>
	public nil OpenJsonEditor(){
		if(!TryBuildBoFromVisual(out var bo, out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowMsg(err);
			return NIL;
		}
		LastError = "";
		OnPropertyChanged(nameof(HasError));
		BoPreFilter = bo;

		var view = new ViewPreFilterJsonEdit();
		view.Ctx?.FromPoPreFilter(bo.PoPreFilter);
		ViewNavi?.GoTo(ToolView.WithTitle("PoPreFilter JSON", view));
		return NIL;
	}

	/// <summary>
	/// 導航到 PreFilter（無 Po）GUI 子編輯頁。
	/// </summary>
	public nil OpenPreFilterDataEditor(){
		var view = new ViewPreFilterDataEdit(this);
		ViewNavi?.GoTo(ToolView.WithTitle("PreFilter", view));
		return NIL;
	}

	/// <summary>
	/// 保存 GUI 子編輯頁變更到本 VM（不寫庫）。
	/// </summary>
	public bool CommitPreFilterDataDraft(){
		if(!TryBuildBoFromVisual(out var bo, out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowMsg(err);
			return false;
		}

		LastError = "";
		OnPropertyChanged(nameof(HasError));
		BoPreFilter = bo;
		RefreshTextPreview();
		ShowMsg("PreFilter draft updated");
		return true;
	}

	/// <summary>
	/// 保存到後端。
	/// 新建時調 BatAddPreFilter，編輯時調 BatUpdPreFilter。
	/// </summary>
	public async Task<nil> Save(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			ShowMsg("Service not ready");
			return NIL;
		}
		if(!TryBuildBoFromVisual(out var bo, out var err)){
			LastError = err;
			OnPropertyChanged(nameof(HasError));
			ShowMsg(err);
			return NIL;
		}

		try{
			var dbCtx = UserCtxMgr.GetDbUserCtx();
			var po = bo.PoPreFilter;
			if(po.Id == IdPreFilter.Zero){
				await SvcStudyPlan.BatAddPreFilter(dbCtx, ToolAsyE.ToAsyE([po]), Ct);
			}else{
				await SvcStudyPlan.BatUpdPreFilter(dbCtx, ToolAsyE.ToAsyE([po]), Ct);
			}

			BoPreFilter = bo;
			SyncFromBo();
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			ShowMsg("Saved");
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}

	/// <summary>
	/// 軟刪除當前 PreFilter。
	/// </summary>
	public async Task<nil> Delete(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			ShowMsg("Service not ready");
			return NIL;
		}
		try{
			var po = BoPreFilter?.PoPreFilter ?? new PoPreFilter();
			if(po.Id != IdPreFilter.Zero){
				await SvcStudyPlan.BatSoftDelPreFilter(UserCtxMgr.GetDbUserCtx(), ToolAsyE.ToAsyE([po]), Ct);
			}
			BoPreFilter = MkEmptyBoPreFilter();
			SyncFromBo();
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			ShowMsg("Deleted");
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}

	public nil AddCoreGroup(){
		CoreFilterRows.Add(MkFieldsFilterRow());
		RefreshFieldsFilterCards();
		return NIL;
	}

	public nil AddPropGroup(){
		PropFilterRows.Add(MkFieldsFilterRow());
		RefreshFieldsFilterCards();
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

	void SyncFromBo(){
		var po = BoPreFilter?.PoPreFilter ?? MkEmptyBoPreFilter().PoPreFilter;
		var pre = BoPreFilter?.PreFilter ?? MkEmptyBoPreFilter().PreFilter;

		_isHydrating = true;
		try{
			// step 1: 同步 Po 主字段（主頁展示）
			PoIdText = po.Id.ToString();
			PoUniqName = po.UniqName ?? "";
			PoDescr = po.Descr;
			PoTypeIndex = ClampIndex((i32)po.Type, PoTypeOptions.Count);

			// step 2: 同步 PreFilter 可視編輯字段（子頁展示）
			PreFilterVersion = pre.Version?.ToString() ?? "1.0.0.0";
			CoreFilterRows.Clear();
			foreach(var row in (pre.CoreFilter ?? []).Select(MkRowFromModel)){
				CoreFilterRows.Add(row);
			}
			PropFilterRows.Clear();
			foreach(var row in (pre.PropFilter ?? []).Select(MkRowFromModel)){
				PropFilterRows.Add(row);
			}
			if(CoreFilterRows.Count == 0){
				CoreFilterRows.Add(MkFieldsFilterRow());
			}
			if(PropFilterRows.Count == 0){
				PropFilterRows.Add(MkFieldsFilterRow());
			}
			RefreshFieldsFilterCards();
			RefreshTextPreview();

			LastError = "";
			OnPropertyChanged(nameof(HasError));
		}finally{
			_isHydrating = false;
		}
	}

	void RefreshTextPreview(){
		var fullText = BoPreFilter?.PoPreFilter?.Text ?? "";
		if(str.IsNullOrWhiteSpace(fullText)){
			PoTextPreview = "";
			return;
		}

		const int maxLen = 320;
		PoTextPreview = fullText.Length <= maxLen
			? fullText
			: fullText[..maxLen] + "...";
	}

	VmFieldsFilterRow MkRowFromModel(FieldsFilter row){
		var vm = new VmFieldsFilterRow{
			FieldsText = string.Join(", ", row.Fields ?? []),
		};
		foreach(var item in (row.Filters ?? [])){
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
		out BoPreFilter Bo,
		out str Err
	){
		Bo = BoPreFilter ?? MkEmptyBoPreFilter();
		Err = "";
		try{
			// step 1: 組裝 PoPreFilter
			var po = ClonePoPreFilter(Bo.PoPreFilter);
			po.UniqName = str.IsNullOrWhiteSpace(PoUniqName) ? null : PoUniqName.Trim();
			po.Descr = PoDescr?.Trim() ?? "";
			po.Type = EnumOrDefault<EPreFilterType>(PoTypeIndex);
			if(po.Type == EPreFilterType.Unknown){
				po.Type = EPreFilterType.Json;
			}

			// step 2: 組裝 PreFilter 並做格式校驗
			if(!Version.TryParse((PreFilterVersion ?? "").Trim(), out var ver)){
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

			// step 3: 回填 Text 載荷（不使用 Binary）
			po.DataSchemaVer = pre.Version;
			po.Text = JsonSerializer.Stringify(pre);
			po.Binary = null;

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

	static PoPreFilter ClonePoPreFilter(PoPreFilter? src){
		src ??= new PoPreFilter();
		return new PoPreFilter{
			DbCreatedAt = src.DbCreatedAt,
			DbUpdatedAt = src.DbUpdatedAt,
			DelAt = src.DelAt,
			BizCreatedAt = src.BizCreatedAt,
			BizUpdatedAt = src.BizUpdatedAt,
			Id = src.Id,
			Owner = src.Owner,
			UniqName = src.UniqName,
			Descr = src.Descr,
			Type = src.Type,
			DataSchemaVer = src.DataSchemaVer,
			Text = src.Text,
			Binary = src.Binary?.ToArray() ?? [],
		};
	}

	IList<FieldsFilter> BuildFieldsFilterList(
		IEnumerable<VmFieldsFilterRow> Rows,
		out str? Err
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

	static BoPreFilter MkEmptyBoPreFilter(){
		return new BoPreFilter{
			PoPreFilter = new PoPreFilter{
				Type = EPreFilterType.Json,
			},
			PreFilter = new Ngaq.Core.Shared.StudyPlan.Models.PreFilter.PreFilter(),
		};
	}
}
