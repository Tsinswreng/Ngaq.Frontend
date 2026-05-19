using Avalonia.Controls;
namespace Ngaq.Ui;


public static partial class Extn {
	extension<TSelf>(TSelf z)
		where TSelf : Grid
	{
		public TSelf RowDefs(
			params IEnumerable<RowDef> RowDefs
		){
			z.RowDefinitions = [..RowDefs];
			return z;
		}

		public TSelf ColDefs(
			params IEnumerable<ColDef> ColDefs
		){
			z.ColumnDefinitions= [..ColDefs];
			return z;
		}

		// public TSelf RowDefs(
		// 	params IEnumerable<RowDef> RowDefs
		// ){
		// 	z.RowDefinitions = [..RowDefs];
		// 	return z;
		// }

		// public TSelf ColDefs(
		// 	IEnumerable<ColDef> ColDefs
		// ){
		// 	z.ColumnDefinitions= [..ColDefs];
		// 	return z;
		// }
	}
}
