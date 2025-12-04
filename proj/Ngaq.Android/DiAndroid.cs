namespace Ngaq.Android;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class DiAndroid{
	public static IServiceCollection SetupAndroid(this IServiceCollection z){
/*
adb logcat -c # 清除日誌
# 显示该 Tag 的所有级别日志（V/D/I/W/E/F），并保留时间戳格式
adb logcat Tsinswreng.Ngaq:V *:S -v time
 */
		var AndroidLogger = new AndroidLogger("Tsinswreng.Ngaq");
		z.AddSingleton<ILogger>(AndroidLogger);
		return z;
	}
}
