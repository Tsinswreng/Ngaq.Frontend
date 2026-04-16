interface ITypedTemplate{
	type:string
	data:string
}

export type TTemplate = string|ITypedTemplate
//type K = TTemplate
const K = undefined as unknown as TTemplate
const Full = {
	//UI層顯示的文本
	View:{
		Common:{
			Confirm: K,
			Cancel: K,
		},
		Home:{
			Learn: K,
			Library: K,
			Me: K,
		},
		Library:{
			SearchWords: K,
			AddWords: K,
			BackupEtSync: K,
		},
		LearnWord:{
			Start: K,
			Save: K,
			Reset: K,
			Clear: K,
			Settings: K,
			LearnWordSettings: K,
		},
		LoginRegister:{
			Login:K,
			Register:K,
			UserName:K,
			Email:K,
			Password:K,
			ConfirmPassword:K,
			__CannotBeEmpty:K,
		},
		Settings:{
			UIConfig:K,
			About:K,
		},
		About:{
			AppVersion:K,
			Website:K,
		},
	},
	//異常鍵翻譯
	Error:{
		Common: {
			ArgErr: K,
			UnknownErr: K,
		},
		User: {
			UserNotExist: K,
			UserAlreadyExist: K,
			PasswordNotMatch: K,
			InvalidToken: K,
			TokenExpired: K,
		},
	},
	//語言名稱本身的翻譯
	Lang:{
		zh: K,
		zh_CN: K,
		//...
	},
}

type TI18nKv = typeof Full;
export type {TI18nKv};
