namespace Ngaq.Ui.Components.PageBar;

using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using Avalonia.Styling;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmPageBar;
public partial class ViewPageBar
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewPageBar(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls{
		public static str CenterInput = nameof(CenterInput);
		public static str CenterText = nameof(CenterText);
	}


	protected nil Style(){
		var S = Styles;

		var centerText = new Style(
			x=>x.Class(Cls.CenterInput)
		).Set(VerticalContentAlignmentProperty, VAlign.Center)
		.Set(HorizontalContentAlignmentProperty, HAlign.Center)
		.AddTo(S);

		S.A(new Style(
				x=>x.Class(Cls.CenterText)
			).Set(VerticalAlignmentProperty, VAlign.Center)
		);

		return NIL;
	}


	AutoGrid Root = new(IsRow: false);
	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
			ColDef(1, GUT.Auto),
		]);
		Root
		.A(MkPageBtn(), o=>{
			o.BtnContent = Svgs.ArrowCircleLeftFill().ToIcon();
			o.SetExe((Ct)=>Ctx?.FnPrefPage?.Invoke(Ctx, Ct));
		})
		.A(new TextBox(), o=>{
			o.Classes.A(Cls.CenterInput);
			o.CBind<Ctx>(o.PropText, x=>x.PageNum);
		})

		.A(new TextBlock(), o=>{
			o.Classes.A(Cls.CenterText);
			o.CBind<Ctx>(o.PropIsVisible, x=>x.TotCnt,
				Converter: new ParamFnConvtr<u64?, bool>((x,p)=>x is not null)
			);
			o.CBind<Ctx>(o.PropText, x=>x.TotPageCnt,
			Converter: new ParamFnConvtr<u64?, str>((x,p)=>$"/{x}")
			);
		})
		.A(new TextBox(), o=>{
			o.Classes.A(Cls.CenterInput);
			o.CBind<Ctx>(o.PropText, x=>x.PageSize);
		})
		.A(MkPageBtn(), o=>{
			o.BtnContent = Svgs.ArrowCircleRightFill().ToIcon();
			o.SetExe((Ct)=>Ctx?.FnNextPage?.Invoke(Ctx, Ct));
		})
		;
		return NIL;
	}

	OpBtn MkPageBtn(){
		var r = new OpBtn();
		r._Button.HorizontalAlignment = HAlign.Stretch;
		r._Button.VerticalAlignment = VAlign.Stretch;
		return r;
	}
}

