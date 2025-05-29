using System.Collections.ObjectModel;
using Ngaq.Core.Model.Bo;
using Ngaq.Core.Model.Bo.IFWord;
using Ngaq.Core.Model.Po.Kv;
using Ngaq.Core.Model.Samples.Word;
using Ngaq.Ui.ViewModels;
using Tsinswreng.CsCore.Tools.MultiDict;

namespace Ngaq.Ui.Views.WordInfo;
using Ctx = VmWordInfo;
public partial class VmWordInfo
	:ViewModelBase
	,IVmWord
{

	public static ObservableCollection<Ctx> Samples = [];
	static VmWordInfo(){
		var S = SampleWord.Inst;
		{
			var o = new Ctx();
			Samples.Add(o);
			o.FromBo(S.BoWord);
		}
	}

	public Ctx FromBo(BoWord BoWord){
		this.BoWord = BoWord;
		Id = BoWord.Id.ToString();
		Head = BoWord.PoWord.Head;
		Lang = BoWord.PoWord.Lang;

		foreach(var Prop in BoWord.Props){
			if(Prop.KType == (i64)EKvType.Str
				&& Prop.VType == (i64)EKvType.Str
			){
				StrProps.AddInValues(Prop.KStr, Prop.VStr);
			}
		}

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

	public IDictionary<str, IList<str>> StrProps{get;set;} = new Dictionary<str, IList<str>>();






}
