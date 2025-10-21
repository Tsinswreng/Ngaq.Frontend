namespace Ngaq.Ui.Views.Word.WordInfo;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Media;
using Ngaq.Ui.ViewModels;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordInfo;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Tsinswreng.AvlnTools.Controls;
using Tsinswreng.AvlnTools.Dsl;
using Ngaq.Core.Domains.Word.Models.Po.Kv;

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


	public  partial class Cls_{
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
		).Attach(Styles);

		var InputBoxNoBorder = new Style(x=>
			x.Is<TextBlock>()
		).Set(
			BorderThicknessProperty
			,new Thickness(0)
		).Attach(Styles);
		return NIL;
	}

	public AutoGrid Root{get;set;} = new(IsRow: true);

	protected TextBox TxtBox(){
		var R = new TextBox();
		var S = R.Styles;
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

		S.Add(new Style().NoMargin().NoPadding());
		R.MinHeight = 0;
		var NoBdr = new Style(x=>
			x.Is<TextBox>()
			.Class(Cls.TxtBox)
			//.Class(PsdCls.Inst.focus)
			.Template()
			.OfType<Border>()
		).Set(
			BorderThicknessProperty
			,new Thickness(0)
		).Attach(S);

		var FocusNoBdr = new Style(x=>
			x.Is<TextBox>()
			.Class(PsdCls.Inst.focus)
			.Template()
			.OfType<Border>()
		).Attach(S)
		.Set(
			BorderThicknessProperty
			,new Thickness(0)
		);
		return R;
	}

	protected nil Render(){
		this.ContentInit(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(1, GUT.Auto),//LangId
				RowDef(3, GUT.Auto),//Head
				RowDef(1, GUT.Auto),//Summary
				RowDef(100, GUT.Star),//Description
				RowDef(1, GUT.Star),
			]);
		});
		{{
			var LangId = new AutoGrid(IsRow: false);
			Root.AddInit(LangId.Grid, o=>{
				o.ColumnDefinitions.AddRange([
					ColDef(1, GUT.Star),
					ColDef(1, GUT.Auto),
				]);
				o.Classes.Add(Cls.LightGray);//即o.Classes.Add("LightGray");
			});
			{{
				LangId.AddInit(_SelectableTextBlock(), o=>{
					o.Bind(
						o.PropText_()
						,new CBE(CBE.Pth<Ctx>(x=>x.Lang))
					);
					o.HorizontalAlignment = HAlign.Left;
					o.VerticalAlignment = VAlign.Center;
				});
				LangId.AddInit(_SelectableTextBlock(), o=>{
					o.Bind(
						o.PropText_()
						,new CBE(CBE.Pth<Ctx>(x=>x.Id))
					);
					o.VerticalAlignment = VAlign.Center;
					o.HorizontalAlignment = HAlign.Right;
					o.TextAlignment = TxtAlign.Right;
				});
			}}//~LangId

			var BdrHead = _Border();
			Root.AddInit(BdrHead, o=>{
				o.BorderThickness = new Thickness(0, 1, 0, 1);
				o.BorderBrush = new SolidColorBrush(Colors.LightGray);
			});

			var Head = TxtBox();
			BdrHead.Child = Head;
			{var o = Head;
				o.Styles.Add(new Style().NoMargin().NoPadding());
				o.Bind(
					o.PropText_()
					,new CBE(CBE.Pth<Ctx>(x=>x.Head)){
						Mode = BindingMode.TwoWay
					}
				);
				o.VerticalAlignment = VAlign.Stretch;
				//o.VerticalContentAlignment = VertAlign.Center;
				//o.TextAlignment = TxtAlign.Right;
				o.FontSize += UiCfg.Inst.BaseFontSize*1.5;
				//o.ContentFontSize += UiCfg.Inst.BaseFontSize*1.5; //?
			}

			Root.AddInit(TxtBox(), o=>{
				o.Bind(
					o.PropText_()
					,new CBE(CBE.Pth<Ctx>(x=>x.StrProps)){
						//Converter = ConvMultiDictToList(KeysProp.Inst.summary)
						Mode = BindingMode.OneWay
						,Converter = new SimpleFnConvtr<IDictionary<str, IList<str>>, str>(x=>{
							var Key = ConstTokens.Inst.Concat(null, KeysProp.Inst.summary);
							x.TryGetValue(Key, out var v);
							return str.Join("\t",v??[]);
						})//~Converter:
					}//~new CBE
				);//~Bind
			});//~TxtBox


			var BdrScr = new Border{};
			Root.Add(BdrScr);

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
			var Grid = new AutoGrid(IsRow: true);
			Ans.Child = Grid.Grid;
			{var o = Grid.Grid;
				o.RowDefinitions.AddRange([
					RowDef(1, GUT.Auto),
				]);
			}
			{{
				Grid.AddInit(TxtBox(), o=>{
					o.TextWrapping = TextWrapping.Wrap;
					o.Bind(
						o.PropText_()
						,CBE.Mk<str>(x=>x)
					);
				});
			}}
			return Ans;
		});
		return Items;
	}
}
