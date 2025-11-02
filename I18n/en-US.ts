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
	},
	Error:{
		Common:{
			ArgErr: "Argument Error",
			UnknownErr: "Unknown Error"
		},
		User:{
			UserNotExist: "UserNotExist",
			UserAlreadyExist: "UserAlreadyExist",
			PasswordNotMatch: "PasswordNotMatch",
			InvalidToken: "InvalidToken",
			TokenExpired: "TokenExpired"
		}
	}
	,LoginRegister:{
		Login:"Login"
		,Register:"Register"
		,UserName:"UserName"
		,Email:"Email"
		,Password:"Password"
		,ConfirmPassword:"ConfirmPassword"
	}
}
export default a
