using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using PaletteInstaller.utils;

namespace PaletteInstaller
{
    static class Program
    {

        

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            
            // --install site host
            // --uninstall
            if (args[1].Equals("-i"))
            {
                string path = Environment.CurrentDirectory + "\\" + ServiceMain.EXE_NAME ;
                string site = args[2];
                string host = args[3];

                StringBuilder exeArgs = new StringBuilder("create \"" + ServiceMain.SERVICE_NAME + " " + site + " " + host + "\" ");
                exeArgs.Append("binPath=\"" + path + "\" ");
                exeArgs.Append("DisplayName=\"" + ServiceMain.DISPLAY_NAME + "\" ");
                exeArgs.Append("start=auto");
                string outStr = ExecCommand.Execute("sc", exeArgs.ToString());

                System.Console.WriteLine(outStr);

            }
            // --uninstall
            else if (args[1].Equals("-u"))
            {
                StringBuilder exeArgs = new StringBuilder("delete \"" + ServiceMain.SERVICE_NAME + "\"");
                string outStr = ExecCommand.Execute("sc", exeArgs.ToString());

                System.Console.WriteLine(outStr);

            }
            if (args[1].Equals("-d"))   //Debug
            {
                string path = Environment.CurrentDirectory + "\\" + ServiceMain.EXE_NAME;
                ServiceMain app = new ServiceMain();

                app.debugOnStart();
                System.Console.ReadLine();
            }
            else if (args.Length == 3)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {   
                    new ServiceMain()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
