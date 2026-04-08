namespace Ngaq.Ui.Views.Dictionary.SimpleWord;

using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Shared.Audio;
using Ngaq.Core.Shared.Dictionary.Models;
using Ngaq.Core.Shared.Dictionary.Svc;
using Ngaq.Core.Shared.Word.Models.DictionaryApi;
using Ngaq.Ui.Infra;

using Ctx = VmSimpleWord;
public partial class VmSimpleWord: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmSimpleWord(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmSimpleWord(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
			o.Head = "word";
			o.Pronunciations = [Pronunciation.Sample.Samples[0]];
			o.Description =
"""
n.	詞；單詞；字；消息
v.	措辭；用詞
int.	說得對
网絡	話；一個字；文字處理
""";
		}
		#endif
	}

	/// TTS服務：把詞頭轉成可播放音頻。
	ISvcTts? SvcTts;
	/// 音頻播放器：真正執行播放。
	IAudioPlayer? AudioPlayer;
	/// 字典服務：用於拿到當前源語言配置。
	ISvcDictionary? SvcDictionary;
	/// 前端用戶上下文：獲取 DB 上下文給字典服務。
	IFrontendUserCtxMgr? FrontendUserCtxMgr;

	public VmSimpleWord(
		ISvcTts? SvcTts
		,IAudioPlayer? AudioPlayer
		,ISvcDictionary? SvcDictionary
		,IFrontendUserCtxMgr? FrontendUserCtxMgr
	){
		this.SvcTts = SvcTts;
		this.AudioPlayer = AudioPlayer;
		this.SvcDictionary = SvcDictionary;
		this.FrontendUserCtxMgr = FrontendUserCtxMgr;
	}


	public nil FromRespLlmDict(IRespLlmDict Resp){
		Head = Resp.Head;
		Pronunciations = Resp.Pronunciations.Select(p => new Pronunciation{
			TextType = p.TextType,
			Text = p.Text,
		}).ToList();
		var ParsedDescription = string.Join("\n", Resp.Descrs);
		// 解析出的描述為空時，保留流式階段已展示的文本，避免界面在收尾時變空白。
		if(!string.IsNullOrWhiteSpace(ParsedDescription)){
			Description = ParsedDescription;
		}
		return NIL;
	}


	/// 開始新的流式查詢，重置狀態

	public nil StartStreaming(string QueryTerm){
		Head = QueryTerm;
		Description = "";
		return NIL;
	}


	/// 接收流式響應的新片段

	public nil GotNewSeg(DtoOnNewSeg NewSeg){
		Description += NewSeg.NewSeg;
		return NIL;
	}

	/// 播放詞頭讀音：先調用 ISvcTts 取音頻，再調用 IAudioPlayer 播放。
	public async Task<nil> PlayHead(CT Ct){
		if(str.IsNullOrWhiteSpace(Head)){
			LogWarn($"{nameof(PlayHead)} skipped: Head is empty.");
			return NIL;
		}
		if(AnyNull(SvcTts, AudioPlayer, SvcDictionary, FrontendUserCtxMgr)){
			return NIL;
		}

		try{
			// step 1: 取當前詞典源語言，作為 TTS 的語言參數。
			var SrcLangPo = await SvcDictionary.GetCurSrcNormLang(
				FrontendUserCtxMgr.GetDbUserCtx(),
				Ct
			);
			var Lang = new NormLang{
				Type = ELangIdentType.Bcp47,
				Code = "en",
			};
			if(SrcLangPo is not null && !str.IsNullOrWhiteSpace(SrcLangPo.Code)){
				Lang.Type = SrcLangPo.Type;
				Lang.Code = SrcLangPo.Code;
			}else{
				LogWarn($"{nameof(PlayHead)}: CurSrcNormLang is null, fallback to en.");
			}

			// step 2: 把詞頭傳給 ISvcTts，獲取音頻數據。
			var Audio = await SvcTts.GetAudio(
				Head.Trim(),
				Lang
			);

			// step 3: 調用播放器播放（按返回音頻類型自動處理）。
			await AudioPlayer.Play(Audio, Ct);
			LogInfo($"{nameof(PlayHead)} success. Head={Head.Trim()}, Lang={Lang.Code}");
		}catch(Exception Ex){
			LogError($"{nameof(PlayHead)} failed. Head={Head.Trim()}");
			HandleErr(Ex);
		}

		return NIL;
	}



	public str Head{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	public IList<Pronunciation> Pronunciations{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=[];


	public str Description{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

}
