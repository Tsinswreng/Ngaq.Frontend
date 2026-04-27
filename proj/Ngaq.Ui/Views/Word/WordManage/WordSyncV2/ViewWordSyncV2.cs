namespace Ngaq.Ui.Views.Word.WordManage.WordSyncV2;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.WordSync;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ctx = VmWordSyncV2;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;


/// 單詞同步 V2 頁面。
/// 提供：
/// - 文件導入/導出（含文件選擇器）
/// - 雲端 Push / Pull
public partial class ViewWordSyncV2
	:AppViewBase
	,I_MkTitleMenu
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordSyncV2(){
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

	AutoGrid Root = new(IsRow:true);
	protected nil Render(){
		this.SetContent(Root.Grid);
		Root.A(new StackPanel(), Sp=>{
			Sp.A(new OpBtn(), o=>{
				o.SetExe((Ct)=>Ctx?.PushAsy(Ct)!);
				o.BtnContent = ToolIcon.IconWithTitle(
					Icons.CloudUpload().ToIcon(),
					I[K.Push]
				);
			})
			.A(new OpBtn(), o=>{
				o.SetExe((Ct)=>Ctx?.PullAsy(Ct));
				o.BtnContent = ToolIcon.IconWithTitle(
					Icons.CloudDownload().ToIcon(),
					I[K.Pull]
				);
			})

			.A(new Border(), o=>{
				o.Height = UiCfg.Inst.BaseFontSize * 0.5;
			})

			.A(new TextBlock(), o=>{
				o.Text = I[K.ExportPath];
			})
			.A(MkExportPathRow())
			.A(new OpBtn(), o=>{
				o.BtnContent = ToolIcon.IconWithTitle(
					Icons.DatabaseExport().ToIcon(),
					I[K.Export]
				);
				o.SetExe((Ct)=>Ctx?.ExportAsy(Ct));
			})

			.A(new Border(), o=>{
				o.Height = UiCfg.Inst.BaseFontSize * 0.5;
			})

			.A(new TextBlock(), o=>{
				o.Text = I[K.ImportPath];
			})
			.A(MkImportPathRow())
			.A(new OpBtn(), o=>{
				o.BtnContent = ToolIcon.IconWithTitle(
					Icons.DatabaseImport().ToIcon(),
					I[K.Import]
				);
				o.SetExe((Ct)=>Ctx?.ImportAsy(Ct));
			})
			;
		});
		return NIL;
	}

	Control MkImportPathRow(){
		var row = new AutoGrid(IsRow: false);
		row.ColDefs.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Auto),
		]);
		row.A(new TextBox(), o=>{
			o.CBind<Ctx>(o.PropText, x=>x.PathImport);
		})
		.A(new Button(), o=>{
			o.Content = Icons.FolderOutlinepenOutline().ToIcon().WithText(I[K.Browse]);
			o.Click += async (s,e)=>{
				var path = await PickImportPathAsy();
				if(!str.IsNullOrWhiteSpace(path) && Ctx is not null){
					Ctx.PathImport = path;
				}
			};
		});
		return row.Grid;
	}

	Control MkExportPathRow(){
		var row = new AutoGrid(IsRow: false);
		row.ColDefs.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Auto),
		]);
		row.A(new TextBox(), o=>{
			o.CBind<Ctx>(o.PropText, x=>x.PathExport);
		})
		.A(new Button(), o=>{
			o.Content = Icons.FolderOutlinepenOutline().ToIcon().WithText(I[K.Browse]);
			o.Click += async (s,e)=>{
				var path = await PickExportPathAsy();
				if(!str.IsNullOrWhiteSpace(path) && Ctx is not null){
					Ctx.PathExport = path;
				}
			};
		});
		return row.Grid;
	}

	/// 打開導入文件選擇器。
	/// <returns>選中文件絕對路徑；取消時返回 null。</returns>
	async Task<str?> PickImportPathAsy(){
		var provider = TopLevel.GetTopLevel(this)?.StorageProvider;
		if(provider is null){
			Ctx?.ShowDialog(Todo.I18n("StorageProvider unavailable"));
			return null;
		}
		var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions{
			Title = Todo.I18n("Select import file"),
			AllowMultiple = false,
		});
		foreach(var file in files){
			return ToPath(file);
		}
		return null;
	}

	/// 打開導出文件保存選擇器。
	/// <returns>目標文件絕對路徑；取消時返回 null。</returns>
	async Task<str?> PickExportPathAsy(){
		var provider = TopLevel.GetTopLevel(this)?.StorageProvider;
		if(provider is null){
			Ctx?.ShowDialog(Todo.I18n("StorageProvider unavailable"));
			return null;
		}
		var file = await provider.SaveFilePickerAsync(new FilePickerSaveOptions{
			Title = Todo.I18n("Select export file"),
			SuggestedFileName = "words-sync-v2.bin",
		});
		return file is null ? null : ToPath(file);
	}

	/// 把 Avalonia Storage 路徑轉成字符串路徑。
	/// <param name="File">文件句柄。</param>
	/// <returns>可用路徑字符串。</returns>
	static str ToPath(IStorageFile File){
		if(File.Path.IsFile){
			return File.Path.LocalPath;
		}
		return File.Path.AbsolutePath;
	}

	/// <summary>
	/// 頂欄菜單：跳轉到舊版備份同步頁。
	/// </summary>
	/// <returns>標題菜單控件。</returns>
	public Control MkTitleMenu(){
		var menu = new ContextMenu();
		menu.Items.A(new MenuItem(), o=>{
			var title = Todo.I18n("轉到舊版備份同步");
			o.Header = title;
			o.Click += (s,e)=>{
				ViewNavi?.GoTo(
					ToolView.WithTitle(
						I[K.BackupEtSync],
						new ViewWordSync()
					)
				);
			};
		});
		return menu;
	}
}
