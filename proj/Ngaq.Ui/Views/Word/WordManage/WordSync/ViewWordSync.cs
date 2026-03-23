namespace Ngaq.Ui.Views.Word.WordManage.WordSync;

using Avalonia.Controls;
using Avalonia.Threading;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Ngaq.Ui.Tools;
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
		Ctx = App.GetRSvc<Ctx>();
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
		this.InitContent(Root.Grid, o=>{

		});
		Root.A(_StackPanel(), Sp=>{

			Sp.A(new OpBtn(), op=>{
				var o = op._Button;
				op.SetExe((Ct)=>Ctx?.PushAsy(Ct)!);
				//o.Content = I[K.Push];
				o.Content = ToolIcon.IconWithTitle(
					Svgs.CloudUpload.ToIcon()
					,I[K.Push]
				);
			})
			.A(new OpBtn(), o=>{
				o.SetExe((Ct)=>Ctx?.PullAsy(Ct));
				o.BtnContent = ToolIcon.IconWithTitle(
					Svgs.CloudDownload.ToIcon()
					,I[K.Pull]
				);
			})
			.A(_TextBlock(), o=>{
				o.Text = I[K.ExportPath];
			})
			.A(_TextBox(), o=>{
				o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.PathExport));
			})
			.A(new OpBtn(), o=>{
				o.BtnContent = ToolIcon.IconWithTitle(
					Svgs.DatabaseExport.ToIcon()
					,I[K.Export]
				);
				o.SetExe((Ct)=>Ctx?.ExportAsy(Ct));
			})

			.A(_Border(), o=>{
				o.Height = 10;
			})

			.A(_TextBlock(), o=>{
				o.Text = I[K.ImportPath];
			})
			.A(_TextBox(), o=>{
				o.Bind(o.PropText, CBE.Mk<Ctx>(x=>x.PathImport));
			})
			.A(new OpBtn(), o=>{
				o.BtnContent = ToolIcon.IconWithTitle(
					Svgs.DatabaseImport.ToIcon()
					,I[K.Import]
				);
				o.SetExe((Ct)=>Ctx?.ImportAsy(Ct));
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
