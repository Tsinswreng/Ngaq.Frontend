using System.Collections.ObjectModel;
using Ngaq.Core.Model.Bo;
using Ngaq.Core.Model.Bo.IFWord;
using Ngaq.Ui.ViewModels;

namespace Ngaq.Ui.Views.WordInfo;
using Ctx = VmWordInfo;
public partial class VmWordInfo
	:ViewModelBase
	,IVmWord
{

	public static ObservableCollection<Ctx> Samples = [];
	static VmWordInfo(){
		{
			var o = new Ctx();
			Samples.Add(o);
		}
	}

	public Ctx FromBo(BoWord BoWord){
		this.BoWord = BoWord;
		Id = BoWord.Id.ToString();
		Head = BoWord.PoWord.Head;
		Lang = BoWord.PoWord.Lang;
		return this;
	}

	public BoWord? BoWord{get;set;}

	protected str _Id = "";
	public str Id{
		get{return _Id;}
		set{SetProperty(ref _Id, value);}
	}


	protected str _Head = "";
	public str Head{
		get{return _Head;}
		set{SetProperty(ref _Head, value);}
	}

	protected str _Lang = "";
	public str Lang{
		get{return _Lang;}
		set{SetProperty(ref _Lang, value);}
	}

}
