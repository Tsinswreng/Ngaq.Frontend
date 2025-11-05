namespace Ngaq.Ui.Views.Word.WordManage;

using Ngaq.Ui.Infra.I18n;

using Avalonia.Controls;


using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.AddWord;
using Ngaq.Ui.Views.Word.WordManage.SearchWords;
using Ngaq.Ui.Views.Word.WordManage.WordSync;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = Ngaq.Ui.Infra.ViewModelBase;

using K = Ngaq.Ui.Infra.I18n.ItemsUiI18n.Library;
using Ngaq.Ui.Infra.Ctrls;

public partial class ViewWordManage
	:UserControl
{

	public II18n I = I18n.Inst;
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordManage(){
		Ctx = new Ctx();
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
			o.RowDefinitions.AddRange([
				RowDef(9999, GUT.Star),
				RowDef(1, GUT.Star),
			]);
		});
		Root.AddInit(_StackPanel(), stk=>{
			stk.AddInit(_Item(I[K.SearchWords], new ViewSearchWords()));
			stk.AddInit(_Item(I[K.AddWords], new ViewAddWord()));
			stk.AddInit(_Item(I[K.BackupEtSync], new ViewWordSync()));
			stk.AddInit(new OpBtn(), o=>{
				o.SetExt(async(Ct)=>{
					await Task.Run(async ()=>{
						await Task.Delay(600000);
					});
					return NIL;
				});
				o.BtnContent = "測試";
			});
		});


		return NIL;
	}

	protected Control _Item(str Title, ContentControl Target){
		var R = new SwipeLongPressBtn();
		var titled = ToolView.WithTitle(Title, Target);
		R.Click += (s,e)=>{
			Ctx?.ViewNavi?.GoTo(titled);
		};
		R.HorizontalContentAlignment = HAlign.Left;
		R.ContentInit(_TextBlock(), o=>{
			o.Text = Title;
		});
		return R;
	}


}
