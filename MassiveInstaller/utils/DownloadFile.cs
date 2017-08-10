using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using log4net;

namespace PaletteInstaller.utils
{
    class DownloadFile 
    {
        private static readonly ILog log = LogManager.GetLogger("DownUtil");

        //private static Logger logger = LoggerHelper.GetLogger("DownloadFile");

        private string source;
        private string destination;

        public DownloadFile(string source, string destination)
        {
            this.source = source;
            this.destination = destination;
        }
        
        public bool downloadFile()
        {
            WebClient webClient = new WebClient();
            try
            {
                webClient.DownloadFileTaskAsync(this.source, this.destination).Wait();
            }catch(Exception ex)
            {
                log.Error("Download File Error: " + ex.Message);
                return false;
            }

            return true;
        }

        public StringBuilder DownloadTextFileToMemory()
        {
            StringBuilder sb =  new StringBuilder();

            try
            {
                HttpWebRequest request = WebRequest.Create(source) as HttpWebRequest;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream());
                sb.Append(reader.ReadToEnd());
            }catch (Exception ex)
            {
                log.Error("Download Script Error: " + ex.Message);
            }
            return sb;
        }
    }
}
