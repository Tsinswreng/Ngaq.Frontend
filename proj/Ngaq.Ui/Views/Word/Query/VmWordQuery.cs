using System.Collections.ObjectModel;
using Ngaq.Core.Model.Bo;
using Ngaq.Ui.ViewModels;
using Ngaq.Ui.Views.Word.WordCard;
using Ngaq.Ui.Views.Word.WordInfo;

namespace Ngaq.Ui.Views.Word.Query;
using Ctx = VmWordQuery;
public partial class VmWordQuery: ViewModelBase{

	public static ObservableCollection<Ctx> Samples = [];
	static VmWordQuery(){
		{
			var o = new Ctx();
			Samples.Add(o);
			for(var i = 0; i < 99; i++){
				o.WordCards.Add(VmWordListCard.Samples[0]);
				o.WordCards.Add(VmWordListCard.Samples[1]);
			}
		}
	}

	protected ObservableCollection<VmWordListCard> _WordCards = new();
	public ObservableCollection<VmWordListCard> WordCards{
		get{return _WordCards;}
		set{SetProperty(ref _WordCards, value);}
	}

	protected VmWordInfo _VmWordInfo = new();
	public VmWordInfo CurWordInfo{
		get{return _VmWordInfo;}
		set{SetProperty(ref _VmWordInfo, value);}
	}

	public nil SetCurBoWord(BoWord BoWord){
		CurWordInfo.FromBo(BoWord);
		// if(CurWordInfo != null){
		// 	CurWordInfo.FromBo(BoWord);
		// }
		// CurWordInfo = new();
		// CurWordInfo.FromBo(BoWord);
		return Nil;
	}


}
