using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Threading;
using Ngaq.Core.Shared.User.UserCtx;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

namespace Ngaq.Ui.CodeTemplate;

using Ctx = Ngaq.Ui.CodeTemplate.VmSample;

public partial class VmSample:ViewModelBase, IMk<Ctx>{
	//無參構造器聲明爲protected
	protected VmSample(){}
	//另置靜態無參工廠函數
	public static Ctx Mk(){
		return new Ctx();
	}
	public static ObservableCollection<Ctx> Samples = [];
	static VmSample(){
		#if DEBUG//放示例 便于調試樣式
		{
			var o = new Ctx();
			Samples.Add(o);
			//此處可 o.Foo = Bar
		}
		{//又一組示例
			var o = new Ctx();
			Samples.Add(o);
			//此處可 o.Foo = Bar
		}
		//...
		#endif
	}

	#region 依賴注入
	//依賴字段聲明成這樣。全部聲明爲可空類型
	ISvcSample? SvcSample;
	IUserCtxMgr? UserCtxMgr;
	//公開的有參構造器用于依賴注入。
	public VmSample(
		ISvcSample? SvcSample
		,IUserCtxMgr? UserCtxMgr
	){
		this.SvcSample = SvcSample;
		this.UserCtxMgr = UserCtxMgr;
	}
	#endregion 依賴注入


	//可綁定字段必須定義成這樣、無特殊情況(如轉發其他屬性)必須使用filed關鍵字
	public int Cnt1{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=0;
	public str Input{
		get{return field;}
		set{SetProperty(ref field, value);}
	}="";

	public ObservableCollection<str> List{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=["a","b","c"];

//給普通按鈕綁定的函數、無參非異步、只涉及ViewModel內部狀態的修改 無耗時操作
	public void Click1(){
		Cnt1++;
	}


//在ViewModel中調用後端服務示例
// 聲明爲異步函數、函數名不需特殊後綴、參數設爲CT Ct即可。
// 此函數用于給OpBtn綁定
	public async Task<nil> CallService(CT Ct){
//由于注入的依賴都是可空類型、調用時需先判空。
		if(AnyNull(SvcSample)){
			return NIL;
		}
//另開Task.Run來執行服務、防止UI卡頓
		await Task.Run(async ()=>{
			var R = await SvcSample.Serve(null, Ct);
//示例: 如果要在子線程中修改UI，須這樣寫
			Dispatcher.UIThread.Post(()=>{
				this.Input += R+"";
			});
		},Ct);
		return NIL;
	}

}

//模擬的服務類接口
public interface ISvcSample{
	public Task<obj?> Serve(obj? O, CT Ct);
}
