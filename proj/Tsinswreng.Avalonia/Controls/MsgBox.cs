using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.Avalonia.Sugar;
using Tsinswreng.Avalonia.Tools;

namespace Tsinswreng.Avalonia.Controls;

public class MsgBox : UserControl{
	// public Ctx? Ctx{
	// 	get{return DataContext as Ctx;}
	// 	set{DataContext = value;}
	// }

	public MsgBox(){
		//Ctx = new Ctx();
		Render();
		_Style();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	public IndexGrid Root {get;protected set;} = new(IsRow: true);

	// public Func<nil> OnLeftBtn = ()=>Nil;
	// public Func<nil> OnRightBtn = ()=>Nil;

	public Border _Border{get;protected set;} = new ();
	public Border _BdrTitle{get; protected set;} = new ();
	public ContentControl _Title{get;} = new();
	public Border _BdrBody{get; protected set;} = new ();
	public ContentControl _Body{get;} = new();
	public SwipeLongPressBtn _CloseBtn{get; protected set;}
	public Border _BdrBottomView{get; protected set;} = new();
	public ContentControl? _BottomView{get;} = new();



	protected nil _Style(){

		return Nil;
	}

	protected nil Render(){
		//Content = Root.Grid;
		Content = _Border;
		_Border.Child = Root.Grid;
		{var o = Root.Grid;

			o.RowDefinitions.AddRange([
				new RowDef(1, GUT.Auto),//Title
				new RowDef(1, GUT.Auto),//Body
				new RowDef(1, GUT.Auto),//Bottom
			]);
		}
		{{

			var TitleRow = new IndexGrid(IsRow: false);
			_BdrTitle.Child = TitleRow.Grid;
			Root.Add(_BdrTitle);
			{var o = TitleRow.Grid;
				o.ColumnDefinitions.AddRange([
					new ColDef(999, GUT.Star),
					new ColDef(1, GUT.Auto),
				]);
			}
			{{
				TitleRow.Add(_Title);
				{var o = _Title;
					o.VerticalAlignment = VertAlign.Center;
				}

				_CloseBtn = new SwipeLongPressBtn{};
				TitleRow.Add(_CloseBtn);
				{var o = _CloseBtn;
					o.Content = "×";
					o.HorizontalAlignment = HoriAlign.Right;
					o.Background = Brushes.Red;
				}
			}}//~TitleLine


			Root.Add(_BdrBody);
			{{
				var Scr = new ScrollViewer();
				_BdrBody.Child = Scr;
				{var o = Scr;

				}
				{{
					Scr.Content = _Body;
					{var o = _Body;
					}
				}}//~Scr
			}}

			Root.Add(_BdrBottomView);
			_BdrBottomView.Child = _BottomView;
		}}//~Root
		return Nil;
	}

	// protected Style _Sty2Btn(){
	// 	// o.VerticalAlignment = VertAlign.Stretch;
	// 	// o.HorizontalAlignment = HoriAlign.Stretch;
	// 	// var o = new Style(x=>
	// 	// 	x.Is<Button>()
	// 	// );
	// 	// o.Set(
	// 	// 	Button.VerticalContentAlignmentProperty
	// 	// 	,VertAlign.Center
	// 	// );
	// 	// o.Set(
	// 	// 	Button.HorizontalContentAlignmentProperty
	// 	// 	,HoriAlign.Center
	// 	// );
	// 	// o.Set(
	// 	// 	Button.VerticalAlignmentProperty
	// 	// 	,VertAlign.Stretch
	// 	// );
	// 	// o.Set(
	// 	// 	Button.HorizontalAlignmentProperty
	// 	// 	,HoriAlign.Stretch
	// 	// );
	// 	// return o;
	// }


}
