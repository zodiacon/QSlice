using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using QSlice.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using Prism.Mvvm;
using System.Threading.Tasks;

namespace QSlice.ViewModels {
    class QSliceViewModel : BindableBase {
        DispatcherTimer _timer;
        ObservableCollection<ProcessViewModel> _processes;
        List<Process> _processesRaw;

        static ProcessEqualityComparer ProcessComparer = new ProcessEqualityComparer();
        static ProcessViewModelEqualityComparer ProcessComparer2 = new ProcessViewModelEqualityComparer();

        public IList<ProcessViewModel> Processes => _processes;

        public QSliceViewModel() {
            //Process.EnterDebugMode();

            InitProcesses();
            StartTimer();
        }


        private void InitProcesses() {
            _processesRaw = Process.GetProcesses().Where(p => p.Id != 0).ToList();
            _processes = new ObservableCollection<ProcessViewModel>(_processesRaw.Select(p => new ProcessViewModel(p)));

            var liveView = CollectionViewSource.GetDefaultView(_processes) as ICollectionViewLiveShaping;
            liveView.IsLiveSorting = true;
        }

        private int _interval = 1000;

        public int Interval {
            get { return _interval; }
            set {
                if(SetProperty(ref _interval, value)) {
                    StartTimer();
                }
            }
        }

        private void StartTimer() {
            if(_timer == null) {
                _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(Interval), DispatcherPriority.Normal, delegate {
                    _timer.Stop();
                    var processes = Process.GetProcesses().Where(p => p.Id != 0).ToArray();
                    var oldProcesses = _processesRaw.Except(processes, ProcessComparer).ToArray();
                    var newProcesses = processes.Except(_processesRaw, ProcessComparer).ToArray();

                    foreach(var p in oldProcesses) {
                        _processes.Remove(_processes.First(pr => pr.Process.Id == p.Id));
                        _processesRaw.Remove(_processesRaw.First(pr => pr.Id == p.Id));
                    }

                    foreach(var p in newProcesses) {
                        _processesRaw.Add(p);
                        _processes.Add(new ProcessViewModel(p));
                    }

                    foreach(var process in _processes) {
                        process.Update();
                    }

                    CollectionViewSource.GetDefaultView(_processes).Refresh();

                    _timer.Start();
                }, Dispatcher.CurrentDispatcher);
            }
            else {
                _timer.Interval = TimeSpan.FromMilliseconds(Interval);
            }
        }
    }
}
