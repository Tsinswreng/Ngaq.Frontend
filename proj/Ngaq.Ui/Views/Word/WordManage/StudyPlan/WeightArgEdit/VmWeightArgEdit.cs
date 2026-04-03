namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.WeightArgEdit;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ngaq.Core.Frontend.User;
using Ngaq.Core.Infra;
using Ngaq.Core.Infra.IF;
using Ngaq.Core.Shared.StudyPlan.Models.Po.WeightArg;
using Ngaq.Core.Shared.StudyPlan.Models.Po.WeightCalculator;
using Ngaq.Core.Shared.StudyPlan.Svc;
using Ngaq.Ui.Infra;
using Ngaq.Ui.Tools;
using Ngaq.Ui.Views.Word.WordManage.StudyPlan.WeightArgPayloadJsonEdit;
using Tsinswreng.CsTools;

using Ctx = VmWeightArgEdit;

/// WeightArg 編輯頁 ViewModel。
/// 負責新增/修改/刪除，並提供 Payload JSON 子頁入口。
public partial class VmWeightArgEdit: ViewModelBase, IMk<Ctx>{
	protected VmWeightArgEdit(){}

	public static Ctx Mk(){
		return new Ctx();
	}

	public static ObservableCollection<Ctx> Samples = [];
	static VmWeightArgEdit(){
		#if DEBUG
		{
			var o = new Ctx();
			Samples.Add(o);
		}
		#endif
	}

	ISvcStudyPlan? SvcStudyPlan{get;set;}
	IFrontendUserCtxMgr? UserCtxMgr{get;set;}

	public VmWeightArgEdit(
		ISvcStudyPlan? SvcStudyPlan
		,IFrontendUserCtxMgr? UserCtxMgr
	){
		this.SvcStudyPlan = SvcStudyPlan;
		this.UserCtxMgr = UserCtxMgr;
	}

	/// 當前頁面是否爲新增模式。true=新增，false=編輯既有實體。
	public bool IsCreateMode{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = true;

	public str LastError{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public bool HasError => !str.IsNullOrWhiteSpace(LastError);

	public IReadOnlyList<str> TypeOptions{get;} = Enum.GetNames<EWeightArgType>();

	public str PoIdText{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str PoUniqName{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public str PoDescr{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	public i32 PoTypeIndex{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = 1;

	public str WeightCalculatorName{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	/// 主頁只顯示 Payload 預覽，完整編輯放到 JSON 子頁。
	public str PayloadTextPreview{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "";

	/// 實際要保存的 Payload 文本。
	public str PayloadText{
		get{return field;}
		set{
			if(SetProperty(ref field, value)){
				RefreshPayloadPreview();
			}
		}
	} = "";

	/// 當前編輯中的實體快照。
	public PoWeightArg PoWeightArg{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = new PoWeightArg{
		Type = EWeightArgType.Json,
	};

	/// 顯式設置當前編輯模式，供列表頁跳轉時調用。
	public nil SetCreateMode(bool IsCreate){
		IsCreateMode = IsCreate;
		return NIL;
	}

	/// 由列表頁傳入 Po 實體初始化編輯頁。
	public nil FromPoWeightArg(PoWeightArg? PoWeightArg){
		this.PoWeightArg = ClonePoWeightArg(PoWeightArg);
		IsCreateMode = PoWeightArg is null || PoWeightArg.Id == IdWeightArg.Zero;
		SyncFromPo();
		return NIL;
	}

	/// 打開 Payload JSON 子編輯頁。
	public nil OpenPayloadJsonEditor(){
		var view = new ViewWeightArgPayloadJsonEdit();
		view.Ctx?.Load(PayloadText, OnPayloadJsonApplied);
		ViewNavi?.GoTo(ToolView.WithTitle(Todo.I18n("WeightArg Payload(JSON)"), view));
		return NIL;
	}

	/// 保存到後端。新增走 BatAddWeightArg，編輯走 BatUpdWeightArg。
	public async Task<nil> Save(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			return NIL;
		}
		try{
			var po = BuildPoFromFields();
			var dbCtx = UserCtxMgr.GetDbUserCtx();
			if(IsCreateMode){
				await SvcStudyPlan.BatAddWeightArg(dbCtx, ToolAsyE.ToAsyE([po]), Ct);
			}else{
				await SvcStudyPlan.BatUpdWeightArg(dbCtx, ToolAsyE.ToAsyE([po]), Ct);
			}

			PoWeightArg = po;
			IsCreateMode = false;
			SyncFromPo();
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			ShowMsg(Todo.I18n("Saved"));
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}

	/// 軟刪除當前 WeightArg。新增模式下只清空界面，不調後端刪除接口。
	public async Task<nil> Delete(CT Ct = default){
		if(AnyNull(SvcStudyPlan, UserCtxMgr)){
			return NIL;
		}
		try{
			var po = BuildPoFromFields();
			if(!IsCreateMode && po.Id != IdWeightArg.Zero){
				await SvcStudyPlan.BatSoftDelWeightArg(UserCtxMgr.GetDbUserCtx(), ToolAsyE.ToAsyE([po]), Ct);
			}
			PoWeightArg = new PoWeightArg{
				Type = EWeightArgType.Json,
			};
			IsCreateMode = true;
			SyncFromPo();
			LastError = "";
			OnPropertyChanged(nameof(HasError));
			ShowMsg(Todo.I18n("Deleted"));
		}catch(Exception e){
			HandleErr(e);
		}
		return NIL;
	}

	void OnPayloadJsonApplied(str PayloadJson){
		PayloadText = PayloadJson ?? "";
	}

	public nil ApplySelectedWeightCalculator(PoWeightCalculator? Po){
		if(Po is null){
			return NIL;
		}
		WeightCalculatorName = Po.UniqName ?? "";
		return NIL;
	}

	void SyncFromPo(){
		var po = PoWeightArg ?? new PoWeightArg{
			Type = EWeightArgType.Json,
		};
		PoIdText = po.Id.ToString();
		PoUniqName = po.UniqName ?? "";
		PoDescr = po.Descr ?? "";
		WeightCalculatorName = po.WeightCalculatorName ?? "";
		PoTypeIndex = ClampIndex((i32)po.Type, TypeOptions.Count);
		PayloadText = po.Text ?? "";
		LastError = "";
		OnPropertyChanged(nameof(HasError));
	}

	PoWeightArg BuildPoFromFields(){
		var po = ClonePoWeightArg(PoWeightArg);
		po.UniqName = str.IsNullOrWhiteSpace(PoUniqName) ? null : PoUniqName.Trim();
		po.Descr = PoDescr?.Trim() ?? "";
		po.WeightCalculatorName = WeightCalculatorName?.Trim() ?? "";
		po.Type = EnumOrDefault<EWeightArgType>(PoTypeIndex);
		if(po.Type == EWeightArgType.Unknown){
			po.Type = EWeightArgType.Json;
		}
		po.Text = PayloadText;
		po.Binary = null;
		return po;
	}

	void RefreshPayloadPreview(){
		if(str.IsNullOrWhiteSpace(PayloadText)){
			PayloadTextPreview = "";
			return;
		}
		const int maxLen = 320;
		PayloadTextPreview = PayloadText.Length <= maxLen
			? PayloadText
			: PayloadText[..maxLen] + "...";
	}

	static PoWeightArg ClonePoWeightArg(PoWeightArg? src){
		src ??= new PoWeightArg{
			Type = EWeightArgType.Json,
		};
		return new PoWeightArg{
			DbCreatedAt = src.DbCreatedAt,
			DbUpdatedAt = src.DbUpdatedAt,
			DelAt = src.DelAt,
			BizCreatedAt = src.BizCreatedAt,
			BizUpdatedAt = src.BizUpdatedAt,
			Id = src.Id,
			Owner = src.Owner,
			UniqName = src.UniqName,
			Type = src.Type,
			Text = src.Text,
			Binary = src.Binary?.ToArray() ?? [],
			WeightCalculatorName = src.WeightCalculatorName,
			Descr = src.Descr,
		};
	}

	static i32 ClampIndex(i32 value, i32 count){
		if(count <= 0){
			return 0;
		}
		if(value < 0){
			return 0;
		}
		if(value >= count){
			return count - 1;
		}
		return value;
	}

	static TEnum EnumOrDefault<TEnum>(i32 index)
		where TEnum : struct, Enum
	{
		var values = Enum.GetValues<TEnum>();
		if(index < 0 || index >= values.Length){
			return values[0];
		}
		return values[index];
	}
}
