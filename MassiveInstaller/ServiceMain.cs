using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using PaletteInstaller.utils;
using log4net;
using log4net.Config;

namespace PaletteInstaller
{
    public partial class ServiceMain : ServiceBase
    {
        private readonly string VERSION = "Palette Installer V1.0.0";

        private static readonly ILog log = LogManager.GetLogger("Service");

        public static string SERVICE_NAME = "ISAT Palette Installer";
        public static string DISPLAY_NAME = "ISAT Palette Installer.";
        public static string EXE_NAME = "PaletteInstaller.exe";

        private readonly int TimeToCheck = 36000; //36,000 segundos = 10 minutos
        private bool bKeepRunning;
        private string site;
        private string host;

        string[] argsRT; 

        public ServiceMain()
        {
            site = Environment.GetCommandLineArgs()[1];
            host = Environment.GetCommandLineArgs()[2];

            InitializeComponent();
        }

        public void debugOnStart()
        {
            string[] args = Environment.GetCommandLineArgs();
            string[] argsDeb = { args[0], args[2], args[3]};

            site = args[2];
            host = args[3];

            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            Task t = new Task(new Action(this.Run));
            t.Start();
        }

        protected override void OnStop()
        {
            log.Info("Stopping service...");
            bKeepRunning = false;
        }


        public void Run()
        {
            string data = string.Format("Starting service with site=[{0}] and host=[{1}]", site, host);
            XmlConfigurator.Configure();
            log.Info("*********************************************************************");
            log.Info(VERSION);
            log.Info(data);
            log.Info("*********************************************************************");

            
            bKeepRunning = true;

            //Thread.Sleep(10000); //Wait 10 seconds before to a network port.

            int counter = 10;

            do
            {
                if(counter == 0)
                {
//                    log.Info("DO");
                    try
                    {
                        Core cUp = new Core(site, host);
                        cUp.DoReview();
                        cUp = null;
                    }catch(Exception ex)
                    {
                        log.Error("FALLO: " + ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                    counter = TimeToCheck; // 
                    
                }
                else
                {
//                    log.Info("COUNTER");
                    counter--;
                    Thread.Sleep(1000);  //wait one second
                }

            }while (bKeepRunning) ;

            log.Info("Installer Service has been stoped.");
        }


    }
}
