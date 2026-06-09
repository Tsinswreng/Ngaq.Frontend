namespace Ngaq.Ui.Views.Word.PoWordEdit;


public interface IViewPoWordEdit{
	public str? Id{get;}
	public str? Head{get;set;}
	public str? Lang{get;set;}
	public str? StoredAt{get;set;}
	public str? BizCreatedAt{get;set;}
	public str? BizUpdatedAt{get;set;}
	public str? DelAt{get;set;}
	public Task<nil> ClickDelete(CT Ct);
	public event Func<CT, Task<nil>>? DoneDelete;

	public Task<nil> ClickSave(CT Ct);
	public event Func<CT, Task<nil>>? DoneSave;
}
