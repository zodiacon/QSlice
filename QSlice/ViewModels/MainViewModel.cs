using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro;
using Prism.Commands;
using Prism.Mvvm;

namespace QSlice.ViewModels {
    class MainViewModel : BindableBase {
        public QSliceViewModel QSliceViewModel { get; } = new QSliceViewModel();

        Settings _settings;

        public SelectedShowNumber[] ShowNumbers { get; } = new[] {
            new SelectedShowNumber { Text = "All", Count = -1 },
            new SelectedShowNumber { Text = "5", Count = 5 },
            new SelectedShowNumber { Text = "10", Count = 10 },
            new SelectedShowNumber { Text = "15", Count = 15 },
            new SelectedShowNumber { Text = "20", Count = 20 },
            new SelectedShowNumber { Text = "30", Count = 30 },
            new SelectedShowNumber { Text = "50", Count = 50 },
        };

        public void Close() {
            SaveSettings();
        }

        private void SaveSettings() {
            Serializer.Save(GetFileName("Settings.xml"), _settings);
        }

        public RefreshInterval[] RefreshIntervals { get; } = new[] {
            new RefreshInterval { Text = "0.5 sec", Interval = 500 },
            new RefreshInterval { Text = "1 sec", Interval = 1000 },
            new RefreshInterval { Text = "2 sec", Interval = 2000 },
            new RefreshInterval { Text = "5 sec", Interval = 5000 },
        };

        private bool _isAlwaysOnTop;

        public bool IsAlwaysOnTop {
            get { return _isAlwaysOnTop; }
            set {
                if(SetProperty(ref _isAlwaysOnTop, value)) {
                    Application.Current.MainWindow.Topmost = value;
                    _settings.IsAlwaysOnTop = value;
                };
            }
        }

        private RefreshInterval _selectedInterval;

        public RefreshInterval SelectedInterval {
            get { return _selectedInterval; }
            set {
                if(SetProperty(ref _selectedInterval, value)) {
                    QSliceViewModel.Interval = value.Interval;
                }
            }
        }

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
            SelectedInterval = RefreshIntervals[1];
            LoadSettings();
        }

        private void LoadSettings() {
            var settings = Serializer.Load<Settings>(GetFileName("Settings.xml"));
            string accentName = null;
            if(settings != null) {
                accentName = settings.AccentName;
                Application.Current.MainWindow.Topmost = settings.IsAlwaysOnTop;
            }
            _settings = settings ?? new Settings();

            if(accentName == null)
                accentName = "Cobalt";

            ChangeAccentCommand.Execute(Accents.First(acc => acc.Name == accentName));
        }

        private static string GetFileName(string name) {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create) + "\\QSlice\\";
            if(!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder + name;
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
            _settings.AccentName = _currentAccent.Name;
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
