using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.Avalonia.Sugar;
using Tsinswreng.Avalonia.Tools;

namespace Tsinswreng.Avalonia.Controls;

public class ConfirmBox : UserControl{
	// public Ctx? Ctx{
	// 	get{return DataContext as Ctx;}
	// 	set{DataContext = value;}
	// }

	public ConfirmBox(){
		//Ctx = new Ctx();
		Render();
		_Style();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	public IndexGrid Root = new(IsRow: true);

	public static readonly StyledProperty<str> TitleProperty =
		AvaloniaProperty.Register<ConfirmBox, str>(
			nameof(Title)
		);

	public str Title{
		get{return GetValue(TitleProperty);}
		set{ SetValue(TitleProperty, value);}
	}

	public static readonly StyledProperty<str> BodyProperty =
		AvaloniaProperty.Register<ConfirmBox, str>(
			nameof(Body)
		);

	public str Body{
		get{return GetValue(BodyProperty);}
		set{ SetValue(BodyProperty, value);}
	}

	public static readonly StyledProperty<object?> LeftBtnContentProperty =
		AvaloniaProperty.Register<ConfirmBox, object?>(
			nameof(LeftBtnContent)
		);

	public object? LeftBtnContent{
		get{return GetValue(LeftBtnContentProperty);}
		set{ SetValue(LeftBtnContentProperty, value);}
	}

	public static readonly StyledProperty<object?> RightBtnContentProperty =
		AvaloniaProperty.Register<ConfirmBox, object?>(
			nameof(RightBtnContent)
		);

	public object? RightBtnContent{
		get{return GetValue(RightBtnContentProperty);}
		set{ SetValue(RightBtnContentProperty, value);}
	}


	// public Func<nil> OnLeftBtn = ()=>Nil;
	// public Func<nil> OnRightBtn = ()=>Nil;

	public TextBlock _Title;
	public TextBlock _Body;
	public Button _LeftBtn;
	public Button _RightBtn;
	public Border _TitleBdr;



	protected nil _Style(){
		Styles.Add(SugarStyle.NoCornerRadius());
		return Nil;
	}

	protected nil Render(){
		Content = Root.Grid;
		{var o = Root.Grid;
			o.RowDefinitions.AddRange([
				new RowDef(1, GUT.Auto),
				new RowDef(1, GUT.Auto),
				new RowDef(1, GUT.Auto),
				new RowDef(1, GUT.Auto),
			]);
		}
		{{
			_Title = new TextBlock{};
			Root.Add(_Title);
			{var o = _Title;
				o.VerticalAlignment = VertAlign.Center;
				o.FontSize = 26.0;
				o.Bind(
					TextBlock.TextProperty
					,this.GetObservable(TitleProperty)
				);
			}

			_TitleBdr = new Border{};
			Root.Add(_TitleBdr);
			{var o = _TitleBdr;
				o.BorderThickness = new Thickness(0, 1, 0, 0);
				o.BorderBrush = Brushes.White;
			}

			var Scr = new ScrollViewer();
			Root.Add(Scr);
			{{
				_Body = new SelectableTextBlock{};
				Scr.Content = _Body;
				{var o = _Body;
					// o.Bind(
					// 	TextBlock.TextProperty
					// 	,this.GetObservable(BodyProperty)
					// );
				}
			}}//~Scr

			var BtnGrid = new IndexGrid(IsRow:false);
			Root.Add(BtnGrid.Grid);
			{var o = BtnGrid.Grid;
				o.ColumnDefinitions.AddRange([
					new ColDef(1, GUT.Star),
					new ColDef(1, GUT.Star),
				]);
				o.Styles.Add(_Sty2Btn());
			}
			{{
				_LeftBtn = new SwipeLongPressBtn{};
				BtnGrid.Add(_LeftBtn);
				{var o = _LeftBtn;
					// o.Bind(
					// 	Button.ContentProperty
					// 	,this.GetObservable(LeftBtnContentProperty)
					// );
				}

				_RightBtn = new SwipeLongPressBtn{};
				BtnGrid.Add(_RightBtn);
			}}//~BtnGrid
		}}//~Root
		return Nil;
	}

	protected Style _Sty2Btn(){
		// o.VerticalAlignment = VertAlign.Stretch;
		// o.HorizontalAlignment = HoriAlign.Stretch;
		var o = new Style(x=>
			x.Is<Button>()
		);
		o.Set(
			Button.VerticalContentAlignmentProperty
			,VertAlign.Center
		);
		o.Set(
			Button.HorizontalContentAlignmentProperty
			,HoriAlign.Center
		);
		o.Set(
			Button.VerticalAlignmentProperty
			,VertAlign.Stretch
		);
		o.Set(
			Button.HorizontalAlignmentProperty
			,HoriAlign.Stretch
		);
		return o;
	}


}
