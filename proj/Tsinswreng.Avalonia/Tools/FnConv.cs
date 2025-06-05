using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Tsinswreng.Avalonia.Tools;

public class ParamFnConvtr<TIn, TRet>
	:IValueConverter
{
	public Func<TIn, object?, TRet>? FnConv{get;set;}
	public Func<TRet, object?, TIn>? FnBack{get;set;}
		public ParamFnConvtr(Func<TIn, object?, TRet> FnConv){
			this.FnConv = FnConv;
		}

	public ParamFnConvtr(
		Func<TIn, object?, TRet> FnConv
		,Func<TRet, object?, TIn> FnBack
	){
		this.FnConv = FnConv;
		this.FnBack = FnBack;
	}

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is TIn val) {
			if(FnConv == null){
				return AvaloniaProperty.UnsetValue;
			}
			return FnConv.Invoke(val, parameter);
		}
		throw new ArgumentException();
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is TRet val) {
			if(FnBack == null){
				return AvaloniaProperty.UnsetValue;
			}
			return FnBack.Invoke(val, parameter);
		}
		throw new ArgumentException();
	}
}



public class SimpleFnConvtr<TIn, TRet>
	:IValueConverter
{
	public Func<TIn, TRet>? FnConv{get;set;}
	public Func<TRet, TIn>? FnBack{get;set;}
		public SimpleFnConvtr(Func<TIn, TRet> FnConv){
			this.FnConv = FnConv;
		}

	public SimpleFnConvtr(
		Func<TIn, TRet> FnConv
		,Func<TRet, TIn> FnBack
	){
		this.FnConv = FnConv;
		this.FnBack = FnBack;
	}

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is TIn val) {
			if(FnConv == null){
				return AvaloniaProperty.UnsetValue;
			}
			return FnConv.Invoke(val);
		}
		throw new ArgumentException();
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is TRet val) {
			if(FnBack == null){
				return AvaloniaProperty.UnsetValue;
			}
			return FnBack.Invoke(val);
		}
		throw new ArgumentException();
	}
}
