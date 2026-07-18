namespace Ngaq.Ui.Views.Word.WordEditV2.WordLearnPage;

using System.Collections.ObjectModel;
using System.Globalization;
using Ngaq.Core.Infra;
using Ngaq.Core.Model.Po.Learn_;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Shared.Word.Models.Po.Learn;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word;
using Tsinswreng.CsTempus;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// 學習記錄分頁 ViewModel：管理列表、新增、轉換。
public partial class VmWordLearnPage: ViewModelBase{
	public event Action<VmWordLearnRow>? OnEditRequested;

	public ObservableCollection<VmWordLearnRow> Rows{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = [];

	/// 已標記刪除的行，保存時走 Del，成功後清空。
	public List<VmWordLearnRow> RemovedRows{get;} = [];

	/// 新增的行（DmlState == Added），保存時走 BatAdd。
	public IEnumerable<VmWordLearnRow> AddedRows => Rows.Where(r => r.DmlState == EDmlState.Added);

	/// 已修改的行（DmlState == Modified），保存時走 BatUpd。
	public IEnumerable<VmWordLearnRow> ModifiedRows => Rows.Where(r => r.DmlState == EDmlState.Modified);

	public nil LoadFromPoLearns(IList<PoWordLearn> Learns){
		Rows = new ObservableCollection<VmWordLearnRow>(Learns.Select(VmWordLearnRow.FromPo));
		return NIL;
	}

	public nil AddRow(){
		Rows.Add(VmWordLearnRow.NewRow());
		return NIL;
	}

	/// 新增行直接移除；已存在的行標記爲 Removed，保存時再刪。
	public nil RemoveRow(VmWordLearnRow Row){
		if(Row.DmlState == EDmlState.Added){
			Rows.Remove(Row);
		}else{
			Row.DmlState = EDmlState.Removed;
			Rows.Remove(Row);
			RemovedRows.Add(Row);
		}
		return NIL;
	}

	/// 已直接調後端刪除成功的行，只更新本地列表，不再掛到總保存刪除隊列。
	public nil RemovePersistedRow(VmWordLearnRow Row){
		Rows.Remove(Row);
		RemovedRows.Remove(Row);
		return NIL;
	}

	public nil RequestEdit(VmWordLearnRow Row){
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

	public bool TryBuildPoLearns(IdWord WordId, out List<PoWordLearn> Learns, out str Err){
		Learns = [];
		Err = "";
		for(i32 i = 0; i < Rows.Count; i++){
			var row = Rows[i];
			if(!row.TryToPo(WordId, out var po, out var rowErr)){
				Err = I18n.Get(K.Learn__Err__, i+1, rowErr);
				return false;
			}
			Learns.Add(po);
		}
		return true;
	}
}

/// 學習記錄行 ViewModel。
public partial class VmWordLearnRow: ViewModelBase{
	public static IReadOnlyList<ELearn> LearnResults{get;} = [
		ELearn.Add,
		ELearn.Rmb,
		ELearn.Fgt,
	];

	public PoWordLearn Raw{get;set;} = new();

	/// 行記錄 DML 狀態：決定保存時走 BatAdd/BatUpd/Del。
	public EDmlState DmlState{get;set;}

	public i32 LearnResultIndex{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				MarkModified();
			}
		}
	} = 0;

	/// 行記錄 Id（僅供顯示，只讀）。
	public str IdText => Raw.Id.ToString();

	public str BizCreatedAtIso{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				MarkModified();
			}
		}
	} = UnixMs.Now().ToIso();

	public str BizCreatedAtDisplay{
		get{
			try{
				var t = UnixMs.FromIso(BizCreatedAtIso);
				var dto = DateTimeOffset.FromUnixTimeMilliseconds(t.Value);
				var local = TimeZoneInfo.ConvertTime(dto, TimeZoneInfo.Local);
				return local.ToString("yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			}catch{
				return BizCreatedAtIso;
			}
		}
	}

	public str LearnResultText => GetLearnResultByIndex(LearnResultIndex).ToString();
	public str LearnResultDisplayText => TranslateLearnResult(GetLearnResultByIndex(LearnResultIndex));

	/// 只有 Unchanged → Modified 才切換，保持 Added / Removed 不被意外覆蓋。
	void MarkModified(){
		if(DmlState == EDmlState.Unchanged){
			DmlState = EDmlState.Modified;
		}
	}

	public static VmWordLearnRow NewRow(){
		return new VmWordLearnRow{
			Raw = new PoWordLearn{
				Id = new IdWordLearn(),
			},
			LearnResultIndex = 0,
			// 放最後，覆蓋屬性初始化時觸發的 MarkModified
			DmlState = EDmlState.Added,
		};
	}

	public static VmWordLearnRow FromPo(PoWordLearn Po){
		return new VmWordLearnRow{
			Raw = (PoWordLearn)Po.ShallowCloneSelf(),
			LearnResultIndex = GetLearnResultIndex(Po.LearnResult),
			BizCreatedAtIso = Po.BizCreatedAt.ToIso(),
			// 放最後，覆蓋屬性初始化時觸發的 MarkModified
			DmlState = EDmlState.Unchanged,
		};
	}

	public bool TryToPo(IdWord WordId, out PoWordLearn Po, out str Err){
		Err = "";
		Po = (PoWordLearn)Raw.ShallowCloneSelf();
		Po.WordId = WordId;
		Po.LearnResult = GetLearnResultByIndex(LearnResultIndex);
		try{
			Po.BizCreatedAt = UnixMs.FromIso(BizCreatedAtIso);
		}catch{
			Err = I18n[K.BizCreatedAtMustBeIsoTime];
			return false;
		}
		return true;
	}

	static i32 GetLearnResultIndex(ELearn Learn){
		for(i32 i = 0; i < LearnResults.Count; i++){
			if(LearnResults[i] == Learn){
				return i;
			}
		}
		return 0;
	}

	static ELearn GetLearnResultByIndex(i32 Index){
		if(Index < 0 || Index >= LearnResults.Count){
			return ELearn.Add;
		}
		return LearnResults[Index];
	}

	public str TranslateLearnResult(ELearn Learn){
		return Learn switch{
			ELearn.Add => I18n[K.Learn_Add],
			ELearn.Rmb => I18n[K.Learn_Rmb],
			ELearn.Fgt => I18n[K.Learn_Fgt],
			_ => Learn.ToString(),
		};
	}
}
