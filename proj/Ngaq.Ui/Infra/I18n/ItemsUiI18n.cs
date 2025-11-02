namespace Ngaq.Ui.Infra.I18n;
using static Ngaq.Ui.Infra.I18n.I18nKey;
using K = II18nKey;

public static partial class ItemsUiI18n{
	public static K? View = Mk(null, ["View"]);

public class Common{
	public static readonly K _Root = Mk(View, [nameof(Common)]);
	public static readonly K Confirm = Mk(_Root, [nameof(Confirm)]);
	public static readonly K Cancel = Mk(_Root, [nameof(Cancel)]);
}

public class Home{
	public static readonly K _Root = Mk(View, [nameof(Home)]);
	public static readonly K Learn = Mk(_Root, [nameof(Learn)]);
	public static readonly K Library = Mk(_Root, [nameof(Library)]);
	public static readonly K Me = Mk(_Root, [nameof(Me)]);
}


public class LearnWord{
	public static readonly K _Root = Mk(View, [nameof(LearnWord)]);
	public static readonly K Start = Mk(_Root, [nameof(Start)]);
	public static readonly K Save = Mk(_Root, [nameof(Save)]);
	public static readonly K Reset = Mk(_Root, [nameof(Reset)]);
}

public class Library{
	public static readonly K R = Mk(View, [nameof(Library)]);
	public static readonly K SearchWords = Mk(R, [nameof(SearchWords)]);
	public static readonly K AddWords = Mk(R, [nameof(AddWords)]);
	public static readonly K BackupEtSync = Mk(R, [nameof(BackupEtSync)]);
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

}

public class SyncWord{
	public static readonly K _R = Mk(View, [nameof(SyncWord)]);
	public static readonly K Push = Mk(_R, [nameof(Push)]);
	public static readonly K Pull = Mk(_R, [nameof(Pull)]);
}

}


