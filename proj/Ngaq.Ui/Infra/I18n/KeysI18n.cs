namespace Ngaq.Ui.Infra.I18n;
using static Ngaq.Ui.Infra.I18n.I18nKey;
using Tsinswreng.CsCfg;
using Microsoft.Win32.SafeHandles;
using K = II18nKey;

public static class KeysUiI18nRoot{
	public static K? _R = Mk(null, ["View"]);
	public static readonly K Common = Mk(_R, [nameof(Common)]);
	public static readonly K ViewHome = Mk(_R, [nameof(ViewHome)]);
	public static readonly K ViewLearnWord = Mk(_R, [nameof(ViewLearnWord)]);
	public static readonly K ViewLibrary = Mk(_R, [nameof(ViewLibrary)]);
}


public class Common{
	public static readonly K _Root = KeysUiI18nRoot.Common;
	public static readonly K Confirm = Mk(_Root, [nameof(Confirm)]);
	public static readonly K Cancel = Mk(_Root, [nameof(Cancel)]);
}

public class ViewHome{
	public static readonly K _Root = KeysUiI18nRoot.ViewHome;
	public static readonly K Learn = Mk(_Root, [nameof(Learn)]);
	public static readonly K Library = Mk(_Root, [nameof(Library)]);
	public static readonly K Me = Mk(_Root, [nameof(Me)]);
}


public class ViewLearnWord{
	public static readonly K _Root = KeysUiI18nRoot.ViewLearnWord;
	public static readonly K Start = Mk(_Root, [nameof(Start)]);
	public static readonly K Save = Mk(_Root, [nameof(Save)]);
	public static readonly K Reset = Mk(_Root, [nameof(Reset)]);
}


public class ViewLibrary{
	public static readonly K R = KeysUiI18nRoot.ViewLibrary;
	public static readonly K SearchWords = Mk(R, [nameof(SearchWords)]);
	public static readonly K AddWords = Mk(R, [nameof(AddWords)]);
	public static readonly K BackupEtSync = Mk(R, [nameof(BackupEtSync)]);
}
