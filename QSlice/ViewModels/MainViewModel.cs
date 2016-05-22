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

		public SelectedShowNumber[] ShowNumbers { get; } = new[] {
			new SelectedShowNumber { Text = "All", Count = -1 },
			new SelectedShowNumber { Text = "5", Count = 5 },
			new SelectedShowNumber { Text = "10", Count = 10 },
			new SelectedShowNumber { Text = "15", Count = 15 },
			new SelectedShowNumber { Text = "20", Count = 20 },
			new SelectedShowNumber { Text = "30", Count = 30 },
			new SelectedShowNumber { Text = "50", Count = 50 },
		};

		private SelectedShowNumber _selectedShowNumber;

		public SelectedShowNumber SelectedShowNumber {
			get { return _selectedShowNumber; }
			set {
				if(SetProperty(ref _selectedShowNumber, value)) {
					QSliceViewModel.MaxCount = value.Count;
				}
			}
		}

		public MainViewModel() {
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			SelectedShowNumber = ShowNumbers[0];
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

		bool _isRunning = true;
		public ICommand PlayPauseCommand => new DelegateCommand(() => {
			_isRunning = !_isRunning;
			QSliceViewModel.IsEnabled = _isRunning;
			OnPropertyChanged(nameof(PlayPauseImage));
		});

		public string PlayPauseImage => _isRunning ? "/images/pause.png" : "/images/play.png";

	}
}
