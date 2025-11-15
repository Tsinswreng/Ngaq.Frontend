namespace Ngaq.Ui.Views.Word.WordManage.SearchWords.SearchedWordCard;
using System.Collections.ObjectModel;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Word.Models.Dto;
using Tsinswreng.CsTools;
using Ctx = VmSearchedWordCard;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Core.Shared.User.Models.Po;

public partial class VmSearchedWordCard
	:VmBaseWordListCard
{

	public new static ObservableCollection<Ctx> Samples = [];
	static VmSearchedWordCard(){
		#if DEBUG
		#endif
	}

	public static JnWord GetJnWordFromTypedObj(ITypedObj Obj){
		if(Obj.Type == typeof(JnWord)){
			return (JnWord)Obj.Data!;
		}else{
			var Dto = (DtoJnWordEtAsset)(Obj.Data!);
			return Dto.JnWord;
		}
	}

	public ITypedObj? TypedObj{get;set;}

	public Ctx FromTypedObj(ITypedObj Obj){
		TypedObj = Obj;
		WordForLearn = new WordForLearn(GetJnWordFromTypedObj(Obj));
		Init();
		return this;
	}

	protected override nil Init(){
		try{
			if(Bo == null){
				return NIL;
			}
			DelAt = Bo.DelAt;
			Head = Bo.Head;
			Lang = Bo.Lang;
			Index = Bo.Index;
			Weight = Bo.Weight;
			Learn_Records = Bo.Learn_Records;
			SavedLearnRecords = Bo.LearnRecords;
			LastLearnedTime = Bo.LastLearnedTime_();
			if(Learn_Records.TryGetValue(ELearn.Add, out var AddRecords)){
				FontColor = AddCntToFontColor(
					(u64)AddRecords.Count
				);
			}

			return NIL;
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}

	public IdDel? DelAt{
		get=>field;
		set {
			if (Bo is not null && value is not null) {
				Bo.DelAt = value.Value;
			}
			SetProperty(ref field, value);
		}
	}

}
