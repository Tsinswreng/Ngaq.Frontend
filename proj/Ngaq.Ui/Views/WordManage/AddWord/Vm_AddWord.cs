using System.Collections.ObjectModel;
using Ngaq.Core.Service.Word;
using Ngaq.Ui.ViewModels;

namespace Ngaq.Ui.Views.WordManage.AddWord;
using Ctx = Vm_AddWord;
public partial class Vm_AddWord
	:ViewModelBase
{

	public Vm_AddWord(){

	}

	public Vm_AddWord(I_Svc_Word? SvcWord = null){
		this.Svc_Word = SvcWord;
	}

	I_Svc_Word? Svc_Word{get;set;}


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
		if(str.IsNullOrEmpty(Path) || str.IsNullOrEmpty(Text)){
			return null!;
		}
		if(!str.IsNullOrEmpty(Text)){

		}
		return null!;
	}



}
