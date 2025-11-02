import type { TI18nKv } from "./i18n";
const a: TI18nKv = {
	View: {
		Common: {
			Confirm: "Confirm"
			, Cancel: "Cancel"
		}
		, Home: {
			Learn: "Learn"
			, Library: "Library"
			, Me: "Me"
		}
		, Library: {
			SearchWords: "Search Words"
			, AddWords: "Add Words"
			, BackupEtSync: "Backup & Sync"
		}
		, LearnWord: {
			Start: "Start"
			, Save: "Save"
			, Reset: "Reset"
		}
		, LoginRegister: {
			Login: "Login",
			Register: "Register",
			UserName: "UserName",
			Email: "Email",
			Password: "Password",
			ConfirmPassword: "Confirm Password",
			__CannotBeEmpty: "{0} cannot be empty"
		}
	},
	Error: {
		Common: {
			ArgErr: "Argument Error",
			UnknownErr: "Unknown Error"
		},
		User: {
			UserNotExist: "UserNotExist",
			UserAlreadyExist: "UserAlreadyExist",
			PasswordNotMatch: "PasswordNotMatch",
			InvalidToken: "InvalidToken",
			TokenExpired: "TokenExpired"
		}
	}
}
export default a
