using Avalonia.Controls;

namespace Tsinswreng.Avalonia.Controls;

public partial class BtnView<T> : UserControl{

	public T TypedContent{get;protected set;}
	public Button Button{get;protected set;}

	public BtnView(
		Button Button
		,T TypedContent
	){
		this.Button = Button;
		this.TypedContent = TypedContent;
		Content = Button;
		Button.Content = TypedContent;
	}
}
