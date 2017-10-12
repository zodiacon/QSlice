using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using QSlice.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using Prism.Mvvm;
using System.Diagnostics;
using Syncfusion.Data;

namespace QSlice.ViewModels {
    class QSliceViewModel : BindableBase {
        DispatcherTimer _timer;
        ObservableCollection<ProcessViewModel> _processes;
        IList<ProcessInfo> _processesRaw;

        static ProcessEqualityComparer ProcessComparer = new ProcessEqualityComparer();
        static ProcessViewModelEqualityComparer ProcessViewModelComparer = new ProcessViewModelEqualityComparer();

        public IList<ProcessViewModel> Processes => _processes;

        public QSliceViewModel() {
            InitProcesses();
            StartTimer();
        }


        ICollectionViewAdv _view;
        public ICollectionViewAdv View {
            get => _view;
            set => SetProperty(ref _view, value);
        }

        private void InitProcesses() {
            _processesRaw = ProcessHelper.EnumProcesses();
            _processes = new ObservableCollection<ProcessViewModel>(_processesRaw.Select(p => new ProcessViewModel(p.Id, p.Name)));
        }

        private int _maxCount = -1;

        public int MaxCount {
            get { return _maxCount; }
            set { SetProperty(ref _maxCount, value); }
        }

        private int _interval;

        public int Interval {
            get { return _interval; }
            set {
                if (SetProperty(ref _interval, value)) {
                    _timer.Interval = TimeSpan.FromMilliseconds(value);
                }
            }
        }

        public void SetSearchText(string text) {
            if (string.IsNullOrWhiteSpace(text))
                View.Filter = null;
            else {
                var ltext = text.ToLower();
                View.Filter = o => ((ProcessViewModel)o).LowerName.Contains(ltext);
            }
            View.RefreshFilter();
        }

        private void StartTimer() {
            if (_timer == null) {
                _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(Interval), DispatcherPriority.Background, delegate {
                    // find out what processes have terminated and created

                    var processes = ProcessHelper.EnumProcesses();
                    var oldProcesses = _processesRaw.Except(processes, ProcessComparer).ToArray();
                    var newProcesses = processes.Except(_processesRaw, ProcessComparer).ToArray();

                    foreach (var p in oldProcesses) {
                        _processes.Remove(_processes.First(pr => pr.Id == p.Id));
                    }

                    foreach (var p in newProcesses) {
                        _processes.Add(new ProcessViewModel(p.Id, p.Name));
                    }

                    Debug.Assert(_processes.Distinct().Count() == _processes.Count());

                    _processesRaw = processes;

                    double totalCpu = 0;
                    foreach (var process in _processes) {
                        process.Update();
                        totalCpu += process.TotalCPU;
                    }

                    TotalCPU = totalCpu;

                }, Dispatcher.CurrentDispatcher);
            }
            else {
                _timer.Interval = TimeSpan.FromMilliseconds(Interval);
            }
        }

        private double _totalCPU;

        public double TotalCPU {
            get { return _totalCPU; }
            set { SetProperty(ref _totalCPU, value); }
        }

        public bool IsEnabled {
            get { return _timer.IsEnabled; }
            set { _timer.IsEnabled = value; }
        }

    }
}
