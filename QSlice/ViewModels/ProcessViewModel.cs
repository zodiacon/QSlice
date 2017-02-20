using System;
using Prism.Mvvm;

namespace QSlice.ViewModels {
	class ProcessViewModel : BindableBase, IDisposable {
		long _lastKernelTime, _lastUserTime;
		int _lastUpdate;
		IntPtr _handle;

		static int _processorCount = Environment.ProcessorCount;

		public string ProcessName { get; }
		public uint Id { get; }

		public ProcessViewModel(uint id, string name) {
			Id = id;
			ProcessName = name;
			_lowerName = name;

			_handle = Win32.OpenProcess(Win32.ProcessQueryLimitedProcessInformation, false, id);
			long dummy;
			Win32.GetProcessTimes(_handle, out dummy, out dummy, out _lastKernelTime, out _lastUserTime);

			_lastUpdate = Environment.TickCount;
		}

		double _kernelTime, _userTime, _totalTime;

		public double KernelCPU => _kernelTime;
		public double UserCPU => _userTime;
		public double TotalCPU => _totalTime;

		string _lowerName;
		public string LowerName => _lowerName;

		public void Update() {

			var diff = Environment.TickCount - _lastUpdate;
			if(diff == 0)
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

			OnPropertyChanged(nameof(TotalCPU));
			OnPropertyChanged(nameof(KernelCPU));
			OnPropertyChanged(nameof(UserCPU));
		}

		void Dispose(bool disposing) {
			if(disposing)
				GC.SuppressFinalize(this);
			Win32.CloseHandle(_handle);
		}

#pragma warning disable CC0029 // Disposables Should Call Suppress Finalize
		public void Dispose() {
#pragma warning restore CC0029 // Disposables Should Call Suppress Finalize
			Dispose(true);
		}

		~ProcessViewModel() {
			Dispose(false);
		}
	}
}
