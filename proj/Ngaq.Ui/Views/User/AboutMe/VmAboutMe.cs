namespace Ngaq.Ui.Views.User.AboutMe;
using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Shared.User.UserCtx;
using Ngaq.Core.Tools;
using Ngaq.Ui.Infra;

using Ctx = VmAboutMe;
public partial class VmAboutMe: ViewModelBase{
	protected VmAboutMe(){}
	public static Ctx Mk(){
		return new Ctx();
	}
	public static ObservableCollection<Ctx> Samples = [];
	static VmAboutMe(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	IFrontendUserCtxMgr? UserCtxMgr;
	public VmAboutMe(
		IFrontendUserCtxMgr? UserCtxMgr
	){
		this.UserCtxMgr = UserCtxMgr;
		Init();
	}

	public nil Init(){
		if(UserCtxMgr is null){
			return NIL;
		}
		var User = UserCtxMgr.GetUserCtx();
		if(!User.LoginUserId.IsNullOrDefault()){
			UserIdRepr = User.LoginUserId+"";
		}
		return NIL;
	}


	protected str _UserIdRepr = "Not Logged in";//TODO i18n
	public str UserIdRepr{
		get{return _UserIdRepr;}
		set{SetProperty(ref _UserIdRepr, value);}
	}

}
