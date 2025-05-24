using System;

namespace Tsinswreng.Avalonia.Controls.IF;

public interface ISwipeBtn{
	public event EventHandler<SwipeEventArgs> OnSwipe;
	public f64 SwipeThreshold{get;set;}


#region SwipeDirection
public enum SwipeDirection {
	Up,
	Down,
	Left,
	Right
}

#endregion SwipeDirection


#region SwipeEventArgs
public class SwipeEventArgs : EventArgs {
	public SwipeDirection Direction { get; }

	public SwipeEventArgs(SwipeDirection direction) {
		Direction = direction;
	}
}

#endregion SwipeEventArgs

}
