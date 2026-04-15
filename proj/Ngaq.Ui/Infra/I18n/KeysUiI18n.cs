using Ngaq.Core.Infra.Errors;
using Tsinswreng.CsCore;
using Tsinswreng.CsI18n;

namespace Ngaq.Ui.Infra.I18n;
using static Tsinswreng.CsI18n.I18nKey;
using K = II18nKey;
[Doc(@$"僅定義Ngaq.Ui 界面上的文字。
如需定義異常鍵 移步 {nameof(KeysErr)}。
")]
public static partial class KeysUiI18n{
	public static K? View = Mk(null, [nameof(View)]);
	//此層 不應再有其他屬性
public class Common{
	public static readonly K _R = Mk(View, [nameof(Common)]);
	public static readonly K Confirm = Mk(_R, [nameof(Confirm)]);
	public static readonly K Cancel = Mk(_R, [nameof(Cancel)]);
	public static readonly K PageSize = Mk(_R, [nameof(PageSize)]);
	public static readonly K ConfirmChange = Mk(_R, [nameof(ConfirmChange)]);
	public static readonly K UnboundConfirmCallback = Mk(_R, [nameof(UnboundConfirmCallback)]);
}

public class Home{
	public static readonly K _R = Mk(View, [nameof(Home)]);
	public static readonly K Learn = Mk(_R, [nameof(Learn)]);
	public static readonly K Library = Mk(_R, [nameof(Library)]);
	public static readonly K Me = Mk(_R, [nameof(Me)]);
}


public class LearnWord{
	public static readonly K _R = Mk(View, [nameof(LearnWord)]);
	public static readonly K Start = Mk(_R, [nameof(Start)]);
	public static readonly K Save = Mk(_R, [nameof(Save)]);
	public static readonly K Reset = Mk(_R, [nameof(Reset)]);
	public static readonly K Clear = Mk(_R, [nameof(Clear)]);
	public static readonly K Settings = Mk(_R, [nameof(Settings)]);
	public static readonly K LearnWordSettings = Mk(_R, [nameof(LearnWordSettings)]);
}

public class Library{
	public static readonly K _R = Mk(View, [nameof(Library)]);
	public static readonly K SearchWords = Mk(_R, [nameof(SearchWords)]);
	public static readonly K SearchMyWords = Mk(_R, [nameof(SearchMyWords)]);
	public static readonly K AddWords = Mk(_R, [nameof(AddWords)]);
	public static readonly K BackupEtSync = Mk(_R, [nameof(BackupEtSync)]);
}
public class LoginRegister{
	public static readonly K _R = Mk(View, [nameof(LoginRegister)]);
	public static readonly K Login = Mk(_R, [nameof(Login)]);
	public static readonly K Register = Mk(_R, [nameof(Register)]);
	public static readonly K UserName = Mk(_R, [nameof(UserName)]);
	public static readonly K Email = Mk(_R, [nameof(Email)]);
	public static readonly K Password = Mk(_R, [nameof(Password)]);
	public static readonly K ConfirmPassword = Mk(_R, [nameof(ConfirmPassword)]);
	public static readonly K __CannotBeEmpty = Mk(_R, [nameof(__CannotBeEmpty)]);
	public static readonly K PasswordMismatch = Mk(_R, [nameof(PasswordMismatch)]);
	public static readonly K FillAllFields = Mk(_R, [nameof(FillAllFields)]);
	public static readonly K PasswordChangeSuccess = Mk(_R, [nameof(PasswordChangeSuccess)]);
}

public class SyncWord{
	public static readonly K _R = Mk(View, [nameof(SyncWord)]);
	public static readonly K Push = Mk(_R, [nameof(Push)]);
	public static readonly K Pull = Mk(_R, [nameof(Pull)]);
	public static readonly K Export = Mk(_R, [nameof(Export)]);
	public static readonly K ExportPath = Mk(_R, [nameof(ExportPath)]);
	public static readonly K Import = Mk(_R, [nameof(Import)]);
	public static readonly K ImportPath = Mk(_R, [nameof(ImportPath)]);
}

public class Settings{
	public static readonly K _R = Mk(View, [nameof(Settings)]);
	public static readonly K UIConfig = Mk(_R, [nameof(UIConfig)]);
	public static readonly K About = Mk(_R, [nameof(About)]);
	public static readonly K BaseFontSize = Mk(_R, [nameof(BaseFontSize)]);
	public static readonly K Try = Mk(_R, [nameof(Try)]);
	public static readonly K Apply = Mk(_R, [nameof(Apply)]);
	public static readonly K FontSize = Mk(_R, [nameof(FontSize)]);
		public static readonly K FontChangeRelaunchNotice = Mk(_R, [nameof(FontChangeRelaunchNotice)]);
		public static readonly K FontSizeRangeError = Mk(_R, [nameof(FontSizeRangeError)]);
		public static readonly K SettingsTitle = Mk(_R, [nameof(SettingsTitle)]);
		}

public class About{
	public static readonly K _R = Mk(View, [nameof(About)]);
	public static readonly K AppVersion = Mk(_R, [nameof(AppVersion)]);
	public static readonly K Website = Mk(_R, [nameof(Website)]);
	public static readonly K UserProfile = Mk(_R, [nameof(UserProfile)]);
}

public class Dictionary{
	public static readonly K _R = Mk(View, [nameof(Dictionary)]);
	public static readonly K SelectNormLang = Mk(_R, [nameof(SelectNormLang)]);
	public static readonly K ConfigureLangMapping = Mk(_R, [nameof(ConfigureLangMapping)]);
	public static readonly K ViewLlmRawOutput = Mk(_R, [nameof(ViewLlmRawOutput)]);
}

}
