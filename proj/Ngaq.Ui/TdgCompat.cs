namespace Ngaq.Ui;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;

/// 基於免費 DataGrid 的最小兼容層，保留現有 TreeDataGrid 的平面列表用法。
public static class TdgCompat{
	/// 兼容舊 `TextColumn` 工廠：直接接受值選擇器，而不是只限屬性路徑綁定。
	public static TreeDataGridColumn TextColumn<TModel, TValue>(
		object? Header
		,Expression<Func<TModel, TValue>> Getter
		,GridLength Width
	){
		return new FuncTextColumn<TModel, TValue>(Header, Getter.Compile(), Width);
	}
}

/// 免費替代的平面源：保留 `Rows + Columns` 形狀，便於現有頁面直接沿用。
public sealed class FlatTreeDataGridSource<TModel>{
	public IEnumerable<TModel> Rows{get;}
	public IList<TreeDataGridColumn> Columns{get;} = [];

	public FlatTreeDataGridSource(IEnumerable<TModel> Rows){
		this.Rows = Rows;
	}
}

/// 以免費 `DataGrid` 包裝舊 `TreeDataGrid` 名稱，盡量不動既有頁面代碼。
public class TreeDataGrid: DataGrid{
	public static readonly StyledProperty<object?> SourceProperty = AvaloniaProperty.Register<TreeDataGrid, object?>(nameof(Source));

	public object? Source{
		get{return GetValue(SourceProperty);}
		set{SetValue(SourceProperty, value);}
	}

	static TreeDataGrid(){
		SourceProperty.Changed.AddClassHandler<TreeDataGrid>((o, e)=>o.ApplySource(e.NewValue));
	}

	public TreeDataGrid(){
		AutoGenerateColumns = false;
		IsReadOnly = true;
		CanUserResizeColumns = false;
		CanUserSortColumns = false;
		CanUserReorderColumns = false;
		GridLinesVisibility = DataGridGridLinesVisibility.Horizontal;
		HeadersVisibility = DataGridHeadersVisibility.Column;
		SelectionMode = DataGridSelectionMode.Single;
	}

	void ApplySource(object? Source){
		Columns.Clear();
		ItemsSource = null;
		if(Source is null){
			return;
		}
		var t = Source.GetType();
		if(!t.IsGenericType || t.GetGenericTypeDefinition() != typeof(FlatTreeDataGridSource<>)){
			throw new NotSupportedException($"Unsupported grid source type: {t.FullName}");
		}
		var rowsProp = t.GetProperty(nameof(FlatTreeDataGridSource<int>.Rows));
		var colsProp = t.GetProperty(nameof(FlatTreeDataGridSource<int>.Columns));
		if(rowsProp?.GetValue(Source) is System.Collections.IEnumerable rows){
			ItemsSource = rows;
		}
		if(colsProp?.GetValue(Source) is IEnumerable<TreeDataGridColumn> cols){
			foreach(var col in cols){
				Columns.Add(col);
			}
		}
	}
}

/// 兼容舊列基類型名，現改用免費 `DataGridColumn`。
public abstract class TreeDataGridColumn: DataGridColumn{}

/// 使用委託渲染文字列，兼容原先 `x => ...` 的任意表達式。
sealed class FuncTextColumn<TModel, TValue>: TreeDataGridColumn{
	readonly Func<TModel, TValue> _Getter;

	public FuncTextColumn(object? Header, Func<TModel, TValue> Getter, GridLength Width){
		ArgumentNullException.ThrowIfNull(Getter);
		_Getter = Getter;
		this.Header = Header;
		this.Width = ToDataGridLength(Width);
		IsReadOnly = true;
	}

	protected override Control GenerateElement(DataGridCell Cell, object DataItem){
		var tb = new TextBlock{
			Text = FormatValue(DataItem),
			TextTrimming = TextTrimming.CharacterEllipsis,
			VerticalAlignment = VAlign.Center,
			Margin = new Thickness(8, 0),
			Foreground = Brushes.White,
		};
		return tb;
	}

	protected override Control GenerateEditingElement(DataGridCell Cell, object DataItem, out BindingExpressionBase Binding){
		Binding = null!;
		return GenerateElement(Cell, DataItem);
	}

	protected override object? PrepareCellForEdit(Control EditingElement, RoutedEventArgs EditingEventArgs){
		return null;
	}

	protected override void EndCellEdit(){}

	protected override void CancelCellEdit(Control EditingElement, object UneditedValue){}

	str FormatValue(object DataItem){
		if(DataItem is not TModel model){
			return "";
		}
		var v = _Getter(model);
		return v?.ToString() ?? "";
	}

	static DataGridLength ToDataGridLength(GridLength Width){
		if(Width.IsAuto){
			return DataGridLength.Auto;
		}
		if(Width.IsStar){
			return new DataGridLength(Width.Value, DataGridLengthUnitType.Star);
		}
		return new DataGridLength(Width.Value, DataGridLengthUnitType.Pixel);
	}
}

