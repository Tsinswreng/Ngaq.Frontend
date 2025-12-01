namespace Ngaq.Ui.Converters;

using Avalonia.Data;
using Ngaq.Core.Infra;
using Tsinswreng.AvlnTools.Tools;



public class ConvtrTempus{
	protected static ConvtrTempus? _Inst = null;
	public static ConvtrTempus Inst => _Inst??= new ConvtrTempus();

	public SimpleFnConvtr<obj?, obj?> Int64{get;protected set;}
	public SimpleFnConvtr<obj?, obj?> Iso{get;protected set;}
	ConvtrTempus(){
		Int64 = new SimpleFnConvtr<obj?, obj?>(
			(tempusO)=>{
				if(tempusO is Tempus t){
					return t.Value;
				}
				return BindingNotification.UnsetValue;
			}
			,(i64O)=>{
				try{
					var I64 = Convert.ToInt64(i64O);
					return new Tempus(I64);
				}catch{}
				return BindingNotification.UnsetValue;
			}
		);
		Iso = new SimpleFnConvtr<obj?, obj?>(
			(tempus)=>{
				if(tempus is Tempus t){
					return t.ToIso();
				}
				return BindingNotification.UnsetValue;
			}
			,(iso)=>{
				if(iso is str strIso && Tempus.TryFromIso(strIso, out var R)){
					return R;
				}
				// 轉不過 → 告訴綁定引擎「維持現狀」
				return BindingNotification.UnsetValue;   // 或 UndefinedValue
			}
		);
	}
}
