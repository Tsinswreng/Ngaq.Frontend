using Ngaq.Core.Frontend.Clipboard;

namespace Ngaq.Windows.Domains.Clipboard;

public class SvcClipboard : ISvcClipboard{
	public async Task<str?> GetText(CT Ct){
		return WinClipBoard.GetText();
	}
}
