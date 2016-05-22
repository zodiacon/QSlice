using System;
using System.Diagnostics;
using Prism.Mvvm;

namespace QSlice.ViewModels {
    class ProcessViewModel : BindableBase, IDisposable {
        long _lastKernelTime, _lastUserTime;
        int _lastUpdate;
        IntPtr _handle;

        static int _processorCount = Environment.ProcessorCount;

        public Process Process { get; }

        public ProcessViewModel(Process process) {
            Process = process;
            _handle = Win32.OpenProcess(Win32.ProcessQueryLimitedProcessInformation, false, process.Id);
            long dummy;
            Win32.GetProcessTimes(_handle, out dummy, out dummy, out _lastKernelTime, out _lastUserTime);

            _lastUpdate = Environment.TickCount;
        }

        double _kernelTime, _userTime, _totalTime;

        public double KernelCPU => _kernelTime;
        public double UserCPU => _userTime;
        public double TotalCPU => _totalTime;

        string _lowerName;
        public string LowerName => _lowerName ?? (_lowerName = Process.ProcessName.ToLower());

        public void Update() {

            var diff = Environment.TickCount - _lastUpdate;
            if (diff == 0)
                return;

            long dummy, user, kernel;
            Win32.GetProcessTimes(_handle, out dummy, out dummy, out kernel, out user);

            var factor = (double)(diff * _processorCount * 100);

            _kernelTime = (kernel - _lastKernelTime) / factor;
            _userTime = (user - _lastUserTime) / factor;
            _totalTime = _kernelTime + _userTime;

            _lastKernelTime = kernel;
            _lastUserTime = user;
            _lastUpdate = Environment.TickCount;

            OnPropertyChanged(nameof(KernelCPU));
            OnPropertyChanged(nameof(UserCPU));
            OnPropertyChanged(nameof(TotalCPU));
        }

        void Dispose(bool disposing) {
            if (disposing)
                GC.SuppressFinalize(this);
            Win32.CloseHandle(_handle);
        }

        public void Dispose() {
            Dispose(true);
        }

        ~ProcessViewModel() {
            Dispose(false);
        }
    }
}
