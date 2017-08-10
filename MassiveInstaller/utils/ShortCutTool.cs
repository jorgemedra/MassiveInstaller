using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteInstaller.utils
{
    class ShortCutTool
    {
        
        public void CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation,
                                    string pathIcon)
        {
            string shortcutLocation = System.IO.Path.Combine(shortcutPath, shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
            
            shortcut.IconLocation = pathIcon;  
            shortcut.TargetPath = targetFileLocation;
            shortcut.Save(); 
        }

    }
}
