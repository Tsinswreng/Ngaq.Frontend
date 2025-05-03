using Avalonia.Controls;
using System;

namespace Tsinswreng.Avalonia.Util;
public partial struct IndexGrid{

	public Grid Grid = new Grid();
	public i32 Index = 0;
	public bool IsRow = true;

	public IndexGrid(bool IsRow = true){
		this.IsRow = IsRow;
	}

	public nil Add(Control? control= default){
		if(control == null){
			Index++;
			return null!;
		}
		Grid.Children.Add(control);
		if(IsRow){
			Grid.SetRow(control, Index++);
		}else{
			Grid.SetColumn(control, Index++);
		}
		return null!;
	}

}
