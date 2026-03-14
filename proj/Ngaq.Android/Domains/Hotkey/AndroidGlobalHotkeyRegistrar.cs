namespace Ngaq.Android.Domains.Hotkey;

using Ngaq.Core.Frontend.Hotkey;


/// Android 平台的全局快捷键注册器（当前无实际行为）

public class AndroidGlobalHotkeyRegistrar : I_RegisterGlobalHotKeys{
	public nil RegisterGlobalHotKeys(){
		// Android 目前不支持全局快捷键。
		return NIL;
	}
}
