namespace Ngaq.Ui.Views.Word.WordManage.StudyPlan;

using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Ngaq.Core.Infra;
using Tsinswreng.AvlnTools.Dsl;
using Tsinswreng.AvlnTools.Tools;
using Tsinswreng.CsTempus;

/// StudyPlan 相关页面的通用视图拼装工具。

public static class ToolStudyPlanView{
	/// 统一格式化 UniqName：超长时保留头尾，中间用省略号替代。

	public static str FormatUniqName(str? UniqName, int HeadLen = 10, int TailLen = 6){
		var raw = UniqName?.Trim() ?? "";
		if(str.IsNullOrWhiteSpace(raw)){
			return "-";
		}
		var minLen = HeadLen + TailLen + 3;
		if(raw.Length <= minLen){
			return raw;
		}
		return $"{raw[..HeadLen]}...{raw[^TailLen..]}";
	}

	/// 统一格式化日期：按当前用户所在时区显示短格式 yy-MM-dd。

	public static str FormatDateShort(Tempus Time, TimeZoneInfo? TimeZone = null){
		if(Time == Tempus.Zero){
			return "-";
		}
		var tz = TimeZone ?? TimeZoneInfo.Local;
		var dto = DateTimeOffset.FromUnixTimeMilliseconds(Time.Value);
		var local = TimeZoneInfo.ConvertTime(dto, tz);
		return local.ToString("yy-MM-dd", CultureInfo.InvariantCulture);
	}

	/// 统一处理“更新时间优先，否则创建时间”的短日期显示。
	public static str FormatUpdatedDateShort(Tempus UpdatedAt, Tempus CreatedAt, TimeZoneInfo? TimeZone = null){
		var t = UpdatedAt == Tempus.Zero ? CreatedAt : UpdatedAt;
		return FormatDateShort(t, TimeZone);
	}

	/// 创建“只读文本 + 选择按钮”的一行输入控件。
	/// <param name="Label">行标题文本。</param>
	/// <param name="Binding">文本框绑定。</param>
	/// <param name="BtnText">按钮文本。</param>
	/// <param name="OnBtnClick">按钮点击回调。</param>
	/// <param name="ReadOnly">文本框是否只读。</param>
	/// <returns>可直接加入布局树的控件。</returns>
	public static Control MkInputWithBtnRow(
		str Label, IBinding Binding, str BtnText, Action OnBtnClick, bool ReadOnly = false
	){
		var sp = new StackPanel{Spacing = 3};
		sp.Children.Add(new TextBlock{Text = Label});
		var row = new AutoGrid(IsRow:false);
		row.Grid.ColumnDefinitions.AddRange([
			ColDef(7, GUT.Star),
			ColDef(2, GUT.Star),
		]);
		row.A(new TextBox(), o=>{
			o.IsReadOnly = ReadOnly;
			o.Bind(TextBox.TextProperty, Binding);
		})
		.A(new Button(), o=>{
			o.Content = BtnText;
			o.Click += (s,e)=>OnBtnClick();
		});
		sp.Children.Add(row.Grid);
		return sp;
	}
}

