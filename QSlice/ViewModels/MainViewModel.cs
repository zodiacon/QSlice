using System.Threading;
using Prism.Mvvm;

namespace QSlice.ViewModels {
    class MainViewModel : BindableBase {
		public QSliceViewModel QSliceViewModel { get; } = new QSliceViewModel();

        public MainViewModel() {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

		private string _searchText;

		public string SearchText {
			get { return _searchText; }
			set {
				if(SetProperty(ref _searchText, value)) {
					QSliceViewModel.SetSearchText(value);
				}
			}
		}

	}
}
