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
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

namespace Ngaq.Ui.CodeTemplate;
using Ctx = Ngaq.Ui.CodeTemplate.VmSpec;
public partial class VmSpec:ViewModelBase{
	public static ObservableCollection<Ctx> Samples = [];
	static VmSpec(){
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

	//可綁定字段必須定義成這樣
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

public partial class ViewSpec:AppViewBase{
	public Ctx? Ctx{//能用Ctx作別名的地方就寫Ctx、勿寫原全名
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}
	//大多數場景下我們用AutoGrid作爲視圖的根節點。
	//AutoGrid支持 或全爲行 或全爲列 的佈局 不建議同時設置行和例。每次Add時會自動設置行號或列號
	AutoGrid Root = new(IsRow: true);//IsRow: true 表示行佈局
	//視圖的初始化羅輯寫在Render裏
	public void Render(){
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Auto),
			RowDef(100, GUT.Pixel),
			//....略
		]);
		this.Content = Root.Grid;
		//AutoGrid 與 所有的Panel都有AddInit<TControl>(TControl C, Action<TControl>? FnInit=null)擴展方法。
		//常規寫法一
		Root
		//可鏈式調用
		.AddInit(new TextBox(), o=>{
			o.AcceptsReturn = true;
			o.Bind(
				o.PropText
				,CBE.Mk<Ctx>(
					x=>x.Cnt1
					#if false
					可加其他可選命名參數如:
					,Converter:
					,ConverterParameter:
					,Mode:
					#endif
				)
			);
			//參數不複雜時可只寫一句 o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.Cnt));
			//優先用o.PropText的寫法。如當o爲TextBox時o.PropText即等於TextBox.TextProperty。不得已時再用 類名.XxxProperty的寫法
		})
		//靜態綁定寫法
		.AddInit(new Button(), o=>{
			o.ContentInit(new TextBlock(), t=>{
				t.Text = "按鈕一";
			});
			//你也可以直接給o.Content賦值 o.Content = new TextBlock(){Text="按鈕一"};
			//按鈕綁定事件的寫法
			o.Click += (s,e)=>{
				Ctx?.Click1();
			};
		})
		//第二套寫法(不推薦)
		.AddInit(new TextBox{
			AcceptsReturn = true,
			//對于[無法用屬性初始化賦值語法來初始化]的部分、可在Init函數中初始化
			Init=o=>{
				o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.Cnt2));
			}
		})
		.AddInit(new Button{
			Content = "按鈕二",
			Init=o=>{
				o.Click+=(s,e)=>{
					Ctx?.Click2();
				};
			}
		})
		;
	}
	#region Style
	//樣式(非必選)示例
	//樣式簡單旹建議直接和控件一起初始化。有[需批量設置樣式]等需求旹再用Style
	public partial class Cls{//類名枚舉
		public const string MenuBtn = nameof(MenuBtn);
	}
	public void Style(){
		var S = this.Styles;
		var SomeStyle = new Style(x=>
			x.Is<Control>()
			.Class(Cls.MenuBtn)//禁止硬編碼字符串作類名
		).Set(
			VerticalAlignmentProperty
			,VAlign.Stretch
		).AddTo(S);
		;
	}

	#endregion Style

}
#if false
其他建議:
當View中縮進層次過多旹、可將Render中的部分代碼抽到一個單獨的函數中、把控件返回出去、再在Render中調用
#endif


