using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using MahApps.Metro;
using Prism.Commands;
using Prism.Mvvm;

namespace QSlice.ViewModels {
    class MainViewModel : BindableBase {
		public QSliceViewModel QSliceViewModel { get; } = new QSliceViewModel();

        public MainViewModel() {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
			LoadSettings();
        }

		private void LoadSettings() {
			var accentName = "Cobalt";
			ChangeAccentCommand.Execute(Accents.First(acc => acc.Name == accentName));
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

		AccentViewModel[] _accents;
		public AccentViewModel[] Accents => _accents ?? (_accents = ThemeManager.Accents.Select(accent => new AccentViewModel(accent)).ToArray());

		AccentViewModel _currentAccent;

		public AccentViewModel CurrentAccent => _currentAccent;

		public ICommand ChangeAccentCommand => new DelegateCommand<AccentViewModel>(accent => {
			if(_currentAccent != null)
				_currentAccent.IsCurrent = false;
			_currentAccent = accent;
			accent.IsCurrent = true;
			OnPropertyChanged(nameof(CurrentAccent));
		}, accent => accent != _currentAccent)
			.ObservesProperty(() => CurrentAccent);

	}
}
