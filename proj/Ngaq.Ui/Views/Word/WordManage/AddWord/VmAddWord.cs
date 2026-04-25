namespace Ngaq.Ui.Views.Word.WordManage.AddWord;

using System.Collections.ObjectModel;
using MethodTimer;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Ui.Infra;
using Tsinswreng.CsI18n;
using Tsinswreng.CsTools;

using Ctx = VmAddWord;

public partial class VmAddWord
	:ViewModelBase
{
	public VmAddWord(){

	}

	public VmAddWord(
		ISvcWord? SvcWord = null
		,IFrontendUserCtxMgr? UserCtxMgr = null
	){
		this.SvcWord = SvcWord;
		this.UserCtxMgr = UserCtxMgr;
	}

	ISvcWord? SvcWord{get;set;}
	IFrontendUserCtxMgr? UserCtxMgr{get;set;}

	public static ObservableCollection<Ctx> Samples = [];
	static VmAddWord(){
		#if DEBUG
		{
			var o = new Ctx();
			o.Text = "hello\\ndictionary\\nexample";
			Samples.Add(o);
		}
		#endif
	}

	public str Text{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str ErrStr{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	[Time]
	public async Task<nil> Confirm(CT Ct){
		if(AnyNull(SvcWord, UserCtxMgr)){
			return NIL;
		}
		if(str.IsNullOrWhiteSpace(Text)){
			ShowDialog(Todo.I18n("Text is empty"));
			return NIL;
		}
		try{
			await SvcWord.AddWordsFromText(
				UserCtxMgr.GetUserCtx(),
				Text,
				Ct
			);
			ShowToast(Todo.I18n("Submitted"));
		}catch(Exception ex){
			ErrStr = ex.Message;
			HandleErr(ex);
		}
		return NIL;
	}
}
