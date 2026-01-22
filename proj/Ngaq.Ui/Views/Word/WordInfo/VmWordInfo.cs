using System.Collections.ObjectModel;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
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
		Descrs = ["Press start button to begin learning."];
		return NIL;
	}

	public nil SetPromptAfterStart(){
		Descrs = [
			"• Click on a word card above to start learning a word."+'\n'+
			""+'\n'+
			"• Top menu buttons:"+'\n'+
			"  - ▶️ Start: Load and begin learning words."+'\n'+
			"  - 💾 Save: Save your learning progress."+'\n'+
			"  - 🔄 Reset: Clear all progress and start over."+'\n'+
			"  - ⚙️ Settings: Configure learning preferences."+'\n'+
			""+'\n'+
			"• Learning a word:"+'\n'+
			"  - Click a word card to mark as remembered (green)."+'\n'+
			"  - Click again to mark as forgotten (red)."+'\n'+
			"  - Click once more to clear the mark (transparent)."+'\n'+
			""+'\n'+
			"• Other operations:"+'\n'+
			"  - Long-press a word card for context menu options."+'\n'+
			"  - Use the settings button to edit or add words."+'\n'+
			"  - Save progress regularly to avoid data loss."
		];
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
