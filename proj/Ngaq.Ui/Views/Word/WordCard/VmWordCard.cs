namespace Ngaq.Ui.Views.Word.WordCard;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Media;
using Ngaq.Core.Shared.Word.Models.Learn_;
using Ngaq.Core.Infra.Core;
using Ngaq.Ui.Infra;
using Ctx = VmWordListCard;
public partial class VmWordListCard
	:VmBaseWordListCard
{
	public new static ObservableCollection<Ctx> Samples = [];
	static VmWordListCard(){
		#if DEBUG

		#endif
	}

}
