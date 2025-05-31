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
					// return new StackPanel{
					// 	Orientation = Orientation.Horizontal
					// 	,
					// };
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

	// protected nil Render(){
	// 	Content = Root.Grid;
	// 	{
	// 		var o = Root.Grid;
	// 		o.ColumnDefinitions.AddRange([
	// 			//new ColDef(1, GUT.Star),
	// 			new ColDef(4, GUT.Auto),
	// 			new ColDef(4, GUT.Auto),
	// 			new ColDef(4, GUT.Auto),
	// 			//new ColDef(1, GUT.Star),
	// 		]);
	// 	}
	// 	{{
	// 		var Learn = BarItem("Learn", "📖");
	// 		Root.Add(Learn);

	// 		var Settings = BarItem("Lib", "📚");
	// 		Root.Add(Settings);

	// 		var Me = BarItem("Me", "👤");
	// 		Root.Add(Me);
	// 	}}
	// 	return Nil;
	// }

	protected Control BarItem(
		str Title
		,str IconStr
	){
		return StrBarItem.Inst.BarItem(Title, IconStr);
	}

}



public class UniformHorizontalPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        double maxHeight = 0;
        int count = Children.Count;
        if (count == 0)
            return new Size();

        double itemWidth = availableSize.Width / count; // 均分宽度

        foreach (var child in Children)
        {
            child.Measure(new Size(itemWidth, availableSize.Height));
            if (child.DesiredSize.Height > maxHeight)
                maxHeight = child.DesiredSize.Height;
        }

        return new Size(availableSize.Width, maxHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int count = Children.Count;
        if (count == 0)
            return finalSize;

        double itemWidth = finalSize.Width / count;

        double x = 0;
        foreach (var child in Children)
        {
            // 按均分宽度排列，每个元素宽度相同，高度为自己的DesiredSize.Height（或者 finalSize.Height）
            Rect rect = new Rect(x, 0, itemWidth, finalSize.Height);
            child.Arrange(rect);
            x += itemWidth;
        }

        return finalSize;
    }
}
