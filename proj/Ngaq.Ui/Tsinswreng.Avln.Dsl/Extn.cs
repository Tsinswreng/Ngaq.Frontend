using Avalonia.Controls;
using Tsinswreng.Avln.Grid;
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
	}
}

public static partial class ExtnGridStack{
	extension<TSelf>(TSelf z)
		where TSelf : GridStack
	{
		public TSelf RowDefs(
			params IEnumerable<RowDef> RowDefs
		){
			z.Grid.RowDefinitions = [..RowDefs];
			return z;
		}

		public TSelf ColDefs(
			params IEnumerable<ColDef> ColDefs
		){
			z.Grid.ColumnDefinitions= [..ColDefs];
			return z;
		}
	}
}
