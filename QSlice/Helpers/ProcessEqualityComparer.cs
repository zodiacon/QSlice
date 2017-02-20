using QSlice.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSlice.Helpers {
	sealed class ProcessEqualityComparer : IEqualityComparer<ProcessInfo> {
		public bool Equals(ProcessInfo x, ProcessInfo y) => x.Id == y.Id;
		public int GetHashCode(ProcessInfo obj) => obj.Id.GetHashCode();
	}

	sealed class ProcessViewModelEqualityComparer : IEqualityComparer<ProcessViewModel> {
		public bool Equals(ProcessViewModel x, ProcessViewModel y) => x.Id == y.Id;

		public int GetHashCode(ProcessViewModel obj) => obj.Id.GetHashCode();
	}
}
