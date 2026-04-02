using Ngaq.Core.Shared.StudyPlan.Models;
using Ngaq.Core.Shared.StudyPlan.Models.Po.WeightArg;

namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;
file class DirDoc{
	str Doc =
$$"""
#Sum[
{{nameof(StudyPlan)}}相關編輯頁。

涉及實體:
- {{nameof(JnStudyPlan)}}
- {{nameof(BoStudyPlan)}}
]

#Descr[
頁面編寫規範:

- 先在外層 用TreeDataGrid 做 分頁頁面。 該頁面中有新增條目的按鈕。
- 點擊分頁表格中的某一項的時候即進入到這一項的特定的編輯頁面(ViewXxxEdit)。
- 點擊 新增按鈕 也能進到 編輯頁面 (即新增和修改是用同一個頁面的)。

需要在 ViewXxxEdit對應的Ctx 即 VmXxxEdit中 設一個字段、判斷用戶是通過 點擊新增按鈕 進入的、
還是通過 修改實體進入的。 根據這個字段的不同 可能調用不同的後端接口。比如新增就用Add相關接口、修改就用Upd相關接口。


WeightArg 編輯視圖 要求:
底欄兩個按鈕、右邊是保存、左邊是刪除。

中間 把各個字段顯示出來。
其中{{nameof(PoWeightArg.Text)}} 是文本格式的。Payload。
除了在主界面顯示之外、還要加一個按鈕、點擊該按鈕後進入 PayLoad的 Json編輯頁。



]
""";
}
