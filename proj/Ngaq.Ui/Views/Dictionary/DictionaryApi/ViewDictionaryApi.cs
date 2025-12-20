namespace Ngaq.Ui.Views.Dictionary.DictionaryApi;

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Ngaq.Core.Shared.Word.Models.DictionaryApi;
using Ngaq.Ui;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Infra.I18n;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Ctx = VmDictionaryApi;
public partial class ViewDictionaryApi
	: AppViewBase {

	public Ctx? Ctx {
		get { return DataContext as Ctx; }
		set { DataContext = value; }
	}

	public ViewDictionaryApi() {
		//Ctx = App.DiOrMk<Ctx>();
		Ctx = Ctx.Samples[0];
		Style();
		Render();
	}
	public II18n I = I18n.Inst;
	public partial class Cls {

	}

	protected nil Style() {
		return NIL;
	}

	AutoGrid Root = new(IsRow: true);

	SelectableTextBlock Txt(){
		return new();
	}

	public void Render() {
		// 整頁只有一個列表，高度自適應
		Root.Grid.RowDefinitions.AddRange([
			RowDef(1, GUT.Star)
		]);
		this.Content = Root.Grid;

		// ListBox 綁定 Words
		Root.AddInit(new ListBox(), lst => {
			lst.Bind(ItemsControl.ItemsSourceProperty, CBE.Mk<Ctx>(x => x.Words));
			// 每條目自己長什麼樣
			lst.ItemTemplate = new FuncDataTemplate<DictionaryApiWord>((word, _) => {
				var itemPanel = new AutoGrid(IsRow: true);

				// 單詞 + 音標
				itemPanel.AddInit(Txt(), o=>{
					o.FontSize = 20;
					o.FontWeight = Avalonia.Media.FontWeight.Bold;
					o.Foreground = Brushes.DarkCyan;
					o.Text = $"{word.word}  {word.phonetics?.FirstOrDefault()?.text ?? ""}";
				});

				// 每個 meaning
				foreach (var m in word.meanings ?? new List<Meaning>()) {
					itemPanel.AddInit(Txt(), o=>{
						o.FontWeight = FontWeight.SemiBold;
						o.Margin = new(0, 6, 0, 0);
						o.Text = m.partOfSpeech ?? "" ;
					});

					foreach (var d in m.definitions ?? new List<Definition>()) {
						itemPanel.AddInit(Txt(), o=>{
							o.TextWrapping = TextWrapping.Wrap;
							o.Text = $"• {d.definition}" +
								(string.IsNullOrEmpty(d.example) ? "" : $"\n    E.g. {d.example}");
						});
					}
				}

				return itemPanel.Grid;
			});
		});
	}


}
/*
我現在調後端接口拿到了IList<DictionaryApiWord>
你把拿到的數據顯示出來
 */
