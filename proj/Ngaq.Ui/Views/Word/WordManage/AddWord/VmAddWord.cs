using System.Collections.ObjectModel;
using Ngaq.Core.Model.UserCtx;
using Ngaq.Core.Word.Svc;
using Ngaq.Ui.Infra;

namespace Ngaq.Ui.Views.Word.WordManage.AddWord;
using Ctx = VmAddWord;
public partial class VmAddWord
	:ViewModelBase
{

	public VmAddWord(){

	}

	public VmAddWord(
		ISvcWord? SvcWord = null
		,IUserCtxMgr? UserCtxMgr = null
	){
		this.SvcWord = SvcWord!;
		this.UserCtxMgr = UserCtxMgr!;
	}

	ISvcWord SvcWord{get;set;} = null!;
	IUserCtxMgr UserCtxMgr{get;set;} = null!;

	public static ObservableCollection<Ctx> Samples = [];
	static VmAddWord(){
		{
			var o = new Ctx();
			Samples.Add(o);

		}
	}

	protected str _Path = "";
	public str Path{
		get{return _Path;}
		set{SetProperty(ref _Path, value);}
	}

	protected str _Text = "";
	public str Text{
		get{return _Text;}
		set{SetProperty(ref _Text, value);}
	}


	protected str _ErrStr="";//t
	public str ErrStr{
		get{return _ErrStr;}
		set{SetProperty(ref _ErrStr, value);}
	}


	public nil Confirm(){
		if(str.IsNullOrEmpty(Path) && str.IsNullOrEmpty(Text)){
			return NIL;
		}
		if(!str.IsNullOrEmpty(Text)){
			SvcWord?.AddWordsFromText(
				UserCtxMgr.GetUserCtx()
				,Text
				,default //TODO ct
			).ContinueWith(d=>{
				if(d.IsFaulted){
					Console.WriteLine(d.Exception);//t
					AddMsg(d.Exception.ToString());
					//ErrStr = d.Exception.ToString();
					ShowMsg();
				}
			});
		}
		return NIL;
	}



}
