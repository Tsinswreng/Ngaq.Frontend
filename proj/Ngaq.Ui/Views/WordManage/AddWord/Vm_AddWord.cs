using System.Collections.ObjectModel;
using Ngaq.Core.Model.UserCtx;
using Ngaq.Core.Service.Word;
using Ngaq.Ui.ViewModels;

namespace Ngaq.Ui.Views.WordManage.AddWord;
using Ctx = Vm_AddWord;
public partial class Vm_AddWord
	:ViewModelBase
{

	public Vm_AddWord(){

	}

	public Vm_AddWord(
		I_Svc_Word? SvcWord = null
		,I_UserCtxMgr? UserCtxMgr = null
	){
		this.Svc_Word = SvcWord!;
		this.UserCtxMgr = UserCtxMgr!;
	}

	I_Svc_Word Svc_Word{get;set;} = null!;
	I_UserCtxMgr UserCtxMgr{get;set;} = null!;

	public static ObservableCollection<Ctx> Samples = [];
	static Vm_AddWord(){
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
