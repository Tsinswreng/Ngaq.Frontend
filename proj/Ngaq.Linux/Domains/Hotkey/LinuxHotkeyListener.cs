namespace Ngaq.Linux.Domains.Hotkey;

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Ngaq.Core.Frontend.Hotkey;
using Tsinswreng.CsErr;

/// Linux 平台熱鍵監聽器。
/// 當前版本先提供註冊管理與生命周期能力，爲後續接入桌面環境級全局熱鍵 API 保留擴展點。
public class LinuxHotkeyListener : IHotkeyListener{
	private readonly ILogger _logger;

	/// 保存已註冊熱鍵，避免重複註冊並支持註銷/清理。
	private readonly Dictionary<str, IHotKey> _registeredHotkeys = [];

	public LinuxHotkeyListener(ILogger Logger){
		_logger = Logger;
	}

	/// 註冊熱鍵。
	/// <param name="HotKey">熱鍵定義。</param>
	/// <returns>成功時返回 Ok，失敗時返回錯誤信息。</returns>
	public IAnswer<obj?> Register(IHotKey HotKey){
		var Answer = new Answer<obj?>();

		if(_registeredHotkeys.ContainsKey(HotKey.Id)){
			Answer.AddErr($"Hotkey already registered: {HotKey.Id}");
			return Answer;
		}

		_registeredHotkeys[HotKey.Id] = HotKey;
		_logger?.LogWarning(
			"Linux global hotkey backend is not wired yet. Registered as placeholder only: {HotkeyId} ({Modifiers}+{Key})",
			HotKey.Id,
			HotKey.Modifiers,
			HotKey.Key
		);
		return Answer.OkWith(NIL);
	}

	/// 註銷已註冊熱鍵。
	/// <param name="HotkeyId">熱鍵唯一標識。</param>
	/// <returns>成功時返回 Ok，未找到時返回錯誤。</returns>
	public IAnswer<obj?> Unregister(str HotkeyId){
		var Answer = new Answer<obj?>();
		var Removed = _registeredHotkeys.Remove(HotkeyId);
		if(!Removed){
			Answer.AddErr($"Hotkey not found: {HotkeyId}");
			return Answer;
		}
		return Answer.OkWith(NIL);
	}

	/// 清理全部熱鍵註冊。
	/// <returns>始終返回 Ok。</returns>
	public IAnswer<obj?> Cleanup(){
		_registeredHotkeys.Clear();
		return new Answer<obj?>().OkWith(NIL);
	}
}
