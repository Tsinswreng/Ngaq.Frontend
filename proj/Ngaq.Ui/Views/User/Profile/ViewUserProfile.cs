namespace Ngaq.Ui.Views.User.Profile;

using Avalonia.Controls;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmXxx;
public partial class ViewUserProfile
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewUserProfile(){
		Ctx = Ctx.Mk();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();
	public AutoGrid Root = new(IsRow: true);

	protected nil Style(){
		return NIL;
	}

	protected TextBox TxtBox(){
		return new TextBox();
	}

	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),
				RowDef(1, GUT.Auto),
			]);
		});
		Root.AddInit(TxtBox(), o=>{

		});

		Root.AddInit(new OpBtn(), o=>{
			o.BtnContent = "Logout"; //TODO i18n
			o.SetExt((Ct)=>Ctx?.LogoutAsy(Ct));
		});
		return NIL;
	}


}
