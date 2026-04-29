namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit.PreFilterPayloadJsonEdit;

using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using AvaloniaEdit;
using Ngaq.Ui;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.Ctrls;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

using Ctx = VmPreFilterPayloadJsonEdit;using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;

/// <summary>
/// PreFilter 載荷 JSON 文本編輯頁。
/// </summary>
public class ViewPreFilterPayloadJsonEdit: AppViewBase{
	public Ctx? Ctx{
		get{return DataContext as Ctx;}
		set{DataContext = value;}
	}

	public ViewPreFilterPayloadJsonEdit(){
		Ctx = App.DiOrMk<Ctx>();
		Render();
	}

	AutoGrid Root = new(IsRow: true);
	TextEditor? PayloadEditor;
	bool IsSyncingPayloadText = false;
	protected nil Render(){
		Content = Root.Grid;
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(1, GUT.Star),
			RowDef(1, GUT.Auto),
		]);
		Root.A(MkErrorBar());
		Root.A(JsonText());
		Root.A(MkBottomBar());
		return NIL;
	}

	Control MkErrorBar(){
		var b = new Border{
			Background = new SolidColorBrush(Color.FromArgb(80, 180, 30, 30)),
			Padding = new Thickness(10, 6),
			IsVisible = false,
			Margin = new Thickness(10, 10, 10, 4),
		};
		b.CBind<Ctx>(IsVisibleProperty, x=>x.HasError, Mode: BindingMode.OneWay);
		var txt = new TextBlock{
			Foreground = Brushes.White,
		};
		txt.CBind<Ctx>(TextBlock.TextProperty, x=>x.LastError, Mode: BindingMode.OneWay);
		b.Child = txt;
		return b;
	}

	Control MkBottomBar(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.ColumnDefinitions.AddRange([
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
			ColDef(1, GUT.Star),
		]);
		g.A(new Button(), o=>{
			o.Content = I[K.Back];
			o.Click += (s,e)=>ViewNavi?.Back();
		});
		g.A(new OpBtn(), o=>{
			o._Button.HorizontalContentAlignment = HAlign.Center;
			o.BtnContent = Icons.Delete().ToIcon().WithText(I[K.Delete]);
			o._Button.Background = UiCfg.Inst.DelBtnBg;
			o.SetExe((Ct)=>Ctx?.Delete(Ct));
		});
		g.A(new OpBtn(), o=>{
			o._Button.HorizontalContentAlignment = HAlign.Center;
			o.BtnContent = Icons.Save().ToIcon().WithText(I[K.Save]);
			o._Button.Background = UiCfg.Inst.MainColor;
			o.SetExe((Ct)=>Ctx?.Save(Ct));
		});
		return g.Grid;
	}

	Control JsonText(){
		var editor = JsonTextEditorCtrl.Mk(Ctx?.PayloadJson, IsReadOnly: false, MinHeight: 220);
		editor.Margin = new Thickness(10, 4, 10, 10);
		PayloadEditor = editor;
		editor.TextChanged += (s,e)=>{
			if(IsSyncingPayloadText || Ctx is null){
				return;
			}
			IsSyncingPayloadText = true;
			Ctx.PayloadJson = editor.Text;
			IsSyncingPayloadText = false;
		};
		if(Ctx is not null){
			Ctx.PropertyChanged += OnVmPropertyChanged;
		}
		DetachedFromVisualTree += (s,e)=>{
			if(Ctx is not null){
				Ctx.PropertyChanged -= OnVmPropertyChanged;
			}
		};
		return editor;
	}

	void OnVmPropertyChanged(object? Sender, PropertyChangedEventArgs E){
		if(E.PropertyName != nameof(Ctx.PayloadJson) || PayloadEditor is null || Ctx is null || IsSyncingPayloadText){
			return;
		}
		if(PayloadEditor.Text == Ctx.PayloadJson){
			return;
		}
		IsSyncingPayloadText = true;
		PayloadEditor.Text = Ctx.PayloadJson;
		IsSyncingPayloadText = false;
	}
}
