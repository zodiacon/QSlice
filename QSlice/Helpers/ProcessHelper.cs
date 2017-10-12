using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSlice.Helpers {
	static class ProcessHelper {

		public static IList<ProcessInfo> EnumProcesses() {
			var hSnapshot = Win32.CreateToolhelp32Snapshot(CreateToolhelpSnapshotFlags.SnapProcess, 0);
			var processes = new List<ProcessInfo>(256);
			var pe = new ProcessEntry();
			pe.Init();

			if(!Win32.Process32First(hSnapshot, ref pe))
				return processes;

			do {
				// skip idle process
				if(pe.th32ProcessID == 0)
					continue;

				processes.Add(new ProcessInfo { Id = pe.th32ProcessID, Name = pe.szExeFile });
			} while(Win32.Process32Next(hSnapshot, ref pe));

			//Win32.CloseHandle(hSnapshot);
			return processes;
		}
	}
}
