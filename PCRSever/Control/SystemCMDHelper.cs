using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PCRSever.Control
{
    public class SystemCMDHelper
    {
        private static SystemCMDHelper _SystemCMDHelper=null;
        public static SystemCMDHelper Singleton()
        {
            if (_SystemCMDHelper==null)
            {
                _SystemCMDHelper = new SystemCMDHelper();
            }
            return _SystemCMDHelper;
        }
        private SystemCMDHelper()
        {
            
        }
        public void ShutDown()
        {
            CreateProcess("shutdown -s -t 10");
        }
        public void Cancel_ShutDown()
        {
            CreateProcess("shutdown -a");
        }
        public void Restart()
        {
            CreateProcess("shutdown -r -t 10");
        }
        public void Dormancyd()
        {
            CreateProcess("shutdown -h -t 10");
        }
        private void CreateProcess(string cmdstring)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.Start();
            process.StandardInput.WriteLine(cmdstring);
            process.StandardInput.WriteLine("exit");
            process.Close();
        }
    }
}
