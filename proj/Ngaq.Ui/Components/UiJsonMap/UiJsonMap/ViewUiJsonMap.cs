namespace Ngaq.Ui.Components.KvMap.JsonMap;

using System.ComponentModel;
using Avalonia.Controls;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmUiJsonMap;
public partial class ViewUiJsonMap
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewUiJsonMap(){
		Ctx = App.DiOrMk<Ctx>();
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls{

	}


	protected nil Style(){
		return NIL;
	}


	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
		]);
		Root
		.A(new ScrollViewer(), sv=>{
			sv.Content = mkList();
		});

		return NIL;
	}

	ItemsControl mkList(){
		var R = new ItemsControl();
		R.Bind(R.PropItemsSource, CBE.Mk<Ctx>(x=>x.ItemVms));
		R.SetItemsPanel(()=>{
			var R = new StackPanel();
			R.Spacing = 5;
			return R;
		});
		R.SetItemTemplate<VmJsonMapItem>((ele, ns)=>{
			var R = new ViewJsonMapItem(){
				Ctx = ele
			};
			return R;
		});

		return R;
	}
}

