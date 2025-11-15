namespace Ngaq.Ui.Views.Word.WordManage.WordSync;

using Avalonia.Controls;
using Avalonia.Threading;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordSync;
using K = Infra.I18n.ItemsUiI18n.SyncWord;

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

	public partial class Cls_{

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

			Sp.AddInit(new OpBtn(), op=>{
				var o = op._Button;
				op.SetExt((Ct)=>Ctx?.PushAsy(Ct)!);
				o.Content = I[K.Push];
			})
			.AddInit(new OpBtn(), o=>{
				o.BtnContent = I[K.Pull];
				o.SetExt((Ct)=>Ctx?.PullAsy(Ct));
			})
			.AddInit(_TextBlock(), o=>{
				o.Text = I[K.ExportPath];
			})
			.AddInit(_TextBox(), o=>{
				o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.PathExport));
			})
			.AddInit(new OpBtn(), o=>{
				o.BtnContent = I[K.Export];
				o.SetExt((Ct)=>Ctx?.ExportAsy(Ct));
			})

			.AddInit(_Border(), o=>{
				o.Height = 10;
			})

			.AddInit(_TextBlock(), o=>{
				o.Text = I[K.ImportPath];
			})
			.AddInit(_TextBox(), o=>{
				o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.PathImport));
			})
			.AddInit(new OpBtn(), o=>{
				o.BtnContent = I[K.Import];
				o.SetExt((Ct)=>Ctx?.ImportAsy(Ct));
			})
			;
			// .AddInit(_Button(), o=>{
			// 	o.Content = "Test";
			// 	o.Click+=(s,e)=>{
			// 		Ctx?.ShowMsg("Test");
			// 	};
			// })
			// .AddInit(_Button(), o=>{
			// 	o.Content = "TestLong";
			// 	o.Click+=(s,e)=>{
			// 		Ctx?.ShowMsg(LongText);
			// 	};
			// })

			;
		});
		return NIL;
	}

}
