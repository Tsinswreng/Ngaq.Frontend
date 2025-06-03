namespace Ngaq.Ui.Views.BottomBar;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Tsinswreng.Avalonia.Controls;
using Tsinswreng.Avalonia.Sugar;
using Tsinswreng.Avalonia.Tools;
using Ctx = Vm_BottomBar;
public partial class ViewBottomBar
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public IList<Btn_Control> Items{get;set;} = new List<Btn_Control>();


	public ViewBottomBar(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public class Cls_{
		public str BarItem = nameof(BarItem);
	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return Nil;
	}

	IndexGrid Root = new(IsRow:true);

	public ItemsControl ItemsControl{get;set;} = new ItemsControl();

	public ContentControl Cur{get; protected set;} = new ContentControl();


	protected nil Render(){
		Content = Root.Grid;
		{var o = Root.Grid;
			o.RowDefinitions.AddRange([
				new RowDef(18, GUT.Star),
				new RowDef(1, GUT.Star),
			]);
		}
		{{
			Root.Add(Cur);
			ItemsControl = new ItemsControl();
			Root.Add(ItemsControl);
			{var o = ItemsControl;
				o.ItemsSource = Items;
				o.ItemsPanel = new FuncTemplate<Panel?>(()=>{
					return new UniformHorizontalPanel();
				});
			}
			ItemsControl.ItemTemplate = new FuncDataTemplate<Btn_Control>((Btn_Control, b)=>{
				var Ans = Btn_Control.Button;
				Ans.Click += (s,e)=>{
					Cur.Content = Btn_Control.Control;
				};
				return Ans;
			});

		}}
		return Nil;
	}

}



public class UniformHorizontalPanel : Panel{
	protected override Size MeasureOverride(Size availableSize){
		f64 maxHeight = 0;
		i32 count = Children.Count;
		if (count == 0)
			return new Size();

		f64 itemWidth = availableSize.Width / count; // 均分宽度

		foreach (var child in Children){
			child.Measure(new Size(itemWidth, availableSize.Height));
			if (child.DesiredSize.Height > maxHeight){
				maxHeight = child.DesiredSize.Height;
			}
		}

		return new Size(availableSize.Width, maxHeight);
	}

	protected override Size ArrangeOverride(Size finalSize){
		i32 count = Children.Count;
		if (count == 0){
			return finalSize;
		}

		f64 itemWidth = finalSize.Width / count;

		f64 x = 0;
		foreach (var child in Children){
			// 按均分宽度排列，每个元素宽度相同，高度为自己的DesiredSize.Height（或者 finalSize.Height）
			Rect rect = new Rect(x, 0, itemWidth, finalSize.Height);
			child.Arrange(rect);
			x += itemWidth;
		}

		return finalSize;
	}
}
