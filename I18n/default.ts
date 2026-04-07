interface ITypedTemplate{
	type:string
	data:string
}

export type TTemplate = string|ITypedTemplate
type K = TTemplate
type Full = {
	View:{
		Common:{
			Confirm: K
			Cancel: K
		}
		Home:{
			Learn: K
			Library: K
			Me: K
		}
		Library:{
			SearchWords: K
			AddWords: K
			BackupEtSync: K
		}
		LearnWord:{
			Start: K
			Save: K
			Reset: K
			Clear: K
			Settings: K
			LearnWordSettings: K
		}
		LoginRegister:{
			Login:K
			Register:K
			UserName:K
			Email:K
			Password:K
			ConfirmPassword:K
			__CannotBeEmpty:K
		}
		Settings:{
			UIConfig:K
			About:K
		}
		About:{
			AppVersion:K
			Website:K
		}
	}
	//----Errors----
	Error:{
		Common: {
			ArgErr: K
			UnknownErr: K
		}
		User: {
			UserNotExist: K
			UserAlreadyExist: K
			PasswordNotMatch: K
			InvalidToken: K
			TokenExpired: K
		}
	}
	Lang:{
		zh: K
		zh_CN: K
		//...
	}
}

type TI18nKv = Full;
export type {TI18nKv};
