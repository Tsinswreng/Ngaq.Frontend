import type { TI18nKv } from "./i18n";
const a: TI18nKv = {
	View: {
		Common: {
			Confirm: "Confirm"
			, Cancel: "Cancel"
		}
		, ViewHome: {
			Learn: "Learn"
			, Library: "Library"
			, Me: "Me"
		}
		, ViewLibrary: {
			SearchWords: "Search Words"
			, AddWords: "Add Words"
			, BackupEtSync: "Backup & Sync"
		}
		, ViewLearnWord: {
			Start: "Start"
			, Save: "Save"
			, Reset: "Reset"
		}
	},
	Error:{
		Common:{
			ArgErr:"ArgErr"
		},
		User:{
			UserNotExist: "UserNotExist",
			UserAlreadyExist: "UserAlreadyExist",
			PasswordNotMatch: "PasswordNotMatch",
			InvalidToken: "InvalidToken",
			TokenExpired: "TokenExpired"
		}
	}
}
export default a
