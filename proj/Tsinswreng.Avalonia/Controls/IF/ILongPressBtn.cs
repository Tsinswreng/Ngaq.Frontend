using System;

namespace Tsinswreng.Avalonia.Controls.IF;

public interface ILongPressBtn{
	public event EventHandler? OnLongPressed;
	public i64 LongPressDurationMs{get;set;}
}
