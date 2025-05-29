namespace Ngaq.Ui.Views.WordInfo;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Media;
using Ngaq.Ui.ViewModels;
using Tsinswreng.Avalonia.Sugar;
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
		//Ctx = new Ctx();
		Ctx = Ctx.Samples[0];
		Style();
		Render();
	}

	public class Cls_{

	}
	public Cls_ Cls{get;set;} = new Cls_();

	protected nil Style(){
		Styles.Add(SugarStyle.GridShowLines());

		var InputBoxNoBorder = new Style(x=>
			x.Is<TextBlock>()
		).Set(
			BorderThicknessProperty
			,new Thickness(0)
		);
		Styles.Add(InputBoxNoBorder);

		return Nil;
	}

	public IndexGrid Root{get;set;} = new(IsRow: true);

	protected nil Render(){
		Content = Root.Grid;
		{var o = Root.Grid;
			o.RowDefinitions.AddRange([
				new RowDef(1, GUT.Star),//LangId
				new RowDef(3, GUT.Star),//Head
			]);
		}
		{{
			var LangId = new IndexGrid(IsRow: false);
			Root.Add(LangId.Grid);
			{var o = LangId.Grid;
				o.ColumnDefinitions.AddRange([
					new ColDef(1, GUT.Star),
					new ColDef(1, GUT.Auto),
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
					o.VerticalAlignment = VertAlign.Center;
					o.HorizontalAlignment = HoriAlign.Right;
					o.TextAlignment = TxtAlign.Right;
				}
			}}//~LangId

			var Head = new TextBox{};
			Root.Add(Head);
			{var o = Head;
				var a = TextBlock.TextProperty;
				var b = TextBox.TextProperty;
				o.Bind(
					TextBox.TextProperty
					,new CBE(CBE.Pth<Ctx>(x=>x.Head)){
						Mode = BindingMode.TwoWay
					}
				);
				o.VerticalAlignment = VertAlign.Stretch;
				//o.VerticalContentAlignment = VertAlign.Center;
				o.FontSize = 32.0;
				//o.FontSize += UiCfg.Inst.BaseFontSize + 6;

			}


		}}//~Root
		return Nil;
	}


}
