using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSlice.Helpers {
	class ProcessEqualityComparer : IEqualityComparer<ProcessInfo> {
		public bool Equals(ProcessInfo x, ProcessInfo y) => x.Id == y.Id;
		public int GetHashCode(ProcessInfo obj) => obj.Id.GetHashCode();
	}
}
