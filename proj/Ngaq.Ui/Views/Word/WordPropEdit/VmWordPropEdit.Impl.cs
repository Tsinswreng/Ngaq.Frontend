namespace Ngaq.Ui.Views.Word.WordPropEdit;

public partial class VmWordPropEdit{
	public async partial Task<bool> Delete(CT Ct){
		if(OnDelete is null){
			return false;
		}
		return await OnDelete(Ct);
	}
}
