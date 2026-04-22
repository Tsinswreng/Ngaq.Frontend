namespace Ngaq.Windows.Domains.Hotkey;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using Ngaq.Core.Frontend.Hotkey;
using Ngaq.Core.Infra.Cfg;
using Ngaq.Ui.Views.Dictionary;
using Tsinswreng.CsCfg;
using Tsinswreng.CsErr;

/// Windows 专用的全局快捷键注册器
/// TODO 把查詞快捷键之註冊邏輯放到新文件中、然後在此文件中調用
public class WinGlobalHotkeyRegistrar : I_RegisterGlobalHotKeys{
	private const str DictionaryLookupHotkeyId = "dictionary_lookup_from_clipboard";

	private readonly IHotkeyListener _hotkeyListener;
	private readonly IHotkeyDictionaryLookupAction _dictionaryLookupAction;
	private readonly ILogger _logger;

	public WinGlobalHotkeyRegistrar(
		IHotkeyListener hotkeyListener,
		IHotkeyDictionaryLookupAction dictionaryLookupAction,
		ILogger logger
	){
		_hotkeyListener = hotkeyListener;
		_dictionaryLookupAction = dictionaryLookupAction;
		_logger = logger;
	}

	//TODO 叶 ˋ有蠹、AOT編譯後運行即卡住不動、頁面不出
	public IAnswer<obj?> RegisterGlobalHotKeys(){
		var R = new Answer<obj?>();

		try{
			// 從配置讀取查詞熱鍵，避免在代碼中寫死。
			var hotkeyCfg = ReadDictionaryLookupHotkeyCfg();
			var hotkey = new HotKey{
				Id = DictionaryLookupHotkeyId,
				Modifiers = hotkeyCfg.Modifiers,
				Key = hotkeyCfg.Key,
				OnHotkey = (Req, Ct) => {
					Dispatcher.UIThread.Post(async () => {
						try{
							ShowMainWindow();
							await _dictionaryLookupAction.Run(Ct);
						}catch(Exception ex){
							_logger?.LogError(ex, "Dictionary lookup hotkey handler failed");
						}
					});
					return Task.FromResult<IRespHotKey?>(null);
				}
			};

			var result = _hotkeyListener.Register(hotkey);
			if(!result.Ok){
				_logger?.LogWarning(
					"Dictionary lookup hotkey registration failed ({Modifiers}+{Key})\n{Errors}",
					hotkeyCfg.Modifiers,
					hotkeyCfg.Key,
					string.Join(";", result.Errors ?? new[]{""})
				);
			}else{
				_logger?.LogInformation(
					"Dictionary lookup hotkey registered successfully: {Modifiers}+{Key}",
					hotkeyCfg.Modifiers,
					hotkeyCfg.Key
				);
			}

			_logger?.LogInformation("Windows global hotkeys registered successfully");
			return R.OkWith(NIL);
		}catch(Exception ex){
			return R.AddErr(ex);
		}
	}

	/// 讀取「查詞熱鍵」配置，並在配置非法時回退到默認值。
	/// <returns>已解析的熱鍵配置。</returns>
	private DictionaryLookupHotkeyCfg ReadDictionaryLookupHotkeyCfg(){
		var cfg = AppCfg.Inst;
		var rawModifiers = ExtnICfgAccessor.Get(cfg, KeysClientCfg.Hotkey.DictionaryLookup.Modifiers) ?? "";
		var rawKey = ExtnICfgAccessor.Get(cfg, KeysClientCfg.Hotkey.DictionaryLookup.Key) ?? "";

		var modifiers = ParseModifiers(rawModifiers, EHotkeyModifiers.Alt);
		var key = ParseKey(rawKey, EHotkeyKey.E);
		return new DictionaryLookupHotkeyCfg(modifiers, key);
	}

	/// 解析修飾鍵字符串，支持多值（如 Ctrl|Shift）。
	/// <param name="Raw">原始配置值。</param>
	/// <param name="Fallback">解析失敗時的回退值。</param>
	/// <returns>修飾鍵枚舉。</returns>
	private EHotkeyModifiers ParseModifiers(str Raw, EHotkeyModifiers Fallback){
		if(str.IsNullOrWhiteSpace(Raw)){
			return Fallback;
		}

		var merged = EHotkeyModifiers.None;
		var tokens = Raw.Split(['|', '+', ',', ' '], StringSplitOptions.RemoveEmptyEntries);
		foreach(var token in tokens){
			if(Enum.TryParse<EHotkeyModifiers>(token.Trim(), true, out var item)){
				merged |= item;
				continue;
			}
			_logger?.LogWarning("Unknown hotkey modifier in config: {Modifier}", token);
		}

		return merged == EHotkeyModifiers.None ? Fallback : merged;
	}

	//TODO 違背單一職責 可慮抽取複用
	/// 解析主鍵字符串。
	/// <param name="Raw">原始配置值。</param>
	/// <param name="Fallback">解析失敗時的回退值。</param>
	/// <returns>主鍵枚舉。</returns>
	private EHotkeyKey ParseKey(str Raw, EHotkeyKey Fallback){
		if(str.IsNullOrWhiteSpace(Raw)){
			return Fallback;
		}

		if(Enum.TryParse<EHotkeyKey>(Raw.Trim(), true, out var key)){
			return key;
		}

		_logger?.LogWarning("Unknown hotkey key in config: {Key}", Raw);
		return Fallback;
	}

	private static nil ShowMainWindow(){
		var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
		var mainWindow = desktop?.MainWindow;
		if(mainWindow == null){
			return NIL;
		}

		if(!mainWindow.IsVisible){
			mainWindow.Show();
		}
		if(mainWindow.WindowState == WindowState.Minimized){
			mainWindow.WindowState = WindowState.Normal;
		}
		mainWindow.Activate();
		return NIL;
	}

	/// 查詞熱鍵配置 DTO。
	private sealed record DictionaryLookupHotkeyCfg(EHotkeyModifiers Modifiers, EHotkeyKey Key);
}
