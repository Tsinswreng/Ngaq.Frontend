using Ngaq.Core.Shared.Audio;
using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Ngaq.Windows.Domains.Audio;
/// <summary>基于 NAudio 的简单实现</summary>
public class NAudioPlayer : IAudioPlayer {
	public async Task<IPlayState?> Play(Stream s, EAudioType type, CT Ct) {
		// 在后台线程播放，避免阻塞调用方
		await Task.Run(() => {
			// 根据类型选 Reader
			IWaveProvider reader = type switch {
				EAudioType.Wav => new WaveFileReader(s),
				EAudioType.Mp3 => new Mp3FileReader(s),
				_ => throw new NotSupportedException()
			};

			using var wo = new WaveOutEvent();
			wo.Init(reader);
			wo.Play();
			// 简单等待播完
			while (wo.PlaybackState == PlaybackState.Playing){
				System.Threading.Thread.Sleep(100);
			}
		}, Ct);

		// 题目要求：直接返回 null
		return null;
	}
}
