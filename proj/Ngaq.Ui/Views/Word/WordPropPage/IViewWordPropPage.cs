using Ngaq.Ui.Infra;

namespace Ngaq.Ui.Views.Word.WordPropPage;

public interface IViewWordPropPage{
	public IBtn BtnAddProp{get;}
	public IList<IViewWordPropRow>? Rows{get;}
}

public interface IViewWordPropRow{
	public str? No{get;}
	public str? Key{get;}
	public str? Values{get;}

}
