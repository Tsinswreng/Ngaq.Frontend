namespace Ngaq.Ui.Views.Word.WordEdit;

using System.Linq.Expressions;
using Avalonia.Controls;
using DynamicData.Binding;
using Ngaq.Core.Infra;
using Ngaq.Core.Tools.JsonMap;
using Ngaq.Ui;
using Ngaq.Ui.Components.KvMap.JsonMap;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmWordEdit;
public partial class ViewWordEdit
	:AppViewBase
{

	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewWordEdit(){
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

	Expander _Expander(){
		var R = new Expander();
		R.Styles.Add(new Style().NoMargin().NoPadding());
		R.HorizontalAlignment = HAlign.Stretch;
		return R;
	}

	AutoGrid Root = new(IsRow: true);
	protected nil Render(){
		this.Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Star),
		]);

		Root.AddInit(new ScrollViewer(), Sv=>{
			Sv.ContentInit(new StackPanel(), root2=>{
				root2.AddInit(_Expander(), o=>{
					Todo.I18n();
					o.Header = "Word Core";
					o.IsExpanded = true;
					o.ContentInit(new ViewUiJsonMap(), jm=>{
						Ctx?.WhenPropertyChanged(x=>x.JnWord).Subscribe(propValue=>{
							var currentJnWord = propValue.Value;
							if (currentJnWord == null) return;
							var poWord = JnWordToUiJsonMap.MkPoWord(
								CoreDictMapper.Inst.ToDictShallowT(currentJnWord.Word)
							);
							jm.Ctx!.FromBo(poWord);
						});
					});
				});
			});
		});

		Root.AddInit(new OpBtn(), o=>{
			Todo.I18n();
			o.BtnContent = "Save";
		});

		return NIL;
	}


}
