using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Configuration;

namespace MasiveInstallerWeb.Controllers
{
    public class InstallerController : ApiController
    {

        private readonly string ROOT_PATH = System.Web.Configuration.WebConfigurationManager.AppSettings.Get("Root_Path");

        [Route("api/Installer/GET/CurrentVersion")]
        [HttpGet]
        public HttpResponseMessage CurrentVersion(string site)
        {
            string result = "0";
            
            try
            {
                result = System.IO.File.ReadAllText(ROOT_PATH + site + "CurrentVersion.txt");

            } catch (Exception ex)
            {
                result = ex.Message;
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(result.ToString(), Encoding.UTF8, "text/text");

            return response;
        }

        [Route("api/Installer/GET/Script")]
        [HttpGet]
        public HttpResponseMessage Script(string site, string Type)
        {
            string result = "0";

            try
            {
                if (Type == "I")
                    result = System.IO.File.ReadAllText(ROOT_PATH + site + "InstallScript.txt");
                else if (Type == "U")
                    result = System.IO.File.ReadAllText(ROOT_PATH + site + "UpdateScript.txt");
                else
                    result = "Invalid Type";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(result.ToString(), Encoding.UTF8, "text/text");
            
            return response;
        }

        [Route("api/Installer/GET/Files")]
        [HttpGet]
        public HttpResponseMessage GetFiles(string site, string Type)
        {
            byte[] result = { 0x00 };
            string filename = "";
            
            if (Type == "I")
                filename = ROOT_PATH + site + "InstallFiles.zip";
            else if (Type == "U")
                filename = ROOT_PATH + site + "UpdateFiles.zip";
            else if (Type == "B") //Obtiene el paquete basico para instalar el servicio de instalacion y actualizacion.
                filename = ROOT_PATH + site + "InstallService.zip";
            
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(filename, FileMode.Open, FileAccess.Read));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("applitacion/zip");
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Package.ZIP";
            
            return response;
        }

    }
}
