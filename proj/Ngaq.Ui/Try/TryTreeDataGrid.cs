using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace Ngaq.Ui.Try;

public class TryTreeDataGrid {
	private static Window? _TreeDemoWnd;
	Control? Try() {
		EnsureTreeDataGridTheme();

		var demoItems = new List<TreeRow>{
		new("Root A", "folder", new List<TreeRow>{ new("A-1", "leaf"), new("A-2", "leaf") }),
		new("Root B", "folder", new List<TreeRow>{ new("B-1", "leaf"), new("B-2", "leaf"), new("B-3", "leaf") }),
	};

		var source = new HierarchicalTreeDataGridSource<TreeRow>(demoItems) {
			Columns = {
			new HierarchicalExpanderColumn<TreeRow>(new TextColumn<TreeRow, string>("Name", x=>x.Name), x=>x.Children),
			new TextColumn<TreeRow, string>("Type", x=>x.Type),
		},
		};

		var treeDataGrid = new TreeDataGrid {
			Source = source,
			MinHeight = 280,
			Margin = new Thickness(12),
		};

		var host = new Grid {
			RowDefinitions = new RowDefinitions("Auto,*"),
		};
		host.Children.Add(new TextBlock {
			Text = "TreeDataGrid Demo",
			Margin = new Thickness(12, 10, 12, 0),
			Foreground = Brushes.White,
		});
		Grid.SetRow(treeDataGrid, 1);
		host.Children.Add(treeDataGrid);

		Dispatcher.UIThread.Post(() => {
			_TreeDemoWnd ??= new Window {
				Title = "TreeDataGrid Demo",
				Width = 640,
				Height = 420,
				RequestedThemeVariant = ThemeVariant.Dark,
				Background = new SolidColorBrush(Color.FromRgb(32, 32, 32)),
			};
			_TreeDemoWnd.Content = host;
			_TreeDemoWnd.Show();
		}, DispatcherPriority.Background);

		return treeDataGrid;
	}



	private static void EnsureTreeDataGridTheme() {
		var app = Application.Current;
		if (app is null) {
			return;
		}

		foreach (var style in app.Styles) {
			if (style is StyleInclude si && si.Source?.ToString()?.Contains("Avalonia.Controls.TreeDataGrid", StringComparison.OrdinalIgnoreCase) == true) {
				return;
			}
		}

		app.Styles.Add(new StyleInclude(new Uri("avares://Avalonia.Controls.TreeDataGrid/")) {
			Source = new Uri("avares://Avalonia.Controls.TreeDataGrid/Themes/Fluent.axaml"),
		});
	}

	private sealed class TreeRow {
		public string Name { get; }
		public string Type { get; }
		public IReadOnlyList<TreeRow> Children { get; }

		public TreeRow(string name, string type, IReadOnlyList<TreeRow>? children = null) {
			Name = name;
			Type = type;
			Children = children ?? [];
		}
	}

}
