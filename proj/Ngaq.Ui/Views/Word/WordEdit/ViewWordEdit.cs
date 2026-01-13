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
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.EditWord;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
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
		R.Items.AddInit(new MenuItem(), o=>{
			o.Header = "To Json View";
			o.Click += (s,e)=>{
				var vj = new ViewEditJsonWord();
				if(AnyNull(vj.Ctx, Ctx?.JnWord)){
					Todo.I18n();
					Ctx?.ShowMsg("No Word or Ctx");
					return;
				}
				vj.Ctx.FromJnWord(Ctx.JnWord);

				Ctx?.ViewNavi?.GoTo(ToolView.WithTitle(Ctx?.JnWord.Word.Head??"", vj));
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

		Root.AddInit(new ScrollViewer(), Sv=>{
			Sv.ContentInit(new StackPanel(), root2=>{
				root2.AddInit(_Expander(), Ex=>{
					Todo.I18n();
					Ex.Header = "Word Core";
					Ex.IsExpanded = true;
					Ex.ContentInit(new ScrollViewer(), Sv=>{
						Sv.ContentInit(new ViewUiJsonMap(), jm=>{
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
		});

		Root.AddInit(new OpBtn(), o=>{
			Todo.I18n();
			o.BtnContent = "Save";
		});

		return NIL;
	}


}
