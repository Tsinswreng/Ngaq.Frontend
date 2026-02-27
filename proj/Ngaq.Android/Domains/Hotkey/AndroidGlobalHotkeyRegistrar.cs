namespace Ngaq.Android.Domains.Hotkey;

using Ngaq.Core.Frontend.Hotkey;

/// <summary>
/// Android 平台的全局快捷键注册器（当前无实际行为）
/// </summary>
public class AndroidGlobalHotkeyRegistrar : I_RegisterGlobalHotKeys{
	public nil RegisterGlobalHotKeys(){
		// Android 目前不支持全局快捷键。
		return NIL;
	}
}
