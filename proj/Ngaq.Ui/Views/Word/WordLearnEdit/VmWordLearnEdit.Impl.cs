namespace Ngaq.Ui.Views.Word.WordLearnEdit;

public partial class VmWordLearnEdit{
	public async partial Task<bool> Delete(CT Ct){
		if(OnDelete is null){
			return false;
		}
		return await OnDelete(Ct);
	}
}
