namespace Ngaq.Ui.Views.Word.WordPropPage;

public interface IViewWordPropPage{
	public Task<nil> ClickAddProp(CT Ct);
	public event EventHandler? DoneAddProp;
}

public interface IViewWordPropRow{
	public str? No{get;}
	public str? Key{get;}
	public str? Values{get;}

}
