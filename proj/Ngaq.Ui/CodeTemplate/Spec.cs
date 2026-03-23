#if false
Avalonia前端代碼規範
- 使用純c\# 不用任何xaml
命名規則
- 視圖: ViewXxx
- 視圖模型: VmXxx
保留原有代碼風格
#endif

//Render寫法:
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Styling;
using Ngaq.Core.Shared.User.UserCtx;
using Ngaq.Ui.CodeTemplate.Sample;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

namespace Ngaq.Ui.CodeTemplate;

using Ctx = Ngaq.Ui.CodeTemplate.VmSpec;

public partial class VmSpec:ViewModelBase, IMk<Ctx>{
	//無參構造器聲明爲protected
	protected VmSpec(){}
	//另置靜態無參工廠函數
	public static Ctx Mk(){
		return new Ctx();
	}
	public static ObservableCollection<Ctx> Samples = [];
	static VmSpec(){
		#if DEBUG//放示?便于調試樣式
		{
			var o = new Ctx();
			Samples.Add(o);
			//此處?o.Foo = Bar
		}
		{//又一組示?
			var o = new Ctx();
			Samples.Add(o);
			//此處?o.Foo = Bar
		}
		//...
		#endif
	}

	#region 依賴注入
	//依賴字段聲明成這樣。全部聲明爲可空類型
	ISvcSample? SvcSample;
	IUserCtxMgr? UserCtxMgr;
	//公開的有參構造器用于依賴注入?
	public VmSpec(
		ISvcSample? SvcSample
		,IUserCtxMgr? UserCtxMgr
	){
		this.SvcSample = SvcSample;
		this.UserCtxMgr = UserCtxMgr;
	}
	#endregion 依賴注入


	//可綁定字段必須定義成這樣、無特殊情況(如轉發其他屬?必須使用filed關鍵?
	public int Cnt1{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=0;

	public void Click1(){
		Cnt1++;
	}
	public int Cnt2{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=0;

	public void Click2(){
		Cnt1++;
	}

}


/*
//第二套寫?不推?
		.AddInit(new TextBox{
			AcceptsReturn = true,
			//對于[無法用屬性初始化賦值語法來初始化]的部分、可在Init函數中初始化
			Init=o=>{
				o.CBind<Ctx>(o.PropText,x=>x.Cnt2);
			}
		})
		.AddInit(new Button{
			Content = "按鈕?,
			Init=o=>{
				o.Click+=(s,e)=>{
					Ctx?.Click2();
				};
			}
		})
 */
