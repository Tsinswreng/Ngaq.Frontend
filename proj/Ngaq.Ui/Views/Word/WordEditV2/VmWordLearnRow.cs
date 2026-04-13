namespace Ngaq.Ui.Views.Word.WordEditV2;

using System.Globalization;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Shared.Word.Models.Po.Learn;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Ui.Infra;
using Tsinswreng.CsTempus;

/// 單詞學習記錄行編輯 ViewModel。
public partial class VmWordLearnRow: ViewModelBase{
	public static IReadOnlyList<ELearn> LearnResults{get;} = [
		ELearn.Add,
		ELearn.Rmb,
		ELearn.Fgt,
	];

	public PoWordLearn Raw{get;set;} = new();

	/// LearnResult 下拉索引。
	public i32 LearnResultIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = 0;

	public str BizCreatedAtIso{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = Tempus.Now().ToIso();

	/// 概要表格顯示用時間格式：yy-MM-dd HH:mm:ss。
	public str BizCreatedAtDisplay{
		get{
			try{
				var t = Tempus.FromIso(BizCreatedAtIso);
				var dto = DateTimeOffset.FromUnixTimeMilliseconds(t.Value);
				var local = TimeZoneInfo.ConvertTime(dto, TimeZoneInfo.Local);
				return local.ToString("yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			}catch{
				return BizCreatedAtIso;
			}
		}
	}

	/// 概要列表顯示字段。
	public str LearnResultText => GetLearnResultByIndex(LearnResultIndex).ToString();

	public static VmWordLearnRow NewRow(){
		return new VmWordLearnRow{
			LearnResultIndex = 0,
		};
	}

	public static VmWordLearnRow FromPo(PoWordLearn Po){
		var vm = new VmWordLearnRow{
			Raw = (PoWordLearn)Po.ShallowCloneSelf(),
			LearnResultIndex = GetLearnResultIndex(Po.LearnResult),
			BizCreatedAtIso = Po.BizCreatedAt.ToIso(),
		};
		return vm;
	}

	public bool TryToPo(IdWord WordId, out PoWordLearn Po, out str Err){
		Err = "";
		Po = (PoWordLearn)Raw.ShallowCloneSelf();
		Po.WordId = WordId;
		Po.LearnResult = GetLearnResultByIndex(LearnResultIndex);

		try{
			Po.BizCreatedAt = Tempus.FromIso(BizCreatedAtIso);
		}catch{
			Err = Todo.I18n("BizCreatedAt must be ISO time.");
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
}
