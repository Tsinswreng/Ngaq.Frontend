namespace Ngaq.Ui.KeyBoard.Dks;

using Avalonia.Controls;
using Avalonia.Input;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = Ngaq.Ui.Infra.ViewModelBase;
public partial class ViewDks
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewDks(){
		//Ctx = Ctx.Mk();
		Ctx = new Ctx();
		Style();
		Render();
	}

	public  partial class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		Styles.Add(MkrStyle.GridShowLines());
		return NIL;
	}

	public i32 KeyLen{get;set;} = 100;

	AutoGrid Root = new (IsRow: true);
	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				..Repeat(()=>RowDef(1, GUT.Auto), 6)
			]);
		});
		var Fx = new AutoGrid(IsRow: false);
		Root.AddInit(Fx.Grid, o=>{
			o.ColumnDefinitions.AddRange([
				..Repeat(()=>ColDef(KeyLen, GUT.Pixel), 6),
				ColDef(KeyLen/2, GUT.Pixel),
				..Repeat(()=>ColDef(KeyLen, GUT.Pixel), 4),
				ColDef(KeyLen/2, GUT.Pixel),
			]);
			var K = FnAddKey(Fx);
			K("Esc");
			Fx.AddInit(Blank());
			K("F1");
			K("F2");
			K("F3");
			K("F4");
			Fx.AddInit(Blank());
			K("F5");
			K("F6");
			K("F7");
			K("F8");
			Fx.AddInit(Blank());
			K("F9");
			K("F10");
			K("F11");
			K("F12");
		});
		var Num = new AutoGrid(IsRow: false);
		Root.AddInit(Num.Grid, o=>{
			o.ColumnDefinitions.AddRange([
				..Repeat(()=>ColDef(KeyLen, GUT.Pixel), 13),
				ColDef(KeyLen*2, GUT.Pixel),//backspace
			]);
			var K = FnAddKey2(Num);
			K("`", "~");
			K("1","!");
			K("2","@");
			K("3","#");
			K("4","$");
			K("5","%");
			K("6","^");
			K("7","&");
			K("8","*");
			K("9","(");
			K("0",")");
			K("-","_");
			K("=","+");
			K("⌫", "");
		});
		var Qwe = new AutoGrid(IsRow: false);
		Root.AddInit(Qwe.Grid, o=>{
			o.ColumnDefinitions.AddRange([
				ColDef(KeyLen*1.4, GUT.Pixel),//tab
				..Repeat(()=>ColDef(KeyLen*1.5, GUT.Pixel), 12),
				ColDef(KeyLen*1.7, GUT.Pixel),//\
			]);
			var K = FnAddKey(Qwe);
			var K2 = FnAddKey2(Qwe);
			K("Tab");
			K("Q");
			K("W");
			K("E");
			K("R");
			K("T");
			K("Y");
			K("U");
			K("I");
			K("O");
			K("P");
			K2("[", "{");
			K2("]", "}");
			K2("\\", "|");
		});
		var Asd = new AutoGrid(IsRow: false);
		Root.AddInit(Asd.Grid, o=>{
			o.ColumnDefinitions.AddRange([
				ColDef(KeyLen*1.7, GUT.Pixel),//caps
				..Repeat(()=>ColDef(KeyLen*1.5, GUT.Pixel), 11),
				ColDef(KeyLen*1.7, GUT.Pixel),//enter
			]);
			var K = FnAddKey(Asd);
			var K2 = FnAddKey2(Asd);
			K("CapsLk");
			K("A");
			K("S");
			K("D");
			K("F");
			K("G");
			K("H");
			K("J");
			K("K");
			K("L");
			K2(":", ";");
			K2("\"", "'");
			K("↲");
		});
		var Zxc = new AutoGrid(IsRow: false);
		Root.AddInit(Zxc.Grid, o=>{
			o.ColumnDefinitions.AddRange([
				ColDef(KeyLen*2, GUT.Pixel),//Shift
				..Repeat(()=>ColDef(KeyLen, GUT.Pixel), 10),
				ColDef(KeyLen*2, GUT.Pixel),//shift
			]);
			var K = FnAddKey(Zxc);
			var K2 = FnAddKey2(Zxc);
			K("Shift");
			K("Z");
			K("X");
			K("C");
			K("V");
			K("B");
			K("N");
			K("M");
			K2(",", "<");
			K2(".", ">");
			K("Shift");
		});
		var Ctrl = new AutoGrid(IsRow: false);
		Root.AddInit(Ctrl.Grid, o=>{
			o.ColumnDefinitions.AddRange([
				..Repeat(()=>ColDef(KeyLen*1.2, GUT.Pixel), 3)
				,ColDef(KeyLen*6, GUT.Pixel)//sapce
				,..Repeat(()=>ColDef(KeyLen, GUT.Pixel), 5)
			]);
			var K = FnAddKey(Ctrl);
			K("Ctrl");
			K("Win");
			K("Alt");
			K("Space");
			K("Alt");
			K("Ctrl");
			K("Fn");
			K("");
			K("");
		});
		return NIL;
	}

	Func<str, nil> FnAddKey(AutoGrid Target){
		var R = (str Title)=>{
			Target.AddInit(Key(Title));
			return NIL;
		};
		return R;
	}

	Func<str,str, nil> FnAddKey2(AutoGrid Target){
		var R = (str Title, str Shift)=>{
			Target.AddInit(Key(Title));
			return NIL;
		};
		return R;
	}

	Control Key(str Title, str? Shift = ""){
		return new TextBlock{Text=Title};
	}

	Control Blank(){
		return new TextBlock{};
	}


}
