namespace Ngaq.Ui.Infra.Hotkey;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ngaq.Core.Frontend.Hotkey;
using Microsoft.Extensions.Logging;

/// <summary>
/// 快捷键管理器的默认实现
/// </summary>
public class SvcHotkeyMgr : IHotkeyMgr{
	private IHotkeyListener HotkeyListener;
	private ILogger Logger;

	public SvcHotkeyMgr(IHotkeyListener HotkeyListener, ILogger Logger){
		this.HotkeyListener = HotkeyListener;
		this.Logger = Logger;
	}

	public async Task Init(CT Ct){
		Logger?.LogInformation("Hotkey manager initialized");
		await Task.CompletedTask;
	}

	public async Task<bool> Register(str HotkeyId, EHotkeyModifiers Modifiers, EHotkeyKey Key, Func<CT, Task> OnHotkey, CT Ct){
		var Result = await HotkeyListener.Register(HotkeyId, Modifiers, Key, OnHotkey, Ct);
		if(Result){
			Logger?.LogInformation("Hotkey registered: {HotkeyId} - {Modifiers}+{Key}", HotkeyId, Modifiers, Key);
		}else{
			Logger?.LogWarning("Failed to register hotkey: {HotkeyId}", HotkeyId);
		}
		return Result;
	}

	public async Task<bool> Unregister(str HotkeyId, CT Ct){
		var Result = await HotkeyListener.Unregister(HotkeyId, Ct);
		if(Result){
			Logger?.LogInformation("Hotkey unregistered: {HotkeyId}", HotkeyId);
		}else{
			Logger?.LogWarning("Failed to unregister hotkey: {HotkeyId}", HotkeyId);
		}
		return Result;
	}

	public async Task Shutdown(CT Ct){
		await HotkeyListener.Cleanup(Ct);
		Logger?.LogInformation("Hotkey manager shutdown completed");
	}
}
