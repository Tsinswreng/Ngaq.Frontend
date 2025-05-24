using System.Collections.ObjectModel;
using Ngaq.Core.Model.UserCtx;
using Ngaq.Core.Service.Word;
using Ngaq.Ui.ViewModels;

namespace Ngaq.Ui.Views.WordManage.AddWord;
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
		this.Svc_Word = SvcWord!;
		this.UserCtxMgr = UserCtxMgr!;
	}

	ISvcWord Svc_Word{get;set;} = null!;
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


	public nil Confirm(){
		//System.Console.WriteLine(Text);//t +
		//System.Console.WriteLine(Svc_Word == null); false
		if(str.IsNullOrEmpty(Path) && str.IsNullOrEmpty(Text)){
			return Nil;
		}
		if(!str.IsNullOrEmpty(Text)){
			Svc_Word?.AddWordsFromTextAsy(
				UserCtxMgr.GetUserCtx()
				,Text
				,default //TODO ct
			).ContinueWith(d=>{
				if(d.IsFaulted){
					System.Console.WriteLine(d.Exception);//t
				}
			});
		}
		return Nil;
	}



}
