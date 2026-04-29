namespace Ngaq.Android;

file class DirDoc{
	str Doc =
"""
#Sum[
Android 平臺入口與平臺級交互。
]

#Descr[
`MainActivity` 負責承接 Android 系統事件，再轉交給 Avalonia UI。

目前已接入兩類平臺級入口：
- 持續通知點擊後的剪貼簿查詞
- 系統返回鍵，優先關閉 `MainView` 彈窗，否則回到上一級導航
]
""";
}
