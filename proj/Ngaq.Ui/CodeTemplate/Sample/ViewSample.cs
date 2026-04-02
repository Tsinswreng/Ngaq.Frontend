namespace Ngaq.Ui.CodeTemplate.Sample;

using Avalonia.Controls;
using Avalonia.Styling;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmSample;
public partial class ViewSample
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewSample(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;

	//大多數場景下我們用AutoGrid作爲視圖的根節點。
	//AutoGrid支持 或全爲行 或全爲列 的佈局 不建議同時設置行和例。每次Add時會自動設置行號或列號 因此不要手動設計行/列號
	AutoGrid Root = new(IsRow: true);//IsRow: true 表示行佈局
	//視圖的初始化羅輯寫在Render裏
	public void Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto), //GUT成員有Star,Auto,Pixel
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Auto),
			//....略
		]);
		//AutoGrid 與 所有的Panel都有 A<TControl>(TControl C, Action<TControl>? FnInit=null)擴展方法。
		//常規寫法一
		Root
		//可鏈式調用
		.A(new TextBox(), o=>{
			o.AcceptsReturn = true;
			//靜態綁定寫法。不准用new Binding(字符串)
			o.CBind<Ctx>(
				o.PropText,x=>x.Cnt1
				#if false
				可加其他可選命名參數如:
				,Converter:
				,ConverterParameter:
				,Mode:
				#endif
			);
//參數不複雜時可只寫一句 o.Bind<Ctx>(o.PropText, x=>x.Cnt);
//優先用o.PropText的寫法。如當o爲TextBox時o.PropText即等於TextBox.TextProperty。不得已時再用 類名.XxxProperty的寫法
		})
		.A(new Button(), o=>{
			o.SetContent(new TextBlock(), t=>{
				t.Text = Todo.I18n("按鈕一");//項目要支持i18n。禁止直接硬編碼。臨時硬編碼要寫成這樣。
			});
//你也可以直接給o.Content賦值 o.Content = new TextBlock(){Text="按鈕一"};
//按鈕綁定事件的寫法
			o.Click += (s,e)=>{
				Ctx?.Click1();
			};
		})
		.A(new ScrollViewer(), Sv=>{
			Sv.SetContent(new StackPanel(), Sp=>{
				Sp.A(new OpBtn(), o=>{
					//UI中硬編碼的字符串都要這樣寫Todo
					o._Button.Content = Todo.I18n("調用後端服務");
					o.SetExe(Ct=>Ctx?.CallService(Ct));
				});
				Sp.A(MkList());
			});
		})
		;
	}

//列表示例
//當View中縮進層次過多旹、可將Render中的部分代碼抽到一個單獨的函數中、把控件返回出去、再在Render中調用
	ItemsControl MkList(){
		var R = new ItemsControl();
		R.SetItemTemplate<str>((ele, ns)=>{
			return new TextBox{Text=ele};
		});
		R.SetItemsPanel(()=>{
			return new VirtualizingStackPanel();
		});
		R.Bind(R.PropItemsSource, CBE.Mk<Ctx>(x=>x.List));
		return R;
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

	//導航示例
	public void SampleNavi(Control TargetView){
		//帶標題跳轉
		Ctx?.ViewNavi?.GoTo(ToolView.WithTitle(Todo.I18n("頂欄標題"), TargetView));
		//若不要標題就直接傳進Goto
	}

	public void SampleIcon(){
		//基礎示例
		Avalonia.Controls.Shapes.Path addIcon = Svgs.Add().ToIcon();
		var b1 = new Button();
		b1.SetContent(addIcon);
// Svgs下 可用的圖標 在 Ngaq.Frontend/proj/Ngaq.Ui/Icons/Svgs.Decl.cs
// 禁止閱讀 Svgs.Impl.cs !!!!
		//圖標接文字示例:
		var b2 = new Button();
		b2.SetContent(Svgs.Add().ToIcon().WithText("Add"));
	}

	public void SampleOfUsingA(){
		var 正確示例 = (Panel p)=>{
			//所有Panel都能
			p.A(new TextBox(), o=>{
				//在這裏通過o.Xxx初始化
				o.Text = Todo.I18n("正確示例");
			})
			//鏈試調用
			.A(new Control())//第二個Action<>可省略不傳
			;
		};
		var 錯誤示例 = (Panel p)=>{
			var textBox = new TextBox{
				Text = "錯誤示例"
			};
			p.Children.Add(textBox);
			p.Children.Add(new Control());
			//錯誤原因: 未使用A擴展方法; 初始化控件屬性寫法不符合規範
		};
	}

}
