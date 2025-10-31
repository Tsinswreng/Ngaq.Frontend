import type { TI18nKv } from "./i18n";
const a: TI18nKv = {
	View: {
		ViewLearnWord: {
			Start: "開始",
			Save: "保存",
			Reset: "重設"
		},
		Common: {
			Confirm: "確認",
			Cancel: "取消"
		},
		ViewHome: {
			Learn: "學",
			Library: "庫",
			Me: "我"
		},
		ViewLibrary: {
			SearchWords: "搜詞",
			AddWords: "加詞",
			BackupEtSync: "備份與同步"
		},
	},
	Error: {
		Common: {
			ArgErr: "參數錯誤"
		},
		User: {
			UserNotExist: "用戶不存在"
			, UserAlreadyExist: "用戶已存在"
			, PasswordNotMatch: "密碼不符"
			, InvalidToken: "令牌無效"
			, TokenExpired: "令牌已過期"
		}
	}
}
export default a

