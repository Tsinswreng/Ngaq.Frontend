namespace Ngaq.Ui.Views.WordInfo;

using Avalonia.Controls;
using Ngaq.Ui.ViewModels;
using Tsinswreng.Avalonia.Tools;
using Ctx = VmWordInfo;
public partial class ViewWordInfo
	:UserControl
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordInfo(){
		Ctx = new Ctx();
		Style();
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		return Nil;
	}

	public IndexGrid Root{get;set;} = new(IsRow: true);

	protected nil Render(){
		Content = Root.Grid;
		{var o = Root.Grid;
			o.RowDefinitions.AddRange([
				new RowDef(1, GUT.Star),
			]);
		}
		{{
			var LangId = new IndexGrid(IsRow: false);
			Root.Add(LangId.Grid);
			{var o = LangId.Grid;
				o.ColumnDefinitions.AddRange([
					new ColDef(1, GUT.Star),
					new ColDef(1, GUT.Star),
				]);
			}
			{{
				var Lang = new SelectableTextBlock{};
				LangId.Add(Lang);
				{var o = Lang;
					o.Bind(
						TextBlock.TextProperty
						,new CBE(CBE.Pth<Ctx>(x=>x.Lang))
					);
					o.HorizontalAlignment = HoriAlign.Left;
					o.VerticalAlignment = VertAlign.Center;
				}

				var Id = new SelectableTextBlock{};
				LangId.Add(Id);
				{var o = Id;
					o.Bind(
						TextBlock.TextProperty
						,new CBE(CBE.Pth<Ctx>(x=>x.Id))
					);
					o.HorizontalAlignment = HoriAlign.Right;
					o.VerticalAlignment = VertAlign.Center;
				}
			}}//~LangId

			var Head = new SelectableTextBlock{};
			Root.Add(Head);
			{var o = Head;
				o.Bind(
					TextBlock.TextProperty
					,new CBE(CBE.Pth<Ctx>(x=>x.Head))
				);
			}
		}}//~Root
		return Nil;
	}


}
