namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan.PreFilterEdit;

using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Ngaq.Core.Shared.StudyPlan.Models;
using Ngaq.Core.Shared.StudyPlan.Models.Po.PreFilter;
using Ngaq.Core.Shared.StudyPlan.Models.PreFilter;

public partial class VmPreFilterEdit{
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

	public str PreFilterVersion{
		get{return field;}
		set{SetProperty(ref field, value);}
	} = "1.0.0.0";

	void SyncVisualFromBo(){
		var po = BoPreFilter?.PoPreFilter ?? MkEmptyBoPreFilter().PoPreFilter;
		var pre = BoPreFilter?.PreFilter ?? MkEmptyBoPreFilter().PreFilter;

		_isHydrating = true;
		try{
			PoIdText = po.Id.ToString();
			PoUniqName = po.UniqName ?? "";
			PoDescr = po.Descr;
			PoTypeIndex = ClampIndex((i32)po.Type, PoTypeOptions.Count);
			PreFilterVersion = pre.Version?.ToString() ?? "1.0.0.0";

			CoreFilterRows.Clear();
			foreach(var row in (pre.CoreFilter ?? []).Select(MkRowFromModel)){
				CoreFilterRows.Add(row);
			}
			PropFilterRows.Clear();
			foreach(var row in (pre.PropFilter ?? []).Select(MkRowFromModel)){
				PropFilterRows.Add(row);
			}

			if(CoreFilterRows.Count == 0){
				CoreFilterRows.Add(MkFieldsFilterRow());
			}
			if(PropFilterRows.Count == 0){
				PropFilterRows.Add(MkFieldsFilterRow());
			}

			LastError = "";
			OnPropertyChanged(nameof(HasError));
		}finally{
			_isHydrating = false;
		}
	}

	VmFieldsFilterRow MkRowFromModel(FieldsFilter row){
		var vm = new VmFieldsFilterRow{
			FieldsText = string.Join(", ", row.Fields ?? []),
		};
		foreach(var item in (row.Filters ?? [])){
			vm.Items.Add(new VmFilterItemRow{
				OperationIndex = ClampIndex((i32)item.Operation, OperationOptions.Count),
				ValueTypeIndex = ClampIndex((i32)item.ValueType, ValueTypeOptions.Count),
				ValuesText = string.Join(", ", (item.Values ?? []).Select(x => x?.ToString() ?? "")),
			});
		}
		if(vm.Items.Count == 0){
			vm.Items.Add(MkFilterItemRow());
		}
		return vm;
	}

	bool TryBuildBoFromVisual(
		out BoPreFilter Bo,
		out str Err
	){
		Bo = BoPreFilter ?? MkEmptyBoPreFilter();
		Err = "";
		try{
			var po = ClonePoPreFilter(Bo.PoPreFilter);
			po.UniqName = str.IsNullOrWhiteSpace(PoUniqName) ? null : PoUniqName.Trim();
			po.Descr = PoDescr?.Trim() ?? "";
			po.Type = EnumOrDefault<EPreFilterType>(PoTypeIndex);
			if(po.Type == EPreFilterType.Unknown){
				po.Type = EPreFilterType.Json;
			}

			if(!Version.TryParse((PreFilterVersion ?? "").Trim(), out var ver)){
				Err = "Version format invalid. Example: 1.0.0.0";
				return false;
			}

			var pre = new Ngaq.Core.Shared.StudyPlan.Models.PreFilter.PreFilter{
				Version = ver,
				CoreFilter = BuildFieldsFilterList(CoreFilterRows, out var coreErr),
				PropFilter = [],
			};
			if(coreErr is not null){
				Err = coreErr;
				return false;
			}

			pre.PropFilter = BuildFieldsFilterList(PropFilterRows, out var propErr);
			if(propErr is not null){
				Err = propErr;
				return false;
			}

			po.DataSchemaVer = pre.Version;
			po.Text = JsonSerializer.Stringify(pre);
			po.Binary = null;

			Bo = new BoPreFilter{
				PoPreFilter = po,
				PreFilter = pre,
			};
			return true;
		}catch(Exception e){
			Err = e.Message;
			return false;
		}
	}

	static PoPreFilter ClonePoPreFilter(PoPreFilter? src){
		src ??= new PoPreFilter();
		return new PoPreFilter{
			DbCreatedAt = src.DbCreatedAt,
			DbUpdatedAt = src.DbUpdatedAt,
			DelAt = src.DelAt,
			BizCreatedAt = src.BizCreatedAt,
			BizUpdatedAt = src.BizUpdatedAt,
			Id = src.Id,
			Owner = src.Owner,
			UniqName = src.UniqName,
			Descr = src.Descr,
			Type = src.Type,
			DataSchemaVer = src.DataSchemaVer,
			Text = src.Text,
			Binary = src.Binary?.ToArray() ?? [],
		};
	}

	IList<FieldsFilter> BuildFieldsFilterList(
		IEnumerable<VmFieldsFilterRow> Rows,
		out str? Err
	){
		Err = null;
		var ans = new List<FieldsFilter>();
		var rowIdx = 0;
		foreach(var row in Rows){
			rowIdx++;
			var fields = SplitCsv(row.FieldsText).ToList();
			var filters = new List<FilterItem>();
			var itemIdx = 0;
			foreach(var item in row.Items){
				itemIdx++;
				var valueType = EnumOrDefault<EValueType>(item.ValueTypeIndex);
				var values = ParseValues(item.ValuesText, valueType, out var parseErr);
				if(parseErr is not null){
					Err = $"Row#{rowIdx}, Item#{itemIdx}: {parseErr}";
					return [];
				}
				filters.Add(new FilterItem{
					Operation = EnumOrDefault<EFilterOperationMode>(item.OperationIndex),
					ValueType = valueType,
					Values = values,
				});
			}
			ans.Add(new FieldsFilter{
				Fields = fields,
				Filters = filters,
			});
		}
		return ans;
	}

	IList<obj?> ParseValues(str Text, EValueType ValueType, out str? Err){
		Err = null;
		var parts = SplitCsv(Text).ToList();
		if(parts.Count == 0){
			return [];
		}

		if(ValueType == EValueType.String || ValueType == EValueType.Null){
			return parts.Cast<obj?>().ToList();
		}
		if(ValueType == EValueType.Number){
			var ans = new List<obj?>();
			foreach(var p in parts){
				if(long.TryParse(p, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i)){
					ans.Add(i);
					continue;
				}
				if(double.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)){
					ans.Add(d);
					continue;
				}
				Err = $"'{p}' is not a valid number";
				return [];
			}
			return ans;
		}
		return parts.Cast<obj?>().ToList();
	}

	static IEnumerable<str> SplitCsv(str? Text){
		if(str.IsNullOrWhiteSpace(Text)){
			return [];
		}
		return Text
			.Split([',', '\n', '\r', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Where(x=>!str.IsNullOrWhiteSpace(x));
	}

	static TEnum EnumOrDefault<TEnum>(i32 Index)
		where TEnum : struct, Enum
	{
		var values = Enum.GetValues<TEnum>();
		if(Index < 0 || Index >= values.Length){
			return values[0];
		}
		return values[Index];
	}

	static i32 ClampIndex(i32 Value, i32 Count){
		if(Count <= 0){
			return 0;
		}
		if(Value < 0){
			return 0;
		}
		if(Value >= Count){
			return Count - 1;
		}
		return Value;
	}
}
