using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QSlice.ViewModels;

namespace QSlice.Helpers {
    class ProcessViewModelEqualityComparer : IEqualityComparer<ProcessViewModel> {
        public bool Equals(ProcessViewModel x, ProcessViewModel y) {
            return x.Process.Id == y.Process.Id;
        }

        public int GetHashCode(ProcessViewModel obj) {
            return obj.Process.Id;
        }
    }
}
