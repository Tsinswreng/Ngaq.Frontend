namespace Ngaq.Ui.Views.BottomBar;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
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
		return NIL;
	}

	AutoGrid Root = new(IsRow:true);

	public ItemsControl ItemsControl{get;set;} = new ItemsControl();

	public ContentControl Cur{get; protected set;} = new ContentControl();


	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(18, GUT.Star),
				RowDef(1, GUT.Star),
			]);
		});
		{{
			Root.AddInit(Cur)
			.AddInit(_ItemsControl(), o=>{
				o.ItemsSource = Items;
				o.ItemsPanel = new FuncTemplate<Panel?>(()=>{
					return new UniformHorizontalPanel();
				});
				o.ItemTemplate = new FuncDataTemplate<Btn_Control>((Btn_Control, b)=>{
					var Ans = Btn_Control.Button;
					Ans.Click += (s,e)=>{
						Cur.Content = Btn_Control.Control;
					};
					return Ans;
				});
			});
		}}
		return NIL;
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
