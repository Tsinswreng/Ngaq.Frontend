using System;

namespace Tsinswreng.Avalonia.Controls.IF;

public interface ILongPressBtn{
	public event EventHandler? LongPressed;
	public i64 LongPressDurationMs{get;set;}
}
