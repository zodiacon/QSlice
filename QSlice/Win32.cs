using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace QSlice {
    [Flags]
    enum CreateToolhelpSnapshotFlags {
        SnapProcess = 2
    };

    [StructLayout(LayoutKind.Sequential)]
    struct ProcessEntry {
        public uint dwSize;
        public uint cntUsage;
        public uint th32ProcessID;
        public UIntPtr th32DefaultHeapID;
        public uint th32ModuleID;
        public uint cntThreads;
        public uint th32ParentProcessID;
        public int pcPriClassBase;
        public uint dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;

        public void Init() {
            dwSize = (uint)Marshal.SizeOf<ProcessEntry>();
        }

    }

    [SuppressUnmanagedCodeSecurity]
    static class Win32 {
        public const int ProcessQueryLimitedProcessInformation = 0x1000;

        [DllImport("kernel32")]
        public static extern bool GetProcessTimes(IntPtr hProcess, out long create, out long exit, out long kernel, out long user);

        [DllImport("kernel32")]
        public static extern IntPtr OpenProcess(int accessMask, bool inheritHandle, uint pid);

        [DllImport("kernel32")]
        public static extern bool CloseHandle(IntPtr hProcess);

        [DllImport("kernel32")]
        public static extern SafeFileHandle CreateToolhelp32Snapshot(CreateToolhelpSnapshotFlags flags, uint pid);

        [DllImport("kernel32")]
        public static extern bool Process32First(SafeFileHandle hSnapshot, ref ProcessEntry pe);

        [DllImport("kernel32")]
        public static extern bool Process32Next(SafeFileHandle hSnapshot, ref ProcessEntry pe);
    }
}
