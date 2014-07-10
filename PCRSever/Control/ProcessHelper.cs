using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;

namespace PCRSever.Control
{
    class ProcessHelper
    {
        private ProcessHelper()
        {
        }
        private static ProcessHelper _ProcessHelper;
        public static ProcessHelper Singletion()
        {
            if (_ProcessHelper==null)
            {
                _ProcessHelper = new ProcessHelper();
            }
            return _ProcessHelper;
        }
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        private static extern bool DestroyWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        private const int SW_RESTORE = 9;

        public void RaiseOtherProcess(string PPTprocessName)
        {
            Process proc = Process.GetCurrentProcess();
            Process.GetProcesses();
            string oldname=Process.GetCurrentProcess().ProcessName;

            foreach (Process otherProc in
                Process.GetProcessesByName(PPTprocessName))
            {
                //ignore "this" process
                if (proc.Id != otherProc.Id)
                {
                    // Found a "same named process".
                    // Assume it is the one we want brought to the foreground.
                    // Use the Win32 API to bring it to the foreground.
                    IntPtr hWnd = otherProc.MainWindowHandle;
                    if (IsIconic(hWnd))
                    {
                        ShowWindowAsync(hWnd, 9);
                    }
                    SetForegroundWindow(hWnd);
                    break;
                }
            }
        }
        public void closeProc()
        {
            Defines.MainW.Dispatcher.BeginInvoke(new Action(() =>
            {
                IntPtr activeWind = GetActiveWindow();
                WindowInteropHelper wndHelper = new WindowInteropHelper(Defines.MainW);
                IntPtr wpfHwnd = wndHelper.Handle;
                if (wpfHwnd != activeWind)
                {
                    bool result = DestroyWindow(activeWind);
                }
            }));
        }
    }
}
