namespace Ngaq.Ui.Views.Word.WordLearnPage;

using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
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

/// 學習記錄分頁：列表 + 新增，點行進入編輯頁。
public partial class ViewWordLearnPage: AppViewBase{
	public VmWordLearnPage? Ctx{
		get{return DataContext as VmWordLearnPage;}
		set{DataContext = value;}
	}

	TreeDataGrid? Grid;
	INotifyCollectionChanged? RowsNotifier;
	VmWordLearnPage? SubscribedCtx;

	public ViewWordLearnPage(){
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
		o.Content = Icons.Add().ToIcon().WithText(" "+I[K.AddLearn]);
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
		var source = new FlatTreeDataGridSource<VmWordLearnRow>(Ctx.Rows){
			Columns = {
				new TextColumn<VmWordLearnRow, str>("", x=>GetIdxText(x)),
				new TextColumn<VmWordLearnRow, str>(I[K.LearnResult], x=>x.LearnResultText),
				new TextColumn<VmWordLearnRow, str>(I[K.Biz_CreatedAt], x=>x.BizCreatedAtDisplay),
			},
		};
		Grid.Source = source;
	}

	/// `Ctx` 後置注入後才有真正的行數據，這裏補建表格源。
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

	/// `LoadFromPoLearns` 會直接替換 `Rows`，需重新綁定集合事件。
	void OnCtxPropertyChanged(object? Sender, PropertyChangedEventArgs E){
		if(E.PropertyName == nameof(VmWordLearnPage.Rows)){
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

	str GetIdxText(VmWordLearnRow Row){
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
				if(row.DataContext is VmWordLearnRow vmRow){
					Ctx.RequestEdit(vmRow);
					E.Handled = true;
				}
				return;
			}
		}
	}
}
