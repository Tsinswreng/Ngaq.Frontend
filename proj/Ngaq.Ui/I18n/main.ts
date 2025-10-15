// mkdir -p json && esno main.ts ./json
import { I18nForOne, I18nMgr, TI18nKv } from "./i18n"

function R(lang:string, kv:TI18nKv){
	return I18nMgr.inst.register(I18nForOne.mk(lang, kv))
}

import _default from "./default"
R("default", _default)

import zh_tw from "./zh-tw"
R("zh-tw", zh_tw)

let firstArg:string
//@ts-ignore
firstArg = process.argv[2]
I18nMgr.inst.writeJson(firstArg)
