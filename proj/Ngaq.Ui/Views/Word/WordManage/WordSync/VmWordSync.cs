namespace Ngaq.Ui.Views.Word.WordManage.WordSync;
using System.Collections.ObjectModel;
using Ngaq.Client.Svc;
using Ngaq.Client.Word.Svc;
using Ngaq.Core.Shared.Kv.Svc;
using Ngaq.Core.Shared.User.Svc;
using Ngaq.Ui.Infra;

using Ctx = VmWordSync;
public partial class VmWordSync: ViewModelBase{
	ISvcKv? SvcKv;
	ClientWordSync? ClientWordSync;

	public VmWordSync(
		ISvcKv? SvcKv
		,ClientWordSync? ClientWordSync
	){
		this.SvcKv = SvcKv;
		this.ClientWordSync = ClientWordSync;
	}

	protected VmWordSync(){}
	public static Ctx Mk(){
		return new Ctx();
	}
	public static ObservableCollection<Ctx> Samples = [];
	static VmWordSync(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}


	public CancellationTokenSource Cts = new();
	public async Task<nil> PushAsy(CT Ct){
		if(ClientWordSync is null){
			return NIL;
		}
		await ClientWordSync.AllWordsToServerNonStream(Ct);
		return NIL;
	}

	public nil Push(){
		PushAsy(Cts.Token).ContinueWith(t=>{
			if(t.IsFaulted){
				System.Console.Error.WriteLine(t.Exception);//t
			}
		});
		return NIL;
	}


}
