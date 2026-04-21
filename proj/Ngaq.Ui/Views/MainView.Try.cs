using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;

namespace Ngaq.Ui.Views;

public partial class MainView {
	void Try() {
		var editor = new TextEditor {
			Text = "{\n  \"name\": \"Ngaq\",\n  \"enabled\": true,\n  \"items\": [\n    1,\n    2,\n    3\n  ]\n}",
			ShowLineNumbers = true,
			FontFamily = new FontFamily("Consolas"),
			FontSize = 14,
			Foreground = Brushes.White,
			SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JSON"),
			HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
		};
		editor.Options.ConvertTabsToSpaces = true;
		editor.Options.IndentationSize = 2;
		editor.Options.EnableHyperlinks = false;
		editor.Options.EnableEmailHyperlinks = false;

		var window = new Window {
			Title = "AvaloniaEdit JSON Demo",
			Width = 900,
			Height = 600,
			Content = editor,
		};

		window.Show();
	}
}
