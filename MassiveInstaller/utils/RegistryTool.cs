using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using log4net;

namespace PaletteInstaller.utils
{
    class RegistryTool
    {
        private static readonly ILog log = LogManager.GetLogger("RegistryTool");

        public static readonly string DEFAULT = "N/A";

        // The name of the key must include a valid root.
        private const string userRoot = "HKEY_LOCAL_MACHINE";
        private const string subkey = "ISAT";
        private string keyName;



        public RegistryTool(string secction)
        {
            keyName = subkey + "\\" + secction;
        }

        public string readString(string field)
        {
            
            string value = "";
            RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ISAT\\Palette", true);

            if (rk == null)
                rk = Registry.LocalMachine.CreateSubKey("SOFTWARE\\ISAT\\Palette");

            value = (string)rk.GetValue(field, DEFAULT);

            return value;   
        }


        public bool writeString(string field, string value)
        {
            bool result = true;

            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ISAT\\Palette", true);
                rk.SetValue(field, value);
            }catch(Exception ex)
            {
                log.Error("Registry Tool Error: " + ex.Message);
                result = false;
            }
            return result;
            
        }


    }
}
