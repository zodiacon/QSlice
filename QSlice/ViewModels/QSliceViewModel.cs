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
using System.Windows.Input;
using Prism.Commands;

namespace QSlice.ViewModels {
	class QSliceViewModel : BindableBase {
		DispatcherTimer _timer;
		ObservableCollection<ProcessViewModel> _processes;
		List<Process> _processesRaw;

		static ProcessEqualityComparer ProcessComparer = new ProcessEqualityComparer();

		public IList<ProcessViewModel> Processes => _processes;

		public QSliceViewModel() {
			InitProcesses();
			StartTimer();
		}

		ICollectionView _view;

		private void InitProcesses() {
			_processesRaw = Process.GetProcesses().Where(p => p.Id != 0).ToList();
			_processes = new ObservableCollection<ProcessViewModel>(_processesRaw.Select(p => new ProcessViewModel(p)));

			var liveView = (_view = CollectionViewSource.GetDefaultView(_processes)) as ICollectionViewLiveShaping;
			liveView.IsLiveSorting = true;
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
				_view.Filter = null;
			else {
				var ltext = text.ToLower();
				_view.Filter = o => ((ProcessViewModel)o).LowerName.Contains(ltext);
			}
		}

		private void StartTimer() {
			if (_timer == null) {
				_timer = new DispatcherTimer(TimeSpan.FromMilliseconds(Interval), DispatcherPriority.ApplicationIdle, delegate {
					// find out what processes have terminated and born

					var processes = Process.GetProcesses().Where(p => p.Id != 0).ToArray();
					var oldProcesses = _processesRaw.Except(processes, ProcessComparer).ToArray();
					var newProcesses = processes.Except(_processesRaw, ProcessComparer).ToArray();

					foreach (var p in oldProcesses) {
						_processes.Remove(_processes.First(pr => pr.Process.Id == p.Id));
						_processesRaw.Remove(_processesRaw.First(pr => pr.Id == p.Id));
					}

					foreach (var p in newProcesses) {
						_processesRaw.Add(p);
						_processes.Add(new ProcessViewModel(p));
					}

					double totalCpu = 0;
					foreach (var process in _processes) {
						process.Update();
						totalCpu += process.TotalCPU;
					}

					TotalCPU = totalCpu;

					// due to bug in live shaping

					_view.Refresh();

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
