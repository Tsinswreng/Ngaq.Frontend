namespace Ngaq.Windows.Domains.Hotkey;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Ngaq.Core.Frontend.Hotkey;
using Microsoft.Extensions.Logging;

/// <summary>
/// Windows 平台的全局快捷键监听器实现
/// 使用 Win32 API RegisterHotKey 实现全局快捷键
/// </summary>
public class WinHotkeyListener : IHotkeyListener{
	private ILogger Logger;
	private Dictionary<str, HotkeyRegistration> RegisteredHotkeys = [];
	private int NextId = 1;

	private class HotkeyRegistration{
		public int Id { get; set; }
		public str HotkeyId { get; set; } = "";
		public FnOnHotKey OnHotkey { get; set; } = null!;
	}

	public WinHotkeyListener(ILogger Logger){
		this.Logger = Logger;
	}

	#region Win32 API

	[DllImport("user32.dll")]
	private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

	[DllImport("user32.dll")]
	private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

	[DllImport("kernel32.dll")]
	private static extern uint GetCurrentThreadId();

	[DllImport("user32.dll", SetLastError = true)]
	private static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

	private const uint MOD_CTRL = 2;
	private const uint MOD_SHIFT = 4;
	private const uint MOD_ALT = 1;
	private const uint MOD_WIN = 8;

	#endregion

	public bool Register(
		str HotkeyId, EHotkeyModifiers Modifiers, EHotkeyKey Key
		, FnOnHotKey OnHotkey
	){
		try{
			if(RegisteredHotkeys.ContainsKey(HotkeyId)){
				Logger?.LogWarning("Hotkey {HotkeyId} already registered", HotkeyId);
				return (false);
			}

			int Id = NextId++;
			uint ModifiersCode = ConvertModifiers(Modifiers);
			uint KeyCode = ConvertKey(Key);

			// 注册全局快捷键到当前线程窗口
			// 注意：在 Windows 中，RegisterHotKey 需要一个窗口句柄
			// 对于无窗口应用，我们使用隐藏窗口或在 UI 线程中注册
			bool Success = RegisterHotKey(IntPtr.Zero, Id, ModifiersCode, KeyCode);

			if(Success){
				var Registration = new HotkeyRegistration{
					Id = Id,
					HotkeyId = HotkeyId,
					OnHotkey = OnHotkey
				};
				RegisteredHotkeys[HotkeyId] = Registration;
				Logger?.LogInformation("Hotkey {HotkeyId} registered successfully with id {Id}", HotkeyId, Id);
				return (true);
			}else{
				Logger?.LogError("Failed to register hotkey {HotkeyId}", HotkeyId);
				return (false);
			}
		}catch(System.Exception Ex){
			Logger?.LogError(Ex, "Exception during hotkey registration {HotkeyId}", HotkeyId);
			return (false);
		}
	}

	public bool Unregister(str HotkeyId){
		try{
			if(!RegisteredHotkeys.TryGetValue(HotkeyId, out var Registration)){
				Logger?.LogWarning("Hotkey {HotkeyId} not found", HotkeyId);
				return false;
			}

			bool Success = UnregisterHotKey(IntPtr.Zero, Registration.Id);
			if(Success){
				RegisteredHotkeys.Remove(HotkeyId);
				Logger?.LogInformation("Hotkey {HotkeyId} unregistered successfully", HotkeyId);
				return (true);
			}else{
				Logger?.LogError("Failed to unregister hotkey {HotkeyId}", HotkeyId);
				return (false);
			}
		}catch(System.Exception Ex){
			Logger?.LogError(Ex, "Exception during hotkey unregistration {HotkeyId}", HotkeyId);
			return (false);
		}
	}

	public async void Cleanup(){
		var HotkeyIds = new List<str>(RegisteredHotkeys.Keys);
		foreach(var HotkeyId in HotkeyIds){
			Unregister(HotkeyId);
		}
		Logger?.LogInformation("All hotkeys cleaned up");
	}

	/// <summary>
	/// 将修饰符枚举转换为 Win32 API 代码
	/// </summary>
	private uint ConvertModifiers(EHotkeyModifiers Modifiers){
		uint Code = 0;
		if((Modifiers & EHotkeyModifiers.Ctrl) != 0) Code |= MOD_CTRL;
		if((Modifiers & EHotkeyModifiers.Shift) != 0) Code |= MOD_SHIFT;
		if((Modifiers & EHotkeyModifiers.Alt) != 0) Code |= MOD_ALT;
		if((Modifiers & EHotkeyModifiers.Win) != 0) Code |= MOD_WIN;
		return Code;
	}

	/// <summary>
	/// 将快捷键枚举转换为 Win32 虚拟键代码
	/// </summary>
	private uint ConvertKey(EHotkeyKey Key){
		return Key switch{
			// 字母键 A-Z: 0x41-0x5A
			EHotkeyKey.A => 0x41, EHotkeyKey.B => 0x42, EHotkeyKey.C => 0x43, EHotkeyKey.D => 0x44,
			EHotkeyKey.E => 0x45, EHotkeyKey.F => 0x46, EHotkeyKey.G => 0x47, EHotkeyKey.H => 0x48,
			EHotkeyKey.I => 0x49, EHotkeyKey.J => 0x4A, EHotkeyKey.K => 0x4B, EHotkeyKey.L => 0x4C,
			EHotkeyKey.M => 0x4D, EHotkeyKey.N => 0x4E, EHotkeyKey.O => 0x4F, EHotkeyKey.P => 0x50,
			EHotkeyKey.Q => 0x51, EHotkeyKey.R => 0x52, EHotkeyKey.S => 0x53, EHotkeyKey.T => 0x54,
			EHotkeyKey.U => 0x55, EHotkeyKey.V => 0x56, EHotkeyKey.W => 0x57, EHotkeyKey.X => 0x58,
			EHotkeyKey.Y => 0x59, EHotkeyKey.Z => 0x5A,

			// 数字键 0-9: 0x30-0x39
			EHotkeyKey.D0 => 0x30, EHotkeyKey.D1 => 0x31, EHotkeyKey.D2 => 0x32, EHotkeyKey.D3 => 0x33,
			EHotkeyKey.D4 => 0x34, EHotkeyKey.D5 => 0x35, EHotkeyKey.D6 => 0x36, EHotkeyKey.D7 => 0x37,
			EHotkeyKey.D8 => 0x38, EHotkeyKey.D9 => 0x39,

			// 功能键 F1-F12: 0x70-0x7B
			EHotkeyKey.F1 => 0x70, EHotkeyKey.F2 => 0x71, EHotkeyKey.F3 => 0x72, EHotkeyKey.F4 => 0x73,
			EHotkeyKey.F5 => 0x74, EHotkeyKey.F6 => 0x75, EHotkeyKey.F7 => 0x76, EHotkeyKey.F8 => 0x77,
			EHotkeyKey.F9 => 0x78, EHotkeyKey.F10 => 0x79, EHotkeyKey.F11 => 0x7A, EHotkeyKey.F12 => 0x7B,

			// 特殊键
			EHotkeyKey.Enter => 0x0D,
			EHotkeyKey.Escape => 0x1B,
			EHotkeyKey.Backspace => 0x08,
			EHotkeyKey.Tab => 0x09,
			EHotkeyKey.Space => 0x20,
			EHotkeyKey.Delete => 0x2E,
			EHotkeyKey.Home => 0x24,
			EHotkeyKey.End => 0x23,
			EHotkeyKey.PageUp => 0x21,
			EHotkeyKey.PageDown => 0x22,
			EHotkeyKey.Up => 0x26,
			EHotkeyKey.Down => 0x28,
			EHotkeyKey.Left => 0x25,
			EHotkeyKey.Right => 0x27,

			// 其他键
			EHotkeyKey.Print => 0x2C,
			EHotkeyKey.Pause => 0x13,
			EHotkeyKey.Insert => 0x2D,
			EHotkeyKey.NumLock => 0x90,
			EHotkeyKey.CapsLock => 0x14,
			EHotkeyKey.ScrollLock => 0x91,

			_ => 0
		};
	}

	// 注意：此实现在 Avalonia 中需要额外配置来接收 WM_HOTKEY 消息
	// 建议在平台启动时设置消息处理器
	// 具体实现需要根据 Avalonia 的事件系统集成
}
