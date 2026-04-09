namespace Ngaq.Ui.Views.Word.WordEditJsonMap;

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
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordEditV2;
using Ngaq.Ui.Views.Word.WordManage.EditWord;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.Srefl;
using Ctx = VmWordEdit;
public partial class ViewWordEdit
	:AppViewBase
	,I_MkTitleMenu
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

	public Control MkTitleMenu(){
		var R = new ContextMenu();
		R.Styles.Add(new Style().NoMargin().NoPadding());
		R.Items.A(new MenuItem(), o=>{
			o.Header = "To Json View";
			o.Click += (s,e)=>{
				var vj = new ViewEditJsonWord();
				if(AnyNull(vj.Ctx, Ctx?.JnWord)){
					Ctx?.ShowDialog(Todo.I18n("No Word or Ctx"));
					return;
				}
				vj.Ctx.FromJnWord(Ctx.JnWord);

				ViewNavi?.GoTo(ToolView.WithTitle(Ctx?.JnWord.Word.Head??"", vj));
			};
		});
		R.Items.A(new MenuItem(), o=>{
			o.Header = "To New Form View";
			o.Click += (s,e)=>{
				var v2 = new ViewWordEditV2();
				if(AnyNull(v2.Ctx, Ctx?.JnWord)){
					Ctx?.ShowDialog("No Word or Ctx");
					return;
				}
				v2.Ctx.FromJnWord(Ctx.JnWord);
				ViewNavi?.GoTo(ToolView.WithTitle(Ctx?.JnWord.Word.Head??"", v2));
			};
		});
		return R;
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
			RowDef(9999, GUT.Star),
			RowDef(1, GUT.Auto),
		]);

		Root.A(new ScrollViewer(), Sv=>{
			Sv.SetContent(new StackPanel(), root2=>{
				root2.A(_Expander(), Ex=>{
					Ex.Header = Todo.I18n("Word Core");
					Ex.IsExpanded = true;
					Ex.SetContent(new ScrollViewer(), Sv=>{
						Sv.SetContent(new ViewUiJsonMap(), jm=>{
							Ctx?.WhenPropertyChanged(x=>x.JnWord).Subscribe(propValue=>{
								var currentJnWord = propValue.Value;
								if (currentJnWord == null) return;
								var poWord = JnWordToUiJsonMap.MkPoWord(
									CoreDictMapper.Inst.PropAccessorReg.ToPropDict(currentJnWord.Word)
								);
								jm.Ctx!.FromBo(poWord);
							});
						});
					});
				});
			});
		});

		Root.A(new OpBtn(), o=>{
			o.BtnContent = Todo.I18n("Save");
		});

		return NIL;
	}


}
