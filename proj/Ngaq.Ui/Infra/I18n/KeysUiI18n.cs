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
	public static readonly K RawOutputEmptyCannotParse = Mk(_R, [nameof(RawOutputEmptyCannotParse)]);
	public static readonly K CompleteDictionaryQueryBeforeSave = Mk(_R, [nameof(CompleteDictionaryQueryBeforeSave)]);
	public static readonly K GoToLanguageConfigPage = Mk(_R, [nameof(GoToLanguageConfigPage)]);
	public static readonly K SkipConfigAndGoEditPage = Mk(_R, [nameof(SkipConfigAndGoEditPage)]);
	public static readonly K LanguageMappingNotConfiguredChooseNext = Mk(_R, [nameof(LanguageMappingNotConfiguredChooseNext)]);
	public static readonly K AddNormLangToUserLang = Mk(_R, [nameof(AddNormLangToUserLang)]);
	public static readonly K WordEditorContextIsNull = Mk(_R, [nameof(WordEditorContextIsNull)]);
	public static readonly K WordEdit = Mk(_R, [nameof(WordEdit)]);
}

public class WordManage{
	public static readonly K _R = Mk(View, [nameof(WordManage)]);
	public static readonly K Dictionary = Mk(_R, [nameof(Dictionary)]);
	public static readonly K StudyPlan = Mk(_R, [nameof(StudyPlan)]);
	public static readonly K Statistics = Mk(_R, [nameof(Statistics)]);
	public static readonly K UserLang = Mk(_R, [nameof(UserLang)]);
	public static readonly K NormLang = Mk(_R, [nameof(NormLang)]);
}

public class WordSync{
	public static readonly K _R = Mk(View, [nameof(WordSync)]);
	public static readonly K FileAlreadyExistsNoOverwriteChangePath = Mk(_R, [nameof(FileAlreadyExistsNoOverwriteChangePath)]);
	public static readonly K InvalidPath = Mk(_R, [nameof(InvalidPath)]);
}

public class UserProfile{
	public static readonly K _R = Mk(View, [nameof(UserProfile)]);
	public static readonly K ChangeAccount = Mk(_R, [nameof(ChangeAccount)]);
	public static readonly K Logout = Mk(_R, [nameof(Logout)]);
}

public class WordCard{
	public static readonly K _R = Mk(View, [nameof(WordCard)]);
	public static readonly K Edit = Mk(_R, [nameof(Edit)]);
	public static readonly K NoWordSelected = Mk(_R, [nameof(NoWordSelected)]);
	public static readonly K Pronounce = Mk(_R, [nameof(Pronounce)]);
	public static readonly K CurrentPageNoPronounceAction = Mk(_R, [nameof(CurrentPageNoPronounceAction)]);
	public static readonly K PronounceFailed = Mk(_R, [nameof(PronounceFailed)]);
	public static readonly K GoConfigureUserLang = Mk(_R, [nameof(GoConfigureUserLang)]);
	public static readonly K UserLang = Mk(_R, [nameof(UserLang)]);
	public static readonly K WordLangNotMappedCannotPronounce = Mk(_R, [nameof(WordLangNotMappedCannotPronounce)]);
	public static readonly K WordLangIsEmpty = Mk(_R, [nameof(WordLangIsEmpty)]);
	public static readonly K ServiceUnavailable = Mk(_R, [nameof(ServiceUnavailable)]);
}

public class SearchWords{
	public static readonly K _R = Mk(View, [nameof(SearchWords)]);
	public static readonly K WordNotFound = Mk(_R, [nameof(WordNotFound)]);
}

public class UserLangPage{
	public static readonly K _R = Mk(View, [nameof(UserLangPage)]);
	public static readonly K AddedAllUnregisteredUserLangs = Mk(_R, [nameof(AddedAllUnregisteredUserLangs)]);
	public static readonly K AutoAddMissing = Mk(_R, [nameof(AutoAddMissing)]);
	public static readonly K Empty = Mk(_R, [nameof(Empty)]);
	public static readonly K Name = Mk(_R, [nameof(Name)]);
	public static readonly K RelLangType = Mk(_R, [nameof(RelLangType)]);
	public static readonly K RelLang = Mk(_R, [nameof(RelLang)]);
	public static readonly K Modified = Mk(_R, [nameof(Modified)]);
	public static readonly K NewUserLang = Mk(_R, [nameof(NewUserLang)]);
}

public class UserLangEdit{
	public static readonly K _R = Mk(View, [nameof(UserLangEdit)]);
	public static readonly K PoUserLang = Mk(_R, [nameof(PoUserLang)]);
	public static readonly K Id = Mk(_R, [nameof(Id)]);
	public static readonly K Name = Mk(_R, [nameof(Name)]);
	public static readonly K Description = Mk(_R, [nameof(Description)]);
	public static readonly K RelLangType = Mk(_R, [nameof(RelLangType)]);
	public static readonly K RelLang = Mk(_R, [nameof(RelLang)]);
	public static readonly K Save = Mk(_R, [nameof(Save)]);
	public static readonly K Saved = Mk(_R, [nameof(Saved)]);
}



public class WordEditJsonMap{
	public static readonly K _R = Mk(View, [nameof(WordEditJsonMap)]);
	public static readonly K NoWordOrCtx = Mk(_R, [nameof(NoWordOrCtx)]);
	public static readonly K WordCore = Mk(_R, [nameof(WordCore)]);
	public static readonly K Save = Mk(_R, [nameof(Save)]);
}

}
