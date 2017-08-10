using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using log4net;

namespace PaletteInstaller.utils
{
    class ZipTool
    {
        private static readonly ILog log = LogManager.GetLogger("ZipTool");

        public bool CreateZipFileFromDirectory(string dirSource, string zipFileDest, bool addBaseDir)
        {
            bool result = true;
            try
            {
                ZipFile.CreateFromDirectory(dirSource, zipFileDest, CompressionLevel.Optimal, addBaseDir);
            }catch(Exception ex)
            {
                log.Error("Zip Tool Error: " + ex.Message);
                result = false;
            }
            return result;
        }

        public bool UnZipFile(string fileSource, string dirDest)
        {
            bool result = true;
            try
            {
                ZipFile.ExtractToDirectory(fileSource, dirDest);
            }
            catch (Exception ex)
            {
                log.Error("UnZip Tool Error: " + ex.Message);
                result = false;
            }
            return result;
        }


    }
}
