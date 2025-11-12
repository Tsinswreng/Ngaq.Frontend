interface ITypedTemplate{
	type:string
	data:string
}

export type TTemplate = string|ITypedTemplate

type Full = {
	View:{
		Common:{
			Confirm: TTemplate
			,Cancel: TTemplate
		}
		,Home:{
			Learn: TTemplate
			,Library: TTemplate
			,Me: TTemplate
		}
		,Library:{
			SearchWords: TTemplate
			,AddWords: TTemplate
			,BackupEtSync: TTemplate
		}
		,LearnWord:{
			Start: TTemplate
			,Save: TTemplate
			,Reset: TTemplate
			,Clear: TTemplate
			,Settings: TTemplate
			,LearnWordSettings: TTemplate
		}
		,LoginRegister:{
			Login:TTemplate
			Register:TTemplate
			UserName:TTemplate
			Email:TTemplate
			Password:TTemplate
			ConfirmPassword:TTemplate
			__CannotBeEmpty:TTemplate
		}
		,Settings:{
			UIConfig:TTemplate
			About:TTemplate
		}
		,About:{
			AppVersion:TTemplate
			Website:TTemplate
		}
	}
	//----Errors----
	,Error:{
		Common: {
			ArgErr: TTemplate
			UnknownErr: TTemplate
		}
		,User: {
			UserNotExist: TTemplate
			UserAlreadyExist: TTemplate
			PasswordNotMatch: TTemplate
			InvalidToken: TTemplate
			TokenExpired: TTemplate
		}
	}
}

type TI18nKv = Full;
export type {TI18nKv};


/*

{
"downloaded": "已下载{0}个文件。",
"downloaded#zero": "还没下载任何文件。",
"userLike": "{0}觉得有用。",
"userLike#plural": "{0}等{1}人觉得有用。"
}
*/
