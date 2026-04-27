namespace Ngaq.Ui.CodeTemplate.Sample;

using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Styling;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsCore;
using Ctx = VmSample;using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;
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
		Loaded += (s,e)=>{
			//只在構造函數中 做 UI初始化相關操作
			//如果希望在進入界面的時候做一些其他的初始化操作 如調用接口獲取數據/耗時操作 等、應放在Loaded回調中
		};
	}

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
			//初始化ContentControl.Content時使用SetContent擴展方法、不要直接給Content賦值
			o.SetContent(new TextBlock(), t=>{
				t.Text = I[K.ButtonOne];//項目要支持i18n。禁止直接硬編碼。臨時硬編碼要寫成這樣。
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
					o._Button.Content = I[K.CallBackendService];
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
	public void SampleNavi(Button Btn, Func<Control> MkTargetView){
		//帶標題跳轉
		Btn.Click += (s,e)=>{
			//若不要標題就直接傳進Goto
			// 界面跳轉 只能在View層操作。不要在Vm層做頁面跳轉 或使用任何View層的東西
			var TargetView = MkTargetView(); //用Func<>實現延遲加載
			ViewNavi?.GoTo(ToolView.WithTitle(I[K.TopbarTitle], TargetView));
			_ = @$"如果你希望頂欄標題有自定義菜單按鈕 需讓TargetView實現{nameof(I_MkTitleMenu)}";
		};
	}

	public void SampleIcon(){
		//基礎示例
		Avalonia.Controls.Shapes.Path addIcon = Icons.Add().ToIcon();
		var b1 = new Button();
		b1.SetContent(addIcon);
// Svgs下 可用的圖標 在 Ngaq.Frontend/proj/Ngaq.Ui/Icons/Svgs.Decl.cs
// 禁止閱讀 Svgs.Impl.cs !!!!
		//圖標接文字示例:
		var b2 = new Button();
		b2.SetContent(Icons.Add().ToIcon().WithText("Add"));
	}


	[Doc(@$"使用Tsinswreng.AvlnTool的Dsl。
	要求:
	- 在lambda中 用o.Xxx = Yyy的寫法初始化 加入控件樹的子控件、而不是使用屬性初始化塊
	- 組織子控件並加入控件樹時、代碼塊的嵌套 要和 樹的邏輯結構 保持一致
	")]
	public void SampleOfUsingDsl(){
		var 正確示例 = (Panel p)=>{
			//所有Panel都能用.A()擴展方法
			p.A(new TextBox(), o=>{
				//在這裏通過o.Xxx初始化
				o.Text = I[K.CorrectExample];
			})
			//鏈試調用
			.A(new Control())//第二個Action<>可省略不傳
			.A(new Button(), o=>{
				//第二個Action<>可省略。
				// 當給Content賦值 需要初始化的對象時、使用SetContent+lambda的寫法。
				// 如果只是賦值簡單對象 不需額外初始化的、就可以直接 o.Content=xxx
				o.SetContent(new TextBlock(), o=>{
					o.FontSize = UiCfg.Inst.BaseFontSize*1.2;
				});
			})
			.A(new Border(), bdr=>{ // 組織子控件並加入控件樹時、代碼塊的嵌套 要和 樹的邏輯結構 保持一致
				bdr.SetChild(new ScrollViewer(), sv=>{
					sv.SetContent(new StackPanel(), sp=>{
						sp.A(new TextBlock())
						.A(new TextBox());
					});
				});
			})
			;
		};
		var 錯誤示例 = (Panel p)=>{
			var textBox = new TextBox{
				Text = "錯誤示例"
			};
			p.Children.Add(textBox);
			p.Children.Add(new Control());
			var btn = new Button();
			var textBlock = new TextBlock(){
				FontSize = 20
			};
			btn.Content = textBlock;
			//錯誤原因: 未使用A和SetContent擴展方法;
			// 初始化控件屬性寫法不符合規範; 硬編碼字體大小; 硬編碼文本未使用Todo.I18n
			// 代碼塊的嵌套 未和 樹的邏輯結構 保持一致
		};
	}

	public void SampleNoDupliStyle(){
		var 錯誤示例 = (Panel P)=>{
			P.A(new Button(), o=>{
				o.Background = Brushes.Gray;
				o.VAlign(x=>x.Center);
				o.HAlign(x=>x.Center);
				o.SetContent(I[K.File]);
			})
			.A(new Button(), o=>{
				o.Background = Brushes.Gray;
				o.VAlign(x=>x.Center);
				o.HAlign(x=>x.Center);
				o.SetContent(I[K.Edit]);
			})
			;
		};
		var 正確示例1 = (Panel P)=>{
			var _Btn = ()=>{
				var o = new Button();
				o.Background = Brushes.Gray;
				o.VAlign(x=>x.Center);
				o.HAlign(x=>x.Center);
				return o;
			};
			P.A(_Btn(), o=>{
				o.SetContent(I[K.File]);
			})
			.A(_Btn(), o=>{
				o.SetContent(I[K.Edit]);
			})
			;
		};
		var 正確示例2 = (Panel P)=>{
			P.Styles.A(
				new Style(x=>x.Is<Button>().Class(Cls.MenuBtn))
				.Set(HorizontalAlignmentProperty, HAlign.Center)
				.Set(VerticalAlignmentProperty, VAlign.Center)
				.Set(BackgroundProperty, Brushes.Gray)
			)

			;
			P.A(new Button(), o=>{
				o.Classes.Add(Cls.MenuBtn);
				o.SetContent(I[K.File]);
			})
			.A(new Button(), o=>{
				o.Classes.Add(Cls.MenuBtn);
				o.SetContent(I[K.Edit]);
			});
		};
	}


}

