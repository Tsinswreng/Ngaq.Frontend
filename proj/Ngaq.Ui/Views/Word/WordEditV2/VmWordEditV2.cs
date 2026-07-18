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
	public partial nil FromJnWord(IJnWord JnWord);
	public partial nil InitFreeAddDraft(str Lang = "");
	partial void LoadFromDraft();
	public partial Task<nil> Save(CT Ct);
	private partial Task<nil> SaveByMerge(CT Ct);
	private partial Task<nil> SaveByDetailOps(CT Ct);
	private partial Task<IdWord> UpdRootAndGetFinalId(IDbUserCtx DbCtx, CT Ct);
	private partial Task<nil> SavePropsAtomized(IDbUserCtx DbCtx, IdWord WordId, CT Ct);
	private partial Task<nil> SaveLearnsAtomized(IDbUserCtx DbCtx, IdWord WordId, CT Ct);
	public partial Task<bool> DeletePropRow(VmWordPropRow Row, CT Ct);
	public partial Task<bool> DeleteLearnRow(VmWordLearnRow Row, CT Ct);
	partial void SyncDeletedPropToLocalState(IdWordProp Id);
	partial void SyncDeletedLearnToLocalState(IdWordLearn Id);
	public partial Task<nil> Delete(CT Ct);
	private partial bool TryApplyFormToDraft(out str Err);
}
