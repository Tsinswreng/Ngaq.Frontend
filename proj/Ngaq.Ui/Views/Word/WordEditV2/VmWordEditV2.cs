namespace Ngaq.Ui.Views.Word.WordEditV2;

using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Model.Po.Kv;
using Ngaq.Core.Model.Po.Learn_;
using Ngaq.Core.Shared.Word.Models;
using Ngaq.Core.Shared.Word.Models.Po.Kv;
using Ngaq.Core.Shared.Word.Models.Po.Learn;
using Ngaq.Core.Shared.Word.Models.Po.Word;
using Ngaq.Core.Shared.Word.Svc;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Views.Word.WordEditV2.PoWordEdit;
using Ngaq.Ui.Views.Word.WordEditV2.WordLearnPage;
using Ngaq.Ui.Views.Word.WordEditV2.WordPropPage;
using Ctx = VmWordEditV2;

/// 協調單詞基本資料、屬性與學習記錄的編輯和保存。
public partial class VmWordEditV2: ViewModelBase, IMk<Ctx>{
	protected partial VmWordEditV2();
	/// 建立  所需的 UI 組件，供頁面組裝時重用。
	public static partial Ctx Mk();
	ISvcWordV2? SvcWordV2;
	IFrontendUserCtxMgr? UserCtxMgr;
	public partial VmWordEditV2(ISvcWordV2? SvcWordV2, IFrontendUserCtxMgr? UserCtxMgr);
	public enum ESaveMode{DetailOps = 0, Merge = 1,}
	public IJnWord? Src{get;set;}
	public JnWord? Draft{get;set;}
	public i32 TabIndex{get;set;}
	public VmPoWordEdit PoWordEdit{get;set;} = new();
	public VmWordPropPage WordPropPage{get;set;} = new();
	public VmWordLearnPage WordLearnPage{get;set;} = new();
	public str LastError{get;set;} = "";
	public bool HasError => !str.IsNullOrWhiteSpace(LastError);
	public bool IsDirty{get;set;}
	public ESaveMode SaveMode{get;set;} = ESaveMode.DetailOps;
	/// 執行 FromJnWord 所代表的內部協調操作。
	public partial nil FromJnWord(IJnWord JnWord);
	/// 執行 InitFreeAddDraft 所代表的內部協調操作。
	public partial nil InitFreeAddDraft(str Lang = "");
	/// 將持久化資料載入編輯用的 ViewModel 狀態。
	partial void LoadFromDraft();
	/// 執行 Save 所代表的內部協調操作。
	public partial Task<nil> Save(CT Ct);
	/// 執行 SaveByMerge 所代表的內部協調操作。
	private partial Task<nil> SaveByMerge(CT Ct);
	/// 執行 SaveByDetailOps 所代表的內部協調操作。
	private partial Task<nil> SaveByDetailOps(CT Ct);
	/// 執行 UpdRootAndGetFinalId 所代表的內部協調操作。
	private partial Task<IdWord> UpdRootAndGetFinalId(IDbUserCtx DbCtx, CT Ct);
	/// 執行 SavePropsAtomized 所代表的內部協調操作。
	private partial Task<nil> SavePropsAtomized(IDbUserCtx DbCtx, IdWord WordId, CT Ct);
	/// 執行 SaveLearnsAtomized 所代表的內部協調操作。
	private partial Task<nil> SaveLearnsAtomized(IDbUserCtx DbCtx, IdWord WordId, CT Ct);
	/// 執行 DeletePropRow 所代表的內部協調操作。
	public partial Task<bool> DeletePropRow(VmWordPropRow Row, CT Ct);
	/// 執行 DeleteLearnRow 所代表的內部協調操作。
	public partial Task<bool> DeleteLearnRow(VmWordLearnRow Row, CT Ct);
	/// 執行 SyncDeletedPropToLocalState 所代表的內部協調操作。
	partial void SyncDeletedPropToLocalState(IdWordProp Id);
	/// 執行 SyncDeletedLearnToLocalState 所代表的內部協調操作。
	partial void SyncDeletedLearnToLocalState(IdWordLearn Id);
	/// 執行 Delete 所代表的內部協調操作。
	public partial Task<nil> Delete(CT Ct);
	/// 驗證目前狀態並嘗試完成轉換；失敗時透過回傳值提供原因。
	private partial bool TryApplyFormToDraft(out str Err);
}
