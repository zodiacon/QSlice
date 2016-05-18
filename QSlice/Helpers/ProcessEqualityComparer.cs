using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSlice.Helpers {
	class ProcessEqualityComparer : IEqualityComparer<Process> {
		public bool Equals(Process x, Process y) => x.Id == y.Id;
		public int GetHashCode(Process obj) => obj.Id;
	}
}
