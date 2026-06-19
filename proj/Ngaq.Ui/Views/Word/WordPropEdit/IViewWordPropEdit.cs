using Ngaq.Ui.Infra;

namespace Ngaq.Ui.Views.Word.WordPropEdit;

public interface IViewWordPropEdit{
	public str? KType{get;set;}
	public str? KStr{get;set;}
	public str? KI64{get;set;}
	public str? VType{get;set;}
	public str? VStr{get;set;}
	public str? VI64{get;set;}
	public IBtn BtnSave{get;set;}
	public IBtn BtnRemove{get;set;}
}

