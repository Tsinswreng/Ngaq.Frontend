namespace Ngaq.Ui.Components.KvMap.JsonMap;
using System.Collections.ObjectModel;
using Ngaq.Core.Tools.JsonMap;
using Ngaq.Ui.Infra;

using Ctx = VmJsonMap;
//using Bo = Ngaq.Core.Tools.JsonMap.UiJsonMap;
public partial class VmJsonMap: ViewModelBase, IMk<Ctx>{
	//蔿從構造函數依賴注入、故以靜態工廠代無參構造器
	protected VmJsonMap(){}
	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmJsonMap(){
		#if DEBUG
		{
			var o = new Ctx();
			o.FromBo(SampleJsonMap.Inst.UiJsonMap);
			Samples.Add(o);
		}
		#endif
	}

	public UiJsonMap? UiJsonMap{get;set;}

	public void FromBo(UiJsonMap Bo){
		this.UiJsonMap = Bo;
		if(Bo.PathToUiMap is null){
			ItemVms = new();
			return;
		}
		ItemVms = new(Bo.PathToUiMap.Select((kv)=>{
			var vm = VmJsonMapItem.Mk();
			kv.Value.Path = kv.Key;
			vm.FromBo(Bo, kv.Value);
			return vm;
		}));
	}


	public ObservableCollection<VmJsonMapItem> ItemVms{
		get{return field;}
		set{SetProperty(ref field, value);}
	}=[];

	public void UpdData(){
		foreach(var vm in ItemVms){
			vm.UpdData();
		}
	}

}
