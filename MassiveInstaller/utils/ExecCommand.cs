using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using log4net;

namespace PaletteInstaller.utils
{
    class ExecCommand
    {
        private static readonly ILog log = LogManager.GetLogger("ExecCommand");

        public static string Execute(string exe, string args)
        {
            try
            {
                if (args != null)
                    System.Console.WriteLine("$>" + exe + " " + args);
                else
                    System.Console.WriteLine(exe);

                Process process = new System.Diagnostics.Process();
                ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

                startInfo.CreateNoWindow = true; // .WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.FileName = exe;
                startInfo.Arguments = args;
                process.StartInfo = startInfo;
                process.Start();

                return process.StandardOutput.ReadToEnd();
            }catch(Exception ex)
            {
                log.Error("Execute command Error: " + ex.Message);
            }

            return "";
        }

    }
}
