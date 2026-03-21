namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Ngaq.Core.Tools.JsonMap;
using Ngaq.Ui.Components.KvMap.JsonMap;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;

public partial class VmWeightArgQuery: ViewModelBase, IMk<VmWeightArgQuery>{
	protected VmWeightArgQuery(){ StudyPlanUiStore.EnsureInit(); Refresh(); }
	public static VmWeightArgQuery Mk()=>new();

	public str SearchText{ get=>field; set{ if(SetProperty(ref field,value)){ PageIdx=0; Refresh(); } } }="";
	public i32 PageIdx{ get=>field; set{ if(SetProperty(ref field,value)){ Refresh(); OnPropertyChanged(nameof(PageText)); } } }=0;
	public i32 PageSize{get;set;} = 8;
	public ObservableCollection<UiWeightArg> PageItems{ get=>field; set=>SetProperty(ref field,value);}=[];
	public i32 Total{ get=>field; set=>SetProperty(ref field,value);} = 0;
	public str PageText => $"Page {PageIdx+1}";

	public nil Prev(){ if(PageIdx>0){ PageIdx--; } return NIL; }
	public nil Next(){ if((PageIdx+1)*PageSize < Total){ PageIdx++; } return NIL; }

	public nil Refresh(){
		IEnumerable<UiWeightArg> seq = StudyPlanUiStore.WeightArgs;
		var q = (SearchText??"").Trim();
		if(!str.IsNullOrWhiteSpace(q)){
			seq = seq.Where(x=>x.Id.Contains(q, StringComparison.OrdinalIgnoreCase)
				|| x.UniqName.Contains(q, StringComparison.OrdinalIgnoreCase)
				|| x.Descr.Contains(q, StringComparison.OrdinalIgnoreCase));
		}
		var list = seq.ToList();
		Total = list.Count;
		PageItems = new ObservableCollection<UiWeightArg>(list.Skip(PageIdx*PageSize).Take(PageSize));
		OnPropertyChanged(nameof(PageText));
		return NIL;
	}
}

public partial class VmWeightArgEdit: ViewModelBase, IMk<VmWeightArgEdit>{
	protected VmWeightArgEdit(){ StudyPlanUiStore.EnsureInit(); }
	public static VmWeightArgEdit Mk()=>new();

	public str Id{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str UniqName{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str Descr{ get=>field; set=>SetProperty(ref field, value);} = "";
	public str JsonText{ get=>field; set=>SetProperty(ref field, value);} = "{}";
	public VmUiJsonMap WeightArgMapVm{ get=>field; set=>SetProperty(ref field, value);} = VmUiJsonMap.Mk();

	public nil Load(UiWeightArg? src){
		if(src is null){
			Id = Guid.NewGuid().ToString("N");
			UniqName = "";
			Descr = "";
			BindMap(new Dictionary<str, object?>{
				["DfltAddBonus"] = 100d,
				["DebuffNumerator"] = 36d,
				["AddCnt_Bonus"] = new List<object?>{64d, 128d},
			});
			SyncJsonFromMap();
			return NIL;
		}
		Id = src.Id;
		UniqName = src.UniqName;
		Descr = src.Descr;
		BindMap(src.Raw.ToDictionary(x=>x.Key, x=>x.Value));
		SyncJsonFromMap();
		return NIL;
	}

	public nil Save(){
		if(str.IsNullOrWhiteSpace(Id) || str.IsNullOrWhiteSpace(UniqName)){
			ShowMsg("Id / UniqName 不能為空");
			return NIL;
		}
		WeightArgMapVm.UpdData();
		var raw = ExtractRaw();
		var got = StudyPlanUiStore.WeightArgs.FirstOrDefault(x=>x.Id == Id);
		if(got is null){
			StudyPlanUiStore.WeightArgs.Add(new UiWeightArg{ Id = Id.Trim(), UniqName = UniqName.Trim(), Descr = Descr, Raw = raw });
		}else{
			got.UniqName = UniqName.Trim();
			got.Descr = Descr;
			got.Raw = raw;
		}
		SyncJsonFromMap();
		ShowMsg("WeightArg 已保存");
		return NIL;
	}

	public nil Delete(){
		var got = StudyPlanUiStore.WeightArgs.FirstOrDefault(x=>x.Id == Id);
		if(got is not null){
			StudyPlanUiStore.WeightArgs.Remove(got);
			ShowMsg("WeightArg 已刪除");
		}
		return NIL;
	}

	public nil SyncJsonFromMap(){
		WeightArgMapVm.UpdData();
		JsonText = JsonSerializer.Serialize(ExtractRaw(), new JsonSerializerOptions{ WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
		return NIL;
	}

	public nil ApplyJsonToMap(){
		try{
			var node = JsonNode.Parse(JsonText);
			var rawObj = NodeToRaw(node);
			if(rawObj is Dictionary<str, object?> dict){
				BindMap(dict);
				ShowMsg("Json 已套用到 UiJsonMap");
			}else{
				ShowMsg("WeightArg Json 須為 object");
			}
		}catch(Exception ex){
			ShowMsg(ex.Message);
		}
		return NIL;
	}

	Dictionary<str, object?> ExtractRaw(){
		if(WeightArgMapVm.UiJsonMap?.Raw.ValueObj is IDictionary<str, object?> d){
			return d.ToDictionary(x=>x.Key, x=>x.Value);
		}
		return [];
	}

	void BindMap(Dictionary<str, object?> raw){
		var map = new UiJsonMap{ Raw = new Tsinswreng.CsTools.JsonNode(raw) };
		var pathMap = new Dictionary<str, IUiJsonMapItem>();

		foreach(var kv in raw){
			var key = kv.Key;
			var value = kv.Value;
			if(IsScalar(value)){
				pathMap[key] = new UiJsonMapItem(map, key){
					JsonValueType = ToValueType(value),
					DisplayName = UiText.FromRawText(key),
					Descr = UiText.FromRawText(key),
				};
				continue;
			}
			if(value is IList<object?> list){
				for(var i=0; i<list.Count; i++){
					var item = list[i];
					if(!IsScalar(item)){ continue; }
					var path = $"{key}[{i}]";
					pathMap[path] = new UiJsonMapItem(map, path){
						JsonValueType = ToValueType(item),
						DisplayName = UiText.FromRawText(path),
						Descr = UiText.FromRawText(path),
					};
				}
			}
		}
		map.PathToUiMap = pathMap;
		WeightArgMapVm.FromBo(map);
	}

	static bool IsScalar(object? v){
		return v is null || v is str || v is bool || v is int || v is long || v is float || v is double || v is decimal;
	}
	static EJsonValueType ToValueType(object? v){
		if(v is bool){ return EJsonValueType.Boolean; }
		if(v is str){ return EJsonValueType.String; }
		if(v is int || v is long || v is float || v is double || v is decimal){ return EJsonValueType.Number; }
		return EJsonValueType.String;
	}

	static object? NodeToRaw(JsonNode? node){
		if(node is null){ return null; }
		if(node is JsonObject jo){
			var d = new Dictionary<str, object?>();
			foreach(var kv in jo){ d[kv.Key] = NodeToRaw(kv.Value); }
			return d;
		}
		if(node is JsonArray ja){
			return ja.Select(NodeToRaw).ToList<object?>();
		}
		if(node is JsonValue jv){
			if(jv.TryGetValue<bool>(out var b)){ return b; }
			if(jv.TryGetValue<double>(out var n)){ return n; }
			if(jv.TryGetValue<str>(out var s)){ return s; }
			return jv.ToJsonString();
		}
		return null;
	}
}

public partial class ViewWeightArgQuery: AppViewBase{
	public VmWeightArgQuery? Ctx{ get=>DataContext as VmWeightArgQuery; set=>DataContext = value; }
	public ViewWeightArgQuery(){ Ctx = VmWeightArgQuery.Mk(); Render(); }

	protected nil Render(){
		var root = new AutoGrid(IsRow:true);
		root.Grid.RowDefinitions.AddRange([ RowDef(1,GUT.Auto), RowDef(8,GUT.Star), RowDef(1,GUT.Auto) ]);
		this.Content = root.Grid;
		root.AddInit(MkTop(), o=>{});
		root.AddInit(new ScrollViewer{ Content = MkList(), Margin = new Thickness(8,0,8,4) }, o=>{});
		root.AddInit(MkPageBar(), o=>{});
		return NIL;
	}

	Control MkTop(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.Margin = new Thickness(8,8,8,4);
		g.Grid.ColumnDefinitions.AddRange([ ColDef(7,GUT.Star), ColDef(3,GUT.Star) ]);
		g.AddInit(new TextBox{ Watermark = "Search WeightArg" }, o=>o.Bind(o.PropText, CBE.Mk<VmWeightArgQuery>(x=>x.SearchText, Mode:BindingMode.TwoWay)));
		g.AddInit(new Button{ Content = "New", MinHeight = 36 }, o=>o.Click += (s,e)=>OpenEdit(null));
		return g.Grid;
	}

	Control MkList(){
		var items = new ItemsControl();
		items.Bind(items.PropItemsSource, CBE.Mk<VmWeightArgQuery>(x=>x.PageItems));
		items.SetItemsPanel(()=>new StackPanel{ Spacing = 6 });
		items.SetItemTemplate<UiWeightArg>((row, ns)=>{
			var b = new Border{ BorderThickness = new Thickness(1), Padding = new Thickness(8) };
			var g = new AutoGrid(IsRow:true);
			g.Grid.RowDefinitions.AddRange([ RowDef(1,GUT.Auto), RowDef(1,GUT.Auto), RowDef(1,GUT.Auto) ]);
			g.AddInit(new TextBlock{ Text = $"{row.UniqName} ({row.Id})" }, o=>{});
			g.AddInit(new TextBlock{ Text = row.Descr, TextWrapping = TextWrapping.Wrap }, o=>{});
			g.AddInit(new Button{ Content = "Edit", MinHeight = 34 }, o=>o.Click += (s,e)=>OpenEdit(row));
			b.Child = g.Grid;
			return b;
		});
		return items;
	}

	Control MkPageBar(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.Margin = new Thickness(8,0,8,8);
		g.Grid.ColumnDefinitions.AddRange([ ColDef(1,GUT.Auto), ColDef(1,GUT.Auto), ColDef(1,GUT.Auto) ]);
		g.AddInit(new Button{ Content="<", MinHeight = 34 }, o=>o.Click+=(s,e)=>Ctx?.Prev());
		g.AddInit(new TextBlock{ VerticalAlignment = VAlign.Center }, o=>o.Bind(o.PropText, CBE.Mk<VmWeightArgQuery>(x=>x.PageText)));
		g.AddInit(new Button{ Content=">", MinHeight = 34 }, o=>o.Click+=(s,e)=>Ctx?.Next());
		return g.Grid;
	}

	void OpenEdit(UiWeightArg? row){
		var v = new ViewWeightArgEdit();
		v.Ctx?.Load(row);
		Ctx?.ViewNavi?.GoTo(ToolView.WithTitle("WeightArg Edit", v));
	}
}

public partial class ViewWeightArgEdit: AppViewBase{
	public VmWeightArgEdit? Ctx{ get=>DataContext as VmWeightArgEdit; set=>DataContext = value; }
	public ViewWeightArgEdit(){ Ctx = VmWeightArgEdit.Mk(); Render(); }

	protected nil Render(){
		var root = new AutoGrid(IsRow:true);
		root.Grid.RowDefinitions.AddRange([ RowDef(8,GUT.Star), RowDef(1,GUT.Auto) ]);
		this.Content = root.Grid;

		var tab = new TabControl();
		tab.Items.AddInit(new TabItem(), o=>{ o.Header = "Form"; o.Content = new ScrollViewer{ Content = MkForm(), Margin = new Thickness(8,8,8,4) }; });
		tab.Items.AddInit(new TabItem(), o=>{ o.Header = "UiJsonMap"; o.Content = new ViewUiJsonMap{ Ctx = Ctx?.WeightArgMapVm }; });
		tab.Items.AddInit(new TabItem(), o=>{ o.Header = "Json"; o.Content = MkJsonTab(); });

		root.AddInit(tab, o=>{});
		root.AddInit(MkOps(), o=>{});
		return NIL;
	}

	Control MkForm(){
		var sp = new StackPanel{ Spacing = 8 };
		sp.Children.Add(MkInput("Id", CBE.Mk<VmWeightArgEdit>(x=>x.Id, Mode:BindingMode.TwoWay), true));
		sp.Children.Add(MkInput("UniqName", CBE.Mk<VmWeightArgEdit>(x=>x.UniqName, Mode:BindingMode.TwoWay)));
		sp.Children.Add(MkInput("Descr", CBE.Mk<VmWeightArgEdit>(x=>x.Descr, Mode:BindingMode.TwoWay)));
		return sp;
	}

	Control MkJsonTab(){
		var g = new AutoGrid(IsRow:true);
		g.Grid.RowDefinitions.AddRange([ RowDef(1,GUT.Auto), RowDef(8,GUT.Star) ]);
		var op = new AutoGrid(IsRow:false);
		op.Grid.ColumnDefinitions.AddRange([ ColDef(1,GUT.Star), ColDef(1,GUT.Star) ]);
		op.AddInit(new Button{ Content = "Map -> Json", MinHeight = 34 }, o=>o.Click += (s,e)=>Ctx?.SyncJsonFromMap());
		op.AddInit(new Button{ Content = "Json -> Map", MinHeight = 34 }, o=>o.Click += (s,e)=>Ctx?.ApplyJsonToMap());
		g.AddInit(op.Grid, o=>o.Margin = new Thickness(8,8,8,4));
		g.AddInit(new TextBox{ AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(8,0,8,8) }, o=>{
			o.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
			o.Bind(o.PropText, CBE.Mk<VmWeightArgEdit>(x=>x.JsonText, Mode:BindingMode.TwoWay));
		});
		return g.Grid;
	}

	Control MkInput(str label, IBinding bind, bool readOnly = false){
		var sp = new StackPanel{ Spacing = 2 };
		sp.Children.Add(new TextBlock{ Text = label });
		var tb = new TextBox{ IsReadOnly = readOnly };
		tb.Bind(tb.PropText, bind);
		sp.Children.Add(tb);
		return sp;
	}

	Control MkOps(){
		var g = new AutoGrid(IsRow:false);
		g.Grid.Margin = new Thickness(8,0,8,8);
		g.Grid.ColumnDefinitions.AddRange([ ColDef(1,GUT.Star), ColDef(1,GUT.Star), ColDef(1,GUT.Star) ]);
		g.AddInit(new Button{ Content = "Save", MinHeight = 36 }, o=>o.Click+=(s,e)=>Ctx?.Save());
		g.AddInit(new Button{ Content = "Delete", MinHeight = 36 }, o=>o.Click+=(s,e)=>Ctx?.Delete());
		g.AddInit(new Button{ Content = "Back", MinHeight = 36 }, o=>o.Click+=(s,e)=>Ctx?.ViewNavi?.Back());
		return g.Grid;
	}
}
