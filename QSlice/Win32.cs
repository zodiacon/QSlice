using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSlice {
    static class Win32 {
        public const int ProcessQueryLimitedProcessInformation = 0x1000;

        [DllImport("kernel32")]
        public static extern bool GetProcessTimes(IntPtr hProcess, out long create, out long exit, out long kernel, out long user);

        [DllImport("kernel32")]
        public static extern IntPtr OpenProcess(int accessMask, bool inheritHandle, int pid);

        [DllImport("kernel32")]
        public static extern bool CloseHandle(IntPtr hProcess);
    }
}
