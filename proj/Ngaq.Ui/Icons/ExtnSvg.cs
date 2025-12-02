namespace Ngaq.Ui.Icons;

public static class ExtnSvg{
	extension(Svg z){
		public Icon ToIcon(){
			return Icon.FromSvg(z);
		}
	}
}
