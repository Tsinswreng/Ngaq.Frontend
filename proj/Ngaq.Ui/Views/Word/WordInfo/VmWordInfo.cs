using System.Collections.ObjectModel;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Word.Models.Samples;
using Ngaq.Ui.Infra;
using Tsinswreng.CsTools;

namespace Ngaq.Ui.Views.Word.WordInfo;
using Ctx = VmWordInfo;
public partial class VmWordInfo
	:ViewModelBase
	//,IVmWord
{

	public static ObservableCollection<Ctx> Samples = [];
	static VmWordInfo(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
			o.FromIWordForLearn(
				new WordForLearn(SampleWord.Inst.Samples[1])
			);
		}
		#endif
	}

	public Ctx FromIWordForLearn(IWordForLearn Word){
		Id = Word.Id.ToString();
		Head = Word.Head;
		Lang = Word.Lang;
		var NeoStrProps = new Dictionary<str, IList<str>>();
		foreach(var (strKey, props) in Word.StrKey_Props){
			foreach(var prop in props){
				NeoStrProps!.AddInValues(strKey, prop.VStr);
			}
		}
		StrProps = NeoStrProps;
		return this;
	}

	// [Obsolete]
	// public Ctx FromBo(JnWord BoWord){
	// 	//this.BoWord = BoWord;
	// 	Id = BoWord.Id.ToString();
	// 	Head = BoWord.PoWord.Head;
	// 	Lang = BoWord.PoWord.Lang;

	// 	var NeoStrProps = new Dictionary<str, IList<str>>();
	// 	foreach(var Prop in BoWord.Props){
	// 		if(Prop.KType == (i64)EKvType.Str
	// 			&& Prop.VType == (i64)EKvType.Str
	// 		){
	// 			NeoStrProps.AddInValues(Prop.KStr, Prop.VStr);
	// 		}
	// 	}
	// 	//斯集合未叶INotifyPropertyChanged、故唯當地址變旹纔緟渲染Ui
	// 	// 直ᵈ 不變地址ᵈ 改StrProps之內容則不效
	// 	// 改內容後汶SetProperty(ref _StrProps, StrProps);亦不效
	// 	this.StrProps = NeoStrProps;
	// 	return this;
	// }

	public IWordForLearn? WordForLearn{get;set;}

	//public JnWord? BoWord{get;set;}

	public str Id{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";



	public str Head{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str Lang{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";


	public IDictionary<str, IList<str>> StrProps{
		get{return field;}
		set{SetProperty(ref field, value);}
	}= new Dictionary<str, IList<str>>();



}
