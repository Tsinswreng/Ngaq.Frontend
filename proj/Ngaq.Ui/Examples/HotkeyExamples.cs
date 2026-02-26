namespace Ngaq.Ui.Examples;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ngaq.Core.Frontend.Hotkey;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 全局快捷键使用示例
/// 展示如何在 Ngaq 应用中使用全局快捷键系统
/// </summary>
public class HotkeyExamples{

	/// <summary>
	/// 示例1: 最简单的快捷键注册
	/// 注册 Ctrl+S 快捷键用于保存
	/// </summary>
	public static async Task Example1_SimpleSave(IServiceProvider Sp, CT Ct){
		var HotkeyListener = Sp.GetRequiredService<IHotkeyListener>();
		
		await HotkeyListener.Register(
			HotkeyId: "save",
			Modifiers: EHotkeyModifiers.Ctrl,
			Key: EHotkeyKey.S,
			OnHotkey: async (Ct) => {
				System.Console.WriteLine("💾 保存操作被触发了!");
				// 执行保存逻辑
				await Task.CompletedTask;
			},
			Ct: Ct
		);
	}

	/// <summary>
	/// 示例2: 多个修饰符的快捷键
	/// 注册 Ctrl+Shift+S 快捷键用于另存为
	/// </summary>
	public static async Task Example2_SaveAs(IServiceProvider Sp, CT Ct){
		var HotkeyListener = Sp.GetRequiredService<IHotkeyListener>();
		
		await HotkeyListener.Register(
			HotkeyId: "save_as",
			Modifiers: EHotkeyModifiers.Ctrl | EHotkeyModifiers.Shift,
			Key: EHotkeyKey.S,
			OnHotkey: async (Ct) => {
				System.Console.WriteLine("📄 另存为操作被触发了!");
				// 执行另存为逻辑
				await Task.CompletedTask;
			},
			Ct: Ct
		);
	}

	/// <summary>
	/// 示例3: 使用功能键
	/// 注册 F5 快捷键用于刷新
	/// </summary>
	public static async Task Example3_Refresh(IServiceProvider Sp, CT Ct){
		var HotkeyListener = Sp.GetRequiredService<IHotkeyListener>();
		
		await HotkeyListener.Register(
			HotkeyId: "refresh",
			Modifiers: EHotkeyModifiers.None,
			Key: EHotkeyKey.F5,
			OnHotkey: async (Ct) => {
				System.Console.WriteLine("🔄 刷新操作被触发了!");
				// 执行刷新逻辑
				await Task.CompletedTask;
			},
			Ct: Ct
		);
	}

	/// <summary>
	/// 示例4: 使用 Alt 键
	/// 注册 Alt+F4 快捷键用于退出
	/// </summary>
	public static async Task Example4_Exit(IServiceProvider Sp, CT Ct){
		var HotkeyListener = Sp.GetRequiredService<IHotkeyListener>();
		
		await HotkeyListener.Register(
			HotkeyId: "exit",
			Modifiers: EHotkeyModifiers.Alt,
			Key: EHotkeyKey.F4,
			OnHotkey: async (Ct) => {
				System.Console.WriteLine("👋 退出操作被触发了!");
				// 执行退出逻辑（如果需要的话）
				await Task.CompletedTask;
			},
			Ct: Ct
		);
	}

	/// <summary>
	/// 示例5: 注册多个快捷键
	/// 一次性注册多个常用快捷键
	/// </summary>
	public static async Task Example5_RegisterMultiple(IServiceProvider Sp, CT Ct){
		var HotkeyListener = Sp.GetRequiredService<IHotkeyListener>();
		
		var Hotkeys = new[]{
			new {
				Id = "new",
				Modifiers = EHotkeyModifiers.Ctrl,
				Key = EHotkeyKey.N,
				Name = "新建",
				Action = (Func<Task>)(async () => {
					System.Console.WriteLine("📝 新建操作被触发了!");
					await Task.CompletedTask;
				})
			},
			new {
				Id = "open",
				Modifiers = EHotkeyModifiers.Ctrl,
				Key = EHotkeyKey.O,
				Name = "打开",
				Action = (Func<Task>)(async () => {
					System.Console.WriteLine("📂 打开操作被触发了!");
					await Task.CompletedTask;
				})
			},
			new {
				Id = "copy",
				Modifiers = EHotkeyModifiers.Ctrl,
				Key = EHotkeyKey.C,
				Name = "复制",
				Action = (Func<Task>)(async () => {
					System.Console.WriteLine("📋 复制操作被触发了!");
					await Task.CompletedTask;
				})
			}
		};

		foreach(var Hotkey in Hotkeys){
			var Success = await HotkeyListener.Register(
				HotkeyId: Hotkey.Id,
				Modifiers: Hotkey.Modifiers,
				Key: Hotkey.Key,
				OnHotkey: async (Ct) => await Hotkey.Action(),
				Ct: Ct
			);
			System.Console.WriteLine($"快捷键 '{Hotkey.Name}' 注册 {(Success ? "成功" : "失败")}");
		}
	}

	/// <summary>
	/// 示例6: 动态注册和注销
	/// 演示如何在运行时添加和移除快捷键
	/// </summary>
	public static async Task Example6_DynamicRegister(IServiceProvider Sp, CT Ct){
		var HotkeyListener = Sp.GetRequiredService<IHotkeyListener>();
		
		// 注册快捷键
		var RegisterSuccess = await HotkeyListener.Register(
			HotkeyId: "temp_hotkey",
			Modifiers: EHotkeyModifiers.Ctrl,
			Key: EHotkeyKey.P,
			OnHotkey: async (Ct) => {
				System.Console.WriteLine("⏸️ 临时快捷键被触发了!");
				await Task.CompletedTask;
			},
			Ct: Ct
		);
		
		System.Console.WriteLine($"临时快捷键注册: {(RegisterSuccess ? "成功" : "失败")}");
		
		// 过一段时间后注销
		await Task.Delay(5000, Ct);
		
		var UnregisterSuccess = await HotkeyListener.Unregister("temp_hotkey", Ct);
		System.Console.WriteLine($"临时快捷键注销: {(UnregisterSuccess ? "成功" : "失败")}");
	}

	/// <summary>
	/// 示例7: 数字键快捷键
	/// 注册 Ctrl+1 到 Ctrl+9 快捷键用于快速切换
	/// </summary>
	public static async Task Example7_NumberKeys(IServiceProvider Sp, CT Ct){
		var HotkeyListener = Sp.GetRequiredService<IHotkeyListener>();
		
		var NumKeys = new[] { EHotkeyKey.D1, EHotkeyKey.D2, EHotkeyKey.D3, EHotkeyKey.D4, EHotkeyKey.D5 };
		
		for(int I = 0; I < NumKeys.Length; I++){
			int Index = I + 1; // 1-based index
			await HotkeyListener.Register(
				HotkeyId: $"number_{Index}",
				Modifiers: EHotkeyModifiers.Ctrl,
				Key: NumKeys[I],
				OnHotkey: async (Ct) => {
					System.Console.WriteLine($"🔢 快捷键 Ctrl+{Index} 被触发了!");
					await Task.CompletedTask;
				},
				Ct: Ct
			);
		}
	}

	/// <summary>
	/// 示例8: 使用 Windows 键
	/// 注册 Win+Q 快捷键（仅 Windows 平台）
	/// </summary>
	public static async Task Example8_WindowsKey(IServiceProvider Sp, CT Ct){
		var HotkeyListener = Sp.GetRequiredService<IHotkeyListener>();
		
		await HotkeyListener.Register(
			HotkeyId: "win_hotkey",
			Modifiers: EHotkeyModifiers.Win,
			Key: EHotkeyKey.Q,
			OnHotkey: async (Ct) => {
				System.Console.WriteLine("🪟 Win+Q 快捷键被触发了!");
				await Task.CompletedTask;
			},
			Ct: Ct
		);
	}

	/// <summary>
	/// 示例9: 快捷键回调中的异步操作
	/// 演示在快捷键回调中执行异步操作
	/// </summary>
	public static async Task Example9_AsyncCallback(IServiceProvider Sp, CT Ct){
		var HotkeyListener = Sp.GetRequiredService<IHotkeyListener>();
		
		await HotkeyListener.Register(
			HotkeyId: "async_action",
			Modifiers: EHotkeyModifiers.Ctrl | EHotkeyModifiers.Shift,
			Key: EHotkeyKey.A,
			OnHotkey: async (Ct) => {
				System.Console.WriteLine("⏳ 异步操作开始...");
				
				// 模拟异步操作（如网络请求）
				await Task.Delay(2000, Ct);
				
				System.Console.WriteLine("✅ 异步操作完成!");
			},
			Ct: Ct
		);
	}

	/// <summary>
	/// 示例10: 清理所有快捷键
	/// 应用关闭时调用此方法
	/// </summary>
	public static async Task Example10_Cleanup(IServiceProvider Sp, CT Ct){
		var HotkeyListener = Sp.GetRequiredService<IHotkeyListener>();
		
		System.Console.WriteLine("清理所有快捷键...");
		await HotkeyListener.Cleanup(Ct);
		System.Console.WriteLine("✅ 所有快捷键已清理");
	}
}
