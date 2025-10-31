interface ITypedTemplate{
	type:string
	data:string
}

export type TTemplate = string|ITypedTemplate

type Full = {
	Common:{
		Confirm: TTemplate
		,Cancel: TTemplate
	}
	,ViewHome:{
		Learn: TTemplate
		,Library: TTemplate
		,Me: TTemplate
	}
	,ViewLibrary:{
		SearchWords: TTemplate
		,AddWords: TTemplate
		,BackupEtSync: TTemplate
	}
	,ViewLearnWord:{
		Start: TTemplate
		,Save: TTemplate
		,Reset: TTemplate
	}

	//----Errors----
	,Errors:{
		Common: {
			ArgErr: TTemplate
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
