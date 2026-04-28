using System.Collections.ObjectModel;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Word.Models.Samples;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.CsTools;

namespace Ngaq.Ui.Views.Word.WordInfo;
using Ctx = VmWordInfo;
using K = KeysUiI18nCommon;
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
			if(strKey == KeysProp.Inst.description){
				Descrs = props.Select(prop => prop.VStr).ToList();
			}else{
				foreach(var prop in props){
					NeoStrProps!.AddInValues(strKey, prop.VStr);
				}
			}
		}
		StrProps = NeoStrProps;
		return this;
	}

	public nil SetPromptBeforeStart(){
		ClearWordFields();
		Descrs = [I18n[K.PressStartButtonToBeginLearning]];
		return NIL;
	}

	public nil SetPromptAfterStart(){
		ClearWordFields();
		Descrs = [I18n[K.WordLearningHelpText_]];
		return NIL;
	}

	/// 清空當前單詞展示狀態，避免提示文本與上一個詞的內容混在一起。
	public nil ClearWordFields(){
		Id = "";
		Head = "";
		Lang = "";
		StrProps = new Dictionary<str, IList<str>>();
		return NIL;
	}


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


	public IList<str> Descrs{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = [];


	public IDictionary<str, IList<str>> StrProps{
		get{return field;}
		set{SetProperty(ref field, value);}
	}= new Dictionary<str, IList<str>>();



}
