namespace Ngaq.Ui.Views.Word.WordPropPage;

using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Layout;
using Avalonia.Media;
using Ngaq.Core.Infra;
using Ngaq.Ui.Icons;
using Ngaq.Ui.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using K = Ngaq.Ui.Infra.I18n.KeysUiI18nCommon;
using Avalonia;
using Ngaq.Ui.Tools;
using Avalonia.Interactivity;

/// 屬性分頁：列表 + 新增，點行進入編輯頁。
public partial class ViewWordPropPage: AppViewBase{
	public VmWordPropPage? Ctx{
		get{return DataContext as VmWordPropPage;}
		set{DataContext = value;}
	}

	TreeDataGrid? Grid;
	INotifyCollectionChanged? RowsNotifier;
	VmWordPropPage? SubscribedCtx;

	public ViewWordPropPage(){
		Render();
		DataContextChanged += (s, e)=>OnCtxChanged();
	}

	void Render(){
		var root = new AutoGrid(IsRow: true);
		root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Auto),
			RowDef(9, GUT.Star),
		]);
		root.A(MkBtnAdd(), o=>{
			o.Click += (s, e)=>{
				Ctx?.AddRow();
				RebuildGrid();
			};
		});
		root.A(MkGrid());
		Content = root.Grid;
	}

	Button MkBtnAdd(){
		var o = new Button();
		o.Margin = new Thickness(10, 10, 10, 4);
		o.StretchCenter();
		o.Content = Icons.Add().ToIcon().WithText(" "+I[K.AddProp]);
		return o;
	}

	Control MkGrid(){
		Grid = new TreeDataGrid{
			Margin = new Thickness(10, 4, 10, 10),
			MinHeight = 260,
		};
		Grid.Styles.Add(
			new Style(x=>x.OfType<TreeDataGridRow>().Class(":pointerover"))
			.Set(TemplatedControl.BackgroundProperty, new SolidColorBrush(Color.FromRgb(46, 46, 46)))
		);
		Grid.Styles.Add(
			new Style(x=>x.OfType<TreeDataGridRow>().Class(":pressed"))
			.Set(TemplatedControl.BackgroundProperty, new SolidColorBrush(Color.FromRgb(70, 70, 70)))
		);
		Grid.AddHandler(InputElement.TappedEvent, OnGridTapped, RoutingStrategies.Bubble, true);
		RebuildGrid();
		return Grid;
	}

	void RebuildGrid(){
		if(Ctx is null || Grid is null){
			return;
		}
		var source = new FlatTreeDataGridSource<VmWordPropRow>(Ctx.Rows){
			Columns = {
				new TextColumn<VmWordPropRow, str>("", x=>GetIdxText(x)),
				new TextColumn<VmWordPropRow, str>(I[K.Key], x=>x.KeyText),
				new TextColumn<VmWordPropRow, str>(I[K.KType], x=>x.KTypeText),
				new TextColumn<VmWordPropRow, str>(I[K.VType], x=>x.VTypeText),
			},
		};
		Grid.Source = source;
	}

	/// `Ctx` 是在構造後才灌入的，列表頁需在此時補掛監聽並首次建表格源。
	void OnCtxChanged(){
		if(SubscribedCtx is not null){
			SubscribedCtx.PropertyChanged -= OnCtxPropertyChanged;
			SubscribedCtx = null;
		}
		if(RowsNotifier is not null){
			RowsNotifier.CollectionChanged -= OnRowsChanged;
			RowsNotifier = null;
		}
		if(Ctx is null){
			return;
		}
		SubscribedCtx = Ctx;
		Ctx.PropertyChanged += OnCtxPropertyChanged;
		HookRowsChanged();
		RebuildGrid();
	}

	/// `LoadFromPoProps` 會整體替換 `Rows`，需重新掛上集合變更監聽。
	void OnCtxPropertyChanged(object? Sender, PropertyChangedEventArgs E){
		if(E.PropertyName == nameof(VmWordPropPage.Rows)){
			HookRowsChanged();
			RebuildGrid();
		}
	}

	void HookRowsChanged(){
		if(RowsNotifier is not null){
			RowsNotifier.CollectionChanged -= OnRowsChanged;
		}
		RowsNotifier = Ctx?.Rows;
		if(RowsNotifier is not null){
			RowsNotifier.CollectionChanged += OnRowsChanged;
		}
	}

	void OnRowsChanged(object? Sender, NotifyCollectionChangedEventArgs E){
		RebuildGrid();
	}

	str GetIdxText(VmWordPropRow Row){
		if(Ctx is null){
			return "";
		}
		var idx = Ctx.Rows.IndexOf(Row);
		return idx < 0 ? "" : (idx+1)+"";
	}

	void OnGridTapped(object? Sender, TappedEventArgs E){
		if(Ctx is null){
			return;
		}
		if(E.Source is not StyledElement src){
			return;
		}
		for(StyledElement? cur = src; cur is not null; cur = cur.Parent){
			if(cur is TreeDataGridRow row){
				if(row.DataContext is VmWordPropRow vmRow){
					Ctx.RequestEdit(vmRow);
					E.Handled = true;
				}
				return;
			}
		}
	}
}
