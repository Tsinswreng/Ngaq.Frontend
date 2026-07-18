namespace Ngaq.Ui.Views.Word.WordEditV2.WordPropEdit;

public partial class VmWordPropEdit{
	/// 未注入刪除策略時拒絕執行，避免 View 繞過父頁的資料一致性處理。
	public async partial Task<bool> Delete(CT Ct){
		if(OnDelete is null){
			return false;
		}
		return await OnDelete(Ct);
	}
}
