/*
# pwd = Ngaq.Frontend
cd I18n
mkdir -p json
esno main.ts ./json
cd ..
mkdir -p ./proj/Ngaq.Windows/bin/Debug/net10.0/Languages
cp -r ./I18n/json/* ./proj/Ngaq.Windows/bin/Debug/net10.0/Languages/
*/

import { I18nForOne, I18nMgr, TI18nKv } from "./i18n"

function R(lang:string, kv:TI18nKv){
	return I18nMgr.inst.register(I18nForOne.mk(lang, kv))
}

import _default from "./langs/en-US"
R("default", _default)

import zh_TW from "./langs/zh-TW"
R("zh-TW", zh_TW)

let firstArg:string
//@ts-ignore
firstArg = process.argv[2]
I18nMgr.inst.writeJson(firstArg)
