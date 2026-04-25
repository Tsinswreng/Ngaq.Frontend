namespace Ngaq.Ui.Views.Word.WordManage.AddWord;

using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using AvaloniaEdit;
using Microsoft.Extensions.DependencyInjection;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsI18n;
using Ctx = VmAddWord;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

public partial class ViewAddWord
	:AppViewBase
{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewAddWord(){
		Ctx = App.SvcProvider.GetRequiredService<Ctx>();
		Style();
		Render();
	}

	TextEditor? WordEditor;
	bool IsSyncingText = false;
	AutoGrid Root = new(IsRow:true);

	protected nil Style(){
		return NIL;
	}

	protected nil Render(){
		this.SetContent(Root.Grid, o=>{
			o.RowDefinitions.AddRange([
				RowDef(2, GUT.Auto),
				RowDef(8, GUT.Star),
				RowDef(1, GUT.Auto),
			]);
		});

		Root
		.A(MkFormatHint())
		.A(MkTextEditor())
		.A(new OpBtn(), o=>{
			o.BtnContent = I[K.Submit];
			o.HorizontalAlignment = HAlign.Center;
			o.HorizontalContentAlignment = HAlign.Center;
			o.SetExe((Ct)=>Ctx?.Confirm(Ct));
		});

		return NIL;
	}

	Control MkFormatHint(){
		var sp = new StackPanel{
			Spacing = 4,
			Margin = new Thickness(10, 10, 10, 6),
		};
		sp.A(new TextBlock(), o=>{
			o.Text = Todo.I18n("FormatHint");
			o.FontSize = UiCfg.Inst.BaseFontSize * 1.05;
			o.TextWrapping = TextWrapping.Wrap;
			o.Foreground = Brushes.LightGray;
		})
		.A(new TextBlock(), o=>{
			o.Text = Todo.I18n("One entry per line; empty lines are ignored.");
			o.FontSize = UiCfg.Inst.BaseFontSize * 0.9;
			o.TextWrapping = TextWrapping.Wrap;
			o.Foreground = Brushes.LightGray;
		})
		.A(new TextBlock(), o=>{
			o.Text = Todo.I18n("Example: word<space>description");
			o.FontSize = UiCfg.Inst.BaseFontSize * 0.9;
			o.TextWrapping = TextWrapping.Wrap;
			o.Foreground = Brushes.LightGray;
		});
		return sp;
	}

	Control MkTextEditor(){
		var editor = JsonTextEditorCtrl.Mk(Ctx?.Text, IsReadOnly: false, MinHeight: 320);
		WordEditor = editor;
		editor.TextChanged += (s,e)=>{
			if(IsSyncingText || Ctx is null){
				return;
			}
			IsSyncingText = true;
			Ctx.Text = editor.Text;
			IsSyncingText = false;
		};
		if(Ctx is not null){
			Ctx.PropertyChanged += OnVmPropertyChanged;
		}
		DetachedFromVisualTree += (s,e)=>{
			if(Ctx is not null){
				Ctx.PropertyChanged -= OnVmPropertyChanged;
			}
		};
		editor.Margin = new Thickness(10, 0, 10, 10);
		return editor;
	}

	void OnVmPropertyChanged(object? Sender, PropertyChangedEventArgs E){
		if(E.PropertyName != nameof(Ctx.Text) || WordEditor is null || Ctx is null || IsSyncingText){
			return;
		}
		if(WordEditor.Text == Ctx.Text){
			return;
		}
		IsSyncingText = true;
		WordEditor.Text = Ctx.Text;
		IsSyncingText = false;
	}
}
