using System.Globalization;
using Avalonia.Data.Converters;
using Tsinswreng.Avalonia.Tools;

namespace Tsinswreng.Avalonia.Converters;

public class ConvDictEntry<K, V> : IValueConverter
	where K : notnull
{

/// <summary>
///
/// </summary>
/// <param name="value">IDict</param>
/// <param name="targetType"></param>
/// <param name="parameter"></param>
/// <param name="culture"></param>
/// <returns></returns>
/// <exception cref="ArgumentException"></exception>
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if (value is IDictionary<K,V> dict && parameter is IDict_Key<K,V> Dict_Key) {
			return dict[Dict_Key.Key];
		}
		throw new ArgumentException("Convert: Invalid input");
	}

/// <summary>
///
/// </summary>
/// <param name="value">輸入框更改後之值</param>
/// <param name="targetType"></param>
/// <param name="parameter">IDict_Key<K,V></param>
/// <param name="culture"></param>
/// <returns></returns>
	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
		if(value is V v && parameter is IDict_Key<K,V> Dict_Key){
			Dict_Key.Dict[Dict_Key.Key] = v;
			return Dict_Key.Dict;
		}
		throw new ArgumentException("ConvertBack: Invalid input");
	}
}
