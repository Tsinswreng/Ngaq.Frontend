using Ngaq.Ui.Views.Word.PoWordEdit;

namespace Ngaq.Ui.Views.Word.WordEditV2;

public interface IViewWordEditV2{
	public Task<nil> ClickDelete(CT Ct);
	public event EventHandler DoneDelete;

	public Task<nil> ClickSave(CT Ct);
	public event EventHandler DoneSave;

	public IViewPoWordEdit? PoWordEdit{get;}

}
