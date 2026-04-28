using Ngaq.Core.Shared.Audio;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Ngaq.Windows.Domains.Audio;

public class NAudioPlayer : IAudioPlayer {
	public async Task<IPlayState?> Play(Stream s, EAudioType type, CT Ct) {
		try{
			// 在后台线程播放，避免阻塞调用方
			await Task.Run(() => {
				using WaveStream reader = type switch {
					EAudioType.Wav => new WaveFileReader(s),
					EAudioType.Mp3 => new Mp3FileReader(s),
					_ => throw ToolAudioErr.MkAudioPlayFailedErr(
						null,
						$"UnsupportedAudioType={type}",
						"Platform=Windows"
					)
				};

				using var wo = new WaveOutEvent();
				wo.Init(reader);
				wo.Play();
				while (wo.PlaybackState == PlaybackState.Playing){
					Ct.ThrowIfCancellationRequested();
					Thread.Sleep(100);
				}
			}, Ct);
			return null;
		}catch(OperationCanceledException) when(Ct.IsCancellationRequested){
			throw;
		}catch(Exception ex) when(ex is not Tsinswreng.CsErr.AppErr){
			throw ToolAudioErr.MkAudioPlayFailedErr(ex, $"Type={type}", "Platform=Windows");
		}
	}
}
