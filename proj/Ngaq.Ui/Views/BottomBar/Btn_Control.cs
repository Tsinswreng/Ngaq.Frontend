using Avalonia.Controls;

namespace Ngaq.Ui.Views.BottomBar;

public class Btn_Control{

	public Btn_Control(Button Button, Control Control){
		this.Button = Button;
		this.Control = Control;
	}

	public Button Button{get;set;}
	public Control Control{get;set;}

}
