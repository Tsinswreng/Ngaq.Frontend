namespace Ngaq.Android.Domains.Hotkey;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ngaq.Core.Frontend.Hotkey;
using Microsoft.Extensions.Logging;

/// <summary>
/// Android 平台的全局快捷键监听器实现
/// </summary>
public class AndroidHotkeyListener : IHotkeyListener{
	private ILogger Logger;
	private Dictionary<str, HotkeyRegistration> RegisteredHotkeys = [];

	private class HotkeyRegistration{
		public str HotkeyId { get; set; } = "";
		public Func<CT, Task> OnHotkey { get; set; } = null!;
	}

	public AndroidHotkeyListener(ILogger Logger){
		this.Logger = Logger;
	}

	public Task<bool> Register(str HotkeyId, EHotkeyModifiers Modifiers, EHotkeyKey Key, Func<CT, Task> OnHotkey, CT Ct){
		try{
			if(RegisteredHotkeys.ContainsKey(HotkeyId)){
				Logger?.LogWarning("Hotkey {HotkeyId} already registered", HotkeyId);
				return Task.FromResult(false);
			}

			var Registration = new HotkeyRegistration{
				HotkeyId = HotkeyId,
				OnHotkey = OnHotkey
			};
			RegisteredHotkeys[HotkeyId] = Registration;
			Logger?.LogInformation("Hotkey {HotkeyId} registered on Android", HotkeyId);

			// TODO: 实现 Android 平台特定的快捷键监听逻辑
			// Android 可能需要通过 KeyEvent 或其他平台特定 API 实现

			return Task.FromResult(true);
		}catch(System.Exception Ex){
			Logger?.LogError(Ex, "Exception during hotkey registration {HotkeyId}", HotkeyId);
			return Task.FromResult(false);
		}
	}

	public Task<bool> Unregister(str HotkeyId, CT Ct){
		try{
			if(!RegisteredHotkeys.TryGetValue(HotkeyId, out _)){
				Logger?.LogWarning("Hotkey {HotkeyId} not found", HotkeyId);
				return Task.FromResult(false);
			}

			RegisteredHotkeys.Remove(HotkeyId);
			Logger?.LogInformation("Hotkey {HotkeyId} unregistered", HotkeyId);
			return Task.FromResult(true);
		}catch(System.Exception Ex){
			Logger?.LogError(Ex, "Exception during hotkey unregistration {HotkeyId}", HotkeyId);
			return Task.FromResult(false);
		}
	}

	public async Task Cleanup(CT Ct){
		var HotkeyIds = new List<str>(RegisteredHotkeys.Keys);
		foreach(var HotkeyId in HotkeyIds){
			await Unregister(HotkeyId, Ct);
		}
		Logger?.LogInformation("All hotkeys cleaned up");
	}
}
