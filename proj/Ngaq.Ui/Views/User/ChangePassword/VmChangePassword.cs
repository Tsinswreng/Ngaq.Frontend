namespace Ngaq.Ui.Views.User.ChangePassword;
using System.Collections.ObjectModel;
using Ngaq.Ui.Infra;
using Ngaq.Core.Frontend.User;
using Avalonia.Threading;

using Ctx = VmChangePassword;
public partial class VmChangePassword: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmChangePassword(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmChangePassword(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	#region 依賴注入
	//依賴字段聲明成這樣。全部聲明爲可空類型
	IFrontendUserCtxMgr? UserCtxMgr;
	//公開的有參構造器用于依賴注入。
	public VmChangePassword(
		IFrontendUserCtxMgr? UserCtxMgr
	){
		this.UserCtxMgr = UserCtxMgr;
	}
	#endregion 依賴注入

	public str OldPassword{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	public str NewPassword{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	public str ConfirmPassword{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

//異步函數用于給OpBtn綁定
	public async Task<nil> ChangePassword(CT Ct){
		if(AnyNull(UserCtxMgr)){
			return NIL;
		}
		if(NewPassword != ConfirmPassword){
			ShowDialog("新密碼和確認密碼不一致");
			return NIL;
		}
		if(str.IsNullOrWhiteSpace(OldPassword) || str.IsNullOrWhiteSpace(NewPassword)){
			ShowDialog("請填寫所有密碼字段");
			return NIL;
		}
		try{
			await Task.Run(async ()=>{
				// TODO: 調用用戶服務修改密碼
				// 目前先模擬成功
				await Task.Delay(1000); // 模擬網絡延遲
				Dispatcher.UIThread.Post(()=>{
					ShowDialog("密碼修改成功");
					ClearFields();
				});
			},Ct);
		}catch(Exception ex){
			HandleErr(ex);
		}
		return NIL;
	}

	private void ClearFields(){
		OldPassword = "";
		NewPassword = "";
		ConfirmPassword = "";
	}
}
