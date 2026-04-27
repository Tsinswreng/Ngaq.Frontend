namespace Ngaq.Linux.Domains.Audio;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ngaq.Core.Shared.Audio;

/// Linux 平臺音頻播放器。
/// 使用外部命令行播放器進行回放，避免依賴 Windows 專用音頻庫。
public class LinuxAudioPlayer : IAudioPlayer{
	private sealed record PlayerCmd(str FileName, str ArgsTemplate);

	// Ubuntu 22 常見播放器命令回退鏈。
	private static readonly IReadOnlyList<PlayerCmd> Mp3Players = [
		new("mpg123", "-q \"{0}\""),
		new("ffplay", "-nodisp -autoexit -loglevel quiet \"{0}\""),
		new("cvlc", "--play-and-exit --quiet \"{0}\"")
	];

	private static readonly IReadOnlyList<PlayerCmd> WavPlayers = [
		new("aplay", "\"{0}\""),
		new("paplay", "\"{0}\""),
		new("ffplay", "-nodisp -autoexit -loglevel quiet \"{0}\"")
	];

	/// 播放音頻流。
	/// <param name="S">音頻數據流。</param>
	/// <param name="Type">音頻類型（Mp3/Wav）。</param>
	/// <param name="Ct">取消令牌。</param>
	/// <returns>暫無可控播放狀態，返回 null。</returns>
	public async Task<IPlayState?> Play(Stream S, EAudioType Type, CT Ct){
		var extension = GetAudioExtension(Type);
		var tempFilePath = await SaveToTempFile(S, extension, Ct);

		try{
			await Task.Run(() => PlayWithFallbackCommands(tempFilePath, Type, Ct), Ct);
			return null;
		}finally{
			TryDeleteTempFile(tempFilePath);
		}
	}

	/// 根據音頻類型選擇臨時文件副檔名。
	private static str GetAudioExtension(EAudioType Type){
		return Type switch{
			EAudioType.Mp3 => ".mp3",
			EAudioType.Wav => ".wav",
			_ => throw new NotSupportedException($"Unsupported audio type: {Type}")
		};
	}

	/// 將流寫入臨時文件，供外部播放器直接讀取。
	private static async Task<str> SaveToTempFile(Stream S, str Extension, CT Ct){
		var tempFilePath = Path.Combine(Path.GetTempPath(), $"ngaq-audio-{Guid.NewGuid():N}{Extension}");
		await using var fs = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read, 81920, useAsync: true);
		await S.CopyToAsync(fs, Ct);
		await fs.FlushAsync(Ct);
		return tempFilePath;
	}

	/// 依序嘗試可用播放器，直到成功播放或全部失敗。
	private static nil PlayWithFallbackCommands(str TempFilePath, EAudioType Type, CT Ct){
		var players = Type == EAudioType.Mp3 ? Mp3Players : WavPlayers;

		foreach(var player in players){
			var ok = TryPlayByCommand(player, TempFilePath, Ct);
			if(ok){
				return NIL;
			}
		}

		throw new NotSupportedException(
			$"No available Linux audio player was found for {Type}. Tried: {string.Join(", ", players)}"
		);
	}

	/// 使用指定命令播放文件；命令不存在或退出失敗時返回 false。
	private static bool TryPlayByCommand(PlayerCmd Player, str TempFilePath, CT Ct){
		try{
			var psi = new ProcessStartInfo{
				FileName = Player.FileName,
				Arguments = string.Format(Player.ArgsTemplate, TempFilePath),
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
			};

			using var proc = Process.Start(psi);
			if(proc == null){
				return false;
			}

			using var reg = Ct.Register(() => {
				try{
					if(!proc.HasExited){
						proc.Kill(entireProcessTree: true);
					}
				}catch{
					// 取消過程的清理錯誤不影響主流程。
				}
			});

			proc.WaitForExit();
			Ct.ThrowIfCancellationRequested();
			return proc.ExitCode == 0;
		}catch{
			return false;
		}
	}

	/// 清理臨時文件，避免磁盤遺留。
	private static nil TryDeleteTempFile(str TempFilePath){
		try{
			if(File.Exists(TempFilePath)){
				File.Delete(TempFilePath);
			}
		}catch{
			// 臨時文件清理失敗不阻斷業務流程。
		}
		return NIL;
	}
}
