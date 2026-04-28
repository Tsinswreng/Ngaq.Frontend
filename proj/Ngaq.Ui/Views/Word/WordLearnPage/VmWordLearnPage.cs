namespace Ngaq.Ui.Views.Word.WordLearnPage;

using System.Collections.ObjectModel;
using System.Globalization;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Shared.Word.Models.Po.Learn;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Ui.Infra;
using Tsinswreng.CsTempus;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// 學習記錄分頁 ViewModel：管理列表、新增、轉換。
public partial class VmWordLearnPage: ViewModelBase{
	public event Action<VmWordLearnRow>? OnEditRequested;

	public ObservableCollection<VmWordLearnRow> Rows{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = [];

	public nil LoadFromPoLearns(IList<PoWordLearn> Learns){
		Rows = new ObservableCollection<VmWordLearnRow>(Learns.Select(VmWordLearnRow.FromPo));
		return NIL;
	}

	public nil AddRow(){
		Rows.Add(VmWordLearnRow.NewRow());
		return NIL;
	}

	public nil RemoveRow(VmWordLearnRow Row){
		Rows.Remove(Row);
		return NIL;
	}

	public nil RequestEdit(VmWordLearnRow Row){
		OnEditRequested?.Invoke(Row);
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

	public i32 LearnResultIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = 0;

	public str BizCreatedAtIso{
		get{return field;}
		set{SetProperty(ref field, value);}
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

	public static VmWordLearnRow NewRow(){
		return new VmWordLearnRow{LearnResultIndex = 0};
	}

	public static VmWordLearnRow FromPo(PoWordLearn Po){
		return new VmWordLearnRow{
			Raw = (PoWordLearn)Po.ShallowCloneSelf(),
			LearnResultIndex = GetLearnResultIndex(Po.LearnResult),
			BizCreatedAtIso = Po.BizCreatedAt.ToIso(),
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
