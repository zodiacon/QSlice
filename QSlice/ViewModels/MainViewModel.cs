using System.Threading;
using Prism.Mvvm;

namespace QSlice.ViewModels {
    class MainViewModel : BindableBase {
		public QSliceViewModel QSliceViewModel { get; } = new QSliceViewModel();

        public MainViewModel() {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }
	}
}
