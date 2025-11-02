namespace Ngaq.Ui.Views.Word.WordManage.WordSync;

using Avalonia.Controls;
using Avalonia.Media;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordSync;
using K = Infra.I18n.ItemsUiI18n.Library;
public partial class ViewWordSync
	:UserControl
{
	public II18n I = I18n.Inst;
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordSync(){
		Ctx = App.GetSvc<Ctx>();
		Style();
		Render();
	}

	public  partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return NIL;
	}

	public AutoGrid Root = new(IsRow:true);
	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{

		});
		Root.AddInit(_StackPanel(), Sp=>{
			Sp.AddInit(_Button(), o=>{
				o.Content = "Push";
				o.Click += (s,e)=>{
					Ctx?.Push();
				};
			});
			Sp.AddInit(_Button(), o=>{
				o.Content = "Pull";
			})
			.AddInit(_Button(), o=>{
				o.Content = "Test";
				o.Click+=(s,e)=>{
					Ctx?.ShowMsg("Test");
				};
			})
			.AddInit(_Button(), o=>{
				o.Content = "TestLong";
				o.Click+=(s,e)=>{
					Ctx?.ShowMsg(LongText);
				};
			})

			;
		});
		return NIL;
	}

		public const str LongText =
"""
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]""Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]""Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
"Font size must be betwen in (0, 64]"
""";



}
