using System;
using System.Diagnostics;
using Prism.Mvvm;

namespace QSlice.ViewModels {
    class ProcessViewModel : BindableBase {
		TimeSpan _lastKernelTime, _lastUserTime, _lastTotalTime;
        int _lastUpdate;

        static int _processorCount = Environment.ProcessorCount;

		public Process Process { get; }

		public ProcessViewModel(Process process) {
			Process = process;
			try {
				_lastKernelTime = process.PrivilegedProcessorTime;
				_lastUserTime = process.UserProcessorTime;
				_lastTotalTime = process.TotalProcessorTime;
				_lastUpdate = Environment.TickCount;
			}
			catch(InvalidOperationException) {
			}
		}

		float _kernelTime, _userTime, _totalTime;

		public float KernelCPU => _kernelTime;
		public float UserCPU => _userTime;
		public float TotalCPU => _totalTime;

		string _lowerName;
		public string LowerName => _lowerName ?? (_lowerName = Process.ProcessName.ToLower());

		public void Update() {
			Process.Refresh();

            var diff = Environment.TickCount - _lastUpdate;
            if(diff == 0)
                return;

            var factor = 100.0f / diff / _processorCount;

            try {
				_kernelTime = (float)(Process.PrivilegedProcessorTime.TotalMilliseconds - _lastKernelTime.TotalMilliseconds) * factor;
				_userTime = (float)(Process.UserProcessorTime.TotalMilliseconds - _lastUserTime.TotalMilliseconds) * factor;
				_totalTime = (float)(Process.TotalProcessorTime.TotalMilliseconds - _lastTotalTime.TotalMilliseconds) * factor;

				_lastKernelTime = Process.PrivilegedProcessorTime;
				_lastUserTime = Process.UserProcessorTime;
				_lastTotalTime = Process.TotalProcessorTime;
				_lastUpdate = Environment.TickCount;

                OnPropertyChanged(nameof(KernelCPU));
                OnPropertyChanged(nameof(UserCPU));
                OnPropertyChanged(nameof(TotalCPU));
            }
            catch {
				_totalTime = _kernelTime = _userTime = 0;
			}
		}
	}
}
