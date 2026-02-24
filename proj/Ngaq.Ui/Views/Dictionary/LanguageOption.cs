namespace Ngaq.Ui.Views.Dictionary;

using Ngaq.Core.Shared.Dictionary.Models;

/// <summary>
/// 語言選項，用於 ComboBox 顯示
/// </summary>
public class LanguageOption {
	/// <summary>
	/// 顯示名稱（如 "English", "中文"）
	/// </summary>
	public string DisplayName { get; set; } = "";

	/// <summary>
	/// 語言信息
	/// </summary>
	public LangInfo LangInfo { get; set; } = new();

	public LanguageOption() { }

	public LanguageOption(string displayName, string iso639_1, string? variety = null, string? script = null) {
		DisplayName = displayName;
		LangInfo = new LangInfo {
			Iso639_1 = iso639_1,
			Variety = variety,
			Script = script
		};
	}

	public override string ToString() => DisplayName;
}

/// <summary>
/// 常用語言選項列表
/// </summary>
public static class LanguageOptions {
	/// <summary>
	/// 源語言列表
	/// </summary>
	public static List<LanguageOption> SourceLanguages { get; } = [
		new LanguageOption("English", "en"),
		new LanguageOption("中文", "zh"),
		new LanguageOption("日本語", "ja"),
		new LanguageOption("Español", "es"),
		new LanguageOption("Français", "fr"),
		new LanguageOption("Deutsch", "de"),
		new LanguageOption("한국어", "ko"),
		new LanguageOption("Português", "pt"),
		new LanguageOption("Русский", "ru"),
	];

	/// <summary>
	/// 根據源語言獲取可選的目標語言列表
	/// </summary>
	public static List<LanguageOption> GetTargetLanguages(LanguageOption? sourceLang) {
		if (sourceLang == null) {
			return DefaultTargetLanguages;
		}

		// 根據源語言返回合適的目標語言列表
		var srcIso = sourceLang.LangInfo.Iso639_1;

		return srcIso switch {
			"en" => [
				new LanguageOption("中文（繁體）", "zh", "tw", "hant"),
				new LanguageOption("中文（简体）", "zh", "cn", "hans"),
				new LanguageOption("日本語", "ja"),
				new LanguageOption("Español", "es"),
				new LanguageOption("Français", "fr"),
				new LanguageOption("Deutsch", "de"),
			],
			"zh" => [
				new LanguageOption("English", "en"),
				new LanguageOption("日本語", "ja"),
			],
			"ja" => [
				new LanguageOption("English", "en"),
				new LanguageOption("中文（繁體）", "zh", "tw", "hant"),
				new LanguageOption("中文（简体）", "zh", "cn", "hans"),
			],
			_ => DefaultTargetLanguages,
		};
	}

	/// <summary>
	/// 默認目標語言列表
	/// </summary>
	public static List<LanguageOption> DefaultTargetLanguages { get; } = [
		new LanguageOption("中文（繁體）", "zh", "tw", "hant"),
		new LanguageOption("中文（简体）", "zh", "cn", "hans"),
		new LanguageOption("English", "en"),
		new LanguageOption("日本語", "ja"),
	];

	/// <summary>
	/// 默認源語言（English）
	/// </summary>
	public static LanguageOption DefaultSourceLanguage => SourceLanguages[0]; // English

	/// <summary>
	/// 默認目標語言（中文繁體）
	/// </summary>
	public static LanguageOption DefaultTargetLanguage => DefaultTargetLanguages[0]; // 中文繁體
}
