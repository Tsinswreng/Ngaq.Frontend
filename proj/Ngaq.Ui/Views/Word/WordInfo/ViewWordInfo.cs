namespace Ngaq.Ui.Views.Word.WordInfo;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Media;
using Ngaq.Ui.ViewModels;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordInfo;
using Ngaq.Core.Model.Po.Kv;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;

public partial class ViewWordInfo
	:UserControl
{

	// public IValueConverter ConvMultiDictToList(str KeyWithoutNs){
	// 	return new FnConvtr< IDictionary<str, IList<str>>, IList<str>>(
	// 		x=>{
	// 			var Key = ConstTokens.Inst.Concat(null, KeyWithoutNs);
	// 			x.TryGetValue(Key, out var v);
	// 			//return str.Join("\t",v??[]);
	// 			return v??[];
	// 		}
	// 	);
	// }

	// public IValueConverter ConvMultiDictToStr(str KeyWithoutNs){
	// 	return new FnConvtr< IDictionary<str, IList<str>>, IList<str>>(
	// 		x=>{
	// 			var Key = ConstTokens.Inst.Concat(null, KeyWithoutNs);
	// 			x.TryGetValue(Key, out var v);
	// 			return str.Join("\t",v??[]);
	// 			//return v??[];
	// 		}
	// 	);
	// }

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordInfo(){
		Ctx = new Ctx();
		//Ctx = Ctx.Samples[0];
		Style();
		Render();
	}


	public class Cls_{
		//o.Foreground = new SolidColorBrush(Colors.LightGray)
		public str LightGray = nameof(LightGray);
		public str TxtBox = nameof(TxtBox);
	}
	public Cls_ Cls{get;set;} = new Cls_();

	public Color Gray = Colors.LightGray;

	protected nil Style(){
		//Styles.Add(SugarStyle.GridShowLines());

		var LightGray = new Style(
			x=>x.Is<Control>()
			.Class(Cls.LightGray)
		).Set(
			ForegroundProperty
			,new SolidColorBrush(Gray)
			//,new SolidColorBrush(Colors.Red)
		);
		Styles.Add(LightGray);

		var InputBoxNoBorder = new Style(x=>
			x.Is<TextBlock>()
		).Set(
			BorderThicknessProperty
			,new Thickness(0)
		);
		Styles.Add(InputBoxNoBorder);

		return NIL;
	}

	public IndexGrid Root{get;set;} = new(IsRow: true);

	protected TextBox TxtBox(){
		var R = new TextBox();
		R.Classes.Add(Cls.TxtBox);
		//R.BorderThickness = new Thickness(0);
		R.IsReadOnly = true;
		R.Focusable = false;
		//R.IsEnabled = false;
		R.Background = new SolidColorBrush(Colors.Transparent);
		R.Foreground = new SolidColorBrush(Colors.White);
		// var Menu = new ContextMenu();
		// R.ContextMenu = Menu;
		// {var o = Menu;
		// 	o.Items.Add(new MenuItem{Header = "複製"});
		// }
		var flyout = new MenuFlyout();
		FlyoutBase.SetAttachedFlyout(R, flyout);

		R.Styles.Add(new Style().NoMargin().NoPadding());
		R.MinHeight = 0;
		var NoBdr = new Style(x=>
			x.Is<TextBox>()
			.Class(Cls.TxtBox)
			//.Class(PsdCls.Inst.focus)
			.Template()
			.OfType<Border>()
		);
		R.Styles.Add(NoBdr);
		{var o = NoBdr;
			o.Set(
				BorderThicknessProperty
				,new Thickness(0)
			);
		}
		var FocusNoBdr = new Style(x=>
			x.Is<TextBox>()
			.Class(PsdCls.Inst.focus)
			.Template()
			.OfType<Border>()
		);
		R.Styles.Add(FocusNoBdr);
		{var o = FocusNoBdr;
			o.Set(
				BorderThicknessProperty
				,new Thickness(0)
			);
		}
		return R;
	}

	protected nil Render(){
		Content = Root.Grid;
		{var o = Root.Grid;
			o.RowDefinitions.AddRange([
				new RowDef(1, GUT.Auto),//LangId
				new RowDef(3, GUT.Auto),//Head
				new RowDef(1, GUT.Auto),//Summary
				new RowDef(100, GUT.Star),//Description
				new RowDef(1, GUT.Star),
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
				o.Classes.Add(Cls.LightGray);//即o.Classes.Add("LightGray");
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

			var BdrHead = new Border{};
			Root.Add(BdrHead);
			{var o = BdrHead;
				o.BorderThickness = new Thickness(0, 1, 0, 1);
				o.BorderBrush = new SolidColorBrush(Colors.LightGray);
			}

			var Head = TxtBox();
			BdrHead.Child = Head;
			{var o = Head;
				o.Styles.Add(new Style().NoMargin().NoPadding());
				o.Bind(
					TextBox.TextProperty
					,new CBE(CBE.Pth<Ctx>(x=>x.Head)){
						Mode = BindingMode.TwoWay
					}
				);
				o.VerticalAlignment = VertAlign.Stretch;
				//o.VerticalContentAlignment = VertAlign.Center;
				//o.TextAlignment = TxtAlign.Right;
				o.FontSize += UiCfg.Inst.BaseFontSize*1.5;
				//o.ContentFontSize += UiCfg.Inst.BaseFontSize*1.5; //?
			}

			var Summary = TxtBox();
			Root.Add(Summary);
			{var o = Summary;
				o.Bind(
					TextBlock.TextProperty
					,new CBE(CBE.Pth<Ctx>(x=>x.StrProps)){
						//Converter = ConvMultiDictToList(KeysProp.Inst.summary)
						Mode = BindingMode.OneWay
						,Converter = new SimpleFnConvtr<IDictionary<str, IList<str>>, str>(
						x=>{
							var Key = ConstTokens.Inst.Concat(null, KeysProp.Inst.summary);
							x.TryGetValue(Key, out var v);
							return str.Join("\t",v??[]);
						}
					)
					}
				);
			}

			var BdrScr = new Border{};
			Root.Add(BdrScr);
			{var o = BdrScr;
				//o.Height = UiCfg.Inst.WindowHeight;
			}

			var ScrDescr = new ScrollViewer();
			BdrScr.Child = ScrDescr;
			{var o = ScrDescr;}
			{{
				var Description = _DescriptionList();
				ScrDescr.Content = Description;
				{var o = Description;

				}
			}}
			Root.Add();

		}}//~Root
		return NIL;
	}


	Control _DescriptionList(){
		var Items = new ItemsControl();
		{var o = Items;
			o.Bind(
				ItemsControl.ItemsSourceProperty
				,new CBE(CBE.Pth<Ctx>(x=>x.StrProps)){
					Mode = BindingMode.OneWay
					//,Converter = ConvMultiDictToList(KeysProp.Inst.description)
					,Converter = new SimpleFnConvtr<IDictionary<str, IList<str>>, IList<str>>(
						x=>{
							var Key = ConstTokens.Inst.Concat(null, KeysProp.Inst.description);
							x.TryGetValue(Key, out var v);
							return v??[];
						}
					)
				}
			);
		}
		Items.ItemTemplate = new FuncDataTemplate<str>((Descr,b)=>{
			var Ans = new Border();
			{var o = Ans;
				o.BorderThickness = new Thickness(0, 1, 0, 0);
				o.BorderBrush = new SolidColorBrush(Gray);
			}
			var Grid = new IndexGrid(IsRow: true);
			Ans.Child = Grid.Grid;
			{var o = Grid.Grid;
				o.RowDefinitions.AddRange([
					new RowDef(1, GUT.Auto),
				]);
			}
			{{
				var Text = TxtBox();
				Grid.Add(Text);
				{var o = Text;
					o.TextWrapping = TextWrapping.Wrap;
					o.Bind(
						TextBlock.TextProperty
						,new CBE(CBE.Pth<str>(x=>x))
					);
				}
			}}
			return Ans;
		});

		return Items;
	}


}
