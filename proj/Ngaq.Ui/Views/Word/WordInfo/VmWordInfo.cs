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

	public nil SetPrompt(){
		Descrs = [
			"• Click on a word card above to start learning a word.",
			"",
			"• Top menu buttons:",
			"  - ▶️ Start: Load and begin learning words.",
			"  - 💾 Save: Save your learning progress.",
			"  - 🔄 Reset: Clear all progress and start over.",
			"  - ⚙️ Settings: Configure learning preferences.",
			"",
			"• Learning a word:",
			"  - Click a word card to mark as remembered (green).",
			"  - Click again to mark as forgotten (red).",
			"  - Click once more to clear the mark (transparent).",
			"",
			"• Other operations:",
			"  - Long-press a word card for context menu options.",
			"  - Use the settings button to edit or add words.",
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
